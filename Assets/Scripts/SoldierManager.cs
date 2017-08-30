using System.Collections.Generic;
using UnityEngine;

public class SoldierManager: Singleton<SoldierManager> {

    [SerializeField] private List<Zombie> zombiePrototypes;
    [SerializeField] private Bomb bombPrototype;
    [SerializeField] private Flag enemyFlagPrototype;

    private List<PlayerSoldier> localPlayerList = new List<PlayerSoldier>();
    private List<PlayerSoldier> enemyList = new List<PlayerSoldier>();
    private Zombie selectedPlayer = null;


    public List<PlayerSoldier> LocalPlayerList { get { return localPlayerList; } }

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void MarkSelectedSoldier(Zombie zombie) {
        if(selectedPlayer != null) {
            selectedPlayer.UnMarkAvailableTilesToStep();
        }
        selectedPlayer = zombie;
    }

    public void RegisterPlayer(PlayerSoldier soldier) {
        if(soldier.CurrentSide == GameSide.LeftSide) {
            localPlayerList.Add(soldier);
        }
        else
            enemyList.Add(soldier);
    }

    public void UnregisterPlayer(PlayerSoldier soldier) {
        if(soldier.CurrentSide == GameSide.LeftSide) {
            localPlayerList.Remove(soldier);
        }
        else
            enemyList.Remove(soldier);
    }

    public void PlaceSoldier(Tile tile, PlayerSoldier soldier, bool isEnemy = false) {
        PlayerSoldier newSoldier = Instantiate(soldier, transform);
        if(isEnemy) {
            newSoldier.FlipSide();
        }
        newSoldier.transform.position = new Vector3(tile.transform.position.x + newSoldier.OffsetX,
            tile.transform.position.y + newSoldier.OffsetY);
        newSoldier.GetComponent<SpriteRenderer>().sortingOrder = tile.Row;
        newSoldier.CurrentTile = tile;
        newSoldier.CurrentTile.Soldier = newSoldier;
        tile.MarkTileInUse();
        RegisterPlayer(newSoldier);
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
}