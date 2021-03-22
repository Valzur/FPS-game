using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    private string songName;
    [SerializeField]
    private Text SongNameText;
    private string NowPlayingPrefix = "Now Playing: ";

    void Start(){
        SwitchSong();
    }

    public void SwitchSong(){
        songName = audioSource.clip.name;
        SongNameText.text = NowPlayingPrefix + songName;
    }
}
