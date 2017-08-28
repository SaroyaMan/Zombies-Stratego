using UnityEngine;
using UnityEngine.UI;

public class MenuLogic: Singleton<MenuLogic> {

    private Text moneyTextLbl;
    private AudioSource audioSource;
    //private MenuScreens currentScreen;
    private MenuScreens prevScreen;

    private void Start() {
        Init();
    }

    private void Init() {
        prevScreen = Globals.Instance.currentScreen = MenuScreens.Default;
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
        if(prevScreen != MenuScreens.Main && Globals.Instance.currentScreen != MenuScreens.Options) {
            prevScreen = MenuScreens.Main;
        }
        ChangeMenuState(prevScreen);
    }

    public void ChangeMenuState(MenuScreens newScreen) {
        var unityObjects = Globals.Instance.UnityObjects;

        prevScreen = Globals.Instance.currentScreen;

        if(prevScreen == MenuScreens.Edit) {
            unityObjects["ImgWindow"].SetActive(true);
            unityObjects["Img_Logo"].SetActive(true);
            Zombie.IsInEdit = false;
        }

        switch(prevScreen) {
            case MenuScreens.Main: unityObjects["ScreenMenu"].SetActive(false); break;

            case MenuScreens.MultiPlayer: unityObjects["ScreenMultiplayer"].SetActive(false); break;

            case MenuScreens.StudentInfo: unityObjects["ScreenStudentInfo"].SetActive(false); break;

            case MenuScreens.Options: unityObjects["ScreenOptions"].SetActive(false); break;

            case MenuScreens.Loading: unityObjects["ScreenLoading"].SetActive(false); break;

            case MenuScreens.Edit: unityObjects["ScreenEdit"].SetActive(false); break;

            default: break;
        }

        if(newScreen == MenuScreens.Edit) {
            unityObjects["ImgWindow"].SetActive(false);
            unityObjects["Img_Logo"].SetActive(false);
            Zombie.IsInEdit = true;
        }


        Globals.Instance.currentScreen = newScreen;
        switch(Globals.Instance.currentScreen) {
            case MenuScreens.Main:
                unityObjects["ScreenMenu"].SetActive(true);
                GameView.Instance.SetText(unityObjects["Title"].GetComponent<Text>(), "Main Menu");
                break;

            case MenuScreens.SinglePlayer:

                break;

            case MenuScreens.MultiPlayer:
                unityObjects["ScreenMultiplayer"].SetActive(true);
                GameView.Instance.SetText(unityObjects["Title"].GetComponent<Text>(), "Multiplayer");
                break;

            case MenuScreens.StudentInfo:
                unityObjects["ScreenStudentInfo"].SetActive(true);
                GameView.Instance.SetText(unityObjects["Title"].GetComponent<Text>(), "Student Info");
                break;

            case MenuScreens.Options:
                unityObjects["ScreenOptions"].SetActive(true);
                GameView.Instance.SetText(unityObjects["Title"].GetComponent<Text>(), "Options");
                break;

            case MenuScreens.Loading: unityObjects["ScreenLoading"].SetActive(true); break;

            case MenuScreens.Edit: unityObjects["ScreenEdit"].SetActive(true); break;

            default: break;
        }
    }

    public void UpdateMoneySliderTxt(float value) {
        GameView.Instance.SetText("MoneyLbl", value + "$");
        PlayerPrefs.SetInt("MoneyBet", (int) value);
    }

    public void UpdateMusicVolume(float value) {
        SoundManager.Instance.Music.volume = value;
        PlayerPrefs.SetFloat("Music",value);
    }


    public void UpdateSfxVolume(float value) {
        SoundManager.Instance.SFX.volume = value;
        PlayerPrefs.SetFloat("SFX", value);
    }

    public void OpenGithub() {
        Application.OpenURL(Globals.GITHUB_PROFILE_URL);
    }

    public void OpenCV() {
        Application.OpenURL(Globals.CV_URL);
    }
}