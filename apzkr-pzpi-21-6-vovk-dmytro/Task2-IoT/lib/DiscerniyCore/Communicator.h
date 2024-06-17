#pragma once
#include "DeviceInfo.h"
#include "Position.h"

class Communicator
{
public:
    virtual void init(DeviceInfo* config, std::function<void(const String& message)>onMessage) = 0;
    virtual void tick() = 0;

    virtual void sendPosition(const Position& position) = 0;
    virtual DeviceInfo getDeviceInfo() = 0;
    virtual bool updateToken() = 0;
    virtual long getServerTime() = 0;
};
