using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Class to manage + persist audio levels across scenes.
    // (attached to the GameManager object)

    [SerializeReference]
    MusicManager musicManager;
    [SerializeReference]
    SFXManager sfxManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(string soundTag)
    {
        sfxManager.PlaySound(soundTag);
    }

    public void PlayTitleScreenMusic()
    {
        musicManager.ChangePlaylist("TitleScene");
        musicManager.PlayRandomSong();
    }

    internal void SceneTransitionStart(string newSceneName)
    {
        // Start fading out music
        musicManager.StopTheMusic();
        musicManager.ChangePlaylist(newSceneName);
    }

    internal void SceneTransitionMiddle(string newSceneName)
    {
        // Queue up next track or w/e based on available data
        //musicManager.ChangePlaylist(newSceneName);

        // actually, just start playing the music now.
        musicManager.PlayRandomSong();
    }

    internal void SceneTransitionEnd(string nextScene)
    {
        // Start playing new music
        //musicManager.PlayRandomSong();
    }
}
