using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager: Singleton<TileManager> {

    public Dictionary<int, Tile> buildTiles;    // all build tiles
    public Dictionary<int, Tile> allTiles;      // all tiles


    private void Awake() {
        DontDestroyOnLoad(this);
    }

    private void Start() {
        buildTiles = new Dictionary<int, Tile>();
        allTiles = new Dictionary<int, Tile>();

        var buildTilesGameObjects = GameObject.FindGameObjectsWithTag("BuildTile");
        foreach(var tile in buildTilesGameObjects) {
            buildTiles.Add(tile.GetHashCode(), tile.GetComponent<Tile>());
        }

        var tilesGameObjects = FindObjectsOfType<Tile>();
        foreach(var tile in tilesGameObjects) {
            allTiles.Add(tile.GetHashCode(), tile);

        }
    }

    public void MarkAvailableBuildTiles() {
        foreach(var tile in buildTiles.Values) {
            if(tile.tag == "BuildTile") {
                tile.GetComponent<SpriteRenderer>().color = new Color(0.929f, 0.850f, 0.670f);
            }
        }
    }

    public void UnmarkAvailableBuildTiles() {
        foreach(var tile in buildTiles.Values) {
            tile.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
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

    public List<Tile> GetAllPotentialFlagTiles() {
        List<Tile> tiles = new List<Tile>();

        foreach(var tile in allTiles.Values) {
            if(tile.tag == "EnemyTile" && tile.Column == Globals.COLUMNS - 1)
                tiles.Add(tile);
        }

        return tiles;
    }
}