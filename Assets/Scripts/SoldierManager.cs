using System.Collections.Generic;
using UnityEngine;

public class SoldierManager: Singleton<SoldierManager> {

    [SerializeField] private List<Zombie> zombiePrototypes;
    [SerializeField] private Bomb bombPrototype;
    [SerializeField] private Flag enemyFlagPrototype;

    private List<PlayerSoldier> localPlayerList = new List<PlayerSoldier>();
    private List<PlayerSoldier> enemyList = new List<PlayerSoldier>();


    public List<PlayerSoldier> LocalPlayerList { get { return localPlayerList; } }

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void RegisterPlayer(PlayerSoldier soldier) {
        localPlayerList.Add(soldier);
    }

    public void UnregisterPlayer(PlayerSoldier soldier) {
        localPlayerList.Remove(soldier);
    }

    public void RegisterEnemy(PlayerSoldier soldier) {
        enemyList.Add(soldier);
    }

    public void UnregisterEnemy(PlayerSoldier soldier) {
        enemyList.Add(soldier);
    }

    public void PlaceSoldier(Tile tile, PlayerSoldier soldier, bool isEnemy = false) {
        PlayerSoldier newSoldier = Instantiate(soldier, this.transform);
        newSoldier.transform.position = new Vector3(tile.transform.position.x + newSoldier.OffsetX,
            tile.transform.position.y + newSoldier.OffsetY);
        newSoldier.GetComponent<SpriteRenderer>().sortingOrder = tile.Row;
        newSoldier.CurrentTile = tile;
        tile.MarkTileInUse();

        if(isEnemy) {
            RegisterEnemy(newSoldier);
        }
        else {
            RegisterPlayer(newSoldier);
        }
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
            if(tile.tag == "EnemyTile" && tile.Row != 5) {
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


        //while(money > Globals.MIN_PRICE && enemyList.Count <= Globals.MAX_ZOMBIES_FOR_PLAYER) {
        //    Zombie zombie = CreateRandomZombie(money);
        //    money -= zombie.Price;
        //    RegisterEnemy(zombie);

        //}
    }

    public Zombie CreateRandomZombie(int money) {
        Zombie randZombie = null, newZombie = null;
        for(; ; ) {
            randZombie = zombiePrototypes[Random.Range(0, zombiePrototypes.Count)];
            if(randZombie.Price <= money) {      //Can afford to create this zombie
                newZombie = Instantiate(randZombie);
                break;
            }
        }
        if(newZombie != null)
            newZombie.gameObject.SetActive(false);
        return newZombie;
    }

    //private Bomb CreateRandomBomb() {

    //}

    //public Tile PlaceZombieInRandomTile(Zombie zombie) {

    //}

}