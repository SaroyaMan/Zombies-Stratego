using UnityEngine;
using UnityEngine.EventSystems;

public class StrategyEditor: Singleton<StrategyEditor> {


    public SoldierBtn PlayerBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    public static bool IsInEdit;
    public static int NumOfBombs;
    public static bool HasFlag;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void Update() {
        HandleEscape();
        if(Input.GetMouseButtonDown(0) && PlayerBtnPressed != null) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if(hit.collider != null && hit.collider.tag == "BuildTile") {   //Check if user clicked on build site
                if(!EventSystem.current.IsPointerOverGameObject() && PlayerBtnPressed != null) {
                    //SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.ZombieBought);
                    
                    Tile tile = hit.transform.gameObject.GetComponent<Tile>();
                    PlaceSoldier(tile, PlayerBtnPressed.SoldierObject);
                    DisableDragSprite();
                }

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

    public void PlaceSoldier(Tile tile, PlayerSoldier soldier, bool isSoundActivated = true) {
        soldier.SoldierPlacedInEditMode(isSoundActivated);
        SoldierManager.Instance.PlaceSoldier(tile, soldier);
        MenuLogic.Instance.BuySoldier(soldier.Price);
    }

    public void SelectedSoldier(SoldierBtn soldierSelected) {
        if(soldierSelected.SoldierObject.Price <= MenuLogic.Instance.Money && SoldierManager.Instance.LocalPlayerList.Count < Globals.MAX_SOLDIERS_FOR_PLAYER) {

            PlayerBtnPressed = soldierSelected;
            EnableDragSprite(PlayerBtnPressed.DragSprite);
            TileManager.Instance.MarkAvailableBuildTiles();

        }
    }

    public bool ChangeSoldierPosition(PlayerSoldier soldier) {
        soldier.gameObject.SetActive(false);
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        soldier.gameObject.SetActive(true);
        if(hit.collider != null && hit.collider.tag == "BuildTile") {   //Check if user clicked on build site
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            soldier.GetComponent<SpriteRenderer>().sortingOrder = tile.Row + 1;
            soldier.CurrentTile.UnmarkTileInUse();
            soldier.CurrentTile.Soldier = null;
            soldier.CurrentTile = tile;
            soldier.CurrentTile.Soldier = soldier;
            soldier.transform.position = new Vector2(tile.transform.position.x + soldier.OffsetX, tile.transform.position.y + soldier.OffsetY);
            tile.MarkTileInUse();
            return true;
        }
        else if(hit.collider != null && (hit.collider.tag == "Zombie" || hit.collider.tag == "Bomb" || hit.collider.tag == "Flag")) {
            PlayerSoldier otherSoldier = hit.transform.gameObject.GetComponent<PlayerSoldier>();
            soldier.GetComponent<SpriteRenderer>().sortingOrder = otherSoldier.CurrentTile.Row + 1;
            otherSoldier.GetComponent<SpriteRenderer>().sortingOrder = soldier.CurrentTile.Row + 1;


            soldier.CurrentTile.Soldier = otherSoldier;
            otherSoldier.CurrentTile.Soldier = soldier;

            var tmpTile = soldier.CurrentTile;
            soldier.CurrentTile = otherSoldier.CurrentTile;
            otherSoldier.CurrentTile = tmpTile;

            var tmpPos = soldier.OriginPosition;
            soldier.transform.position = new Vector2(soldier.CurrentTile.transform.position.x + soldier.OffsetX, soldier.CurrentTile.transform.position.y + soldier.OffsetY);
            otherSoldier.transform.position = new Vector2(otherSoldier.CurrentTile.transform.position.x + otherSoldier.OffsetX, otherSoldier.CurrentTile.transform.position.y + otherSoldier.OffsetY); ;
            return true;
        }
        else if(hit.collider != null && hit.collider.tag == "Trash" && !(soldier is Flag)) {
            soldier.CurrentTile.UnmarkTileInUse();
            SoldierManager.Instance.LocalPlayerList.Remove(soldier);
            MenuLogic.Instance.SellSoldier(soldier.Price);

            if(soldier is Bomb) {
                NumOfBombs--;
                GameView.EnableButton("Btn_Bomb");
            }
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.SoldierSold);
            Destroy(soldier.gameObject);
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
        PlayerBtnPressed = null;
        TileManager.Instance.UnmarkAvailableBuildTiles();
    }
}