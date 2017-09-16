using UnityEngine;

public class Tile: MonoBehaviour {

    [SerializeField] private short row;
    [SerializeField] private short column;

    private Color markColor = new Color(0.929f, 0.850f, 0.670f);
    private Color defaultColor = new Color(1.0f, 1.0f, 1.0f);

    private SpriteRenderer spriteRenderer;
    public bool isReadyToStep;
    private PlayerSoldier soldier;

    private PlayerSoldier attackingZombie;

    public bool IsInUse { get; set; }
    public short Row { get { return row; } }
    public short Column { get { return column; } }
    public PlayerSoldier Soldier { get { return soldier; } set { soldier = value; } }

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ResetTile() {
        if(Column < 4) {
            tag = "BuildTile";
        }
        else if(Column > 11) {
            tag = "EnemyTile";
        }
        else {
            tag = "Tile";
        }
        IsInUse = isReadyToStep = false;
        Soldier = attackingZombie = null;
        UnColorTile();
    }


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

    public void ReadyToStep(PlayerSoldier zombie, bool isPc = false) {
        isReadyToStep = true;
        if(!isPc) ColorTile();
        if(soldier == null || !soldier.IsEnemy(zombie) )
            soldier = zombie;
        else {
            attackingZombie = zombie as Zombie;
        }
    }

    public void UnReadyToStep(PlayerSoldier zombie, bool isPc = false) {
        isReadyToStep = false;
        if(!isPc)  UnColorTile();
        if(soldier != null && zombie == soldier )
            soldier = null;
    }

    public void OnMouseDown() {
        if(soldier != null) {
            Debug.LogError(soldier.name + " " + soldier.CurrentSide.ToString());
        }
        else {
            Debug.LogError("soldier is NULL");
        }
        if(isReadyToStep) {
            isReadyToStep = false;

            if(!Globals.IS_SINGLE_PLAYER)
                GameManager.Instance.PassTurn(soldier.CurrentTile, this);

            if(attackingZombie != null && attackingZombie is Zombie) {
                (attackingZombie as Zombie).GetCloser(soldier);
                attackingZombie = null;
            }

            else if(soldier is Zombie) {
                (soldier as Zombie).Walk(this);
            }
            if(Globals.IS_SINGLE_PLAYER)
                GameManager.Instance.PassTurn();
        }
    }
}