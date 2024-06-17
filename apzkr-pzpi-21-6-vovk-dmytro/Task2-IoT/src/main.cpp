#include <Arduino.h>
#include <ArduinoOTA.h>

#include "DiscerniyCore.h"

#include "TestDataSource.h"
#include "WebSocketCommunicator.h"

DiscerniyCore* discerniyCore;

void setup() {
    Serial.begin(115200);
    Serial.println("Booting");

    DataSource* dataSources = new TestDataSource();
    Communicator* communicator = new WebSocketCommunicator();
    discerniyCore = new DiscerniyCore(dataSources, communicator);
    discerniyCore->init();

    ArduinoOTA.begin();
    ArduinoOTA.setPassword("DiscerniyCore");

    Serial.println("Ready");
}

void loop() {
    discerniyCore->tick();
    ArduinoOTA.handle();
}
