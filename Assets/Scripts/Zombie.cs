using UnityEngine;

public class Zombie : MonoBehaviour {

    [SerializeField] private short rank;

    private Vector3 originPosition;
    private Animator anim;

    public static bool IsInEdit;
    public Tile CurrentTile { get; set; }
    public short Rank { get { return rank; } }

    private void Start() {
        originPosition = transform.position;
        anim = GetComponent<Animator>();
    }

    void OnMouseDown() {
        if(ZombieManager.Instance.ZombieBtnPressed == null && IsInEdit) {
            TileManager.Instance.MarkAvailableBuildTiles();
        }

    }

    void OnMouseDrag() {
        if(ZombieManager.Instance.ZombieBtnPressed == null && IsInEdit) {
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    void OnMouseUp() {
        if(ZombieManager.Instance.ZombieBtnPressed == null && IsInEdit) {
            TileManager.Instance.UnmarkAvailableBuildTiles();
            if(ZombieManager.Instance.ChangeZombiePosition(this, originPosition)) {
                originPosition = transform.position;
            }
            else {
                transform.position = originPosition;
            }
        }
    }
}