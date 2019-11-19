//
// Created by Hynek Bernard on 7.10.19.
//
#include <iostream>
#include <cstring>
#include <arpa/inet.h>
#include "DNSPacket.h"
#include <sys/socket.h>
#include <netinet/in.h>
/*
    Hynek Bernard
    xberna16
 */

DNSPacket::DNSPacket() {
    memset(buffer, 0, sizeof(char) * 512);
}

void DNSPacket::PrintInfo() {
    int questions = buffer[5], answers = buffer[7], authority = buffer[9], additional = buffer[11];
    ptr = 2;
    std::cout << "Authoritative: " << (((buffer[ptr] & 0b00000100) > 0) ? "Yes" : "No");
    std::cout << ", Recursive: " << (((buffer[ptr] & 0b00000001) > 0) ? "Yes" : "No");
    std::cout << ", Truncated: " << (((buffer[ptr] & 0b00000010) > 0) ? "Yes" : "No");
    std::cout << std::endl;

    ptr++;
    ptr++;
    ptr++;
    questions = buffer[ptr];
    std::cout << "Question section (" << std::to_string(questions) << ")" << std::endl;
    ptr += 7;//queries
    PrintNames(questions, true);
    std::cout << "Answer section (" << std::to_string(answers) << ")" << std::endl;
    PrintNames(answers);
    std::cout << "Authority section (" << std::to_string(authority) << ")" << std::endl;
    PrintNames(authority);
    //doplnit do packetu odkaz na další NS
    std::cout << "Additional section (" << std::to_string(additional) << ")" << std::endl;
    PrintNames(additional);
}

uint16_t DNSPacket::GettID() {

    return (buffer[0] << 8) | (buffer[1]);
}

bool DNSPacket::Prepare(bool recursive, bool reverse, bool ipv6, const char *address) {
    //číslo transakce
    buffer[ptr++] = rand() % 255;
    buffer[ptr++] = rand() % 255;

    if (!SetQuery(recursive, reverse))
        return false;
    if (!SetQuestion(ipv6, address, reverse))
        return false;
    readyToSend = true;
    return readyToSend;
}

bool DNSPacket::SetQuestion(bool ipv6, const char *address, bool reverse) {

    int aptr = 0;
    auto length = static_cast< int >(strlen(address));
    std::string addr = address;
    if (reverse) {
        address = PrepareReverseString(address);
        length += (int) strlen(ptrAppend);
    }
    while (aptr < length) {
        auto dot = (uint8_t) (GetDot(address, aptr, length) - aptr);
        if (dot == 0)
            break;
        buffer[ptr++] = dot;
        dot += aptr;
        while (aptr < dot) {
            buffer[ptr++] = (uint8_t) address[aptr++];
        }
        aptr++;
    }
    buffer[ptr++] = 0x00;//konec adresy
    if (reverse) {
        buffer[ptr++] = 0x00;//Qtype PTR
        buffer[ptr++] = 0x0C;//Qtype PTR
    } else if (!ipv6) {
        buffer[ptr++] = 0x00;//Qtype A
        buffer[ptr++] = 0x01;//Qtype A
    } else {
        buffer[ptr++] = 0x00;//Qtype AAAA
        buffer[ptr++] = 0x1C;//Qtype AAAA
    }

    buffer[ptr++] = 0x00;//IN
    buffer[ptr++] = 0x01;//IN
    return true;
}

bool DNSPacket::SetQuery(bool recursive, bool reverse) {
    if (ptr != 2)
        return false;
    buffer[ptr] = 0x00;
    /*if (reverse) Už se nepoužívá, proto zakomentováno
        buffer[ptr] = buffer[ptr] | 0b00001000;*/
    if (recursive)
        buffer[ptr] = buffer[ptr] | 1;
    ptr++;
    buffer[ptr++] = 0x00;//response
    buffer[ptr++] = 0x00;//počet dotazů
    buffer[ptr++] = 0x01;//počet dotazů
    buffer[ptr++] = 0x00;//answer
    buffer[ptr++] = 0x00;//answer
    buffer[ptr++] = 0x00;//authority
    buffer[ptr++] = 0x00;//authority
    buffer[ptr++] = 0x00;//additional
    buffer[ptr++] = 0x00;//additional
    return true;
}

uint8_t DNSPacket::GetDot(const char *address, int startIndex, int length) {
    while (startIndex < length + 1) {
        if (address[startIndex] == '.' || address[startIndex] == '\0')
            return (uint8_t) startIndex;
        startIndex++;
    }
    return (uint8_t) 0;
}

char *DNSPacket::PrepareReverseString(char const *str) {
    auto n = (int) strlen(str);
    int appendLength = (int) strlen(ptrAppend);
    auto rev = new char[n + appendLength + 1];
    int cntr = 0, revPtr = 0;
    for (int i = n - 1; i >= 0; i--) {
        cntr++;
        if (str[i] == '.' || i == 0) {
            for (int j = i == 0 ? 0 : 1; j < cntr; ++j) {
                rev[revPtr++] = str[i + j];
            }
            cntr = 0;
            rev[revPtr++] = '.';
        }
    }
    n += appendLength;
    memcpy(&rev[n - (appendLength)], ptrAppend, sizeof(char) * appendLength);
    return rev;
}

int DNSPacket::RCode() {
    ptr = 3;
    return buffer[ptr] % 16;
}

