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
    [SerializeField] private AudioClip bombBought;
    [SerializeField] private AudioClip flagBought;
    [SerializeField] private AudioClip bombExplode;
    [SerializeField] private AudioClip soldierSold;
    [SerializeField] private AudioClip zombieWalk;
    [SerializeField] private AudioClip zombieWalkShort;
    [SerializeField] private AudioClip zombieDie;
    [SerializeField] private AudioClip zombieAttack;
    [SerializeField] private AudioClip zombieCancelBomb;
    [SerializeField] private AudioClip clockTickOne;
    [SerializeField] private AudioClip clockTickTwo;
    [SerializeField] private AudioClip description;

    [SerializeField] private AudioClip inGameMusic;


    public AudioClip ButtonPress { get { return buttonPress; } }
    public AudioClip SinglePlayerWin { get { return singlePlayerWin; } }
    public AudioClip SinglePlayerLose { get { return singlePlayerLose; } }
    public AudioClip ZombieBought { get { return zombieBought; } }
    public AudioClip BombBought { get { return bombBought; } }
    public AudioClip FlagBought { get { return flagBought; } }
    public AudioClip InGameMusic { get { return inGameMusic; } }
    public AudioClip BombExplode { get { return bombExplode; } }
    public AudioClip SoldierSold { get { return soldierSold; } }
    public AudioClip ZombieWalk { get { return zombieWalk; } }
    public AudioClip ZombieWalkShort { get { return zombieWalkShort; } }
    public AudioClip ZombieDie { get { return zombieDie; } }
    public AudioClip ZombieAttack { get { return zombieAttack; } }
    public AudioClip ZombieCancelBomb { get { return zombieCancelBomb; } }
    public AudioClip ClockTickOne { get { return clockTickOne; } }
    public AudioClip ClockTickTwo { get { return clockTickTwo; } }
    public AudioClip Description { get { return description; } }


    private void Awake() {
        DontDestroyOnLoad(this);
        AudioSource[] audioSources = GetComponents<AudioSource>();
        music = audioSources[0];
        sfx = audioSources[1];
    }

}