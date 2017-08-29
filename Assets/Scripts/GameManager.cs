
public class GameManager : Singleton<GameManager> {

    private bool isSinglePlayer = true;
    private GameSide gameSide;


    public bool IsSinglePlayer { get { return isSinglePlayer; } }
    public GameSide GameSide { get { return gameSide; } }

    private void Start() {
        Globals.IS_IN_GAME = true;
        if(isSinglePlayer) {
            SoldierManager.Instance.InitPcBoard();
        }

        gameSide = GameSide.LeftSide;
        GameView.Instance.SetText("currTurnTxt", "Current Turn: " + gameSide.ToString());
    }

}