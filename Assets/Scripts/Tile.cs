using UnityEngine;

public class Tile: MonoBehaviour {

    [SerializeField] private short row;
    [SerializeField] private short column;


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
}
