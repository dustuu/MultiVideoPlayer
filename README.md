# MultiVideoPlayer
A VRChat UdonSharp tool to play multiple videos at the same time.

## Features
- Allows multiple videos to be played at the same time safely without causing YouTube rate limiting errors

## Installation
1. Install the latest VRCSDK and latest release of [UdonSharp](https://github.com/MerlinVR/UdonSharp/releases/latest)
2. Install the [latest](https://github.com/dustuu/MultiVideoPlayer/releases) release

## Configuration
1. Drag the MultiVideoPlayerManager prefab into your scene
2. Drag a MultiVideoPlayer prefab in to your scene as a child of the MultiVideoPlayerManager  object for each video you want to play.
3. Set the URL to your desired video on each MultiVideoPlayer component. Make sure you set the URL field of the MultiVideoPlayer component, not the VRCAVProVideoPlayer component.
4. If you want the videos to auto-play as soon as they are ready, set the "Auto Play" field to true on the MultiVideoPlayerManager component.
5. If you want the videdos to play after a button is hit, configure a button to call the "PlayAll" function on the MultiVideoPlayerManager component via SendCustomEvent. You can find a pre-configured example of this at "Assets/MultiVideoPlayer/Examples/Prefabs/ExampleMultiVideoPlayerSystem.prefab".

## Notes
- All VRCAVProVideoPlayer components in the scene must have AutoPlay set to False
- You can adjust how much time is waited between video loads by modifying the "Time Delay Between Load Requests" field on the MultiVideoPlayerManager component.
- If you call "PlayAll" again after the videos are already playing, it will reset all videos back to 0:00 and play them from the beginning again.
