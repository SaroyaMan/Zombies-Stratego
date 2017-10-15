using UnityEngine;
using UnityEngine.UI;

public class MenuController: MonoBehaviour {

    private Slider moneySlider, musicSlider, sfxSlider;
    private SoundManager soundManager;
    private InputField usernameField;

    private void Start() {
        moneySlider = Globals.Instance.UnityObjects["MoneySlider"].GetComponent<Slider>();
        musicSlider = Globals.Instance.UnityObjects["MusicSlider"].GetComponent<Slider>();
        sfxSlider = Globals.Instance.UnityObjects["SfxSlider"].GetComponent<Slider>();
        usernameField = Globals.Instance.UnityObjects["Input_Username"].GetComponent<InputField>();

        soundManager = SoundManager.Instance;

        moneySlider.value = PlayerPrefs.GetInt("MoneyBet", 2);
        musicSlider.value = PlayerPrefs.GetFloat("Music", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1);
        usernameField.text = PlayerPrefs.GetString("Username", "No-Name");
    }


    public void PlayerSelectedInEditMode(SoldierBtn soldierSelected) {
        StrategyEditor.Instance.SelectedSoldier(soldierSelected);
    }

    public void SaveChangesInEditMode() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.Main);
        MenuLogic.Instance.SaveStrategy();
    }

    public void StartSingleGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.StartGame(true);
    }

    public void StartMultiplayerGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.StartGame(false);
    }

    public void Multiplayer() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.MultiPlayer);
    }

    public void EditMode() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.Edit);
    }

    public void StudentInfo() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.StudentInfo);
    }

    public void Options() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ChangeMenuState(MenuScreens.Options);
    }

    public void QuitGame() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.QuitGame();
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

    public void ChangeUsername() {
        MenuLogic.Instance.UpdateUsername(usernameField.text);
    }

    public void CancelConnection() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MultiPlayerManager.Instance.Disconnect();
    }

    public void ConfirmError() {
        soundManager.SFX.PlayOneShot(soundManager.ButtonPress);
        MenuLogic.Instance.ConfirmError();
    }
}