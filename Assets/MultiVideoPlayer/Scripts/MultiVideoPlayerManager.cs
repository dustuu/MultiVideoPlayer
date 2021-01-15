
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
        // Whether or not the MultiVideoPlayerManager will automatically start playing all the videos once they are all ready
        [SerializeField]
        private bool autoPlay = false;
        // Variables
        private bool autoPlayStarted = false;
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
            if (autoPlay && !autoPlayStarted && IsReady())
            {
                PlayAll();
                autoPlayStarted = true;
            }
        }

        // Call this method from a Button via SendCustomEvent
        public void PlayAll()
        {
            // Init Check
            if (!initComplete) { return; }

            if (IsReady()) { foreach (MultiVideoPlayer multiVideoPlayer in multiVideoPlayers) { multiVideoPlayer.Play(); } }
            else { Debug.LogError("Couldn't play videos until all children are finished loading."); }
        }

        // The time at which the final video load request will be made (the final video may not finish loading until shortly after this time)
        private double GetFinalLoadTime() { return timeScriptStart + (timeDelayBetweenLoadRequests * (multiVideoPlayers.Length - 1)); }

        public bool IsReady()
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
    }
}