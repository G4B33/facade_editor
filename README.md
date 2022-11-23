# Façade Editor
Randomizer, corruptor, and more for the game named Façade
![form](./screenshots/form.png)
## What does it do?

- Randomize sounds, textures, cursors, animations
- Replace(mix in) your own sound files (supports automatic downsampling and mp3, ogg, wav files) 
- Decompile .bin, .map, .rul files (All of them are written in Java Jess rule language)
- Enable never seen before(?) built in debug features like "Drama manager monitor" or "AI Log" and more
![dmm](./screenshots/dmm.png)
![console](./screenshots/console.png)

## Download

[Here](https://github.com/G4B33/facade_editor/releases)

## Plans on future releases

- Subtitle editor (The basics are done, I can read and replace it, just need to assign the sound files to their corresponding subtitles, and make some kind of editor that's easy to use)
- Lower Windows requirement to XP (Currently supports Windows 7 x86,x64 and up because of .Net 4.7.2 requirement for NAudio library)
- Face manipulator (Don't count on this one, I'm not 100% sure that it will work)

## How to build

- Open facade_editor.sln in Visual Studio
- Open up NuGet Package Manager Console (Tools->NuGet Package Manager->Package Manager Console)
- Paste these 2 commands:
- NuGet\Install-Package NAudio -Version 2.1.0
- NuGet\Install-Package NAudio.Vorbis -Version 1.5.0
- Press F6

If NuGet didn't work because it's missing default package source(VS 2022 bug?), go to Tools->Options->NuGet Package Manager->Package Sources, add this package source:
- Name: nuget.org
- Source: https://api.nuget.org/v3/index.json

## License

This is licensed under GNU GPLv3.
