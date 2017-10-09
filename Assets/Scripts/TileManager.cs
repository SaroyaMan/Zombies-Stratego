using System.Collections.Generic;
using UnityEngine;

public class TileManager: Singleton<TileManager> {

    private Dictionary<int, Tile> buildTiles;    // all build tiles
    private Dictionary<int, Tile> allTiles;      // all tiles
    private List<Tile> potentialFlagTiles;
    private Tile[,] matrixTiles;

    public Dictionary<int, Tile> BuildTiles { get { return buildTiles; } }
    public Dictionary<int, Tile> AllTiles { get { return allTiles; } }
    public List<Tile> PotentialFlagTiles {  get { return potentialFlagTiles; } }
    public Tile[,] MatrixTiles { get { return matrixTiles; } }


    private void Awake() {
        DontDestroyOnLoad(this);
        Init();
    }

    private void Init() {
        buildTiles = new Dictionary<int, Tile>();
        allTiles = new Dictionary<int, Tile>();
        matrixTiles = new Tile[Globals.ROWS, Globals.COLUMNS];

        var buildTilesGameObjects = GameObject.FindGameObjectsWithTag("BuildTile");
        foreach(var tile in buildTilesGameObjects) {
            buildTiles.Add(tile.GetHashCode(), tile.GetComponent<Tile>());
        }

        var tilesGameObjects = FindObjectsOfType<Tile>();
        foreach(var tile in tilesGameObjects) {
            allTiles.Add(tile.GetHashCode(), tile);
            matrixTiles[tile.Row, tile.Column] = tile;
        }

        potentialFlagTiles = GetAllPotentialFlagTiles();
    }

    public void ClearTiles() {
        foreach(var tile in allTiles.Values) {
            tile.ResetTile();
        }
    }

    public void MarkAvailableBuildTiles() {
        foreach(var tile in buildTiles.Values) {
            if(tile.tag == "BuildTile") {
                tile.ColorTile();
            }
        }
    }

    public void UnmarkAvailableBuildTiles() {
        foreach(var tile in buildTiles.Values) {
            tile.UnColorTile();
        }
    }

    public void RenameTagsBuildTiles() {
        foreach(var tile in buildTiles.Values) {
            tile.UnmarkTileInUse();
        }
    }

    public List<Tile> GetAllEnemyTiles() {
        List<Tile> tiles = new List<Tile>();

        foreach(var tile in allTiles.Values) {
            if(tile.tag == "EnemyTile")
                tiles.Add(tile);
        }

        return tiles;
    }

    private List<Tile> GetAllPotentialFlagTiles() {
        List<Tile> tiles = new List<Tile>();

        //foreach(var tile in allTiles.Values) {
        //    if(tile.tag == "EnemyTile" && tile.Column == Globals.COLUMNS - 1)
        //        tiles.Add(tile);
        //}
        for(int i = 0; i < Globals.ROWS; i++) {
            if(matrixTiles[i, Globals.COLUMNS - 1].tag == "EnemyTile")
                tiles.Add(matrixTiles[i, Globals.COLUMNS - 1]);
        }
        return tiles;
    }

    public List<Tile> GetClosestTiles(Tile tile, Zombie zombie) {
        List<Tile> tiles = new List<Tile>();
        Tile tempTile = null;

        // Check up tile
        if(tile.Row == 0) {
            tempTile = null;
        }
        else tempTile = matrixTiles[tile.Row - 1, tile.Column];
        if(tempTile != null && (tempTile.Soldier == null || tempTile.Soldier.IsEnemy(zombie))) {
            tiles.Add(tempTile);
        }

        // Check right tile
        if(tile.Column + 1 == Globals.COLUMNS) {        
            tempTile = null;
        }
        else tempTile = matrixTiles[tile.Row, tile.Column + 1];
        if(tempTile != null && (tempTile.Soldier == null || tempTile.Soldier.IsEnemy(zombie) )) {
            tiles.Add(tempTile);
        }

        // Check down tile
        if(tile.Row + 1 == Globals.ROWS) {
            tempTile = null;
        }
        else tempTile = matrixTiles[tile.Row + 1, tile.Column];
        if(tempTile != null && (tempTile.Soldier == null || tempTile.Soldier.IsEnemy(zombie))) {
            tiles.Add(tempTile);
        }

        // Check left tile
        if(tile.Column == 0) {
            tempTile = null;
        }
        else tempTile = matrixTiles[tile.Row, tile.Column - 1];
        if(tempTile != null && (tempTile.Soldier == null || tempTile.Soldier.IsEnemy(zombie))) {
            tiles.Add(tempTile);
        }

        return tiles;
    }
}