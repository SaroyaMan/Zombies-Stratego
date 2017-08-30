using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie: PlayerSoldier {

    private const float navigationUpdate = 0.02f;

    private bool isGridMarked;
    private List<Tile> tilesToStep;

    private float navigationTime = 0;
    private bool isWalking;
    private bool isInWar;
    private bool isDying;
    private Vector2 destination;


    private void FixedUpdate() {
        if(isWalking) {
            navigationTime += Time.deltaTime;
            if(navigationTime > navigationUpdate) {
                transform.position = Vector2.MoveTowards(transform.position, destination, navigationTime);
                navigationTime = 0;
            }
            if(Vector2.Distance(transform.position, destination) < 0.01f && !isDying) {     // Zombie reached destionation
                anim.Play("Idle");
                isWalking = false;
            }
        }
    }

    private new void OnMouseDown() {
        base.OnMouseDown();
        if(Globals.IS_IN_GAME && GameManager.Instance.CurrentTurn == CurrentSide) {
            if(!isGridMarked && !isDying) {
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
        if(tile.Column < CurrentTile.Column) {
            //animation walk should be reversed
            //anim.speed = -1;
        }
        anim.Play("Walk");
        UnMarkAvailableTilesToStep();
        CurrentTile.UnmarkTileInUse();
        CurrentTile.Soldier = null;

        CurrentTile = tile;
        GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row;
        if(CurrentTile.Soldier != null && IsEnemy(CurrentTile.Soldier)) {
            print("ATTACK !");
        }
        else {
            CurrentTile.Soldier = this;
            destination = new Vector2(tile.transform.position.x + OffsetX, tile.transform.position.y + OffsetY);
            isWalking = true;
        }
    }

    public void GetCloser(PlayerSoldier enemy) {

        anim.Play("Walk");
        playerCollider.isTrigger = true;
        enemy.PlayerCollider.isTrigger = true;

        if(enemy is Zombie)
            enemy.gameObject.tag = "InWar";

        UnMarkAvailableTilesToStep();
        CurrentTile.UnmarkTileInUse();
        CurrentTile.Soldier = null;
        CurrentTile = enemy.CurrentTile;


        CurrentTile.Soldier = this;
        destination = new Vector2(CurrentTile.transform.position.x + OffsetX, CurrentTile.transform.position.y + OffsetY);
        isWalking = isInWar = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isInWar && other.gameObject.tag == "InWar") {
            isInWar = false;
            Zombie zombie = other.gameObject.GetComponent<Zombie>() as Zombie;
            playerCollider.isTrigger = false;
            zombie.PlayerCollider.isTrigger = false;
            StartCoroutine(Kill(zombie));
        }

        if(isInWar && other.gameObject.tag == "Bomb") {
            isInWar = false;
            Bomb bomb = other.gameObject.GetComponent<Bomb>() as Bomb;
            playerCollider.isTrigger = false;
            bomb.PlayerCollider.isTrigger = false;
            StartCoroutine(Explode(bomb));
        }

        if(isInWar && other.gameObject.tag == "Flag") {
            isInWar = false;
            Flag flag = other.gameObject.GetComponent<Flag>() as Flag;
            playerCollider.isTrigger = false;
            flag.PlayerCollider.isTrigger = false;
            GameManager.Instance.WinGame(CurrentSide);
        }
    }

    private IEnumerator Explode(Bomb bomb) {
        isInWar = false;
        anim.Play("Attack");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(bomb.Explode());
        yield return new WaitForSeconds(1f);
        StartCoroutine(Die());

        yield return null;
    }

    private IEnumerator Kill(Zombie enemy) {
        anim.Play("Attack");
        enemy.Anim.Play("Attack");
        yield return new WaitForSeconds(0.5f);
        if(Rank > enemy.Rank) {         //kill enemy
            CurrentTile.Soldier = this;
            StartCoroutine(enemy.Die());
        }
        else if(Rank < enemy.Rank) {    //kill this zombie
            enemy.Anim.Play("Idle");
            enemy.gameObject.tag = "Zombie";
            enemy.CurrentTile.Soldier = enemy;
            StartCoroutine(Die());
        }
        else {                          // draw - kill both
            CurrentTile.IsInUse = false;
            StartCoroutine(Die());
            StartCoroutine(enemy.Die());
        }
        yield return null;
    }

    private IEnumerator Die() {
        isDying = true;
        SoldierManager.Instance.UnregisterPlayer(this);
        Anim.Play("Die");
        transform.parent = null;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}