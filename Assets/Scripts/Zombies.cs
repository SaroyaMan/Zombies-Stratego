using System.Collections.Generic;
using UnityEngine;

public class Zombies : Singleton<Zombies> {

    [SerializeField] private List<Zombie> zombieTypes;

    private List<Zombie> zombieList = new List<Zombie>();
    private List<Zombie> enemyList = new List<Zombie>();


    public List<Zombie> ZombieList { get { return zombieList; } }

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void RegisterZombie(Zombie zombie) {
        zombieList.Add(zombie);
    }

    public void UnregisterZombie(Zombie zombie) {
        zombieList.Remove(zombie);
    }

    public void RegisterEnemy(Zombie zombie) {
        enemyList.Add(zombie);
    }

    public void UnregisterEnemy(Zombie zombie) {
        enemyList.Add(zombie);
    }

    public void PlaceZombie(Tile tile, Zombie zombie) {
        Zombie newZombie = Instantiate(zombie, transform);
        newZombie.transform.position = new Vector2(tile.transform.position.x + 0.25f, tile.transform.position.y + 0.5f);
        newZombie.GetComponent<SpriteRenderer>().sortingOrder = tile.ZIndex;
        newZombie.CurrentTile = tile;
        tile.MarkTileInUse();
        RegisterZombie(newZombie);
    }



    public void InitPcZombies() {
        int money = Globals.TOTAL_MONEY;

        //Get all enemy tiles


        while(money > Globals.MIN_PRICE && enemyList.Count <= Globals.MAX_ZOMBIES_FOR_PLAYER) {
            Zombie zombie = CreateRandomZombie(money);
            money -= zombie.Price;


        }
    }

    public Zombie CreateRandomZombie(int money) {
        Zombie randZombie = null, newZombie = null;
        for(; ;) {
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