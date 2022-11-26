**⚠️ Tinke by @pleonex is deprecated and not maintained anymore ⚠️**

I appreciate the support over the years. It's time to stop and make way for modern solutions.

I started this project to learn to code when I got curiosity about ROM Hacking in NDS. I can't remember how many times I rewrote it while I was learning more programming patterns. It was fun.
As you can imagine the program design and architecture is not that good. Right now, it's hard to extend and especially maintain, it's very time consuming. It would require another major rewrite again.

Several years ago I started a new project, [SceneGate](https://github.com/scenegate), with the same goal but more focus on developers. I hope one day it can become as useful as Tinke was.
You can follow the progress of these new projects from GitHub:

- Ekona, Nintendo DS formats: https://scenegate.github.io/Ekona/
- SceneGate UI: https://github.com/SceneGate/SceneGate

I welcome anyone to fork this project and continue it.
You can check existing forks here: https://github.com/pleonex/tinke/network

---

# Tinke
<p align="center">
<a href="https://travis-ci.org/pleonex/tinke"><img alt="Build Status" src="https://travis-ci.org/pleonex/tinke.svg?branch=master" align="left" /></a>
<a href="https://ci.appveyor.com/project/pleonex/tinke"><img alt="Build Status" src="https://ci.appveyor.com/api/projects/status/0vyjiqofvtiogueh?svg=true" align="left" /></a>
<a href="http://www.gnu.org/copyleft/gpl.html"><img alt="license" src="https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat" /></a>
</p>

> Tinke is a program to see, convert, and edit the **files of NDS games**. You can see a lot of format files like images, text, sounds, fonts and textures. Furthermore it works with **plugins** made in NET Framework languages (C# and VB.NET) so it's so easy to support new formats.

To run the program you must have installed **[.NET Framework 4.5](https://www.microsoft.com/es-es/download/details.aspx?id=30653)** or **[mono](http://www.mono-project.com/download)**. In the case of *mono* on *Linux*, make sure you have installed the package **mono-locale-extras** too. For Mac, you need *mono* and *pkg-config* installed and configured, you'll use *mono32* to exec Tinke.

Thread at GBAtemp.net: http://gbatemp.net/topic/303529-tinke-072/

# Features

 * Show the ROM header with the banner and edit it.
 * Show and convert to common format a lot of files.
 * Edit a lot of image files from BMP files (NCLR, NCGR, NSCR, NCER), audio files from WAV (SWAV, SWAR, STRM) and fonts (NFTR).
 * Hexadecimal visor.
 * Change the content of the files and save the new ROM.
 * Multilanguage support.

# Supported formats

## Images
 * NCLR => Nitro CoLouR(palette)
 * NCGR => Nitro Character Graphic Resource (tiles)
 * NBGR => Nitro Basic Graphic Resource (tiles)
 * NSCR => Nitro Screen Resource (map)
 * NCER => Nitro CEll Resource (cell/puzzle)
 * NANR => Nitro ANimation Resource (animation)
 * CHAR / CHR => CHARacter (tiles)
 * PLT / PAL => PaLeTte (palette)
 * NBFS => Nitro Basic File Screen (map)
 * NBFP => Nitro Basic File Palette (palette)
 * NBFC => Nitro Basic File Character (tiles)
 * NTFT => NiTro File Tiles (tiles)
 * NTFP => NiTro File Palette (palette)
 * RAW => Raw image (tiles)
 * MAP => Raw map info (map)
 * Common formats => PNG, JPG, TGA, GIF

## Textures
 * BTX0 (NSBTX)
 * BMD0 (NSBMD)

## Audio
 * SDAT => Sound DATa
 * SWAV => Sound WAVe
 * SWAR => Sound Wave ARchive
 * STRM => STReaM
 * SADL
 * Common formats => WAV

## Text
 * Sound definition => SADL, XSADL, SARC, SBDL, SMAP.
 * BMG => Pack text file
 * Common formats => TXT, XML, INI, H, BAT, C, MAKEFILE, LUA, CSV, BUILDTIME, HTML, CSS, JS, DTD

## Compression
  Thanks to DSDEcmp library [DSDecmp](http://code.google.com/p/dsdecmp) (credits to *barubary*)
 * Huffman (id = 0x20)
 * LZ77    (id = 0x10)
 * LZSS    (id = 0x11)
 * RLE     (id = 0x30)

## Pack
 * NARC o ARC => Nintendo ARChives
 * Utility.bin => Wifi data files

# Specific plugin for games
 * 999, nine hours nine persons nine doors (BSKE)
 * Itsu Demo Doko Demo Dekiru Igo (AIIJ)
 * Blood of Bahamut (CYJJ)
 * Dragon Ball Kai Ultimate Butouden (TDBJ)
 * Ace Attorney Investigation Miles Edgeworth (C32P, C32J)
 * Gyakuten Kenji 2 (BXOJ)
 * Kirby Squeak Squad (AKWE)
 * Last Window The secrete of Cape West (YLUP)
 * El profesor Layton y la Villa Misteriosa (A5FE, A5FP)
 * El profesor Layton y la Caja de Pandora (YLTS)
 * Maple Story DS (YMPK)
 * Ninokuni Shikkoku no Madoushi (B2KJ)
 * Rune Factory 3 (BRFE, BRFJ)
 * The world end with you (AWLJ)
 * Tetris DS (YLUP)
 * Tokimeki Memorial Girl's Side 3rd Story (B3SJ)
 * Cake Mania 2 (CAKX)
 * Jump! Ultimate Stars: ALAR, DSIG, DSCP
 * Sonic Rush Adventure (ASCx, A3Yx, BXSx)
 * Club Penguin
 * Big Hero 6

----

Link to web pages with NDS info:

 * http://llref.emutalk.net/docs
 * http://problemkaputt.de/gbatek.htm
 * http://sites.google.com/site/kiwids/sdat.html

----

## Compile instructions
* Windows: run compile.bat
* Unix: ./compile.sh

## Screenshots
![Tinke 0.8.1](https://lh5.googleusercontent.com/-GRKvfv-TAaI/ToBy1_eFrfI/AAAAAAAAASA/9WDkc_OQPC4/s800/Tinke%2525200.8.1.PNG)
![Header editor](https://lh5.googleusercontent.com/-W6YUKmyV3JM/ToBzRa0_pwI/AAAAAAAAASI/D7g1JKFvgC8/s400/header%252520editor.PNG)
![ROM Info](https://lh5.googleusercontent.com/_H6ACRUcYPos/TV1ITC1_ceI/AAAAAAAAAG8/cYKNoa3du98/s400/inforom.PNG)

![BTX support](https://lh4.googleusercontent.com/-0Rv5v3JQ0AQ/Tn-J8C1gaxI/AAAAAAAAARg/4HvC4j-5olU/s400/btx.PNG)
![Layton support](https://lh6.googleusercontent.com/_H6ACRUcYPos/TV1IT9DBy8I/AAAAAAAAAHM/ePmPUmTa4w8/s400/ani.PNG)
![Sprite support](https://lh3.googleusercontent.com/-Un-1FO1rlD4/ToB0NvJ03ZI/AAAAAAAAASU/iNdHYvEehBc/s400/ncerV2.PNG)
![Animation support](https://lh3.googleusercontent.com/_H6ACRUcYPos/TV8C0RtGTzI/AAAAAAAAAHk/wO9ps1DP-EU/s400/nanr.PNG)
![Font support](https://lh6.googleusercontent.com/-pSP4NY3Y9Rw/TqPSrsRc6eI/AAAAAAAAAUg/-QjuDfRdQc4/s400/nftr-2.PNG)
![Sound support](https://lh4.googleusercontent.com/-VSJCC9q9TPQ/TmlKbnvgTaI/AAAAAAAAAOg/s7DFYgpeo3c/s400/sdat.PNG)

![Layton 1](https://lh3.googleusercontent.com/_H6ACRUcYPos/TV1ITRjI1WI/AAAAAAAAAHE/aClaJQdH7xU/s144/imgs2.PNG)
![Layton 2](https://lh6.googleusercontent.com/_H6ACRUcYPos/TV1ITJsYn5I/AAAAAAAAAHA/yAz7oiEKOa4/s144/imgs1.PNG)
![BMG support](https://lh4.googleusercontent.com/_H6ACRUcYPos/TV1IYiOYTOI/AAAAAAAAAHQ/Vdf4K030mdU/s144/text.PNG)
