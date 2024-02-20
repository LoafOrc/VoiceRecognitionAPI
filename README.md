# VoiceRecognitionAPI
Loads Windows Speech recognition into Lethal Company and provides a dev-friendly way of using it.

## What does this do for players?
In short - nothing. This is a simple API, instead other mods use methods from this mod to build their own features. So go check their feature list to see what voice commands they may offer.

## It doesn't work properly help!
Head over to the [troubleshooting page](https://github.com/LoafOrc/VoiceRecognitionAPI/wiki/Troubleshooting) for common issues.

## Mod Implementation
If your a developer and want to implement VoiceRecognitionAPI go to the [For Developers wiki page](https://github.com/LoafOrc/VoiceRecognitionAPI/wiki/For-Developers) to find out how.

## Contributing
Uhhh so this is my first time really trying to do a public Github repo so bare with me if I screw something up.

If you'd like to build the mod for yourself:
 - Clone the repo
 - Create a .csproj.user file in the root containing:
```xml
<Project>
  <PropertyGroup>
    <!--This should be the path to the folder that contains `LethalCompany.exe` usually people have it on the C: drive but incase not, change it here-->
    <LethalCompanyPath>C:\Program Files (x86)\Steam\steamapps\common\Lethal Company</LethalCompanyPath> 
  </PropertyGroup>
</Project>
```
 - Then contribute I guess.
 - Build like you would a normal mod.
