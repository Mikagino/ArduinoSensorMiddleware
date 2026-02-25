int sensorReading = 10;
int intervalMillis = 100;

void setup() {
  Serial.begin(9600);
}

void loop() {
  Serial.println(ReadSensorData());
  Delay(intervalMillis);
}

float ReadSensorData() {
  return sensorReading;
}
