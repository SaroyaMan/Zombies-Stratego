using System;
using System.Collections.Generic;
using UnityEngine;

//public enum GameMode { Edit, SinglePlayer, Multiplayer }

public enum MenuScreens {
    Default, Main, SinglePlayer, MultiPlayer, StudentInfo, Options, Loading, Edit
}

public class Globals : Singleton<Globals> {

    public const int TOTAL_MONEY = 100;
    public const int MAX_ZOMBIES_FOR_PLAYER = 36; //9 rows on 4 columns
    public MenuScreens currentScreen;


    public const string GITHUB_PROFILE_URL = "https://github.com/SaroyaMan";
    public const string CV_URL = "https://drive.google.com/file/d/0B8BaWfqNelVKb3p2MUVUTDQ1WVk/view";

    private Dictionary<string, GameObject> unityObjects;
    private Dictionary<string, object> savedData;

    public Dictionary<string, GameObject> UnityObjects { get { return unityObjects; } }
    public Dictionary<string, object> SavedData { get { return savedData; } }

    private void Awake() {
        Init();
    }

    private void Init() {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach(var ob in gameObjects) {
            unityObjects.Add(ob.name, ob);
        }
        //PlayerPrefs.DeleteAll();
        InitSavedData();
    }

    private void InitSavedData() {
        string savedDataStr = PlayerPrefs.GetString("SaveData", "{}");
        object jsonParsed = MiniJSON.Json.Deserialize(savedDataStr);
        if(jsonParsed != null) {
            savedData = jsonParsed as Dictionary<string, object>;
        }
        else savedData = new Dictionary<string, object>();
        string savedDataString = MiniJSON.Json.Serialize(savedData);
        print(savedDataString);
    }
}