A tool for extracting/modifying the files of the Elite Penguin Force DS games.

### Current features:

- Opening and editing .ARC archives (Both EPF and HR)
- Opening and editing .RDT resource data files (EPF. HR is viewing/exporting only.)
- Opening and editing TSB/MPB tiled images. (Both EPF and HR)
- Exporting and reimporting Lua scripts.
- Editing save files. (Both EPF and HR)
- Dumping audio clips to .wav (PCM or ADPCM format wav, openable if FFMPEG is installed)
- Exporting and replacing music tracks in .XM format, with all sample data
- Opening and editing the game's DLC mission (download.arc)

The music replacing also currently has issues with some tracks (e.g. Dojo and Gadget Room, among others)

### With thanks to
- [Fireboyd78](https://github.com/Fireboyd78) for creating a [C# port of unluac](https://github.com/Fireboyd78/UnluacNET) (originally by [tehtmi](https://sourceforge.net/projects/unluac/))
- [No$GBA](http://problemkaputt.de/gba.htm) for its invaluable debugging tools
- [DSDecmp](https://github.com/barubary/dsdecmp) for its LZ10/11 compression features
- [MCJack123](https://github.com/MCJack123) for their contributions regarding instruments and samples, and PCM audio.
- [Hifss](https://github.com/Ciorro/Hifss)