#include "WebSocketCommunicator.h"

WebSocketCommunicator::WebSocketCommunicator()
{
    this->isWiFiConnected = false;
}

WebSocketCommunicator::~WebSocketCommunicator()
{
}

void WebSocketCommunicator::init(DeviceInfo *config, std::function<void(const String& message)>onMessage)
{
    this->onMessage = onMessage;
    this->deviceInfo = config;
    this->connectWiFi();
    this->webSocket.beginSSL(SERVER_ADDRESS, SERVER_PORT, "/connect/device");
    this->webSocket.setAuthorization(("Bearer " + config->token).c_str());
    this->webSocket.setReconnectInterval(5000);

    this->webSocket.onEvent([this](WStype_t type, uint8_t *payload, size_t length)
                            {
        switch (type)
        {
        case WStype_DISCONNECTED:
            LOG("WS Disconnected");
            LOG("Error: " + String((char *)payload));
            break;
        case WStype_CONNECTED:
            LOG("WS Connected");
            break;
        case WStype_TEXT:
            this->onMessage(String((char *)payload));
            break;
        default:
            break;
        } });
    this->timer.add([this]()
                    { this->pingServer(); }, 10000);
}

void WebSocketCommunicator::tick()
{
    this->connectWiFi();
    this->timer.tick();
    this->webSocket.loop();
}

void WebSocketCommunicator::sendPosition(const Position &position)
{
    String json = "{\"Command\":\"updateLocation\",\"Payload\":{\"Easting\":" + String(position.easting) + ",\"Northing\":" + String(position.northing) + ",\"Compass\":" + String(position.compass) + "}}";

    LOG("Sending position");
    LOG(json);

    this->webSocket.sendTXT(json);
}

void WebSocketCommunicator::pingServer()
{
    this->webSocket.sendTXT("{\"Command\":\"ping\"}");
}

DeviceInfo WebSocketCommunicator::getDeviceInfo()
{
    HTTPClient http;
    http.begin(SERVER_HTTP_URL + "/api/device/self");
    http.addHeader("Authorization", "Bearer " + this->deviceInfo->token);

    int httpCode = http.GET();

    if (httpCode == 200)
    {
        StaticJsonDocument<256> doc;
        deserializeJson(doc, http.getString());

        DeviceInfo info;
        info.updateInterval = doc["updateInterval"].as<unsigned long>();

        LOG("Device info received");
        LOG("Update interval: " + String(info.updateInterval));

        return info;
    }

    LOG("Failed to get device info");

    return DeviceInfo();
}

bool WebSocketCommunicator::updateToken()
{
    LOG("Updating token...");
    HTTPClient http;
    http.begin(SERVER_HTTP_URL + "/api/auth/device/refresh");
    http.addHeader("Authorization", "Bearer " + this->deviceInfo->token);

    int httpCode = http.GET();

    if (httpCode == 200)
    {
        StaticJsonDocument<1024> doc;
        deserializeJson(doc, http.getString());

        deviceInfo->token = doc["token"].as<String>();
        deviceInfo->expiration = doc["expiration"].as<unsigned long>();
        LOG("Token updated");
        return true;
    }
    LOG("Failed to update token!");
    return false;
}

long WebSocketCommunicator::getServerTime()
{
    HTTPClient http;
    http.begin(SERVER_HTTP_URL + "/api/system/time");
    http.addHeader("Authorization", "Bearer " + this->deviceInfo->token);
    int httpCode = http.GET();

    if (httpCode == 200)
    {
        String time = http.getString();
        LOG("Server time: " + time);
        return atol(time.c_str());
    }

    LOG("Failed to get server time");

    return 0;
}

void WebSocketCommunicator::connectWiFi()
{
    this->isWiFiConnected = WiFi.status() == WL_CONNECTED;

    if (isWiFiConnected)
    {
        return;
    }

    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

    LOG("Connecting to WiFi");

    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        LOG(".");
    }

    LOG("Connected to WiFi");

    isWiFiConnected = true;
}