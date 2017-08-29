using UnityEngine;

public class GameManager : Singleton<GameManager> {

    private bool isSinglePlayer = true;

    public bool IsSinglePlayer { get { return isSinglePlayer; } }

    private void Start() {
        if(isSinglePlayer) {
            SoldierManager.Instance.InitPcBoard();
        }

    }
}