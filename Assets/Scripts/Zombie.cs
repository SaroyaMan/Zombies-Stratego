using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie: PlayerSoldier {

    private const float navigationUpdate = 0.02f;

    private bool isGridMarked;
    private bool isFlipped;
    private List<Tile> tilesToStep;

    private float navigationTime = 0;
    private bool isWalking;
    private bool isInWar;
    private bool isDying;
    private Vector2 destination;

    public bool IsDying { get { return isDying; } }
    public List<Tile> TilesToStep { get { return tilesToStep; } set { tilesToStep = value; } }


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
                if(isFlipped) {
                    FlipSide();
                    isFlipped = false;
                    transform.position = new Vector2(CurrentTile.transform.position.x + OffsetX, CurrentTile.transform.position.y + OffsetY);
                };
            }
        }
    }

    private new void OnMouseDown() {
        base.OnMouseDown();
        if(GameManager.Instance != null && GameManager.CURRENT_TURN == CurrentSide && !GameManager.Instance.IsPcPlaying && !GameManager.Instance.IsPaused && !IsDying) {
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
        if(CurrentSide == GameSide.LeftSide && tile.Column < CurrentTile.Column
            || CurrentSide == GameSide.RightSide && tile.Column > CurrentTile.Column) {      //TODO: Fix animation when Zombie walks reverse
            isFlipped = true;
            FlipSide();
            transform.position = new Vector2(CurrentTile.transform.position.x + OffsetX, CurrentTile.transform.position.y + OffsetY);
        }
        anim.Play("Walk");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieWalk);
        UnMarkAvailableTilesToStep();
        CurrentTile.UnmarkTileInUse();
        CurrentTile.Soldier = null;

        CurrentTile = tile;
        GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row + 1;

        CurrentTile.Soldier = this;
        destination = new Vector2(tile.transform.position.x + OffsetX, tile.transform.position.y + OffsetY);
        isWalking = true;
    }

    public void GetCloser(PlayerSoldier enemy) {

        CoverSoldier();
        enemy.CoverSoldier();

        if(CurrentSide == GameSide.LeftSide && enemy.CurrentTile.Column < CurrentTile.Column
            || CurrentSide == GameSide.RightSide && enemy.CurrentTile.Column > CurrentTile.Column) {      //TODO: Fix animation when Zombie walks reverse
            isFlipped = true;
            if(enemy is Zombie) {
                enemy.FlipSide();
                enemy.transform.position = new Vector2(enemy.CurrentTile.transform.position.x + OffsetX, enemy.CurrentTile.transform.position.y + OffsetY);
            }
            FlipSide();
            transform.position = new Vector2(CurrentTile.transform.position.x + OffsetX, CurrentTile.transform.position.y + OffsetY);
        }

        anim.Play("Walk");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieWalkShort);
        playerCollider.isTrigger = true;
        enemy.PlayerCollider.isTrigger = true;

        if(enemy is Zombie)
            enemy.gameObject.tag = "InWar";
        else if(enemy is Bomb) {
            enemy.gameObject.tag = "BombInWar";
        }
        else {      //enemy is Flag)
            enemy.gameObject.tag = "FlagInWar";
        }

        UnMarkAvailableTilesToStep();
        CurrentTile.UnmarkTileInUse();
        CurrentTile.Soldier = null;
        CurrentTile = enemy.CurrentTile;
        //CurrentTile.Soldier = this;
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

        if(isInWar && other.gameObject.tag == "BombInWar") {
            isInWar = false;
            Bomb bomb = other.gameObject.GetComponent<Bomb>() as Bomb;

            playerCollider.isTrigger = false;
            bomb.PlayerCollider.isTrigger = false;
            StartCoroutine(Explode(bomb));
        }

        if(isInWar && other.gameObject.tag == "FlagInWar") {
            isInWar = false;
            Flag flag = other.gameObject.GetComponent<Flag>() as Flag;
            GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row;
            playerCollider.isTrigger = false;
            flag.PlayerCollider.isTrigger = false;
            StartCoroutine(CollectFlag());
        }
    }

    private IEnumerator Explode(Bomb bomb) {
        anim.Play("Attack");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieAttack);
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row;
        StartCoroutine(bomb.Explode());
        yield return new WaitForSeconds(1f);
        isDieRunning = true;
        StartCoroutine(Die());

        yield return null;
    }

    private IEnumerator Kill(Zombie enemy) {
        anim.Play("Attack");
        enemy.Anim.Play("Attack");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieAttack);
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieAttack);
        yield return new WaitForSeconds(0.5f);
        if(Rank > enemy.Rank) {         //kill enemy
            CurrentTile.Soldier = this;
            enemy.isDieRunning = true;
            StartCoroutine(enemy.Die());
        }
        else if(Rank < enemy.Rank) {    //kill this zombie
            enemy.Anim.Play("Idle");
            enemy.gameObject.tag = "Zombie";
            enemy.CurrentTile.Soldier = enemy;
            if(enemy.isFlipped) enemy.FlipSide();
            isDieRunning = true;
            StartCoroutine(Die());
        }
        else {                          // draw - kill both
            CurrentTile.IsInUse = false;
            isDieRunning = enemy.isDieRunning = true;
            StartCoroutine(Die());
            StartCoroutine(enemy.Die());
        }
        yield return null;
    }

    public bool isDieRunning;
    private IEnumerator Die() {
        if(isDieRunning) {
            isDieRunning = false;
            isDying = true;
            GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row;
            Anim.Play("Die");
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieDie);
            transform.parent = null;
            if(CurrentSide == GameManager.Instance.PcSide) SoldierManager.Instance.EnemyList.Remove(this);
            else SoldierManager.Instance.LocalPlayerList.Remove(this);
            GameManager.Instance.UpdateStats();
            yield return new WaitForSeconds(3f);

            GameManager.Instance.CheckWin(CurrentSide);
            Destroy(gameObject);
        }

        yield return null;
    }

    private IEnumerator CollectFlag() {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.WinGame(CurrentSide);
        yield return null;
    }

    public override void SoldierPlacedInEditMode(bool isSoundActivated) {
        if(isSoundActivated)
            MakeNoise();
    }

    public override void MakeNoise() {
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieBought);
    }
}