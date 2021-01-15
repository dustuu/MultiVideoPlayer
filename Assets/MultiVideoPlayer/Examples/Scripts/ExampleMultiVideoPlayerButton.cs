
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Dustuu.VRChat.MultiVideoPlayerSystem.Examples
{
    [RequireComponent(typeof(Button))]
    public class ExampleMultiVideoPlayerButton : UdonSharpBehaviour
    {
        [SerializeField]
        private MultiVideoPlayerManager multiVideoPlayerManager;
        private Button playButton;

        protected void Start() { playButton = GetComponent<Button>(); }

        protected void Update()
        {
            if (multiVideoPlayerManager == null)
            { Debug.LogError("Error: ExampleMultiVideoPlayerButton is missing a reference to a MultiVideoPlayerManager."); }
            else
            { playButton.interactable = multiVideoPlayerManager.AllReady(); }
        }
    }
}