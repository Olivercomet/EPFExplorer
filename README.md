A tool for extracting/modifying the files of the Elite Penguin Force DS games.

### Current features:

- Opening and editing .ARC archives (Both EPF and HR)
- Opening and editing .RDT resource data files (EPF. HR is viewing/exporting only, and lacks filenames)
- Opening and editing TSB/MPB tiled images. (Both EPF and HR)
- Exporting and reimporting LUA scripts.
- Editing save files. (Both EPF and HR)
- Dumping audio clips to .wav (ADPCM format wav, openable if FFMPEG is installed)
- Exporting and replacing music tracks in .XM format (you will then need to convert these to midi with OpenMPT or similar to actually listen to them, as the XM files have no sample data.)

The music replacing also currently has issues with some tracks (e.g. Dojo and Gadget Room, among others)

### With thanks to
- [Fireboyd78](https://github.com/Fireboyd78) for creating a [C# port of unluac](https://github.com/Fireboyd78/UnluacNET) (originally by [tehtmi](https://sourceforge.net/projects/unluac/))
- [No$GBA](https://www.nogba.com/) for its invaluable debugging tools
- [DSDecmp](https://github.com/barubary/dsdecmp) and [Ekona](https://github.com/SceneGate/Ekona) for their LZ10/11 compression features
