using UnityEngine;

public class Tile: MonoBehaviour {

    [SerializeField] private short row;
    [SerializeField] private short column;

    private Color markColor = new Color(0.929f, 0.850f, 0.670f);
    private Color defaultColor = new Color(1.0f, 1.0f, 1.0f);

    private SpriteRenderer spriteRenderer;
    private bool isReadyToStep;
    private PlayerSoldier soldier;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool IsInUse { get; set; }

    public short Row { get { return row; } }
    public short Column { get { return column; } }


    public void MarkTileInUse() {
        IsInUse = true;
        tag = "BuildTileFull";
    }

    public void UnmarkTileInUse() {
        IsInUse = false;
        tag = "BuildTile";
    }

    public void ColorTile() {
        spriteRenderer.color = markColor;
    }

    public void UnColorTile() {
        spriteRenderer.color = defaultColor;
    }

    public void ReadyToStep(Zombie zombie) {
        isReadyToStep = true;
        ColorTile();
        soldier = zombie;
    }

    public void UnReadyToStep() {
        isReadyToStep = false;
        UnColorTile();
        soldier = null;
    }

    public void OnMouseDown() {
        if(isReadyToStep) {
            if(soldier is Zombie) {
                (soldier as Zombie).Walk(this);
                IsInUse = true;
            }
        }
    }
}