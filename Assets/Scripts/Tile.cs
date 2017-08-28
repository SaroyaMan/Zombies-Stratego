using UnityEngine;

public class Tile: MonoBehaviour {

    [SerializeField] private short zIndex;

    public bool IsInUse { get; set; }

    public short ZIndex { get { return zIndex; } }


    public void MarkTileInUse() {
        IsInUse = true;
        tag = "BuildTileFull";
    }

    public void UnmarkTileInUse() {
        IsInUse = false;
        tag = "BuildTile";
    }
}
