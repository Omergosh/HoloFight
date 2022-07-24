using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMusicScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] battleMusicPlaylist;

    public int currentSelectedSong = -1; // -1 means no song is selected.
                                         // Otherwise, value is index of selected song in playlist.

    // Start is called before the first frame update
    void Start()
    {
        if (battleMusicPlaylist.Length > 0)
        {
            Debug.Log("MUSIC PRESENT");
            PlayRandomSong();
        }
    }

    void PlayRandomSong()
    {
        int newSongIndex = -1;
        newSongIndex = Random.Range(0, battleMusicPlaylist.Length);

        audioSource.clip = battleMusicPlaylist[newSongIndex];
        audioSource.Play();
        currentSelectedSong = newSongIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying)
        {
            PlayRandomSong();
        }
    }
}
