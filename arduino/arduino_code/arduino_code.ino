#include <WiFiNINA.h> 
#include <PubSubClient.h>
#include <FastLED.h>
#include <pt.h>

const char* ssid = "Groep8"; //"telenet-65213"
const char* password = "TrappenspelGroep8"; //"0CuyNfV9fJ4c"
const char* mqttServer = "13.81.105.139";
const char* mqttUsername = "";
const char* mqttPassword = "";

WiFiClient wifiClient;
PubSubClient client(wifiClient);

char subTopicStep1[] = "kobemarchal/groep8/step1";
char subTopicStep2[] = "kobemarchal/groep8/step2";
char pubTopicScore[] = "kobemarchal/groep8/score";
char pubTopicStop[] = "kobemarchal/groep8/gamestop";

#define NUM_LEDS      50

#define RGB1          4
#define SENSOR1_ECHO  6
#define SENSOR1_TRIG  5
CRGB leds1[NUM_LEDS];
long duration1;
int distance1;
int distance1prev = -1;

#define RGB2          10
#define SENSOR2_ECHO  12
#define SENSOR2_TRIG  11
CRGB leds2[NUM_LEDS];
long duration2;
int distance2;
int distance2prev = -1;

bool stepQuantity = false;
bool game = false;
bool doOnce = true;

int points = 0;
bool startStep1 = false;
String color1 = "green";

static struct pt pt1, pt2, pt3;

void setup_wifi() {
  delay(10);
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}

void reconnect() {
  while(!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    String clientId = "ArduinoClient-";
    clientId += String(random(0xffff), HEX);
    
    if(client.connect(clientId.c_str(), mqttUsername, mqttPassword)) {
      Serial.println("connected");
      client.subscribe(subTopicStep1);
      client.subscribe(subTopicStep2);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      delay(5000);
    }
  }
}

void callback(char* topic, byte* payload, unsigned int length) 
{
  String message;
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] : ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
    message += (char)payload[i];
  }
  Serial.println();

  if(message == "quantityset") {
    stepQuantity = true;
  }

  if(message == "gamestart") {
    doOnce = true;
    points = 0;
    startStep1 = false;
    color1 = "green";
    stepQuantity = false;
    game = true;
  }

  if(message == "gamestop") {
    game = false;
  }
}

static int gameLoop(struct pt *pt) {
  static unsigned long timestamp = 0;
  PT_BEGIN(pt);

  if(stepQuantity and game == false) {
    fill_solid(leds1, NUM_LEDS, CRGB(250, 0, 0));
    fill_solid(leds2, NUM_LEDS, CRGB(250, 0, 0));
    FastLED.show();
    
    timestamp = millis();
    PT_WAIT_UNTIL(pt, millis() - timestamp > 300);
    
    stepQuantity = false;
  }

  if(game == false) {
    timestamp = millis();
    PT_WAIT_UNTIL(pt, millis() - timestamp > 300);

    if(stepQuantity == false) {
      fill_solid(leds1, NUM_LEDS, CRGB(0, 0, 0));
      fill_solid(leds2, NUM_LEDS, CRGB(0, 0, 0));
      FastLED.show();
    }
  }

  if(game) {
    if(doOnce == true) {
      fill_solid(leds1, NUM_LEDS, CRGB(0, 0, 0));
      fill_solid(leds2, NUM_LEDS, CRGB(0, 0, 0));
      FastLED.show();
      
      fill_solid(leds1, NUM_LEDS, CRGB(0, 250, 0));
      FastLED.show();
      doOnce = false;
    }
  }

  PT_END(pt);
}

static int trapSensor(struct pt *pt) {
  static unsigned long timestamp = 0;
  PT_BEGIN(pt);

  if(game) {
    digitalWrite(SENSOR1_TRIG, LOW);
    delayMicroseconds(2);
    digitalWrite(SENSOR1_TRIG, HIGH);
    delayMicroseconds(10);
    digitalWrite(SENSOR1_TRIG, LOW);
    
    duration1 = pulseIn(SENSOR1_ECHO, HIGH);
    distance1 = duration1 * 0.034 / 2;
    
    Serial.print("Distance: ");
    Serial.print(distance1);
    Serial.println(" cm");
  
    if(distance1prev != -1 and(distance1prev - distance1 > 5)) {
      if(color1 == "green") {
        points += 1;
        startStep1 = true;
      } else if (color1 == "orange") {
        points += 2;
      } else if (color1 == "red") {
        points = -1;
        client.publish(pubTopicStop, "stop");
      }

      if(points != -1) {
        char pointsConverted[16];
        itoa(points, pointsConverted, 10);
        client.publish(pubTopicScore, pointsConverted);

        timestamp = millis();
        PT_WAIT_UNTIL(pt, millis() - timestamp > 400);
      }
    }
  
    distance1prev = distance1;
    
    timestamp = millis();
    PT_WAIT_UNTIL(pt, millis() - timestamp > 50);
  }

  PT_END(pt);
}

static int trapRGB(struct pt *pt) {
  static unsigned long timestamp = 0;
  PT_BEGIN(pt);

  if(game) {
    if(startStep1 == true) {
      fill_solid(leds1, NUM_LEDS, CRGB(250, 250, 0));
      FastLED.show();
      color1 = "orange";
      
      timestamp = millis();
      PT_WAIT_UNTIL(pt, millis() - timestamp > 4000);
  
      fill_solid(leds1, NUM_LEDS, CRGB(250, 0, 0));
      FastLED.show();
      color1 = "red";
  
      timestamp = millis();
      PT_WAIT_UNTIL(pt, millis() - timestamp > 4000);
  
      fill_solid(leds1, NUM_LEDS, CRGB(0, 250, 0));
      FastLED.show();
      color1 = "green";
  
      startStep1 = false;
    }
  }

  PT_END(pt);
}

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqttServer, 1883);
  client.setCallback(callback);

  FastLED.addLeds<WS2812, RGB1, GRB>(leds1, NUM_LEDS);
  pinMode(SENSOR1_ECHO, INPUT);
  pinMode(SENSOR1_TRIG, OUTPUT);

  FastLED.addLeds<WS2812, RGB2, GRB>(leds2, NUM_LEDS);
  pinMode(SENSOR2_ECHO, INPUT);
  pinMode(SENSOR2_TRIG, OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  if (!client.connected()) {
    reconnect();
  }

  client.loop();

  gameLoop(&pt1);
  trapSensor(&pt2);
  trapRGB(&pt3);
}
