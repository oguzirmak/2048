2048 App Icon Set
=================

Copy the PNG files to Assets/Icons in the Unity project.

Windows Player Settings > Icon:
- Enable Override for Windows, Mac, Linux.
- Assign AppIcon_Windows_1024.png to every requested icon size.

Android Player Settings > Icon:
- Legacy: AppIcon_Android_Legacy_1024.png
- Round: AppIcon_Android_Round_1024.png
- Adaptive Foreground: AppIcon_Android_Adaptive_Foreground_1024.png
- Adaptive Background: AppIcon_Android_Adaptive_Background_1024.png
- Reuse each 1024 image for every requested size; Unity resizes it during build.

Recommended texture import settings:
- Texture Type: Default
- Alpha Is Transparency: enabled for Windows and Adaptive Foreground
- Max Size: 1024
- Compression: High Quality

Palette: #FAF8EF, #BBADA0, #EDC22E, #F9F6F2
