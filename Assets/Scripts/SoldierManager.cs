using System.Collections.Generic;
using UnityEngine;

public class SoldierManager: Singleton<SoldierManager> {

    [SerializeField] private List<Zombie> zombieTypes;

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
        PlayerSoldier newSoldier = Instantiate(soldier, transform);
        newSoldier.transform.position = new Vector3(tile.transform.position.x + newSoldier.OffsetX,
            tile.transform.position.y + newSoldier.OffsetY, 0);
        newSoldier.GetComponent<SpriteRenderer>().sortingOrder = tile.ZIndex;
        newSoldier.CurrentTile = tile;
        tile.MarkTileInUse();
        RegisterPlayer(newSoldier);
    }



    public void InitPcBoard() {
        int money = Globals.TOTAL_MONEY;

        //Get all enemy tiles


        while(money > Globals.MIN_PRICE && enemyList.Count <= Globals.MAX_ZOMBIES_FOR_PLAYER) {
            Zombie zombie = CreateRandomZombie(money);
            money -= zombie.Price;


        }
    }

    public Zombie CreateRandomZombie(int money) {
        Zombie randZombie = null, newZombie = null;
        for(; ; ) {
            randZombie = zombieTypes[Random.Range(0, zombieTypes.Count)];
            if(randZombie.Price <= money) {      //Can afford to create this zombie
                newZombie = Instantiate(randZombie);
                break;
            }
        }
        if(newZombie != null)
            newZombie.gameObject.SetActive(false);
        return newZombie;
    }

}