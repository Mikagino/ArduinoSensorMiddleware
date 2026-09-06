# Arduino Sensor Middleware (alt. "Arsemi" / "ArSeMi")

Arsemi is a middleware framework developed in C# for my bachelor's thesis. Many generic sensors (analog, digital) can be accessed with only minimal setup.

This repository contains the interface to the microcontroller from the PC in the folder Arsemi-Core, the microcontroller code in the Arsemi-Arduino folder.

**DISCLAIMER:** Arsemi is still a really rough prototype, based on my bachelor's thesis and there are still bugs and planned features.

## Usage
To learn about the usage of Arsemi, visit the [Wiki](https://github.com/Mikagino/ArduinoSensorMiddleware/wiki) or check out the Example folder.

## External Assets
- [Icon](https://www.vecteezy.com/free-vector/burning-heart-symbol)

## To-Dos
- [ ] logging feature (write acquired data into file in csv format)
- [ ] calculate heartrate variability
- [ ] godot plugin, with a setup file which will be loaded on the click of a button

### Maybe
- [ ] add key to handshake? for more reliable crc8?
- [ ] rearrange error/action categories
- [ ] discard last digit of interval (1er stelle) and only send the rest (10-2550) -> alt: bitshift um 4 bit (16-4080) maybe actually not, makes more problems, just sample more xD
- [ ] Discord Server
