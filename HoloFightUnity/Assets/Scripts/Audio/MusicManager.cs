using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;
    public AudioClip[] titleScreenPlaylist;
    public AudioClip[] mainMenuPlaylist;
    public AudioClip[] charSelectPlaylist;

    public AudioClip[] currentPlaylist;

    public int currentSongIndex = -1; // -1 means no song is selected.
                                         // Otherwise, value is index of selected song in playlist.

    [SerializeField]
    float songEndBuffer = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.loop)
        {
            //if(!audioSource.isPlaying)
            if (audioSource.time + songEndBuffer >= currentPlaylist[currentSongIndex].length)
            {
                PlayRandomSong();
            }
        }
    }

    internal void StopTheMusic()
    {
        audioSource.Stop();
    }

    public void ChangePlaylist(string playlistParameter)
    {
        switch (playlistParameter)
        {
            case "TitleScene":
                currentPlaylist = titleScreenPlaylist;
                audioSource.loop = true;
                break;
            case "MainMenuScene":
                currentPlaylist = mainMenuPlaylist;
                audioSource.loop = true;
                break;
            case "CharacterSelectScene":
                currentPlaylist = charSelectPlaylist;
                break;
            default:
                Debug.Log($"Invalid playlist parameter {playlistParameter}. No playlist found matching that description.");
                currentPlaylist = null;
                currentSongIndex = -1;
                break;
        }

        if (currentPlaylist != null)
        {
            if (currentPlaylist.Length == 1)
            {
                audioSource.loop = true;
            }
            else
            {
                audioSource.loop = false;
            }
        }
    }

    public void PlayRandomSong()
    {
        if (currentPlaylist != null)
        {
            int newSongIndex = Random.Range(0, currentPlaylist.Length);

            audioSource.clip = currentPlaylist[newSongIndex];
            audioSource.time = 1f;
            audioSource.Play();
            currentSongIndex = newSongIndex;
        }
    }
}
