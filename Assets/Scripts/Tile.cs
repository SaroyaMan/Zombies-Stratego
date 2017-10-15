using UnityEngine;

public class Tile: MonoBehaviour {

    [SerializeField] private short row;
    [SerializeField] private short column;

    private Color markColor = new Color(0.929f, 0.850f, 0.670f);
    private Color defaultColor = new Color(1.0f, 1.0f, 1.0f);

    private Color blueColor = new Color(0, 0, 1, 0.6f);
    private Color redColor = new Color(0.615f, 0.027f, 0.054f, 0.7f);

    private SpriteRenderer spriteRenderer;
    private bool isReadyToStep;
    private PlayerSoldier soldier;

    private PlayerSoldier attackingZombie;

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
        else if(Column > 7) {
            tag = "EnemyTile";
        }
        else {
            tag = "Tile";
        }
        isReadyToStep = false;
        Soldier = attackingZombie = null;
        UnColorTile();
    }


    public void MarkTileInUse() {
        tag = "BuildTileFull";
    }

    public void UnmarkTileInUse() {
        tag = "BuildTile";
    }

    public bool IsMarked() {
        return spriteRenderer.color == markColor;
    }

    public void ColorTile() {
        spriteRenderer.color = markColor;
    }

    public void UnColorTile() {
        spriteRenderer.color = defaultColor;
    }

    public void MarkSoldierSide() {
        if(spriteRenderer.color != markColor)
            spriteRenderer.color = soldier.CurrentSide == GameSide.LeftSide ? blueColor : redColor;
    }

    public void ReadyToStep(PlayerSoldier zombie, bool isPc = false) {
        isReadyToStep = true;
        if(!isPc) ColorTile();
        //if(soldier == null || !soldier.IsEnemy(zombie)) {
        //    soldier = zombie;
        //}
        //else {
        //    attackingZombie = zombie as Zombie;
        //}
        if(soldier != null && soldier.IsEnemy(zombie)) {
            attackingZombie = zombie as Zombie;
        }
        else if(soldier == null) {
            soldier = zombie;
        }
    }

    public void UnReadyToStep(PlayerSoldier zombie, bool isPc = false) {
        isReadyToStep = false;
        if(!isPc) UnColorTile();
        if(soldier != null && zombie == soldier)
            soldier = null;
        attackingZombie = null;
    }

    public void OnMouseDown() {
        if(soldier != null) {
            Debug.LogError(soldier.name + " " + soldier.CurrentSide.ToString() + " Rank = " + soldier.Rank + "isReadyToStep = " + isReadyToStep);
        }
        else {
            Debug.LogError("soldier is NULL, isReadyToStep = " + isReadyToStep);
        }
        if(isReadyToStep && !Globals.IS_SINGLE_PLAYER) {
            GameManager.Instance.PassTurn(attackingZombie == null ? soldier.CurrentTile : attackingZombie.CurrentTile, this);
        }
        MakeStep();
    }

    public void MakeStep() {
        if(isReadyToStep) {
            isReadyToStep = false;

            if(attackingZombie != null && attackingZombie is Zombie) {
                (attackingZombie as Zombie).GetCloser(soldier);
                attackingZombie = null;
            }

            else if(soldier is Zombie) {
                (soldier as Zombie).Walk(this);
            }
            if(Globals.IS_SINGLE_PLAYER && !(soldier is Flag) )
                GameManager.Instance.PassTurn();
        }
    }
}