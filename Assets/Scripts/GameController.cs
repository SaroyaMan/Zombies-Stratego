using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private Slider moneySlider, musicSlider, sfxSlider;
    private SoundManager soundManager;

    private void Start() {
        moneySlider = Globals.Instance.UnityObjects["MoneySlider"].GetComponent<Slider>();
        musicSlider = Globals.Instance.UnityObjects["MusicSlider"].GetComponent<Slider>();
        sfxSlider = Globals.Instance.UnityObjects["SfxSlider"].GetComponent<Slider>();
        soundManager = SoundManager.Instance;
    }


    public void ZombieSelectedInEditMode(ZombieBtn zombieSelected) {
        ZombieManager.Instance.SelectedZombie(zombieSelected);
    }

    /* Main Menu Controls */
    public void StartSingleGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.Loading);
    }

    public void StartMultiplayerGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.Loading);
    }

    public void Multiplayer() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.MultiPlayer);
    }

    public void StudentInfo() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.StudentInfo);
    }

    public void Options() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.Options);
    }

    public void Back() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.GoBack();
    }

    public void MoveSfxSlider() {
        MenuLogic.Instance.UpdateSfxVolume(sfxSlider.value);
    }

    public void MoveMusicSlider() {
        MenuLogic.Instance.UpdateMusicVolume(musicSlider.value);
    }

    public void MoveMoneySlider() {
        MenuLogic.Instance.UpdateMoneySliderTxt(moneySlider.value);
    }

    public void StartGithub() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.OpenGithub();
    }

    public void StartCV() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.OpenCV();
    }
}