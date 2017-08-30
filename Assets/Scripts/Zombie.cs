
using System.Collections.Generic;
using UnityEngine;

public class Zombie: PlayerSoldier {

    private const float navigationUpdate = 0.02f;

    private bool isGridMarked;
    private List<Tile> tilesToStep;

    private float navigationTime = 0;
    private bool isWalking;
    private Vector2 destination;


    private void FixedUpdate() {
        if(isWalking) {
            navigationTime += Time.deltaTime;
            if(navigationTime > navigationUpdate) {
                transform.position = Vector2.MoveTowards(transform.position, destination, navigationTime);
                navigationTime = 0;
            }
            if(Vector2.Distance(transform.position, destination) < 0.01f) {     // Zombie reached destionation
                anim.Play("Idle");
                isWalking = false;
            }
        }
    }

    private new void OnMouseDown() {
        base.OnMouseDown();
        if(Globals.IS_IN_GAME && GameManager.Instance.CurrentTurn == CurrentSide) {
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
        tilesToStep = TileManager.Instance.GetClosestTiles(CurrentTile, this);
        foreach(var tile in tilesToStep) {
            tile.ReadyToStep(this);
        }
    }

    public void UnMarkAvailableTilesToStep() {
        if(tilesToStep != null) {
            isGridMarked = false;
            foreach(var tile in tilesToStep) {
                tile.UnReadyToStep(this);
            }
            tilesToStep = null;
        }
    }

    public void Walk(Tile tile) {
        anim.Play("Walk");
        UnMarkAvailableTilesToStep();
        CurrentTile.UnmarkTileInUse();
        CurrentTile.Soldier = null;
        CurrentTile = tile;
        CurrentTile.Soldier = this;
        destination = new Vector2(tile.transform.position.x + OffsetX, tile.transform.position.y + OffsetY);
        isWalking = true;
    }
}