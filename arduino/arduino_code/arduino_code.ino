#include <WiFiNINA.h> 
#include <PubSubClient.h>
#include <FastLED.h>
#include <pt.h>

const char* ssid = "Trappenspel";
const char* password = "EaFjnaefiJAE";
const char* mqttServer = "13.81.105.139";
const char* mqttUsername = "";
const char* mqttPassword = "";

WiFiClient wifiClient;
PubSubClient client(wifiClient);

// Hier alle step[getal] veranderen naar welke treden waarover het gaat
char subTopicStep1[] = "kobemarchal/groep8/step1";
char subTopicStep2[] = "kobemarchal/groep8/step2";
char subTopicSensor1[] = "kobemarchal/groep8/step1/sensor";
char subTopicSensor2[] = "kobemarchal/groep8/step2/sensor";
char pubTopicStep1[] = "kobemarchal/groep8/step1/answer";
char pubTopicStep2[] = "kobemarchal/groep8/step2/answer";
char pubTopicStop[] = "kobemarchal/groep8/gamestop";

#define NUM_LEDS      50

#define RGB1          4
#define SENSOR1_ECHO  6
#define SENSOR1_TRIG  5
CRGB leds1[NUM_LEDS];
long duration1;
int distance1;
int distance1prev = -1;
bool measureSensor1 = false;
int measurementWhenSteppedOn1 = -1;

#define RGB2          10
#define SENSOR2_ECHO  12
#define SENSOR2_TRIG  11
CRGB leds2[NUM_LEDS];
long duration2;
int distance2;
int distance2prev = -1;
bool measureSensor2 = false;
int measurementWhenSteppedOn2 = -1;

bool stepQuantity = false;
bool game = false;

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
      client.subscribe(subTopicSensor1);
      client.subscribe(subTopicSensor2);
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
  String topicStr = topic;
  
  String message;
//  Serial.print("Message arrived [");
//  Serial.print(topic);
//  Serial.print("] : ");
  for (int i = 0; i < length; i++) {
//    Serial.print((char)payload[i]);
    message += (char)payload[i];
  }
//  Serial.println();

  if(message == "quantityset") {
    stepQuantity = true;
    fill_solid(leds1, NUM_LEDS, CRGB(250, 0, 0));
    fill_solid(leds2, NUM_LEDS, CRGB(250, 0, 0));
    FastLED.show();
  }

  if(message == "quantitysetoff") {
    stepQuantity = false;
    fill_solid(leds1, NUM_LEDS, CRGB(0, 0, 0));
    fill_solid(leds2, NUM_LEDS, CRGB(0, 0, 0));
    FastLED.show();
  }

  if(message == "gamestart") {
    game = true;
  }

  if(message == "gamestop") {
    game = false;
  }

  if(topicStr == subTopicStep1) {
    if(message == "1") {
      fill_solid(leds1, NUM_LEDS, CRGB(0, 250, 0));
      FastLED.show();
    } else if(message == "2") {
      Serial.println(message);
      fill_solid(leds1, NUM_LEDS, CRGB(250, 250, 0));
      FastLED.show();
    } else if(message == "3") {
      fill_solid(leds1, NUM_LEDS, CRGB(250, 0, 0));
      FastLED.show();
    }
  }

  if(topicStr == subTopicStep2) {
    if(message == "1") {
      fill_solid(leds2, NUM_LEDS, CRGB(0, 250, 0));
      FastLED.show();
    } else if(message == "2") {
      fill_solid(leds2, NUM_LEDS, CRGB(250, 250, 0));
      FastLED.show();
    } else if(message == "3") {
      fill_solid(leds2, NUM_LEDS, CRGB(250, 0, 0));
      FastLED.show();
    }
  }

  if(topicStr == subTopicSensor1) {
    measureSensor1 = true;
  }

  if(topicStr == subTopicSensor2) {
    measureSensor2 = true;
  }
}

static int gameLoop(struct pt *pt) {
  static unsigned long timestamp = 0;
  PT_BEGIN(pt);

  if(stepQuantity == false and game == false) {
    fill_solid(leds1, NUM_LEDS, CRGB(0, 0, 0));
    fill_solid(leds2, NUM_LEDS, CRGB(0, 0, 0));
    FastLED.show();
  }

  PT_END(pt);
}

static int trapSensor1(struct pt *pt) {
  static unsigned long timestamp = 0;
  PT_BEGIN(pt);

  if(game and measureSensor1) {
    digitalWrite(SENSOR1_TRIG, LOW);
    delayMicroseconds(2);
    digitalWrite(SENSOR1_TRIG, HIGH);
    delayMicroseconds(10);
    digitalWrite(SENSOR1_TRIG, LOW);
    
    duration1 = pulseIn(SENSOR1_ECHO, HIGH);
    distance1 = duration1 * 0.034 / 2;
    
//    Serial.print("Distance: ");
//    Serial.print(distance1);
//    Serial.println(" cm");
  
    if(measurementWhenSteppedOn1 == -1 and (distance1prev != -1 and (distance1prev - distance1 > 20))) {
      client.publish(pubTopicStep1, "{\"stepped\": true}");
      measurementWhenSteppedOn1 = distance1;
    } else {
      if(distance1 < measurementWhenSteppedOn1 + 5) {
        client.publish(pubTopicStep1, "{\"stepped\": true}");
      } else {
        measurementWhenSteppedOn1 = -1;
      }
    }
  
    distance1prev = distance1;
    
    measureSensor1 = false;
  }

  PT_END(pt);
}

static int trapSensor2(struct pt *pt) {
  static unsigned long timestamp = 0;
  PT_BEGIN(pt);

  if(game and measureSensor2) {
    digitalWrite(SENSOR2_TRIG, LOW);
    delayMicroseconds(2);
    digitalWrite(SENSOR2_TRIG, HIGH);
    delayMicroseconds(10);
    digitalWrite(SENSOR2_TRIG, LOW);
    
    duration2 = pulseIn(SENSOR2_ECHO, HIGH);
    distance2 = duration2 * 0.034 / 2;
    
//    Serial.print("Distance: ");
//    Serial.print(distance2);
//    Serial.println(" cm");

    if(measurementWhenSteppedOn2 == -1 and (distance2prev != -1 and (distance2prev - distance2 > 20))) {
      client.publish(pubTopicStep2, "{\"stepped\": true}");
      measurementWhenSteppedOn2 = distance2;
    } else {
      if(distance2 < measurementWhenSteppedOn2 + 5) {
        client.publish(pubTopicStep2, "{\"stepped\": true}");
      } else {
        measurementWhenSteppedOn2 = -1;
      }
    }
  
    distance2prev = distance2;

    measureSensor2 = false;
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
  trapSensor1(&pt2);
  trapSensor2(&pt3);
}
