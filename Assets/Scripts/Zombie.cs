using UnityEngine;

public class Zombie : MonoBehaviour {

    [SerializeField] private short rank;
    [SerializeField] private int price;

    private Vector3 originPosition;
    private Animator anim;
    private StrategyEditor strategyEditor;

    public Tile CurrentTile { get; set; }
    public short Rank { get { return rank; } }
    public int Price { get { return price; } }

    private void Start() {
        originPosition = transform.position;
        anim = GetComponent<Animator>();
        strategyEditor = StrategyEditor.Instance;
    }

    void OnMouseDown() {
        if(strategyEditor.ZombieBtnPressed == null && StrategyEditor.IsInEdit) {
            TileManager.Instance.MarkAvailableBuildTiles();
        }
    }

    void OnMouseDrag() {
        if(strategyEditor.ZombieBtnPressed == null && StrategyEditor.IsInEdit) {
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    void OnMouseUp() {
        if(strategyEditor.ZombieBtnPressed == null && StrategyEditor.IsInEdit) {
            TileManager.Instance.UnmarkAvailableBuildTiles();
            if(strategyEditor.ChangeZombiePosition(this, originPosition)) {
                originPosition = transform.position;
            }
            else {
                transform.position = originPosition;
            }
        }
    }
}