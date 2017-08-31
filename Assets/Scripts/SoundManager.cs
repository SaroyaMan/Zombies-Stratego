using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

    private AudioSource music;
    private AudioSource sfx;

    public AudioSource Music { get { return music; } }
    public AudioSource SFX { get { return sfx; } }


    [SerializeField] private AudioClip buttonPress;
    [SerializeField] private AudioClip singlePlayerWin;
    [SerializeField] private AudioClip singlePlayerLose;
    [SerializeField] private AudioClip zombieBought;

    public AudioClip ButtonPress { get { return buttonPress; } }
    public AudioClip SinglePlayerWin { get { return singlePlayerWin; } }
    public AudioClip SinglePlayerLose { get { return singlePlayerLose; } }
    public AudioClip ZombieBought { get { return zombieBought; } }


    private void Awake() {
        DontDestroyOnLoad(this);
        AudioSource[] audioSources = GetComponents<AudioSource>();
        music = audioSources[0];
        sfx = audioSources[1];
    }

}