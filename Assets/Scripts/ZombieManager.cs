using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZombieManager : Singleton<ZombieManager> {

    public ZombieBtn ZombieBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    private List<Zombie> zombieList = new List<Zombie>();
    //private List<Collider2D> buildList = new List<Collider2D>();
    //private Collider2D buildTile;

    public List<Zombie> ZombieList { get { return zombieList; } }

    private void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;

        //LoadZombies();
    }

	private void Update () {
        HandleEscape();
        if(Input.GetMouseButtonDown(0) && ZombieBtnPressed != null) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if(hit.collider != null && hit.collider.tag == "BuildTile") {   //Check if user clicked on build site
                Tile tile = hit.transform.gameObject.GetComponent<Tile>();
                PlaceZombie(tile);
            }
        }
        if(spriteRenderer.enabled) {
            FollowMouse();
        }
    }

    private void HandleEscape() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1)) {
            DisableDragSprite();
        }
    }

    public void PlaceZombie(Tile tile) {
        if(!EventSystem.current.IsPointerOverGameObject() && ZombieBtnPressed != null) {
            //GameManager.Instance.SoundFx.PlayOneShot(SoundManager.Instance.Towerbuilt);
            Zombie newZombie = Instantiate(ZombieBtnPressed.ZombieObject);
            //newZombie.transform.position = hit.transform.position;
            newZombie.transform.position = new Vector2(tile.transform.position.x + 0.25f, tile.transform.position.y + 0.5f);
            newZombie.GetComponent<SpriteRenderer>().sortingOrder = tile.ZIndex;
            newZombie.CurrentTile = tile;
            tile.MarkTileInUse();
            BuyZombie(ZombieBtnPressed.ZombiePrice);
            RegisterZombie(newZombie);
            DisableDragSprite();
        }
    }

    public void RegisterZombie(Zombie zombie) {
        zombieList.Add(zombie);
    }

    public void UnregisterZombie(Zombie zombie) {
        zombieList.Remove(zombie);
    }

    public void BuyZombie(int price) {
        GameManager.Instance.SubtractMoney(price);
    }

    public void SelectedZombie(ZombieBtn zombieSelected) {
        if(zombieSelected.ZombiePrice <= GameManager.Instance.Money && zombieList.Count <= Globals.MAX_ZOMBIES_FOR_PLAYER) {
            ZombieBtnPressed = zombieSelected;
            EnableDragSprite(ZombieBtnPressed.DragSprite);
            TileManager.Instance.MarkAvailableBuildTiles();
        }
    }

    public bool ChangeZombiePosition(Zombie zombie, Vector3 oldPosition) {
        zombie.gameObject.SetActive(false);
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        zombie.gameObject.SetActive(true);
        if(hit.collider != null && hit.collider.tag == "BuildTile") {   //Check if user clicked on build site
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            zombie.GetComponent<SpriteRenderer>().sortingOrder = tile.ZIndex;
            zombie.CurrentTile.UnmarkTileInUse();
            zombie.CurrentTile = tile;
            zombie.transform.position = new Vector2(tile.transform.position.x + 0.25f, tile.transform.position.y + 0.5f);
            tile.MarkTileInUse();
            return true;
        }
        return false;
    }

    public void FollowMouse() {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void EnableDragSprite(Sprite sprite) {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void DisableDragSprite() {
        spriteRenderer.enabled = false;
        ZombieBtnPressed = null;
        TileManager.Instance.UnmarkAvailableBuildTiles();
    }
}