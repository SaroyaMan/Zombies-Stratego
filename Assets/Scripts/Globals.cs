using System.Collections.Generic;
using UnityEngine;

//public enum GameMode { Edit, SinglePlayer, Multiplayer }

public enum MenuScreens {
    Default, Main, MultiPlayer, StudentInfo, Options, Edit
}

public enum GameScreens {
    Default, InGame, Pause, Options
}

public enum GameSide {
    LeftSide, RightSide
}

public class Globals : Singleton<Globals> {

    public const int TOTAL_MONEY = 200;
    public const int MAX_SOLDIERS_FOR_PLAYER = 35; //9 rows on 4 columns = 36. 36 - 1 (flag) = 35
    public const int MIN_SOLDIERS_ALLOWED = 20;
    public const int ROWS = 9;
    public const int COLUMNS = 16;
    public const int MIN_PRICE = 1;
    public const int MAX_BOMBS = 6;
    public const int MAX_SAPPERS = 3;
    public const int MAX_TURN_TIME = 30;
    public const int RANK_OF_SAPPER = 9;
    public const int RANK_OF_COPYCAT = 11;
    public const int RANK_OF_EXPLODER = 6;
    public const string GITHUB_PROFILE_URL = "https://github.com/SaroyaMan";
    public const string CV_URL = "https://drive.google.com/file/d/0B8BaWfqNelVKTjEwRHFnSkVXMnM/view";

    public const string API_KEY = "1dc818fddbb3315fed817a82c40754049e7ebaa74c2cdfe7df2bfed170262bd9";
    public const string SECRET_KEY = "b95a9bc59465bd0ce4f3bc5b01fc2464a0be156452afb2282b77970adeb00327";

    public static bool IS_IN_GAME;
    public static bool IS_SINGLE_PLAYER = true;

    public MenuScreens currentScreen;

    private Dictionary<string, GameObject> unityObjects;

    public Dictionary<string, GameObject> UnityObjects { get { return unityObjects; } }

    private void Awake() {
        Init();
    }

    private void Init() {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach(var ob in gameObjects) {
            unityObjects.Add(ob.name, ob);
        }
        //PlayerPrefs.DeleteAll();    //For testing
    }

    public Dictionary<string, SoldierBtn> GetAllSoldierBtns() {
        var soldierBtns = FindObjectsOfType<SoldierBtn>();
        Dictionary<string, SoldierBtn> soldierButtonsDic = new Dictionary<string, SoldierBtn>();
        foreach(var btn in soldierBtns) {
            soldierButtonsDic.Add(btn.SoldierObject.name, btn);
        }
        return soldierButtonsDic;
    }
}