void DNSPacket::PrintNames(int count, bool question) {
    int type = 0, datalength = 0;
    if (count < 1)
        return;
    for (int i = 0; i < count; ++i) {
        if (buffer[ptr]==0)
        {
            ptr++;
            ptr++;
            if (buffer[ptr++]==0x02)
            {
                std::cout << "<<ROOT>>";
                ptr++;
                type = buffer[ptr++];
                switch (type) {
                    case 1:
                        std::cout << ", IN";
                        break;
                    case 2:
                        std::cout << ", CS";
                        break;
                    case 3:
                        std::cout << ", CH";
                        break;
                    case 4:
                        std::cout << ", HS";
                        break;
                    default:
                        std::cout << ", UNKNOWN!!!";
                }
                std::cout << ", TTL: "
                          << std::to_string(buffer[ptr++] << 24 | buffer[ptr++] << 16 | buffer[ptr++] << 8 | buffer[ptr++]);
                std::cout << ", ";
                datalength = buffer[ptr++] << 8 | buffer[ptr++];
                ptr = PrintName(ptr);
                std::cout << std::endl;
            }
            else {
                std::cout << "UNKNOWN";
            }
            continue;
        }
        ptr = PrintName(ptr);
        ptr++;
        type = buffer[ptr++];
        switch (type) {
            case 1:
                std::cout << ", A";
                break;
            case 2:
                std::cout << ", NS";
                break;
            case 3:
                std::cout << ", MD";
                break;
            case 4:
                std::cout << ", MF";
                break;
            case 5:
                std::cout << ", CNAME";
                break;
            case 6:
                std::cout << ", SOA";
                break;
            case 7:
                std::cout << ", MB";
                break;
            case 8:
                std::cout << ", MG";
                break;
            case 9:
                std::cout << ", MR";
                break;
            case 10:
                std::cout << ", NULL";
                break;
            case 11:
                std::cout << ", WKS";
                break;
            case 12:
                std::cout << ", PTR";
                break;
            case 13:
                std::cout << ", HINFO";
                break;
            case 14:
                std::cout << ", MINFO";
                break;
            case 15:
                std::cout << ", MX";
                break;
            case 16:
                std::cout << ", TXT";
                break;
            case 28:
                std::cout << ", AAAA";
                break;
            default:
                std::cout << "UNKNOWN!!!";
        }
        ptr++;
        switch (buffer[ptr]) {
            case 1:
                std::cout << ", IN";
                break;
            case 2:
                std::cout << ", CS";
                break;
            case 3:
                std::cout << ", CH";
                break;
            case 4:
                std::cout << ", HS";
                break;
            default:
                std::cout << ", UNKNOWN!!!";
        }
        ptr++;
        if (question) {
            std::cout << std::endl;
            continue;
        }
        std::cout << ", TTL: "
                  << std::to_string(buffer[ptr++] << 24 | buffer[ptr++] << 16 | buffer[ptr++] << 8 | buffer[ptr++]);
        std::cout << ", ";
        datalength = buffer[ptr++] << 8 | buffer[ptr++];
        switch (type) {
            case 1:
                for (int j = 0; j < datalength; ++j) {
                    std::cout << std::to_string(buffer[ptr++]);
                    if (j < datalength - 1)
                        std::cout << '.';
                }
                break;
            case 2://NS
            case 6:
                //std::cout << ", SOA";
            case 5:
                //std::cout << ", CNAME";
                ptr = PrintName(ptr);
                break;
            case 3:
                //std::cout << ", MD";
            case 4:
                //std::cout << ", MF";
            case 7:
                // std::cout << ", MB";
            case 8:
                //std::cout << ", MG";
            case 9:
                //std::cout << ", MR";
            case 10:
                //std::cout << ", NULL";
            case 11:
                //std::cout << ", WKS";
            case 12:
                //std::cout << ", PTR";
            case 13:
                //std::cout << ", HINFO";
            case 14:
                // std::cout << ", MINFO";
            case 15:
                //std::cout << ", MX";
            case 16:
                //std::cout << ", TXT";
                for (int j = 0; j < datalength; ++j) {
                    std::cout << buffer[ptr++];
                }
                break;
            case 28: {
                //std::cout << ", AAAA";
                auto dst = new char[INET6_ADDRSTRLEN + 1];
                inet_ntop(AF_INET6, &buffer[ptr], dst, INET6_ADDRSTRLEN);
                std::cout << dst;
                ptr += datalength;
                break;
            }
            default:
                std::cout << "UNKNOWN!!!";
                break;
        }
        std::cout << std::endl;
    }
}

int DNSPacket::PrintName(int printPoint) {
    if ((buffer[printPoint] & 0b11000000) > 0) {
        PrintName((buffer[printPoint] ^ 0b11000000) << 8 | buffer[printPoint + 1]);
        return ++ ++printPoint;
    }
    while (true) {
        int chCount = buffer[printPoint++];
        for (int j = 0; j < chCount; ++j) {
            std::cout << buffer[printPoint++];
        }
        if ((buffer[printPoint] & 0b11000000) > 0) {
            std::cout << '.';
            PrintName((buffer[printPoint] ^ 0b11000000) << 8 | buffer[printPoint + 1]);
            return ++ ++printPoint;
        }
        if (buffer[printPoint] == 0) {
            printPoint++;
            break;
        }
        std::cout << '.';
    }
    return printPoint;
}
