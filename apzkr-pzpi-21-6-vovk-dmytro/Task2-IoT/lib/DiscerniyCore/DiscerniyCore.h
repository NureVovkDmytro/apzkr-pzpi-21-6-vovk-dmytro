#pragma once
#include <Arduino.h>
#include "DataSource.h"
#include "Communicator.h"
#include "DeviceInfo.h"

#include "TaskTimer.h"

#include "SPIFFS.h"
#include <ArduinoJson.h>

#define DEVICE_INFO_FILE "/config.json"
#define LOG(msg) Serial.println(msg)

class DiscerniyCore
{
private:
    DataSource* dataSources;
    Communicator* communicator;
    DeviceInfo* deviceInfo;
    unsigned long lastUpdateLocation;

    unsigned long serverTime;
    unsigned long lastUpdateServerTime;

    void loadDeviceInfo();
    void updateDeviceInfo();

    void onMessage(const String& message);
public:
    DiscerniyCore(DataSource* dataSources, Communicator* communicator);
    ~DiscerniyCore();

    void init();
    void tick();
};

