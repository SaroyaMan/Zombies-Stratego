using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    private bool isSinglePlayer = true;
    private bool isPcPlaying;
    private bool isPaused;
    private GameSide currentTurn;
    int totalSoldiersLeftSide, totalSoldiersRightSide;
    private GameScreens prevScreen, currentScreen;


    public bool IsSinglePlayer { get { return isSinglePlayer; } }
    public bool IsPcPlaying { get { return isPcPlaying; } }
    public GameSide CurrentTurn { get { return currentTurn; } }
    public bool IsPaused { get { return isPaused; } }

    private void Start() {
        Globals.IS_IN_GAME = true;

        if(isSinglePlayer) {
            SoldierManager.Instance.InitPcBoard();
        }
        totalSoldiersLeftSide = SoldierManager.Instance.LocalPlayerList.Count - 1;
        totalSoldiersRightSide = SoldierManager.Instance.EnemyList.Count - 1;
        UpdateStats();
        currentTurn = GameSide.LeftSide;
        GameView.SetText("CurrTurnTxt", "Current Turn: " + currentTurn.ToString());
        ShutdownScreens();
    }

    public void PassTurn() {
        currentTurn = currentTurn == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
        GameView.SetText("CurrTurnTxt", "Current Turn: " + currentTurn.ToString());

        if(isSinglePlayer && currentTurn == GameSide.RightSide && !isPcPlaying) {
            isPcPlaying = true;
            StartCoroutine(SoldierManager.Instance.MakeRandomMove());
        }
        else {
            isPcPlaying = false;
        }
    }

    public void UpdateStats() {
        GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLeftSide);
        GameView.SetText("ZombiesRightText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersRightSide);
    }

    public void WinGame(GameSide winSide) {
        print("Winner Is: " + winSide);
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
        var unityObjects = Globals.Instance.UnityObjects;
        unityObjects["PauseWindow"].SetActive(false);
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
        SceneManager.LoadSceneAsync("Main_Scene");
    }
}