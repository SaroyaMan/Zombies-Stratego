
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    private bool isSinglePlayer = true;
    private bool isPcPlaying;
    private GameSide currentTurn;
    int totalSoldiersLeftSide, totalSoldiersRightSide;
    private GameScreens prevScreen, currentScreen;


    public bool IsSinglePlayer { get { return isSinglePlayer; } }
    public bool IsPcPlaying { get { return isPcPlaying; } }
    public GameSide CurrentTurn { get { return currentTurn; } }

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
        if(Globals.Instance.UnityObjects.ContainsKey("ZombiesLeftText")) {
            GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLeftSide);
        }
        else {
            print("KEY ZombiesLeftText NOT EXITS!");
        }

        if(Globals.Instance.UnityObjects.ContainsKey("ZombiesRightText")) {
            GameView.SetText("ZombiesRightText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersRightSide);
        }
        else {
            print("KEY ZombiesRightText NOT EXITS!");
        }
        //GameView.SetText("ZombiesLeftText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLeftSide);
        //GameView.SetText("NumOfZombiesRightText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersRightSide);
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
                unityObjects["PauseWindow"].SetActive(false);
                GameView.MakeScreenNormal();
                break;

            case GameScreens.Pause:
                unityObjects["PauseWindow"].SetActive(true);
                unityObjects["ScreenOptions"].SetActive(false);
                unityObjects["ScreenPause"].SetActive(true);
                GameView.SetText("TitlePause", "Game Paused");
                GameView.MakeScreenDark();
                //Time.timeScale = 0;           //Pause game here
                break;

            case GameScreens.Options:
                unityObjects["ScreenOptions"].SetActive(true);
                GameView.SetText("TitlePause", "Settings");
                break;

            default: break;
        }
    }
}