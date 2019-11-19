//
// Created by hynek on 1.11.19.
//

#include <iostream>
#include <fstream>
#include "test.h"
#include "gtest/gtest.h"
#include "../DNSPacket.h"
/*
    Hynek Bernard
    xberna16
 */

void HexStreamToBuff(std::string hexStream, uint8_t* buffer)
{
    int ptr = 0;
    for (int i = 0; i < hexStream.length(); ++i) {
        buffer[ptr++] =(uint8_t) (hexStream[i] >= 'a' ? (hexStream[i] -'a') +10 : hexStream[i] - '0') << 4 | (hexStream[i+1] >= 'a' ? (hexStream[i+1] -'a')+10 : hexStream[i+1] - '0');
        i++;
    }
}

TEST(vystup, answerA)
{
    std::string hexStream ="a39781800001000300000000037777770469657466036f72670000010001c00c000500010000003b0021037777770469657466036f72670363646e0a636c6f7564666c617265036e657400c02a000100010000003b000468140155c02a000100010000003b000468140055";
    auto testPacket = new DNSPacket();
    std::stringstream buffer;
    std::streambuf * old = std::cout.rdbuf(buffer.rdbuf());
    HexStreamToBuff(hexStream,testPacket->buffer);
    testPacket->PrintInfo();
    delete testPacket;
    ASSERT_EQ(buffer.str(),"Authoritative: No, Recursive: Yes, Truncated: No\n"
                                   "Question section (1)\n"
                                   "www.ietf.org, A, IN\n"
                                   "Answer section (3)\n"
                                   "www.ietf.org, CNAME, IN, TTL: 59, www.ietf.org.cdn.cloudflare.net\n"
                                   "www.ietf.org.cdn.cloudflare.net, A, IN, TTL: 59, 104.20.1.85\n"
                                   "www.ietf.org.cdn.cloudflare.net, A, IN, TTL: 59, 104.20.0.85\n"
                                   "Authority section (0)\n"
                                   "Additional section (0)\n"
                                   "");
    std::cout.rdbuf(old);
}
TEST(vystup, answerB)
{
    std::string hexStream="643981800001000100040005046b617a690366697405767574627202637a0000010001c00c000100010000290b000493e5080cc0110002000100001ea600070467757461c011c0110002000100001ea60002c00cc0110002000100001ea6000c04676174650466656563c015c0110002000100001ea6000c057268696e6f03636973c015c0600001000100000a9f000493e5470ac03f0001000100001e23000493e5090bc03f001c000100001e2300102001067c122008090000000093e5090bc0780001000100000ba4000493e5030ac078001c000100000ba400102001067c1220e0000000000093e5030a";
    auto testPacket = new DNSPacket();
    std::stringstream buffer;
    std::streambuf * old = std::cout.rdbuf(buffer.rdbuf());
    HexStreamToBuff(hexStream,testPacket->buffer);
    testPacket->PrintInfo();
    ASSERT_EQ(buffer.str(),"Authoritative: No, Recursive: Yes, Truncated: No\n"
            "Question section (1)\n"
            "kazi.fit.vutbr.cz, A, IN\n"
            "Answer section (1)\n"
            "kazi.fit.vutbr.cz, A, IN, TTL: 10507, 147.229.8.12\n"
            "Authority section (4)\n"
            "fit.vutbr.cz, NS, IN, TTL: 7846, guta.fit.vutbr.cz\n"
            "fit.vutbr.cz, NS, IN, TTL: 7846, kazi.fit.vutbr.cz\n"
            "fit.vutbr.cz, NS, IN, TTL: 7846, gate.feec.vutbr.cz\n"
            "fit.vutbr.cz, NS, IN, TTL: 7846, rhino.cis.vutbr.cz\n"
            "Additional section (5)\n"
            "gate.feec.vutbr.cz, A, IN, TTL: 2719, 147.229.71.10\n"
            "guta.fit.vutbr.cz, A, IN, TTL: 7715, 147.229.9.11\n"
            "guta.fit.vutbr.cz, AAAA, IN, TTL: 7715, 2001:67c:1220:809::93e5:90b\n"
            "rhino.cis.vutbr.cz, A, IN, TTL: 2980, 147.229.3.10\n"
            "rhino.cis.vutbr.cz, AAAA, IN, TTL: 2980, 2001:67c:1220:e000::93e5:30a\n");

    delete testPacket;
    std::cout.rdbuf(old);
}
TEST(vystup, answerC) {
    std::string hexStream = "a39785800001000000010000046b617a690366697405767574627202637a00001c0001c011000600010000384000240467757461c011066d696368616cc0110c08eae200002a3000000e10000a8c0000015180";
    auto testPacket = new DNSPacket();
    std::stringstream buffer;
    std::streambuf *old = std::cout.rdbuf(buffer.rdbuf());
    HexStreamToBuff(hexStream, testPacket->buffer);
    testPacket->PrintInfo();
    ASSERT_EQ(buffer.str(), "Authoritative: Yes, Recursive: Yes, Truncated: No\n"
            "Question section (1)\n"
            "kazi.fit.vutbr.cz, AAAA, IN\n"
            "Answer section (0)\n"
            "Authority section (1)\n"
            "fit.vutbr.cz, SOA, IN, TTL: 14400, guta.fit.vutbr.cz\n"
            "Additional section (0)\n"
            "");
    std::cout.rdbuf(old);
}
TEST(vystup, answerD) {
    std::string hexStream = "a39785800001000000010000046b617a690366697405767574627202637a00001c0001c011000600010000384000240467757461c011066d696368616cc0110c08eae200002a3000000e10000a8c0000015180";
    auto testPacket = new DNSPacket();
    std::stringstream buffer;
    std::streambuf *old = std::cout.rdbuf(buffer.rdbuf());
    HexStreamToBuff(hexStream, testPacket->buffer);
    testPacket->PrintInfo();
    ASSERT_EQ(buffer.str(), "Authoritative: Yes, Recursive: Yes, Truncated: No\n"
            "Question section (1)\n"
            "kazi.fit.vutbr.cz, AAAA, IN\n"
            "Answer section (0)\n"
            "Authority section (1)\n"
            "fit.vutbr.cz, SOA, IN, TTL: 14400, guta.fit.vutbr.cz\n"
            "Additional section (0)\n"
            "");
    std::cout.rdbuf(old);
}