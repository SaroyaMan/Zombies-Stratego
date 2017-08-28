using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StrategyEditor : Singleton<StrategyEditor> {

    public ZombieBtn ZombieBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    public static bool IsInEdit;

    private void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
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
            Zombies.Instance.PlaceZombie(tile, ZombieBtnPressed.ZombieObject);
            BuyZombie(ZombieBtnPressed.ZombieObject.Price);
            DisableDragSprite();
        }
    }


    public void BuyZombie(int price) {
        MenuLogic.Instance.SubtractMoney(price);
    }

    public void SelectedZombie(ZombieBtn zombieSelected) {
        if(zombieSelected.ZombieObject.Price <= MenuLogic.Instance.Money && Zombies.Instance.ZombieList.Count < Globals.MAX_ZOMBIES_FOR_PLAYER) {
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

    public bool ChangeFlagPosition(Flag flag, Vector3 oldPosition) {
        flag.gameObject.SetActive(false);
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        flag.gameObject.SetActive(true);

        if(hit.collider != null && hit.collider.tag == "BuildTile") {   //Check if user clicked on build site
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            flag.GetComponent<SpriteRenderer>().sortingOrder = tile.ZIndex;
            flag.CurrentTile.UnmarkTileInUse();
            flag.CurrentTile = tile;
            flag.transform.position = new Vector2(tile.transform.position.x + 0.20f, tile.transform.position.y + 0.5f);
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