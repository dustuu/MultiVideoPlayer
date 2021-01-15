
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Components.Base;
using VRC.SDKBase;
using VRC.Udon;

namespace Dustuu.VRChat.MultiVideoPlayerSystem
{
    [RequireComponent(typeof(VRCAVProVideoPlayer))]
    public class MultiVideoPlayer : UdonSharpBehaviour
    {
        // Init
        private bool initComplete = false;
        // Serialized Variables
        [SerializeField] private VRCUrl url;
        // Variables
        private BaseVRCVideoPlayer baseVRCVideoPlayer;
        private bool loaded;

        public void Init()
        {
            // Init Check
            if (initComplete) { return; }
            // Variables
            baseVRCVideoPlayer = (BaseVRCVideoPlayer)GetComponent(typeof(BaseVRCVideoPlayer));
            // GetBaseVRCVideoPlayer().Loop = false;
            // TODO: Add checks to ensure VideoURL is empty and AutoPlay is false
            // This is currently impossible because casting from BaseVRCVideoPlayer to VRCAVProVideoPlayer is not supported
            /*
            VRCAVProVideoPlayer vrcAvProVideoPlayer = (VRCAVProVideoPlayer)GetBaseVRCVideoPlayer();
            if (!string.IsNullOrEmpty(vrcAvProVideoPlayer.VideoURL.Get()))
            { Debug.LogError("Error: URL field must be empty on all VRCAVProVideoPlayer components."); }
            if (vrcAvProVideoPlayer.AutoPlay)
            { Debug.LogError("Error: AutoPlay must be disabled on all VRCAVProVideoPlayer components."); }
            */
            loaded = false;
            // Init Finalize
            initComplete = true;
            Debug.Log("Completed Init MultiVideoPlayer");
        }

        public void LoadURL()
        {
            // Init Check
            if (!initComplete)
            {
                Debug.LogError("Attempted to call LoadURL() on MultiVideoPlayer that had not completed its Init()");
                return;
            }

            if (!loaded)
            {
                Debug.Log("Requesting Load URL: " + GetURLString());
                GetBaseVRCVideoPlayer().LoadURL(url);
                loaded = true;
            }
        }

        public void Play()
        {
            // Init Check
            if (!initComplete)
            {
                Debug.LogError("Attempted to call Play() on MultiVideoPlayer that had not completed its Init()");
                return;
            }

            if (GetBaseVRCVideoPlayer().IsReady)
            {
                Debug.Log("Requesting Play URL: " + GetURLString());
                if (GetBaseVRCVideoPlayer().IsPlaying) { GetBaseVRCVideoPlayer().SetTime(0); }
                else { GetBaseVRCVideoPlayer().Play(); }
            }
        }

        public bool IsReady()
        {
            // Init Check
            if (!initComplete)
            {
                Debug.LogError("Attempted to call IsReady() on MultiVideoPlayer that had not completed its Init()");
                return false;
            }

            return GetBaseVRCVideoPlayer().IsReady;
        }

        public bool IsPlaying()
        {
            // Init Check
            if (!initComplete)
            {
                Debug.LogError("Attempted to call IsPlaying() on MultiVideoPlayer that had not completed its Init()");
                return false;
            }

            return GetBaseVRCVideoPlayer().IsPlaying;
        }

        public void SetTime(float time)
        {
            // Init Check
            if (!initComplete)
            {
                Debug.LogError("Attempted to call SetTime() on MultiVideoPlayer that had not completed its Init()");
                return;
            }

            GetBaseVRCVideoPlayer().SetTime(time);
        }

        public string GetURLString() { return url.Get(); }
        private BaseVRCVideoPlayer GetBaseVRCVideoPlayer() { return baseVRCVideoPlayer; }
    }
}