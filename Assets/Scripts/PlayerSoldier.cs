using UnityEngine;

public abstract class PlayerSoldier: MonoBehaviour {

    [SerializeField] private short rank;
    [SerializeField] private int price;
    [SerializeField] private float offset_x;
    [SerializeField] private float offset_y;

    private StrategyEditor strategyEditor;
    private Vector3 originPosition;
    private Animator anim;

    public short Rank { get { return rank; } }
    public int Price { get { return price; } }
    public float OffsetX { get { return offset_x; } }
    public float OffsetY { get { return offset_y; } }
    public Tile CurrentTile { get; set; }

    private void Awake() {
        originPosition = transform.position;
        anim = GetComponent<Animator>();
        strategyEditor = StrategyEditor.Instance;
    }

    public void FlipSide() {
        offset_x = -offset_x;
        GetComponent<SpriteRenderer>().flipX = true;
    }

    private void OnMouseDown() {
        if(strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            TileManager.Instance.MarkAvailableBuildTiles();
        }
    }

    private void OnMouseDrag() {
        if(strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    private void OnMouseUp() {
        if(strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            TileManager.Instance.UnmarkAvailableBuildTiles();
            if(strategyEditor.ChangeSoldierPosition(this)) {
                originPosition = transform.position;
            }
            else {
                transform.position = originPosition;
            }
        }
    }
}