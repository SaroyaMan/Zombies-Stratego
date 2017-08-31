using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private Slider moneySlider, musicSlider, sfxSlider;
    private SoundManager soundManager;

    private void Start() {
        musicSlider = Globals.Instance.UnityObjects["MusicSlider"].GetComponent<Slider>();
        sfxSlider = Globals.Instance.UnityObjects["SfxSlider"].GetComponent<Slider>();

        soundManager = SoundManager.Instance;

        musicSlider.value = PlayerPrefs.GetFloat("Music", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1);
    }

    public void Options() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        GameManager.Instance.ChangeGameState(GameScreens.Options);
    }

    public void Back() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        GameManager.Instance.GoBack();
    }

    public void MoveSfxSlider() {
        GameManager.Instance.UpdateSfxVolume(sfxSlider.value);
    }

    public void MoveMusicSlider() {
        GameManager.Instance.UpdateMusicVolume(musicSlider.value);
    }

    public void GamePause() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        GameManager.Instance.ChangeGameState(GameScreens.Pause);
    }

    public void ResumeGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        GameManager.Instance.ChangeGameState(GameScreens.InGame);
    }

    public void ResetGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        GameManager.Instance.ResetMatch();
    }

    public void QuitGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        GameManager.Instance.QuitGame();
    }
}


