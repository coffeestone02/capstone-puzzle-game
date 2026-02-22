using UnityEngine;
using UnityEngine.Video;

public class HowToPlayManager : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private VideoPlayer videoPlayer;

    public void Open()
    {
        popupRoot.SetActive(true);
        videoPlayer.SetDirectAudioMute(0, true);

        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    public void Close()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        popupRoot.SetActive(false);
    }
}
