#include <iostream>
#include <unistd.h>
#include <netinet/in.h>
#include <cstring>
#include <netdb.h>
#include <arpa/inet.h>
#include "DNSPacket.h"
#include <sys/socket.h>
#include <netinet/in.h>

/*
    Hynek Bernard
    xberna16
 */

int main(int argc, char *argv[]) {
    struct timeval tv;
    int err;
    tv.tv_sec = 15;
    tv.tv_usec = 0;
    int c, port, sock, fromlen;
    port = 53;
    sockaddr_storage to;//adresa kam posíláme packet
    std::string server, address;
    struct addrinfo *serverInfo;
    bool recursive = false, reverse = false, ipv6 = false;
    bool args[7] = {false, false, false, false, false, false, true};
    auto rcvPacket = new DNSPacket();
    //Začátek kontroly parametrů
    while ((c = getopt(argc, argv, "rx6s:p:")) != -1) {
        switch (c) {
            case 'r':
                if (args[0]) {
                    args[6] = false;
                }
                args[0] = recursive = true;

                break;
            case 'x':
                if (args[1]) {
                    args[6] = false;
                }
                args[1] = reverse = true;
                break;
            case '6':
                if (args[2]) {
                    args[6] = false;
                }
                args[2] = ipv6 = true;
                break;
            case 's':
                if (args[3] || !optarg) {
                    args[6] = false;
                }
                args[3] = true;
                server = optarg;
                break;
            case 'p':
                port = (int)std::strtol(optarg,nullptr,10);
                if (args[4] || !optarg ) {
                    args[6] = false;
                }
                args[4] = true;
                break;
            case '?':
                args[5] = false;
                break;
            default:
                return 2;
        }
    }
    if (argc <= optind) //zápis adresy
        args[6] = false;
    else {
        address = argv[optind++];
        args[5] = true;
    }
    if (!args[6] || !args[3] || argc != optind) //špatný vstup
    {
        std::cerr << "Chybně zapsané parametry" << std::endl;
        printf("Použití: dns [-r] [-x] [-6] -s server [-p port] adresa\n"
                       "\n"
                       "Pořadí parametrů je libovolné. Argument adresa musí být zapsán jako poslední. Popis parametrů:\n"
                       "\n"
                       "    -r: Požadována rekurze (Recursion Desired = 1), jinak bez rekurze.\n"
                       "    -x: Reverzní dotaz místo přímého.\n"
                       "    -6: Dotaz typu AAAA místo výchozího A.\n"
                       "    -s: IP adresa nebo doménové jméno serveru, kam se má zaslat dotaz.\n"
                       "    -p port: Číslo portu, na který se má poslat dotaz, výchozí 53.\n"
                       "    adresa: Dotazovaná adresa.");
        return 1;
    }
    //Konec kontroly parametrů

    //komunikace
    //inicializace komunikace
    if (getaddrinfo(server.c_str(), std::to_string(port).c_str(), nullptr, &serverInfo)) {
        std::cerr << "Nepodařilo se ověřit adresu DNS serveru" << std::endl;
        return 2;
    }

    sock = socket(serverInfo->ai_family, SOCK_DGRAM, IPPROTO_UDP);
    if (sock < 0) {
        std::cerr << "Nepodařilo se otevřít schránku" << std::endl;
        return 2;
    }
    if (setsockopt(sock, SOL_SOCKET, SO_RCVTIMEO, &tv, sizeof(tv)) < 0) {
        std::cerr << "Nepodařilo se nastavit timeout schránce" << std::endl;
        close(sock);
        return 2;
    }
    //IPv4 nebo IPv6
    if (serverInfo->ai_family == AF_INET) {
        memset(&to, 0, sizeof(to));
        to.ss_family = AF_INET;
    } else {
        memset(&to, 0, sizeof(to));
        to.ss_family = AF_INET6;
    }
    //konec inicializace komunikace
    //stavění packetu
    auto sndPacket = new DNSPacket();
    if (!sndPacket->Prepare(recursive, reverse, ipv6, address.c_str())) {
        std::cerr << "Nepodařilo se postavit packet" << std::endl;
        close(sock);
        return 1;
    }
    //konec stavění packetu

    //odeslání packetu
    if (!sendto(sock, sndPacket->buffer, (size_t) sndPacket->ptr, 0, serverInfo->ai_addr, sizeof(to))) {
        std::cerr << "Nepodařilo se odeslat packet" << std::endl;
        close(sock);
        return 2;
    }
    //přijmutí packetu
    fromlen = sizeof(to);
    if (recvfrom(sock, rcvPacket->buffer, 512, 0, (struct sockaddr *) &to, (socklen_t *) &fromlen) < 0) {
        std::cerr << "Chyba komunikace! Nepodařilo se přijmout packet" << std::endl;
        close(sock);
        return 2;
    }
    if (rcvPacket->GettID() != sndPacket->GettID()) {
        std::cerr << "Chyba komunikace! Identifikace packetů nesedí" << std::endl;
        close(sock);
        return 1;
    }
    //kontrola rcode
    switch (rcvPacket->RCode()) {
        case 0:
            break;
        case 1:
            std::cerr << "Chyba komunikace! Špatný formát dotazu" << std::endl;
            close(sock);
            return 2;
        case 2:
            std::cerr << "Chyba komunikace! Server nedokáže zpracovat dotaz" << std::endl;
            close(sock);
            return 2;
        case 3:
            std::cerr << "Chyba komunikace! Hledané zařízení neexistuje" << std::endl;
            close(sock);
            return 2;
        case 4:
            std::cerr << "Chyba komunikace! Nepodporovaný dotaz" << std::endl;
            close(sock);
            return 2;
        case 5:
            std::cerr << "Chyba komunikace! Server zamítl dotaz" << std::endl;
            close(sock);
            return 2;
        default:
            std::cerr << "Chyba komunikace! Neznámá odpověď serveru" << std::endl;
            close(sock);
            return 2;
    }

    rcvPacket->PrintInfo();
    close(sock);
    return 0;
}