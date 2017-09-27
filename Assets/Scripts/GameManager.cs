using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {


    [SerializeField] private Sprite blueFlagSprite;
    [SerializeField] private Sprite redFlagSprite;

    private bool isPcPlaying;
    private bool isPaused;
    public static GameSide CURRENT_TURN;
    private GameSide pcSide = GameSide.RightSide;
    private int totalSoldiersLocalSide, totalSoldiersEnemySide;
    private GameScreens prevScreen, currentScreen;

    public bool IsPcPlaying { get { return isPcPlaying; } }
    //public GameSide CurrentTurn { get { return currentTurn; } set { currentTurn = value; } }
    public GameSide PcSide { get { return pcSide; } set { pcSide = value; } }
    public bool IsPaused { get { return isPaused; } }

    private void Start() {
        Globals.IS_IN_GAME = true;
        //if(Globals.IS_SINGLE_PLAYER) {
        //    MultiPlayerManager.Instance.gameObject.SetActive(false);
        //}
        InitGame();
        ShutdownScreens();
    }

    private void InitGame() {
        if(Globals.IS_SINGLE_PLAYER) {
            SoldierManager.Instance.InitPcBoard();
            pcSide = (GameSide) Random.Range(0, 2);
            if(pcSide == GameSide.LeftSide) {
                SoldierManager.Instance.FlipSide();
            }
            totalSoldiersLocalSide = SoldierManager.Instance.LocalPlayerList.Count - 1;
            totalSoldiersEnemySide = SoldierManager.Instance.EnemyList.Count - 1;
            CURRENT_TURN = (GameSide) Random.Range(0, 2);
            SoldierManager.Instance.HideAllSoldiers();
            UpdateStats();
            PassTurn();
        }
        else {      //game is multiplayer
            
            totalSoldiersLocalSide = SoldierManager.Instance.LocalPlayerList.Count - 1;
            totalSoldiersEnemySide = SoldierManager.Instance.EnemyList.Count - 1;
            UpdateStats();

            Globals.Instance.UnityObjects["ResetBtn"].SetActive(false);
            GameView.DisableButton("ResetWinBtn");
        }
        UpdateTitles();
    }

    private void UpdateTitles() {
        if(Globals.IS_SINGLE_PLAYER) {
            GameView.SetText("TitleGame", "Single Player");
            if(pcSide == GameSide.LeftSide) {
                GameView.SetText("LeftSideNameTxt", "PC");
                GameView.SetText("RightSideNameTxt", PlayerPrefs.GetString("Username", "No-Name"));
            }
            else {
                GameView.SetText("LeftSideNameTxt", PlayerPrefs.GetString("Username", "No-Name"));
                GameView.SetText("RightSideNameTxt", "PC");
            }
        }
        else {
            GameView.SetText("TitleGame", "Multi Player");
            if(MultiPlayerManager.Instance.PlayerSide == GameSide.LeftSide) {
                GameView.SetText("LeftSideNameTxt", PlayerPrefs.GetString("Username", "No-Name"));
                GameView.SetText("RightSideNameTxt", MultiPlayerManager.Instance.RealEnemyUsername);
            }
            else {
                GameView.SetText("LeftSideNameTxt", MultiPlayerManager.Instance.RealEnemyUsername);
                GameView.SetText("RightSideNameTxt", PlayerPrefs.GetString("Username", "No-Name"));
            }
        }
    }

    public void PassTurn(Tile oldTile = null, Tile newTile = null) {
        StartCoroutine(PassTurnAfterTwoSecs(oldTile, newTile));

    }

    private IEnumerator PassTurnAfterTwoSecs(Tile oldTile = null, Tile newTile = null) {
        if(!Globals.IS_SINGLE_PLAYER)
            yield return new WaitForSeconds(1f);
        ChangeTurn();
        if(Globals.IS_SINGLE_PLAYER) {
            if(CURRENT_TURN == pcSide && !isPcPlaying) {
                isPcPlaying = true;
                StartCoroutine(SoldierManager.Instance.MakeRandomMove());
            }
            else {
                isPcPlaying = false;
            }
        }
        else {  //game is multiplayer
            MultiPlayerManager.Instance.SendMove(oldTile, newTile);
        }
    }

    public void ChangeTurn() {
        CURRENT_TURN = CURRENT_TURN == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
        GameView.SetImage("FlagColor", CURRENT_TURN == GameSide.LeftSide ? blueFlagSprite : redFlagSprite);
    }

    public void UpdateStats() {
        if(Globals.IS_SINGLE_PLAYER) {
            if(pcSide == GameSide.RightSide) {
                GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLocalSide);
                GameView.SetText("ZombiesRightText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersEnemySide);
            }
            else {
                GameView.SetText("ZombiesRightText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLocalSide);
                GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersEnemySide);
            }
        }
        else {
            if(MultiPlayerManager.Instance.PlayerSide == GameSide.LeftSide) {
                GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLocalSide);
                GameView.SetText("ZombiesRightText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersEnemySide);
            }
            else {
                GameView.SetText("ZombiesRightText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLocalSide);
                GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersEnemySide);
            }
        }

    }

    public void CheckWin(GameSide potentialLoserSide) {
        GameSide potentialWinnerSide = potentialLoserSide == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
        if(!SoldierManager.Instance.HasZombies(potentialLoserSide)) {
            WinGame(potentialWinnerSide);
        }
    }

    public void WinGame(GameSide winSide, string msg = null) {
        SoldierManager.Instance.CoverAllSoldiers();
        Globals.Instance.UnityObjects["WinWindow"].SetActive(true);
        if(Globals.IS_SINGLE_PLAYER && pcSide != winSide || !Globals.IS_SINGLE_PLAYER && winSide == MultiPlayerManager.Instance.PlayerSide) {
            GameView.SetText("TitleWinner", "You Won !");
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.SinglePlayerWin);
        }
        else {
            GameView.SetText("TitleWinner", "You Lost !");
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.SinglePlayerLose);
        }
        if(msg != null) {
            GameView.SetText("MsgWinner", msg);
        }
        SoundManager.Instance.Music.Stop();
        GameView.MakeScreenDark();
        isPaused = true;
        Time.timeScale = 0;           //Pause game here
    }

    public void UpdateMusicVolume(float value) {
        SoundManager.Instance.Music.volume = value;
        PlayerPrefs.SetFloat("Music", value);
    }

    public void UpdateSfxVolume(float value) {
        SoundManager.Instance.SFX.volume = value;
        PlayerPrefs.SetFloat("SFX", value);
    }

    public void GoBack() {
        ChangeGameState(prevScreen);
    }

    private void ShutdownScreens() {
        Globals.Instance.UnityObjects["PauseWindow"].SetActive(false);
        Globals.Instance.UnityObjects["WinWindow"].SetActive(false);
    }

    public void ChangeGameState(GameScreens newScreen) {
        var unityObjects = Globals.Instance.UnityObjects;

        prevScreen = currentScreen;

        switch(prevScreen) {
            case GameScreens.InGame: unityObjects["PauseWindow"].SetActive(false); break;

            case GameScreens.Pause: unityObjects["ScreenPause"].SetActive(false); break;

            case GameScreens.Options: unityObjects["ScreenOptions"].SetActive(false); break;

            default: break;
        }

        currentScreen = newScreen;
        switch(currentScreen) {
            case GameScreens.InGame:
                ResumeGame();
                break;

            case GameScreens.Pause:
                PauseGame();
                break;

            case GameScreens.Options:
                unityObjects["ScreenOptions"].SetActive(true);
                GameView.SetText("TitlePause", "Settings");
                break;

            default: break;
        }
    }

    private void PauseGame() {
        var unityObjects = Globals.Instance.UnityObjects;
        unityObjects["PauseWindow"].SetActive(true);
        unityObjects["ScreenOptions"].SetActive(false);
        unityObjects["ScreenPause"].SetActive(true);
        GameView.DisableButton("PauseBtn");
        GameView.SetText("TitlePause", "Game Paused");
        GameView.MakeScreenDark();
        isPaused = true;
        Time.timeScale = 0;           //Pause game here
    }

    private void ResumeGame() {
        Globals.Instance.UnityObjects["PauseWindow"].SetActive(false);
        GameView.EnableButton("PauseBtn");
        GameView.MakeScreenNormal();
        isPaused = false;
        Time.timeScale = 1;           //Resume game here
    }

    public void QuitGame() {
        if(Globals.IS_IN_GAME && !Globals.IS_SINGLE_PLAYER) {
            MultiPlayerManager.Instance.SendGameQuit(MultiPlayerManager.ParseUsername(MultiPlayerManager.Instance.Username) + " gave up");
        }
        Destroy(SoldierManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        Destroy(TileManager.Instance.gameObject);
        Destroy(MultiPlayerManager.Instance.gameObject);
        Time.timeScale = 1;
        Globals.IS_IN_GAME = false;

        SceneManager.LoadScene("Main_Scene");
        //Initiate.Fade("Main_Scene", GameView.transitionColor, 3f);
    }

    public void ResetMatch() {
        SoundManager.Instance.Music.Play();     //stop the music and start over
        TileManager.Instance.ClearTiles();
        SoldierManager.Instance.ClearSoldiers();
        SoldierManager.Instance.LoadStrategy();
        InitGame();
        ResumeGame();
        if(Globals.Instance.UnityObjects["WinWindow"]) {
            GameView.SetText("MsgWinner", "");
            Globals.Instance.UnityObjects["WinWindow"].SetActive(false);
        }
    }
}