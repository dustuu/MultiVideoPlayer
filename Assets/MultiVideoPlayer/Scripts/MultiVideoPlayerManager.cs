
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Dustuu.VRChat.MultiVideoPlayerSystem
{
    public class MultiVideoPlayerManager : UdonSharpBehaviour
    {
        // Init
        private bool initComplete = false;
        // Serialized Variables
        // The amount of seconds that the MultiVideoPlayerManager will wait between requesting videos from YouTube
        // A higher delay is less likely to trigger a rate limiting error from YouTube, 10 seconds is recommended to be safe
        [SerializeField]
        private double timeDelayBetweenLoadRequests = 10;
        [SerializeField]
        private bool waitForAll = false;
        // Whether or not the MultiVideoPlayerManager will automatically start playing all the videos once they are all ready
        [SerializeField]
        private bool autoPlay = false;
        // Variables
        private bool autoPlayCompleted = false;
        private double timeScriptStart;
        private MultiVideoPlayer[] multiVideoPlayers;

        protected void Start() { Init(); }

        private void Init()
        {
            // Init Check
            if (initComplete) { return; }
            // Variables
            timeScriptStart = Networking.GetServerTimeInSeconds();
            multiVideoPlayers = GetComponentsInChildren<MultiVideoPlayer>();
            foreach (MultiVideoPlayer multiVideoPlayer in multiVideoPlayers) { multiVideoPlayer.Init(); }
            // Init Finalize
            initComplete = true;
        }

        protected void Update()
        {
            // Init Check
            if (!initComplete) { return; }

            // Load videos on child MultiVideoPlayers as the time that they can be safely loaded is reached
            double timeCurrent = Networking.GetServerTimeInSeconds();
            for (int i = 0; i < multiVideoPlayers.Length; i++)
            {
                double timeLoad = timeScriptStart + (timeDelayBetweenLoadRequests * i);
                if (timeCurrent >= timeLoad) { multiVideoPlayers[i].LoadURL(); }
            }

            // If autoPlay is enabled, play the videos as soon as they are ready
            if (autoPlay && !autoPlayCompleted)
            {
                if (waitForAll)
                {
                    if (AllReady())
                    {
                        PlayAll();
                        autoPlayCompleted = true;
                    }
                }
                else
                {
                    float timeSinceStart = (float)(timeCurrent - timeScriptStart);
                    PlayAllAtTime(timeSinceStart, false);
                    if (AllReady())
                    {
                        autoPlayCompleted = true;
                    }
                }
            }
        }

        // Call this method from a Button via SendCustomEvent
        public void PlayAll()
        {
            // Init Check
            if (!initComplete) { return; }

            if (waitForAll && !AllReady())
            {
                Debug.LogError("When 'Wait For All' is enabled, PlayAll() must be called after all videos finish loading!");
                return;
            }
            PlayAllAtTime(0, true);
        }

        private void PlayAllAtTime(float time, bool resetPlayingVideos)
        {
            foreach (MultiVideoPlayer multiVideoPlayer in multiVideoPlayers)
            {
                bool shouldPlay = true;
                // If it's not ready, don't play it
                if ( !multiVideoPlayer.IsReady()) { shouldPlay = false; }
                // If we don't want to reset playing videos, and it is playing, don't play
                if ( !resetPlayingVideos && multiVideoPlayer.IsPlaying()) { shouldPlay = false; }

                if (shouldPlay)
                {
                    multiVideoPlayer.SetTime(time);
                    multiVideoPlayer.Play();
                }
            }
        }

        // The time at which the final video load request will be made (the final video may not finish loading until shortly after this time)
        private double GetFinalLoadTime() { return timeScriptStart + (timeDelayBetweenLoadRequests * (multiVideoPlayers.Length - 1)); }

        public bool AllReady()
        {
            // Init Check
            if (!initComplete) { return false; }

            // If the final video load request has not yet been made, the MultiVideoPlayerManager is not ready
            double timeCurrent = Networking.GetServerTimeInSeconds();
            if (timeCurrent < GetFinalLoadTime()) { return false; }

            // If any of the child MultiVideoPlayers are not ready, the MultiVideoPlayerManager is not ready
            foreach (MultiVideoPlayer multiVideoPlayer in multiVideoPlayers) { if (!multiVideoPlayer.IsReady()) { return false; } }
            return true;
        }

        public bool WaitForAllEnabled() { return waitForAll; }
        public bool AutoPlayEnabled() { return autoPlay; }
    }
}