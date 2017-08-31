using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic: Singleton<MenuLogic> {

    //private MenuScreens currentScreen;
    private MenuScreens prevScreen;
    private GameObject trash;

    int money = Globals.TOTAL_MONEY;

    public int Money { get { return money; } }

    private void Start() {
        Init();
    }

    private void Init() {
        prevScreen = Globals.Instance.currentScreen = MenuScreens.Default;
        trash = GameObject.FindGameObjectWithTag("Trash");
        trash.SetActive(false);
        //money = PlayerPrefs.GetInt("Money", Globals.TOTAL_MONEY); //TODO: Uncomment it
        GameView.SetText("Txt_CurrMoney", money.ToString());

        ShutdownScreens();    
        ChangeMenuState(MenuScreens.Main);
    }

    public void BuySoldier (int price) {
        money -= price;
        //PlayerPrefs.SetInt("Money", money); //TODO: Uncomment it
        GameView.SetText("Txt_CurrMoney", money.ToString());
    }

    public void SellSoldier(int price) {
        money += price;
        //PlayerPrefs.SetInt("Money", money); //TODO: Uncomment it
        GameView.SetText("Txt_CurrMoney", money.ToString());
    }

    public void SaveStrategy() {

    }

    private void ShutdownScreens() {
        var unityObjects = Globals.Instance.UnityObjects;
        unityObjects["ScreenMenu"].SetActive(false);
        unityObjects["ScreenLoading"].SetActive(false);
        unityObjects["ScreenOptions"].SetActive(false);
        unityObjects["ScreenStudentInfo"].SetActive(false);
        unityObjects["ScreenMultiplayer"].SetActive(false);
        unityObjects["ScreenEdit"].SetActive(false);
        unityObjects["TitleGameImg"].SetActive(false);
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

        switch(prevScreen) {
            case MenuScreens.Main: unityObjects["ScreenMenu"].SetActive(false); break;

            case MenuScreens.SinglePlayer:
                break;

            case MenuScreens.MultiPlayer: unityObjects["ScreenMultiplayer"].SetActive(false); break;

            case MenuScreens.StudentInfo: unityObjects["ScreenStudentInfo"].SetActive(false); break;

            case MenuScreens.Options: unityObjects["ScreenOptions"].SetActive(false); break;

            case MenuScreens.Loading: unityObjects["ScreenLoading"].SetActive(false); break;

            case MenuScreens.Edit:
                unityObjects["ScreenEdit"].SetActive(false);
                unityObjects["TitleGameImg"].SetActive(false);
                trash.SetActive(false);
                ToggleMenuWindow(true);
                StrategyEditor.Instance.DisableDragSprite();
                StrategyEditor.IsInEdit = false;
                break;

            default: break;
        }

        Globals.Instance.currentScreen = newScreen;
        switch(Globals.Instance.currentScreen) {
            case MenuScreens.Main:
                unityObjects["ScreenMenu"].SetActive(true);
                GameView.SetText(unityObjects["TitleMenu"].GetComponent<Text>(), "Main Menu");
                break;

            case MenuScreens.SinglePlayer:
                //Globals.Instance.UnityObjects.Clear();
                SceneManager.LoadSceneAsync("Game_Scene");
                break;

            case MenuScreens.MultiPlayer:
                unityObjects["ScreenMultiplayer"].SetActive(true);
                GameView.SetText(unityObjects["TitleMenu"].GetComponent<Text>(), "Multiplayer");
                break;

            case MenuScreens.StudentInfo:
                unityObjects["ScreenStudentInfo"].SetActive(true);
                GameView.SetText(unityObjects["TitleMenu"].GetComponent<Text>(), "Student Info");
                break;

            case MenuScreens.Options:
                unityObjects["ScreenOptions"].SetActive(true);
                GameView.SetText(unityObjects["TitleMenu"].GetComponent<Text>(), "Options");
                break;

            case MenuScreens.Loading: unityObjects["ScreenLoading"].SetActive(true); break;

            case MenuScreens.Edit:
                unityObjects["ScreenEdit"].SetActive(true);
                unityObjects["TitleGameImg"].SetActive(true);
                trash.SetActive(true);
                GameView.SetText(unityObjects["TitleGame"].GetComponent<Text>(), "Edit mode");
                ToggleMenuWindow(false);
                StrategyEditor.IsInEdit = true;
                break;

            default: break;
        }
    }

    public void UpdateMoneySliderTxt(float value) {
        GameView.SetText("MoneyLbl", value + "$");
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

    private void ToggleMenuWindow(bool isTurnOn) {
        Globals.Instance.UnityObjects["MainWindow"].SetActive(isTurnOn);
        Globals.Instance.UnityObjects["Img_Logo"].SetActive(isTurnOn);
    }
}