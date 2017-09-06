using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierManager: Singleton<SoldierManager> {

    [SerializeField] private List<Zombie> zombiePrototypes;
    [SerializeField] private Bomb bombPrototype;
    [SerializeField] private Flag enemyFlagPrototype;
    [SerializeField] private Flag localFlagPrototype;

    private List<PlayerSoldier> localPlayerList = new List<PlayerSoldier>();
    private List<PlayerSoldier> enemyList = new List<PlayerSoldier>();
    private Zombie selectedPlayer = null;


    public List<PlayerSoldier> LocalPlayerList { get { return localPlayerList; } }
    public List<PlayerSoldier> EnemyList { get { return enemyList; } }

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void MarkSelectedSoldier(Zombie zombie) {
        if(selectedPlayer != null) {
            selectedPlayer.UnMarkAvailableTilesToStep();
        }
        selectedPlayer = zombie;
    }

    public bool HasZombies(GameSide gameSide) {
        if(gameSide == GameManager.Instance.PcSide) {
            foreach(var soldier in enemyList) {
                if(soldier is Zombie)
                    return true;
            }
        }
        else {
            foreach(var solder in localPlayerList) {
                if(solder is Zombie)
                    return true;
            }
        }
        return false;
    }

    public void PlaceSoldier(Tile tile, PlayerSoldier soldier, bool isEnemy = false) {
        PlayerSoldier newSoldier = Instantiate(soldier, transform);
        if(isEnemy) {
            newSoldier.FlipSide();
            EnemyList.Add(newSoldier);
        }
        else {
            LocalPlayerList.Add(newSoldier);
        }
        newSoldier.transform.position = new Vector3(tile.transform.position.x + newSoldier.OffsetX,
            tile.transform.position.y + newSoldier.OffsetY);
        newSoldier.GetComponent<SpriteRenderer>().sortingOrder = tile.Row + 1;
        newSoldier.CurrentTile = tile;
        newSoldier.CurrentTile.Soldier = newSoldier;
        tile.MarkTileInUse();
        //RegisterPlayer(newSoldier);
    }

    public void InitPcBoard() {
        int money = Globals.TOTAL_MONEY;
        Random.InitState(System.DateTime.Now.Millisecond);

        //Get all enemy tiles
        List<Tile> enemyTiles = TileManager.Instance.GetAllEnemyTiles();
        List<Tile> enemyFlagPotentialTiles = TileManager.Instance.GetAllPotentialFlagTiles();
        Tile tile = null;

        //Place Flag
        while(true) {
            tile = enemyFlagPotentialTiles[Random.Range(0, enemyFlagPotentialTiles.Count)];
            if(tile.tag == "EnemyTile") {
                PlaceSoldier(tile, enemyFlagPrototype, true);
                break;
            }
        }

        //Place MAX_BOMBS Bombs
        for(int i = 0; i < Globals.MAX_BOMBS; i++) {
            while(true) {
                tile = enemyTiles[Random.Range(0, enemyTiles.Count)];
                if(tile.tag == "EnemyTile") {
                    PlaceSoldier(tile, bombPrototype, true);
                    money -= bombPrototype.Price;
                    break;
                }
            }
        }

        //Place Zombies
        while(money > Globals.MIN_PRICE && enemyList.Count <= Globals.MAX_SOLDIERS_FOR_PLAYER) {
            Zombie zombie = CreateRandomZombie(money);
            money -= zombie.Price;
            while(true) {
                tile = enemyTiles[Random.Range(0, enemyTiles.Count)];
                if(tile.tag == "EnemyTile") {
                    PlaceSoldier(tile, zombie, true);
                    break;
                }
            }
        }
    }

    private Zombie CreateRandomZombie(int money) {
        Zombie randZombie;
        while(true) {
            randZombie = zombiePrototypes[Random.Range(0, zombiePrototypes.Count)];
            if(randZombie.Price <= money) {      //Can afford to create this zombie
                break;
            }
        }
        return randZombie;
    }

    public IEnumerator MakeRandomMove() {
        List<Tile> tilesToStep;
        Zombie randZombie;
        yield return new WaitForSeconds(2f);
        while(true) {
            randZombie = enemyList[Random.Range(0, enemyList.Count)] as Zombie;
            if(randZombie is Zombie && !randZombie.IsDying) {
                tilesToStep = TileManager.Instance.GetClosestTiles(randZombie.CurrentTile, randZombie);
                if(tilesToStep.Count > 0) {
                    break;
                }
            }
        }
        randZombie.TilesToStep = tilesToStep;
        foreach(var tile in tilesToStep) {
            tile.ReadyToStep(randZombie, true);
        }
        var randTile = tilesToStep[Random.Range(0, tilesToStep.Count)];
        randTile.OnMouseDown();

        yield return null;
    }

    public void ClearSoldiers() {
        foreach(var soldier in localPlayerList) {
            Destroy(soldier.gameObject);
        }
        foreach(var soldier in enemyList) {
            Destroy(soldier.gameObject);
        }
        localPlayerList.Clear();
        enemyList.Clear();
    }

    public void LoadStrategy() {
        int y = 0, z = 0;
        var matrixTile = TileManager.Instance.MatrixTiles;

        Dictionary<string, PlayerSoldier> soldierPrototypes = new Dictionary<string, PlayerSoldier>();
        soldierPrototypes.Add(bombPrototype.name, bombPrototype);
        soldierPrototypes.Add(localFlagPrototype.name, localFlagPrototype);
        soldierPrototypes.Add(enemyFlagPrototype.name, enemyFlagPrototype);
        foreach(var zombie in zombiePrototypes) {
            soldierPrototypes.Add(zombie.name, zombie);
        }

        for(int i = 0; i < Globals.MAX_SOLDIERS_FOR_PLAYER + 1; i++) {
            string tilePattern = PlayerPrefs.GetString(y + "," + z, "");
            if(tilePattern != "") {
                PlaceSoldier(matrixTile[y, z], soldierPrototypes[tilePattern]);
            }
            z++;
            if(z == 4) {
                y++;
                z = 0;
            }
        }
    }

    public void HideAllSoldiers() {
        Animator templateAnimator = zombiePrototypes[8].GetComponent<Animator>();
        foreach(var soldier in enemyList) {
            soldier.HideSoldier(templateAnimator, zombiePrototypes[8].OffsetX, zombiePrototypes[8].OffsetY);
        }
    }

    public void CoverAllSoldiers() {
        foreach(var soldier in localPlayerList)
            soldier.CoverSoldier();
        foreach(var soldier in enemyList)
            soldier.CoverSoldier();
    }

    public void FlipSide() {
        var matrixTiles = TileManager.Instance.MatrixTiles;
        Dictionary<string, Tile> inUsedTiles = new Dictionary<string, Tile>();
        foreach(var soldier in localPlayerList) {
            var tile = matrixTiles[soldier.CurrentTile.Row, Globals.COLUMNS - 1 - soldier.CurrentTile.Column];
            if(tile.IsInUse) {
                inUsedTiles.Add(tile.Row.ToString() + tile.Column.ToString(), tile);
            }
            soldier.FlipSide();
            soldier.CurrentTile.ResetTile();

            soldier.CurrentTile = tile;
            soldier.CurrentTile.IsInUse = true;
            soldier.CurrentTile.Soldier = soldier;
            soldier.transform.position = new Vector2(soldier.CurrentTile.transform.position.x + soldier.OffsetX, soldier.CurrentTile.transform.position.y + soldier.OffsetY);
            if(soldier is Flag) {
                if(GameManager.Instance.PcSide == GameSide.LeftSide) {
                    soldier.Anim.runtimeAnimatorController = enemyFlagPrototype.GetComponent<Animator>().runtimeAnimatorController;
                }
                else {
                    soldier.Anim.runtimeAnimatorController = localFlagPrototype.GetComponent<Animator>().runtimeAnimatorController;
                }
            }
        }


        foreach(var soldier in enemyList) {
            var tile = matrixTiles[soldier.CurrentTile.Row, Globals.COLUMNS - 1 - soldier.CurrentTile.Column];
            if(!inUsedTiles.ContainsKey(soldier.CurrentTile.Row.ToString() + soldier.CurrentTile.Column.ToString())) {
                soldier.CurrentTile.ResetTile();
            }
            soldier.FlipSide();

            soldier.CurrentTile = tile;
            soldier.CurrentTile.IsInUse = true;
            soldier.CurrentTile.Soldier = soldier;
            soldier.transform.position = new Vector2(soldier.CurrentTile.transform.position.x + soldier.OffsetX, soldier.CurrentTile.transform.position.y + soldier.OffsetY);

            if(soldier is Flag) {
                if(GameManager.Instance.PcSide == GameSide.LeftSide) {
                    soldier.Anim.runtimeAnimatorController = localFlagPrototype.GetComponent<Animator>().runtimeAnimatorController;
                }
                else {
                    soldier.Anim.runtimeAnimatorController = enemyFlagPrototype.GetComponent<Animator>().runtimeAnimatorController;
                }
            }
        }
    }
}