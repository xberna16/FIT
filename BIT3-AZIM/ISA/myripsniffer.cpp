#include <iostream>
#include <pcap/pcap.h>
#include <cstring>
#include <arpa/inet.h>
/*Hynek Bernard
 * xberna16@stud.fit.vutbr.cz
 * */
#define IFNAMSIZ 16
#define BUFFSIZE 512
using namespace std;
class Packet{
    u_char *content;
    const int srcMac =0;//indexy dat na packetu   IPV6 má length payload+54    ipv4 ttllength+14
    const int dstMac=6;
    bool ipv4;
    int srcIP, dstIP, id, length, rip, version, type, ripEntry;
private:void setVersion()//nastaví verzi a typ RIP
    {
        type = content[rip];
        version = content[rip+1];
    }
public:Packet(const u_char *packet)
    {
        ipv4 = packet[12] == 0x08;
        if (ipv4)
        {
            length = (packet[16]<<8) | packet[17];
            length = length + 14;
            id = 18;
            srcIP =26;
            dstIP = 30;
            rip = 42;
        }
        else{
            if(packet[12] != 0x86)
            {
                fprintf(stdout, "Unknown packet type %d\n", (packet[12]<<8) | packet[13]);
                return;
            }
            length = (packet[18]<<8) | packet[19];
            length = length +54;
            srcIP = 22;
            dstIP = 38;
            rip = 62;
        }
        content = new u_char[length];
        memcpy(content,packet,sizeof(u_char)*length);
        setVersion();
    }
private:void echoMac(u_char *addr)
    {
        for (int i = 0; i < 5; ++i) {
            printf("%02x:",addr[i]);
        }
        printf("%0x ",addr[5]);
    }
private:void echoIP(u_char *addr, bool IPv4)
    {
        if (IPv4)
        {
            for (int i = 0; i < 3; ++i) {
                printf("%d.", addr[i]);
            }
            printf("%d", addr[3]);
        }
        else
        {
            char output[INET6_ADDRSTRLEN];
            inet_ntop(AF_INET6, (in6_addr*) addr, output,INET6_ADDRSTRLEN);
            cout << output;
        }
    }
public:void echo(){
        cout << "---------------------------------\nsrc: ";
        echoMac(&content[srcMac]);
        echoIP(&content[srcIP],ipv4);

        cout << " dst: ";
        echoMac(&content[dstMac]);
        echoIP(&content[dstIP],ipv4);
        cout << "\nVersion:";
        cout << version;
        cout << " Command:";
        cout << type;
        cout << "\n";
        if(ipv4 && version != 1) {
            if (version !=2 || (type != 2 && type != 1))
                printf("\nNeznámá verze, nebo command\n");
            ripEntry = rip+4;
            if(content[ripEntry] == 0xff && content[ripEntry+1] == 0xff) {
                cout << "AuthType:";
                printf("%x %x", content[rip + 6], content[rip + 7]);
                cout << " Pass: ";
                for (int i = 0; i < 16; ++i) {
                    printf("%c", content[rip + 8 + i]);
                }
                cout << "\n";
                ripEntry = ripEntry+20;
            }
            for (int i = 0; i < (length-ripEntry)/20; ++i) {
                cout << "IP addr:";
                echoIP(&content[ripEntry + 4 + (i * 20)], ipv4);
                cout << " NetMask:";
                echoIP(&content[ripEntry + 8 + (i * 20)], ipv4);
                cout << " NextHop:";
                echoIP(&content[ripEntry + 12 + (i * 20)], ipv4);
                cout << " hopCount:";
                printf("%d",content[ripEntry + 19 + (i * 20)]);
                cout << "\n";
            }
        }
        if(ipv4 && version == 1) {
            ripEntry = rip+4;
            for (int i = 0; i < (length - rip - 2) / 20; ++i) {
                cout << "IP addr:";
                echoIP(&content[ripEntry + 4 + (i * 20)], ipv4);
                cout << " NetMask:";
                echoIP(&content[ripEntry + 9 + (i * 20)], ipv4);
                cout << "NextHop:";
                echoIP(&content[ripEntry + 9 + (i * 20)], ipv4);
                cout << "hopCount:";
                printf("%d",content[ripEntry + 19 + (i * 20)]);
                cout << "\n";
            }
        }
        if(!ipv4)
        {
            if (version !=1 || (type != 2 && type != 1))
                printf("\nNeznámá verze, nebo command\n");
            ripEntry = rip+4;
            for (int i = 0; i < (length - rip  - 2) / 20; ++i) {
                cout << "IPv6 Prefix:";
                echoIP(&content[ripEntry  + (i * 20)], ipv4);
                printf("/%d",content[ripEntry +18 + (i*20)]);
                cout << " Route Tag: 0x";
                printf("%02x%02x",content[ripEntry +16 + (i*20)],content[ripEntry +17 + (i*20)]);
                cout << " hopCount:";
                printf("%d",content[ripEntry  + 19 + (i * 20)]);
                cout << "\n";
            }
        }
        delete(content);
    }
};
void packet_handler(u_char *args, const struct pcap_pkthdr *header, const u_char *packet)//Handler příchozího packetu
    {
        auto *p = new Packet(packet);
        p->echo();
        delete p;
    }
int main(int argc, char * argv[]) {
    char ifName[IFNAMSIZ];//interface
    char errBuff[BUFFSIZE];
    bpf_u_int32 net;//IP
    pcap_t *handle;//otevřené zařízení
    char filter_exp[] = "udp port 520 or udp port 521";	//filtr
    struct bpf_program fp;		//zkompilovaný filtr
    if (argc == 3 && strcmp(argv[1],"-i") == 0)
        strcpy(ifName, argv[2]);
    else
    {
        printf("Použití:sudo ./myripsniffer -i <interface>\n");
        return(1);
    }
    handle = pcap_open_live(ifName, BUFFSIZE, 1, 1000, errBuff);
    if (handle == NULL) {
        fprintf(stdout, "Nedaří se otevřít interface %s: %s\n", ifName, errBuff);
        return(2);
    }
    if (pcap_compile(handle, &fp, filter_exp, 0, net) == -1) {
        fprintf(stderr, "Nelze scanovat porty 520 a 521 %s: %s\n", filter_exp, pcap_geterr(handle));
        return(2);
    }
    if (pcap_setfilter(handle, &fp) == -1) {
        fprintf(stderr, "Nelze scanovat porty 520 a 521 %s: %s\n", filter_exp, pcap_geterr(handle));
        return(2);
    }

    pcap_loop(handle, -100, packet_handler, NULL);// přidělení handleru k loopu
    pcap_freecode(&fp);
    pcap_close(handle);
    return 0;
}
