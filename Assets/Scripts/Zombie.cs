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
    private bool isExploder;
    private bool isCopycat;
    private Vector2 destination;

    public bool IsDying { get { return isDying; } }
    public bool IsCopycat { get { return isCopycat; } set { isCopycat = value; } }
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
                    isFlipped = false;
                    FlipSide(false);
                    transform.position = new Vector2(CurrentTile.transform.position.x + OffsetX, CurrentTile.transform.position.y + OffsetY);
                };
            }
        }
    }

    private new void OnMouseDown() {
        base.OnMouseDown();
        if(IsSinglePlayerAllowClick() || IsMultiPlayerAllowClick()) {
            if(!isGridMarked && !isDying) {
                SoldierManager.Instance.MarkSelectedSoldier(this);
                MarkAvailableTilesToStep();
                StartCoroutine(SoldierManager.Instance.MarkEnemyTiles());
            }
            else {
                UnMarkAvailableTilesToStep();
            }
        }
    }

    private bool IsSinglePlayerAllowClick() {
        return Globals.IS_SINGLE_PLAYER && GameManager.Instance != null && GameManager.CURRENT_TURN == CurrentSide && !GameManager.Instance.IsPcPlaying && !GameManager.Instance.IsPaused && !IsDying;
    }

    private bool IsMultiPlayerAllowClick() {
        return !Globals.IS_SINGLE_PLAYER && !GameManager.Instance.IsPaused && !IsDying && MultiPlayerManager.Instance.PlayerSide == CurrentSide && CurrentSide == GameManager.CURRENT_TURN;
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
            //SoldierManager.Instance.UnMarkEnemyTiles();
        }
    }

    public void Walk(Tile tile) {
        tile.IsInUse = true;
        if(CurrentSide == GameSide.LeftSide && tile.Column < CurrentTile.Column
            || CurrentSide == GameSide.RightSide && tile.Column > CurrentTile.Column) {
            isFlipped = true;
            FlipSide(false);
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
            || CurrentSide == GameSide.RightSide && enemy.CurrentTile.Column > CurrentTile.Column) {
            isFlipped = true;
            if(enemy is Zombie) {
                (enemy as Zombie).isFlipped = true;
                enemy.FlipSide(false);
                enemy.transform.position = new Vector2(enemy.CurrentTile.transform.position.x + OffsetX, enemy.CurrentTile.transform.position.y + OffsetY);
            }
            FlipSide(false);
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
            other.gameObject.tag = "Zombie";
        }

        if(isInWar && other.gameObject.tag == "BombInWar") {
            isInWar = false;
            Bomb bomb = other.gameObject.GetComponent<Bomb>() as Bomb;

            playerCollider.isTrigger = false;
            bomb.PlayerCollider.isTrigger = false;
            StartCoroutine(Explode(bomb));
            other.gameObject.tag = "Zombie";
        }

        if(isInWar && other.gameObject.tag == "FlagInWar") {
            isInWar = false;
            Flag flag = other.gameObject.GetComponent<Flag>() as Flag;
            GameManager.Instance.ShowStars(flag);
            GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row;
            playerCollider.isTrigger = false;
            flag.PlayerCollider.isTrigger = false;
            StartCoroutine(CollectFlag());
            other.gameObject.tag = "Flag";
        }
    }

    private IEnumerator Explode(Bomb bomb) {
        anim.Play("Attack");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieAttack);
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row;
        StartCoroutine(bomb.Explode(Rank != Globals.RANK_OF_SAPPER));
        yield return new WaitForSeconds(1f);
        if(Rank != Globals.RANK_OF_SAPPER) {
            isDieRunning = true;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Kill(Zombie enemy) {
        //bool isCopycat = false;
        if(Globals.IS_SINGLE_PLAYER && Rank == Globals.RANK_OF_COPYCAT) {
            rank = (short) Random.Range(0, 16);
            IsCopycat = true;
        }
        if(IsCopycat) {
            Globals.Instance.UnityObjects["Smoke"].transform.position = new Vector2(transform.position.x, transform.position.y - 1);
            Globals.Instance.UnityObjects["Smoke"].GetComponent<ParticleSystem>().Play();
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.Copycat);
        }

        if(enemy.Rank == Globals.RANK_OF_EXPLODER) {
            enemy.rank = Rank;
            enemy.isExploder = true;
        }

        anim.Play("Attack");
        enemy.Anim.Play("Attack");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieAttack);
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieAttack);
        yield return new WaitForSeconds(0.5f);

        if(Rank > enemy.Rank || !isCopycat && (Rank == 1 && enemy.Rank >= 13 || Rank == 2 && enemy.Rank >= 14 || Rank == 3 && enemy.Rank == 15)) {         //kill enemy
            if(IsCopycat) {
                rank = Globals.RANK_OF_COPYCAT;
                IsCopycat = false;
            };
            CurrentTile.Soldier = this;
            enemy.isDieRunning = true;
            StartCoroutine(enemy.Die());
        }
        else if(Rank < enemy.Rank) {    //kill this zombie
            enemy.Anim.Play("Idle");
            enemy.CurrentTile.Soldier = enemy;
            if(enemy.isFlipped) {
                enemy.FlipSide(false);
                enemy.transform.position = new Vector2(enemy.CurrentTile.transform.position.x + OffsetX, enemy.CurrentTile.transform.position.y + OffsetY);
                enemy.isFlipped = false;
            }
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
            UnMarkAvailableTilesToStep();
            if(isExploder) {
                Anim.Play("Exploding");
                SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.Explode);
            }
            else {
                GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.Row;
                Anim.Play("Die");
                SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieDie);
            }
            transform.parent = null;
            if(Globals.IS_SINGLE_PLAYER && CurrentSide == GameManager.Instance.PcSide || !Globals.IS_SINGLE_PLAYER && MultiPlayerManager.Instance.PlayerSide != CurrentSide)
                SoldierManager.Instance.EnemyList.Remove(this);
            else
                SoldierManager.Instance.LocalPlayerList.Remove(this);
            GameManager.Instance.UpdateStats();
            yield return new WaitForSeconds(3f);
            GameManager.Instance.CloseInfo();
            GameManager.Instance.CheckWin(CurrentSide);
            Destroy(gameObject);
        }

        yield return null;
    }

    private IEnumerator CollectFlag() {
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.WinGame(CurrentSide);
        yield return null;
    }

    public override void SoldierPlacedInEditMode(bool isSoundActivated) {
        if(isSoundActivated)
            MakeNoise();
        if(Rank == Globals.RANK_OF_SAPPER && ++StrategyEditor.NumOfSappers == Globals.MAX_SAPPERS) {
            GameView.DisableButton("Btn_Zombie9");
        }
    }

    public override void MakeNoise() {
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieBought);
    }
}