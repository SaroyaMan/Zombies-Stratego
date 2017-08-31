using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

    private AudioSource music;
    private AudioSource sfx;

    public AudioSource Music { get { return music; } }
    public AudioSource SFX { get { return sfx; } }


    [SerializeField] private AudioClip buttonPress;
    [SerializeField] private AudioClip singlePlayerWin;
    [SerializeField] private AudioClip singlePlayerLose;

    public AudioClip ButtonPress { get { return buttonPress; } }
    public AudioClip SinglePlayerWin { get { return singlePlayerWin; } }
    public AudioClip SinglePlayerLose { get { return singlePlayerLose; } }


    private void Awake() {
        DontDestroyOnLoad(this);
        AudioSource[] audioSources = GetComponents<AudioSource>();
        music = audioSources[0];
        sfx = audioSources[1];
    }

}