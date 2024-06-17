#pragma once
#include <Arduino.h>

struct TokenInfo
{
    String token;
    unsigned long expiration;
    unsigned long createAt;
};

struct DeviceInfo : TokenInfo
{
    int updateInterval;
};
