using System.Collections;
using UnityEngine;

public class Zombie : MonoBehaviour {

    private Vector3 originPosition;
    private Animator anim;

    public Tile CurrentTile { get; set; }

    private void Start() {
        originPosition = transform.position;
        anim = GetComponent<Animator>();
    }


    void OnMouseDown() {
        if(ZombieManager.Instance.ZombieBtnPressed == null) {
            TileManager.Instance.MarkAvailableBuildTiles();
            gameObject.SetActive(false);
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            gameObject.SetActive(true);
        }

    }

    void OnMouseDrag() {
        if(ZombieManager.Instance.ZombieBtnPressed == null) {
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    void OnMouseUp() {
        if(ZombieManager.Instance.ZombieBtnPressed == null) {
            TileManager.Instance.UnmarkAvailableBuildTiles();
            if(ZombieManager.Instance.ChangeZombiePosition(this, originPosition)) {
                originPosition = transform.position;
            }
            else {
                transform.position = originPosition;
            }
        }
    }

    //   [SerializeField] private int target = 0;
    //   [SerializeField] private float navigationUpdate;
    //   //[SerializeField] private int healthPoints;

    //   private Transform zombie;
    //   //private Collider2D zombieCollider;
    //   private float navigationTime = 0;
    //   private bool isDead;
    //   private Animator anim;

    //   public bool IsDead {
    //       get {
    //           return isDead;
    //       }
    //   }
    //   // Use this for initialization
    //   private void Start () {
    //       zombie = GetComponent<Transform>();
    //       //zombieCollider = GetComponent<Collider2D>();
    //       anim = GetComponent<Animator>();
    //       //GameManager.Instance.RegisterEnemy(this);
    //   }

    //   // Update is called once per frame
    //   private void Update () {

    //}
}
