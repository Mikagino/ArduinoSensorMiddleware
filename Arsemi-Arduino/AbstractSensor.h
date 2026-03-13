#ifndef ABSTRACT_SENSOR_H
#define ABSTRACT_SENSOR_H

// Abstraction for setting up multiple sensors in a big array
// Polymorphism for more sophisticated sensors
class AbstractSensor {
protected:
    unsigned int _intervalMillis = 100;     // interval in which new sensor data is sent over serial
    unsigned long _lastReadMillis = 0;  // stores the last time the sensor was read

    
public:
    float _lastValue;
    virtual bool begin() = 0;
    inline bool checkInterval();
    // Checks interval and updates+sends sensor readings
    virtual bool update();
    virtual void updateLastValue() = 0;
};

#endif
