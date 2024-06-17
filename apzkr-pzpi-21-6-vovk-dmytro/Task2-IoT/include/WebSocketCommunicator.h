#include "Communicator.h"
#include <Position.h>

#include <WiFi.h>
#include <HTTPClient.h>
#include <WebSocketsClient.h>
#include <ArduinoJson.h>
#include <TaskTimer.h>
#include <DeviceInfo.h>

#define WIFI_SSID "OpenWrt_SubNet"
#define WIFI_PASSWORD "aiSp()ae(G-4w89tua*(pe))"

#define HTTPS_TEXT "https://"
#define WSS_TEXT "wss://"

#define SERVER_PORT 7225
#define SERVER_ADDRESS "192.168.1.111"

#define SERVER_HTTP_URL (String(HTTPS_TEXT) + String(SERVER_ADDRESS) + ":" + String(SERVER_PORT))
#define SERVER_WS_URL (String(WSS_TEXT) + String(SERVER_ADDRESS) + ":" + String(SERVER_PORT))

#define LOG(x) Serial.println(x)

class WebSocketCommunicator : public Communicator
{
private:
    WebSocketsClient webSocket;
    DeviceInfo* deviceInfo = nullptr;
    TaskTimer timer;
    bool isWiFiConnected = false;

    std::function<void(const String& message)>onMessage;
    
    void connectWiFi();
    void pingServer();
    void onWebSocketEvent(WStype_t type, uint8_t *payload, size_t length);
public:
    WebSocketCommunicator();
    ~WebSocketCommunicator();

    void init(DeviceInfo* config, std::function<void(const String& message)>onMessage);
    void tick();

    void sendPosition(const Position& position);
    DeviceInfo getDeviceInfo();
    bool updateToken();
    long getServerTime();
};