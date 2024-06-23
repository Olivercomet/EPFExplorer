A tool for extracting/modifying the files of First Playable Productions DS games; mainly designed for EPF and Herbert's Revenge, but also compatible with some others, such as Ben 10: Protector of Earth.

The compatibility with post-2010 games slowly decreases. This is because many of the archives after this date are just binary blobs slowly mutating into new layouts.

### Current features:

- Opening and editing .ARC archives
- Opening and editing .RDT resource data files (EPF. HR is viewing/exporting only.)
   - Exporting sprites to PNG sequence, GIF, or (for folders) sprite sheets.
- Editing TSB/MPB and NBFC/NBFS/NBFP tiled images.
- Exporting and reimporting Lua scripts.
- Editing save files for EPF and HR.
- Exporting audio clips to .wav (PCM or ADPCM format wav, openable if FFMPEG is installed)
- Exporting music tracks in .XM format, with all sample data
- Opening and editing EPF's DLC mission (download.arc)

The custom missions 'The Lost Signal' and 'The Way Home' can be found in the 'Useful stuff' folder of this repository.

### With thanks to
- [Fireboyd78](https://github.com/Fireboyd78) for creating a [C# port of unluac](https://github.com/Fireboyd78/UnluacNET) (originally by [tehtmi](https://sourceforge.net/projects/unluac/))
- [No$GBA](http://problemkaputt.de/gba.htm) for its invaluable debugging tools
- [DSDecmp](https://github.com/barubary/dsdecmp) for its LZ10/11 compression features
- [MCJack123](https://github.com/MCJack123) for their contributions regarding instruments and samples, and PCM audio.
- [NGif](https://sourceforge.net/projects/ngif/) for its GIF creation library.
