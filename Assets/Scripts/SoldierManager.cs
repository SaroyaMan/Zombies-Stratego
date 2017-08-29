using System.Collections.Generic;
using UnityEngine;

public class SoldierManager: Singleton<SoldierManager> {

    [SerializeField] private List<Zombie> zombiPrototypes;
    [SerializeField] private Bomb bombPrototype;

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

    public void PlaceSoldier(Tile tile, PlayerSoldier soldier) {
        PlayerSoldier newSoldier = Instantiate(soldier);
        newSoldier.transform.position = new Vector3(tile.transform.position.x + newSoldier.OffsetX,
            tile.transform.position.y + newSoldier.OffsetY);
        newSoldier.GetComponent<SpriteRenderer>().sortingOrder = tile.ZIndex;
        newSoldier.CurrentTile = tile;
        tile.MarkTileInUse();
        RegisterPlayer(newSoldier);
    }



    public void InitPcBoard() {
        int money = Globals.TOTAL_MONEY;

        //Get all enemy tiles

        //Place MAX_BOMBS Bombs
        for(int i = 0; i < Globals.MAX_BOMBS; i++) {
            //Bomb bomb = CreateRandomBomb();
            money -= bomb.Price;
            RegisterEnemy(bomb);
            
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
            randZombie = zombiPrototypes[Random.Range(0, zombiPrototypes.Count)];
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