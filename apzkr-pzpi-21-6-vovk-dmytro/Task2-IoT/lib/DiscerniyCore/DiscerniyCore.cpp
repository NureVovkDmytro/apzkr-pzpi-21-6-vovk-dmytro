#include "DiscerniyCore.h"

DiscerniyCore::DiscerniyCore(DataSource* dataSources, Communicator* communicator)
{
    this->deviceInfo = new DeviceInfo();
    this->dataSources = dataSources;
    this->communicator = communicator;
}

DiscerniyCore::~DiscerniyCore()
{
    delete dataSources;
    delete communicator;
    delete deviceInfo;
}

void DiscerniyCore::init()
{
    SPIFFS.begin(true);
    loadDeviceInfo();
    dataSources->init();
    communicator->init(deviceInfo, [this](const String& message){
        this->onMessage(message);
    });

    serverTime = communicator->getServerTime();
    deviceInfo->updateInterval = communicator->getDeviceInfo().updateInterval;
    LOG("Server time: " + String(serverTime));
    LOG("Token expiration: " + String(deviceInfo->expiration));

    lastUpdateServerTime = millis();

    LOG("Core initialized!");
}

void DiscerniyCore::tick()
{
    dataSources->tick();
    communicator->tick();

    if(millis() - lastUpdateServerTime > 1000 * 60 || deviceInfo->expiration - 60 < serverTime)
    {
        serverTime = communicator->getServerTime();
        lastUpdateServerTime = millis();

        if(deviceInfo->expiration - 60 < serverTime)
        {
            if(communicator->updateToken())
            {
                updateDeviceInfo();
            }
        }
    }

    if (millis() - lastUpdateLocation > deviceInfo->updateInterval * 1000)
    {
        Position position = dataSources->getPosition();
        communicator->sendPosition(position);
        
        lastUpdateLocation = millis();
    }
}

void DiscerniyCore::onMessage(const String& message)
{
    LOG("Message received: " + message);

    StaticJsonDocument<512> doc;
    deserializeJson(doc, message);

    String command = doc["Command"].as<String>();

    LOG("Command: " + command);
    LOG("Payload: " + message);

    if(command == "updateUserUpdateLocationInterval"){
        this->deviceInfo->updateInterval = doc["Payload"]["LocationSecondsInterval"].as<unsigned long>();
        updateDeviceInfo();
        LOG("Update interval: " + String(this->deviceInfo->updateInterval));
    }
}

void DiscerniyCore::loadDeviceInfo()
{
    auto file = SPIFFS.open(DEVICE_INFO_FILE, "r");
    if (!file)
    {
        LOG("Failed to open file for reading");
        return;
    }

    StaticJsonDocument<2048> doc;

    DeserializationError error = deserializeJson(doc, file);
    if (error)
    {
        LOG("Failed to read file, using default configuration");
        return;
    }

    deviceInfo->updateInterval = doc["updateInterval"].as<unsigned long>();
    deviceInfo->token = doc["token"].as<String>();
    deviceInfo->expiration = doc["expiresAt"].as<unsigned long>();
    deviceInfo->createAt = doc["createAt"].as<unsigned long>();

    LOG("Update interval: " + String(deviceInfo->updateInterval));
    LOG("Token: " + deviceInfo->token);
    LOG("Expiration: " + String(deviceInfo->expiration));
    LOG("Created at: " + String(deviceInfo->createAt));

    file.close();
}

void DiscerniyCore::updateDeviceInfo()
{
    auto file = SPIFFS.open(DEVICE_INFO_FILE, "w");
    if (!file)
    {
        LOG("Failed to open file for writing");
        return;
    }

    StaticJsonDocument<2048> doc;

    doc["updateInterval"] = deviceInfo->updateInterval;
    doc["token"] = deviceInfo->token;
    doc["expiresAt"] = deviceInfo->expiration;
    doc["createAt"] = deviceInfo->createAt;

    serializeJson(doc, file);

    LOG("Device info updated");

    file.close();
}