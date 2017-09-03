using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {


    [SerializeField] private Sprite blueFlagSprite;
    [SerializeField] private Sprite redFlagSprite;

    private bool isSinglePlayer = true;
    private bool isPcPlaying;
    private bool isPaused;
    private GameSide currentTurn;
    private GameSide pcSide = GameSide.RightSide;
    int totalSoldiersLocalSide, totalSoldiersEnemySide;
    private GameScreens prevScreen, currentScreen;

    public bool IsSinglePlayer { get { return isSinglePlayer; } }
    public bool IsPcPlaying { get { return isPcPlaying; } }
    public GameSide CurrentTurn { get { return currentTurn; } }
    public GameSide PcSide { get { return pcSide; } set { pcSide = value; } }
    public bool IsPaused { get { return isPaused; } }

    private void Start() {
        Globals.IS_IN_GAME = true;
        InitGame();
        ShutdownScreens();
    }

    private void InitGame() {
        if(isSinglePlayer) {
            SoldierManager.Instance.InitPcBoard();
        }
        totalSoldiersLocalSide = SoldierManager.Instance.LocalPlayerList.Count - 1;
        totalSoldiersEnemySide = SoldierManager.Instance.EnemyList.Count - 1;
        currentTurn = (GameSide) Random.Range(0, 2);

        pcSide = (GameSide) Random.Range(0, 2);
        if(pcSide == GameSide.LeftSide) {
            SoldierManager.Instance.FlipSide();
        }

        SoldierManager.Instance.HideAllSoldiers();
        UpdateStats();
        PassTurn();
    }

    public void PassTurn() {
        currentTurn = currentTurn == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;

        if(isSinglePlayer && currentTurn == pcSide && !isPcPlaying) {
            isPcPlaying = true;
            StartCoroutine(SoldierManager.Instance.MakeRandomMove());
        }
        else {
            isPcPlaying = false;
        }
        GameView.SetImage("FlagColor", currentTurn == GameSide.LeftSide ? blueFlagSprite : redFlagSprite);

    }

    public void UpdateStats() {
        if(isSinglePlayer && pcSide == GameSide.RightSide) {
            GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLocalSide);
            GameView.SetText("ZombiesRightText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersEnemySide);
        }
        else {
            GameView.SetText("ZombiesRightText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLocalSide);
            GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersEnemySide);
        }
    }

    public void CheckWin(GameSide potentialLoserSide) {
        GameSide potentialWinnerSide = potentialLoserSide == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
        if(!SoldierManager.Instance.HasZombies(potentialLoserSide)) {
            WinGame(potentialWinnerSide);
        }
    }

    public void WinGame(GameSide winSide) {
        SoldierManager.Instance.CoverAllSoldiers();
        Globals.Instance.UnityObjects["WinWindow"].SetActive(true);
        if(isSinglePlayer && pcSide != winSide) {
            GameView.SetText("TitleWinner", "You Won !");
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.SinglePlayerWin);
        }
        else if(isSinglePlayer && pcSide == winSide) {
            GameView.SetText("TitleWinner", "PC Won !");
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.SinglePlayerLose);
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
        Destroy(SoldierManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        Destroy(TileManager.Instance.gameObject);
        Time.timeScale = 1;
        Globals.IS_IN_GAME = false;
        SceneManager.LoadSceneAsync("Main_Scene");
    }

    public void ResetMatch() {
        SoundManager.Instance.Music.Play();     //stop the music and start over
        TileManager.Instance.ClearTiles();
        SoldierManager.Instance.ClearSoldiers();
        SoldierManager.Instance.LoadStrategy();
        InitGame();
        ResumeGame();
        if(Globals.Instance.UnityObjects["WinWindow"]) {
            Globals.Instance.UnityObjects["WinWindow"].SetActive(false);
        }
    }
}