
public class GameManager : Singleton<GameManager> {

    private bool isSinglePlayer = true;
    private GameSide currentTurn;


    public bool IsSinglePlayer { get { return isSinglePlayer; } }
    public GameSide CurrentTurn { get { return currentTurn; } }

    private void Start() {
        Globals.IS_IN_GAME = true;
        if(isSinglePlayer) {
            SoldierManager.Instance.InitPcBoard();
        }

        currentTurn = GameSide.LeftSide;
        GameView.SetText("currTurnTxt", "Current Turn: " + currentTurn.ToString());
    }

    public void PassTurn() {
        currentTurn = currentTurn == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
    }

    public void WinGame(GameSide winSide) {
        print("Winner Is: " + winSide);
    }
}