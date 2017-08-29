using UnityEngine;

public class Flag : MonoBehaviour {

    [SerializeField] private Tile currentTile;

    private Vector3 originPosition;


    public Tile CurrentTile {
        get {
            return currentTile;
        }
        set {
            currentTile = value;
        }
    }

    public void Start() {
        originPosition = transform.position;
        currentTile.MarkTileInUse();
    }

    void OnMouseDown() {
        if(StrategyEditor.IsInEdit) {
            TileManager.Instance.MarkAvailableBuildTiles();
        }
    }

    void OnMouseDrag() {
        if(StrategyEditor.IsInEdit) {
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    void OnMouseUp() {
        if(StrategyEditor.IsInEdit) {
            TileManager.Instance.UnmarkAvailableBuildTiles();
            if(StrategyEditor.Instance.ChangeFlagPosition(this, originPosition)) {
                originPosition = transform.position;
            }
            else {
                transform.position = originPosition;
            }
        }
    }
}