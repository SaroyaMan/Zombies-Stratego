using UnityEngine;
using UnityEngine.UI;

public class MenuLogic: Singleton<MenuLogic> {

    private Text moneyTextLbl;
    private AudioSource audioSource;
    private MenuScreens currentScreen;
    private MenuScreens prevScreen;

    private void Start() {
        Init();
    }

    private void Init() {
        prevScreen = currentScreen = MenuScreens.Default;
        moneyTextLbl = Globals.Instance.UnityObjects["MoneyLbl"].GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        ShutdownScreens();
        ChangeMenuState(MenuScreens.Main);
    }

    private void ShutdownScreens() {
        var unityObjects = Globals.Instance.UnityObjects;
        unityObjects["ScreenMenu"].SetActive(false);
        unityObjects["ScreenLoading"].SetActive(false);
        unityObjects["ScreenOptions"].SetActive(false);
        unityObjects["ScreenStudentInfo"].SetActive(false);
        unityObjects["ScreenMultiplayer"].SetActive(false);
        unityObjects["ScreenEdit"].SetActive(false);
        //unityObjects["Map"].SetActive(false);
    }

    public void GoBack() {
        if(prevScreen != MenuScreens.Main && currentScreen != MenuScreens.Options) {
            prevScreen = MenuScreens.Main;
        }
        ChangeMenuState(prevScreen);
    }

    public void ChangeMenuState(MenuScreens newScreen) {
        var unityObjects = Globals.Instance.UnityObjects;

        prevScreen = currentScreen;

        switch(prevScreen) {
            case MenuScreens.Main: unityObjects["ScreenMenu"].SetActive(false); break;

            case MenuScreens.MultiPlayer: unityObjects["ScreenMultiplayer"].SetActive(false); break;

            case MenuScreens.StudentInfo: unityObjects["ScreenStudentInfo"].SetActive(false); break;

            case MenuScreens.Options: unityObjects["ScreenOptions"].SetActive(false); break;

            case MenuScreens.Loading: unityObjects["ScreenLoading"].SetActive(false); break;

            case MenuScreens.Edit: unityObjects["ScreenEdit"].SetActive(false); break;

            default: break;
        }


        currentScreen = newScreen;
        switch(currentScreen) {
            case MenuScreens.Main:
                unityObjects["ScreenMenu"].SetActive(true);
                unityObjects["Title"].GetComponent<Text>().text = "Main Menu";
                break;

            case MenuScreens.SinglePlayer:

                break;

            case MenuScreens.MultiPlayer:
                unityObjects["ScreenMultiplayer"].SetActive(true);
                unityObjects["Title"].GetComponent<Text>().text = "Multiplayer";
                break;

            case MenuScreens.StudentInfo:
                unityObjects["ScreenStudentInfo"].SetActive(true);
                unityObjects["Title"].GetComponent<Text>().text = "Student Info";
                break;

            case MenuScreens.Options:
                unityObjects["ScreenOptions"].SetActive(true);
                unityObjects["Title"].GetComponent<Text>().text = "Options";
                break;

            case MenuScreens.Loading: unityObjects["ScreenLoading"].SetActive(true); break;

            case MenuScreens.Edit: unityObjects["ScreenEdit"].SetActive(true); break;

            default: break;
        }
    }

    public void UpdateMoneySliderTxt(float value) {
        GameView.Instance.SetText("MoneyLbl", value + "$");
    }

    public void UpdateMusicVolume(float value) {
        SoundManager.Instance.Music.volume = value;
    }

    public void UpdateSfxVolume(float value) {
        SoundManager.Instance.SFX.volume = value;
    }

    public void OpenGithub() {
        Application.OpenURL("https://github.com/SaroyaMan");
    }

    public void OpenCV() {
        Application.OpenURL("https://drive.google.com/file/d/0B8BaWfqNelVKb3p2MUVUTDQ1WVk/view");
    }
}