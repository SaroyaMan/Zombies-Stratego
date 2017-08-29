
using System.Collections.Generic;

public class Zombie: PlayerSoldier {

    private bool isGridMarked;
    private List<Tile> tilesToStep;

    private new void OnMouseDown() {
        base.OnMouseDown();
        if(Globals.IS_IN_GAME && GameManager.Instance.GameSide == GameSide.LeftSide) {
            if(!isGridMarked) {
                SoldierManager.Instance.MarkSelectedSoldier(this);
                MarkAvailableTilesToStep();
            }
            else {
                UnMarkAvailableTilesToStep();
            }
        }
    }

    public void MarkAvailableTilesToStep() {
        isGridMarked = true;
        tilesToStep = TileManager.Instance.GetClosestTiles(CurrentTile);
        foreach(var tile in tilesToStep) {
            tile.ColorTile();
        }
    }

    public void UnMarkAvailableTilesToStep() {
        isGridMarked = false;
        foreach(var tile in tilesToStep) {
            tile.UnColorTile();
        }
        tilesToStep = null;
    }
}