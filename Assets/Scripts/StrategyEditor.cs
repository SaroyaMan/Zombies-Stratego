using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StrategyEditor : Singleton<StrategyEditor> {

    public SoldierBtn PlayerBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    public static bool IsInEdit;
    public static int NumOfBombs;
    public static bool HasFlag;

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    private void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        
    }

	private void Update () {
        HandleEscape();
        if(Input.GetMouseButtonDown(0) && PlayerBtnPressed != null) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if(hit.collider != null && hit.collider.tag == "BuildTile") {   //Check if user clicked on build site
                Tile tile = hit.transform.gameObject.GetComponent<Tile>();
                PlaceSoldier(tile);
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

    public void PlaceSoldier(Tile tile) {
        if(!EventSystem.current.IsPointerOverGameObject() && PlayerBtnPressed != null) {
            if(PlayerBtnPressed.SoldierObject is Bomb) {
                NumOfBombs++;
                if(NumOfBombs == Globals.MAX_BOMBS) {
                    GameView.Instance.DisableButton(PlayerBtnPressed.GetComponent<Button>() );
                }
            }
            if(PlayerBtnPressed.SoldierObject is Flag) {
                HasFlag = true;
                GameView.Instance.DisableButton(PlayerBtnPressed.GetComponent<Button>());
            }
            SoldierManager.Instance.PlaceSoldier(tile, PlayerBtnPressed.SoldierObject);
            MenuLogic.Instance.BuySoldier(PlayerBtnPressed.SoldierObject.Price);
            DisableDragSprite();
        }
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
            soldier.GetComponent<SpriteRenderer>().sortingOrder = tile.Row;
            soldier.CurrentTile.UnmarkTileInUse();
            soldier.CurrentTile = tile;
            soldier.transform.position = new Vector2(tile.transform.position.x + soldier.OffsetX, tile.transform.position.y + soldier.OffsetY);
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
        PlayerBtnPressed = null;
        TileManager.Instance.UnmarkAvailableBuildTiles();
    }
}