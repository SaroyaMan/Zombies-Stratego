using UnityEngine;

public abstract class PlayerSoldier: MonoBehaviour {

    [SerializeField] private short rank;
    [SerializeField] private int price;
    [SerializeField] private float offset_x;
    [SerializeField] private float offset_y;

    private StrategyEditor strategyEditor;
    private SpriteRenderer spriteRenderer;
    private Color blueColor = new Color(0, 0, 1, 0.6f);
    private Color redColor = new Color(1, 0, 0, 0.6f);
    private Color defaultColor = new Color(1, 1, 1, 1);


    protected Vector3 originPosition;
    protected Animator anim;
    protected RuntimeAnimatorController originAnim;
    protected PolygonCollider2D playerCollider;
    protected bool isHidden;

    public short Rank { get { return rank; } }
    public int Price { get { return price; } }
    public float OffsetX { get { return offset_x; } }
    public float OffsetY { get { return offset_y; } }
    public Tile CurrentTile { get; set; }
    public GameSide CurrentSide { get; set; }
    public Animator Anim { get { return anim; } set { anim = value; } }
    public RuntimeAnimatorController OriginAnim { get { return originAnim; } }

    public PolygonCollider2D PlayerCollider { get { return playerCollider; } }
    public Vector3 OriginPosition { get { return originPosition; } }


    private void Awake() {
        anim = GetComponent<Animator>();
        originAnim = anim.runtimeAnimatorController;
        playerCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        strategyEditor = StrategyEditor.Instance;
    }

    public void FlipSide() {
        offset_x = -offset_x;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        CurrentSide = CurrentSide == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
    }

    public bool IsEnemy(PlayerSoldier enemy) {
        return CurrentSide != enemy.CurrentSide;
    }

    protected void OnMouseDown() {
        if(strategyEditor != null && strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            originPosition = transform.position;
            TileManager.Instance.MarkAvailableBuildTiles();
        }
    }

    protected void OnMouseDrag() {
        if(strategyEditor != null && strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    protected void OnMouseUp() {
        if(strategyEditor != null && strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            TileManager.Instance.UnmarkAvailableBuildTiles();
            if(strategyEditor.ChangeSoldierPosition(this)) {
                originPosition = transform.position;
            }
            else {
                transform.position = originPosition;
            }
        }
    }

    public void HideSoldier(Animator templateAnimator) {
        if(!isHidden) {
            isHidden = true;
            Anim.runtimeAnimatorController = templateAnimator.runtimeAnimatorController;
            spriteRenderer.color = CurrentSide == GameSide.LeftSide ? blueColor : redColor;
        }
    }

    public void CoverSoldier() {
        if(isHidden) {
            isHidden = false;
            Anim.runtimeAnimatorController = OriginAnim;
            spriteRenderer.color = defaultColor;
        }
    }
}