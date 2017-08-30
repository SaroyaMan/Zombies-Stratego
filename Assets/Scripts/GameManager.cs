
public class GameManager : Singleton<GameManager> {

    private bool isSinglePlayer = true;
    private GameSide currentTurn;
    int totalSoldiersLeftSide, totalSoldiersRightSide;


    public bool IsSinglePlayer { get { return isSinglePlayer; } }
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
    }

    public void PassTurn() {
        currentTurn = currentTurn == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
        GameView.SetText("CurrTurnTxt", "Current Turn: " + currentTurn.ToString());
    }

    public void UpdateStats() {
        GameView.SetText("NumOfZombiesLeftText", (SoldierManager.Instance.LocalPlayerList.Count - 1) + " / " + totalSoldiersLeftSide);
        GameView.SetText("NumOfZombiesRightText", (SoldierManager.Instance.EnemyList.Count - 1) + " / " + totalSoldiersRightSide);
    }

    public void WinGame(GameSide winSide) {
        print("Winner Is: " + winSide);
    }
}