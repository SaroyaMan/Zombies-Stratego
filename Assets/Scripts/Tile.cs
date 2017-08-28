using UnityEngine;

public class Tile: MonoBehaviour {

    [SerializeField] private short zIndex;

    public bool IsInUse { get; set; }

    public short ZIndex { get { return zIndex; } }


    public void MarkTileInUse() {
        IsInUse = true;
        tag = "BuildTileFull";
        GameManager.Instance.SaveTile(this);
    }

    public void UnmarkTileInUse() {
        IsInUse = false;
        tag = "BuildTile";
        GameManager.Instance.SaveTile(this);
    }
}
