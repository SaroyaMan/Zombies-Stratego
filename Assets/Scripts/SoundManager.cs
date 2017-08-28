using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

    private AudioSource music;
    private AudioSource sfx;

    public AudioSource Music { get { return music; } }
    public AudioSource SFX { get { return sfx; } }


    [SerializeField] private AudioClip buttonPress;
    //[SerializeField] private AudioClip death;
    //[SerializeField] private AudioClip fireball;
    //[SerializeField] private AudioClip gameover;
    //[SerializeField] private AudioClip hit;
    //[SerializeField] private AudioClip level;
    //[SerializeField] private AudioClip newgame;
    //[SerializeField] private AudioClip rock;
    //[SerializeField] private AudioClip towerbuilt;

    public AudioClip ButtonPress { get { return buttonPress; } }
    //public AudioClip Death { get { return death; } }
    //public AudioClip Fireball { get { return fireball; } }
    //public AudioClip Gameover { get { return gameover; } }
    //public AudioClip Hit { get { return hit; } }
    //public AudioClip Level { get { return level; } }
    //public AudioClip Newgame { get { return newgame; } }
    //public AudioClip Rock { get { return rock; } }
    //public AudioClip Towerbuilt { get { return towerbuilt; } }

    private void Awake() {
        DontDestroyOnLoad(this);
        AudioSource[] audioSources = GetComponents<AudioSource>();
        music = audioSources[0];
        sfx = audioSources[1];
    }

}
