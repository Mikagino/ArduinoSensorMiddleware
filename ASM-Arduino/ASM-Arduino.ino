int sensorReading = 10;

void setup() {
  Serial.begin(9600);
}

void loop() {
  Serial.println(sensorReading);
}
