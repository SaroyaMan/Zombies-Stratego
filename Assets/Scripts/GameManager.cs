using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {


    [SerializeField] private Sprite blueFlagSprite;
    [SerializeField] private Sprite redFlagSprite;

    private bool isPcPlaying;
    private bool isPaused;
    private bool isGameOver;
    private bool isDescriptionOpen;
    public static GameSide CURRENT_TURN;
    private GameSide pcSide = GameSide.RightSide;
    private int totalSoldiersLocalSide, totalSoldiersEnemySide;
    private GameScreens prevScreen, currentScreen;
    private GameObject cloudInfo;

    private int currentTurnSecondsLeft = Globals.MAX_TURN_TIME;

    public bool IsPcPlaying { get { return isPcPlaying; } }
    //public GameSide CurrentTurn { get { return currentTurn; } set { currentTurn = value; } }
    public GameSide PcSide { get { return pcSide; } set { pcSide = value; } }
    public bool IsPaused { get { return isPaused; } }
    public bool IsGameOver { get { return isGameOver; } }
    public bool IsDescriptionOpen { get { return isDescriptionOpen; } }

    private void Start() {
        Globals.IS_IN_GAME = true;
        cloudInfo = Globals.Instance.UnityObjects["InfoBubble"];
        if(Globals.IS_SINGLE_PLAYER) {
            Globals.Instance.UnityObjects["ClockDisplay"].SetActive(false);
            //MultiPlayerManager.Instance.gameObject.SetActive(false);
        }
        InitGame();
        //Globals.Instance.UnityObjects["Smoke"].SetActive(false);
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

            StartCoroutine(CountTime());
        }
        cloudInfo.SetActive(false);
        UpdateTitles();
    }

    public IEnumerator CountTime() {
        if(MultiPlayerManager.Instance.IsMyTurn) {
            yield return new WaitForSeconds(1);
            currentTurnSecondsLeft -= 1;
            if(currentTurnSecondsLeft < 11 && currentTurnSecondsLeft > -1) {
                if(currentTurnSecondsLeft % 2 != 0) {
                    SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ClockTickOne);
                }
                else {
                    SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ClockTickTwo);
                }
            }
            if(currentTurnSecondsLeft == -1) {
                WinGame(MultiPlayerManager.Instance.PlayerSide == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide, "You didn't make a move!");
                MultiPlayerManager.Instance.SendGameQuit(MultiPlayerManager.Instance.RealUsername + "'s turn time is over");
                yield return null;
            }
            else {
                GameView.SetText("ClockTimeTxt", currentTurnSecondsLeft.ToString());
                yield return CountTime();
            }
        }
        else {
            currentTurnSecondsLeft = Globals.MAX_TURN_TIME;
            GameView.SetText("ClockTimeTxt", currentTurnSecondsLeft.ToString());
            yield return null;
        }
    }

    private void UpdateTitles() {
        if(Globals.IS_SINGLE_PLAYER) {
            GameView.SetText("TitleGame", "Single Player");
            if(pcSide == GameSide.LeftSide) {
                GameView.SetText("LeftSideNameTxt", "COM");
                GameView.SetText("RightSideNameTxt", PlayerPrefs.GetString("Username", "No-Name"));
            }
            else {
                GameView.SetText("LeftSideNameTxt", PlayerPrefs.GetString("Username", "No-Name"));
                GameView.SetText("RightSideNameTxt", "COM");
            }
        }
        else {
            GameView.SetText("TitleGame", "Multi Player");
            if(MultiPlayerManager.Instance.PlayerSide == GameSide.LeftSide) {
                GameView.SetText("LeftSideNameTxt", MultiPlayerManager.Instance.RealUsername);
                GameView.SetText("RightSideNameTxt", MultiPlayerManager.Instance.RealEnemyUsername);
            }
            else {
                GameView.SetText("LeftSideNameTxt", MultiPlayerManager.Instance.RealEnemyUsername);
                GameView.SetText("RightSideNameTxt", MultiPlayerManager.Instance.RealUsername);
            }
        }
    }

    public void PassTurn(Tile oldTile = null, Tile newTile = null) {
        //StartCoroutine(PassTurnAfterTwoSecs(oldTile, newTile));
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
        isGameOver = true;
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
        CloseInfo();
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
        Globals.Instance.UnityObjects["InfoBubble"].SetActive(false);
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
        if(Globals.IS_SINGLE_PLAYER)
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

        //SceneManager.LoadScene("Main_Scene");
        Initiate.Fade("Main_Scene", GameView.transitionColor, 3f);
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

    public IEnumerator DisplayInfo(PlayerSoldier soldier) {
        GameView.SetImage("SoldierImg", soldier.Sprite);
        GameView.SetText("RankInfoTxt", soldier.Rank.ToString());
        GameView.SetText("InfoTitleTxt", soldier.SoldierName);
        //cloudInfo.transform.position = soldier.transform.position;
        cloudInfo.transform.position = new Vector2(Input.mousePosition.x + (soldier.CurrentSide == GameSide.LeftSide ? 70 : -70), Input.mousePosition.y);
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.Description);
        isDescriptionOpen = true;
        cloudInfo.SetActive(true);

        yield return new WaitForSeconds(3f);
        CloseInfo();
    }

    public void CloseInfo() {
        if(IsDescriptionOpen) {
            cloudInfo.SetActive(false);
            isDescriptionOpen = false;
        }
    }

    public static Vector3 GetScreenPosition(Transform transform, Canvas canvas, Camera cam) {
        Vector3 pos;
        float width = canvas.GetComponent<RectTransform>().sizeDelta.x;
        float height = canvas.GetComponent<RectTransform>().sizeDelta.y;
        float x = Camera.main.WorldToScreenPoint(transform.position).x / Screen.width;
        float y = Camera.main.WorldToScreenPoint(transform.position).y / Screen.height;
        pos = new Vector3(width * x - width / 2, y * height - height / 2);
        return pos;
    }
}