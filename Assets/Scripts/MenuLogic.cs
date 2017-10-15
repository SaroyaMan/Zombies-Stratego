using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic: Singleton<MenuLogic> {

    //private MenuScreens currentScreen;
    private MenuScreens prevScreen;
    private GameObject trash;
    private bool isAllowedToPlay;
    int money = Globals.TOTAL_MONEY;
    string username;

    public int Money { get { return money; } }
    public string Username { get { return username; } }

    private void Start() {
        Init();
    }

    private void Init() {
        prevScreen = Globals.Instance.currentScreen = MenuScreens.Default;
        trash = GameObject.FindGameObjectWithTag("Trash");
        trash.SetActive(false);

        LoadStrategy();
        ShutdownScreens();    
        ChangeMenuState(MenuScreens.Main);
    }

    public void BuySoldier (int price) {
        money -= price;
        //PlayerPrefs.SetInt("Money", money);
        GameView.SetText("Txt_CurrMoney", money.ToString());
        if(StrategyEditor.HasFlag && SoldierManager.Instance.LocalPlayerList.Count > Globals.MIN_SOLDIERS_ALLOWED && !isAllowedToPlay) {
            isAllowedToPlay = true;
            GameView.EnableButton("SingleBtn");
            GameView.EnableButton("MultiBtn");
            GameView.SetTextColor("ZombiesLeftText", GameView.GreenColor);
        }
        GameView.SetText("ZombiesLeftText", SoldierManager.Instance.LocalPlayerList.Count - 1 + " / " + Globals.MIN_SOLDIERS_ALLOWED);
    }

    public void SellSoldier(int price) {
        money += price;
        //PlayerPrefs.SetInt("Money", money);
        GameView.SetText("Txt_CurrMoney", money.ToString());
        if((!StrategyEditor.HasFlag || SoldierManager.Instance.LocalPlayerList.Count <= Globals.MIN_SOLDIERS_ALLOWED) && isAllowedToPlay) {
            //Start GAME BTN IS NOT ALLOWED
            isAllowedToPlay = false;
            GameView.DisableButton("SingleBtn");
            GameView.DisableButton("MultiBtn");
            GameView.SetTextColor("ZombiesLeftText", GameView.RedColor);
        }
        GameView.SetText("ZombiesLeftText", SoldierManager.Instance.LocalPlayerList.Count - 1 + " / " + Globals.MIN_SOLDIERS_ALLOWED);
    }

    public void SaveStrategy() {
        var buildTiles = TileManager.Instance.BuildTiles;
        foreach(var tile in buildTiles.Values) {
            //print(tile.Row + ", " + tile.Column + Regex.Match(soldier.name, @"\d+").Value + ", ");
            if(tile.Soldier != null) {
                PlayerPrefs.SetString(tile.Row + "," + tile.Column, Regex.Match(tile.Soldier.name, @"^[a-zA-Z0-9]*").Value);
            }
            else {
                PlayerPrefs.SetString(tile.Row + "," + tile.Column, "");
            }
        }
    }

    public void LoadStrategy() {
        int y = 0, z = 0;
        var matrixTile = TileManager.Instance.MatrixTiles;
        var soldierBtns = Globals.Instance.GetAllSoldierBtns();
        for(int i = 0; i < Globals.MAX_SOLDIERS_FOR_PLAYER + 1; i++) {
            string tilePattern = PlayerPrefs.GetString(y + "," + z, "");
            if(tilePattern != "") {
                StrategyEditor.Instance.PlaceSoldier(matrixTile[y, z], soldierBtns[tilePattern].SoldierObject, false);
            }
            z++;
            if(z == 4) {
                y++;
                z = 0;
            }
        }
        if(!StrategyEditor.HasFlag) {
            StrategyEditor.Instance.PlaceSoldier(matrixTile[0, 0], soldierBtns["BlueFlag"].SoldierObject, false);
        }
    }

    private void ShutdownScreens() {
        var unityObjects = Globals.Instance.UnityObjects;
        unityObjects["ScreenMenu"].SetActive(false);
        //unityObjects["ScreenLoading"].SetActive(false);
        unityObjects["ScreenOptions"].SetActive(false);
        unityObjects["ScreenStudentInfo"].SetActive(false);
        unityObjects["ScreenMultiplayer"].SetActive(false);
        unityObjects["ScreenEdit"].SetActive(false);
        unityObjects["TitleGameImg"].SetActive(false);
        unityObjects["StatusConnectionWindow"].SetActive(false);
        unityObjects["ErrorWindow"].SetActive(false);
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

            case MenuScreens.MultiPlayer: unityObjects["ScreenMultiplayer"].SetActive(false); break;

            case MenuScreens.StudentInfo: unityObjects["ScreenStudentInfo"].SetActive(false); break;

            case MenuScreens.Options: unityObjects["ScreenOptions"].SetActive(false); break;

            //case MenuScreens.Loading: unityObjects["ScreenLoading"].SetActive(false); break;

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
                GameView.SetText("TitleMenu", "Main Menu");
                break;

            case MenuScreens.MultiPlayer:
                if(Application.internetReachability == NetworkReachability.NotReachable) {
                    Globals.Instance.UnityObjects["ErrorWindow"].SetActive(true);
                    GameView.SetText("ErrorTxt", "No Internet Connection !");
                }
                else {
                    unityObjects["ScreenMultiplayer"].SetActive(true);
                    GameView.SetText("TitleMenu", "Multiplayer");
                }
                break;

            case MenuScreens.StudentInfo:
                unityObjects["ScreenStudentInfo"].SetActive(true);
                GameView.SetText("TitleMenu", "Student Info");
                break;

            case MenuScreens.Options:
                unityObjects["ScreenOptions"].SetActive(true);
                GameView.SetText("TitleMenu", "Options");
                break;

            case MenuScreens.Edit:
                unityObjects["ScreenEdit"].SetActive(true);
                unityObjects["TitleGameImg"].SetActive(true);
                trash.SetActive(true);
                GameView.SetText("TitleMenu", "Edit mode");
                ToggleMenuWindow(false);
                StrategyEditor.IsInEdit = true;
                break;

            default: break;
        }
    }

    public void StartGame(bool isSinglePlayer) {
        Globals.IS_SINGLE_PLAYER = isSinglePlayer;

        if(isSinglePlayer) {       // start single player game
            Globals.Instance.UnityObjects["ScreenMenu"].SetActive(false);
            SoundManager.Instance.Music.clip = SoundManager.Instance.InGameMusic;
            SoundManager.Instance.Music.Play();
            //SceneManager.LoadSceneAsync("Game_Scene");
            Initiate.Fade("Game_Scene", GameView.transitionColor, 2f);
        }
        else {                    // start multi player game
            Globals.Instance.UnityObjects["StatusConnectionWindow"].SetActive(true);
            MultiPlayerManager.Instance.ConnectGame();
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

    public void UpdateUsername(string username) {
        this.username = username;
        PlayerPrefs.SetString("Username", username);
    }

    public void OpenGithub() {
        Application.OpenURL(Globals.GITHUB_PROFILE_URL);
    }

    public void OpenCV() {
        Application.OpenURL(Globals.CV_URL);
    }

    public void ConfirmError() {
        Globals.Instance.UnityObjects["ErrorWindow"].SetActive(false);
        GoBack();
    }

    public void QuitGame() {
        Application.Quit();
    }

    private void ToggleMenuWindow(bool isTurnOn) {
        Globals.Instance.UnityObjects["MainWindow"].SetActive(isTurnOn);
        Globals.Instance.UnityObjects["Img_Logo"].SetActive(isTurnOn);
        Globals.Instance.UnityObjects["Input_Username"].SetActive(isTurnOn);
    }
}