//
// Created by Hynek Bernard on 7.10.19.
//

#ifndef DNSRESOLVER_DNSPACKET_H
#define DNSRESOLVER_DNSPACKET_H


#include <stdint-gcc.h>

class DNSPacket {
private:
    bool SetQuery(bool recursive,bool reverse);
    bool readyToSend = false;
    bool SetQuestion(bool ipv6, const char* address, bool reverse);
    char* PrepareReverseString(char const* str);
    uint8_t GetDot(const char* address, int startIndex, int length);
    const char *ptrAppend = ".in-addr.arpa";
    void PrintNames(int count, bool question=false);
    int PrintName(int printPoint);

public:
    int RCode();
    uint16_t GettID();
    DNSPacket();
    uint8_t buffer[512];
    int ptr = 0;
    bool Prepare(bool recursive, bool reverse, bool ipv6, const char* address);
    void PrintInfo();
};


#endif //DNSRESOLVER_DNSPACKET_H
