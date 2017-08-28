using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    //public GameMode GameMode = GameMode.Edit;
    int money = Globals.TOTAL_MONEY;

    public int Money { get { return money; } }

	private void Start () {
        money = PlayerPrefs.GetInt("Money", Globals.TOTAL_MONEY);
        GameView.Instance.SetText("Txt_CurrMoney" ,money.ToString() );
    }
	
    public void SubtractMoney(int price) {
        money -= price;
        PlayerPrefs.SetInt("Money", money);
        GameView.Instance.SetText("Txt_CurrMoney", money.ToString());
    }

    public void SaveStrategy() {
        var savedData = Globals.Instance.SavedData;
        var zombies = ZombieManager.Instance.ZombieList;
        //print("Count " + zombies.Count);
        savedData["Zombies"] = zombies;

        string savedDataString = MiniJSON.Json.Serialize(savedData);
        //print(savedDataString);
        PlayerPrefs.SetString("SaveData", savedDataString);
    }

    public void SaveTile(Tile tile) {
        var savedData = Globals.Instance.SavedData;
        var tileList = new List<TileData>();
        if(savedData.ContainsKey("Tiles")) {
            tileList = savedData["Tiles"] as List<TileData>;
        }
        else savedData.Add("Tiles", tileList);
        tileList.Add(new TileData() { hashCode = tile.GetHashCode(), isInUse = tile.IsInUse});

        string savedDataString = MiniJSON.Json.Serialize(savedData);
        print(savedDataString);
        PlayerPrefs.SetString("SaveData", savedDataString);
    }
}