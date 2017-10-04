using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MultiPlayerManager: Singleton<MultiPlayerManager> {

    private Dictionary<string, object> data;


    private Listener listener;
    private string username = string.Empty;
    private string realUsername = string.Empty;
    private string realEnemyUsername = string.Empty;
    private List<string> rooms;
    private int index = 0;


    //
    private string currentUsernameTurn = string.Empty;
    private GameSide playerSide;
    private bool isMyTurn;

    public GameSide PlayerSide { get { return playerSide; } }
    public bool IsMyTurn { get { return isMyTurn; } }
    public string Username { get { return username; } }
    public string RealUsername { get { return realUsername; } }
    public string RealEnemyUsername { get { return realEnemyUsername; } }
    public Dictionary<string, object> Data { get { return data; } set { data = value; } }

    private void Awake() {
        DontDestroyOnLoad(this);
        Init();
    }

    private void Init() {
        listener = new Listener();
        rooms = new List<string>();
        data = new Dictionary<string, object> {
            { "Password", "12345" }
        };
        WarpClient.initialize(Globals.API_KEY, Globals.SECRET_KEY);
        WarpClient.GetInstance().AddConnectionRequestListener(listener);
        WarpClient.GetInstance().AddChatRequestListener(listener);
        WarpClient.GetInstance().AddUpdateRequestListener(listener);
        WarpClient.GetInstance().AddLobbyRequestListener(listener);
        WarpClient.GetInstance().AddNotificationListener(listener);
        WarpClient.GetInstance().AddRoomRequestListener(listener);
        WarpClient.GetInstance().AddZoneRequestListener(listener);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listener);
        DontDestroyOnLoad(WarpClient.GetInstance());
        username = System.DateTime.UtcNow.Ticks.ToString();
    }

    public void ConnectGame() {
        GameView.SetText("StatusTxt", "Connecting...");
        realUsername = PlayerPrefs.GetString("Username", "No-Name");
        username = realUsername + "#" + username;
        data["HomePlayer" + username] = MiniJSON.Json.Serialize(GetLocalSoldiers().ToArray());
        WarpClient.GetInstance().Connect(username);
        //WarpClient.GetInstance().GetRoomsInRange(1, 2);
    }

    private void OnEnable() {
        Listener.OnConnect += OnConnectOccured;
        Listener.OnRoomsInRange += OnRoomsInRangeOccured;
        Listener.OnCreateRoom += OnCreateRoomOccured;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfoOccured;
        Listener.OnUserJoinRoom += OnUserJoinRoomOccured;
        Listener.OnGameStarted += OnGameStartedOccured;
        Listener.OnMoveCompleted += OnMoveCompletedOccured;
        Listener.OnGameStopped += OnGameStoppedOccured;
    }

    private void OnDisable() {
        Listener.OnConnect -= OnConnectOccured;
        Listener.OnRoomsInRange -= OnRoomsInRangeOccured;
        Listener.OnCreateRoom -= OnCreateRoomOccured;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfoOccured;
        Listener.OnUserJoinRoom -= OnUserJoinRoomOccured;
        Listener.OnGameStarted -= OnGameStartedOccured;
        Listener.OnMoveCompleted -= OnMoveCompletedOccured;
        Listener.OnGameStopped -= OnGameStoppedOccured;
    }

    private void OnConnectOccured(bool _IsSuccess) {
        Debug.Log("OnConnect: " + _IsSuccess);
        if(_IsSuccess) {
            //data["HomePlayer"] = MiniJSON.Json.Serialize(GetLocalSoldiers().ToArray() );
            GameView.SetText("StatusTxt", "Getting Rooms in range...");
            WarpClient.GetInstance().GetRoomsInRange(1, 2);
        }
    }

    private void OnRoomsInRangeOccured(bool _IsSuccess, MatchedRoomsEvent eventObj) {
        //Debug.Log("OnRoomsInRange: " + _IsSuccess + " " + eventObj.getRoomsData());
        if(_IsSuccess) {
            rooms = new List<string>();
            foreach(var roomData in eventObj.getRoomsData()) {
                Debug.Log("Getting Live info on room " + roomData.getId());
                Debug.Log("Room Owner " + roomData.getRoomOwner());
                rooms.Add(roomData.getId());
            }

            index = 0;
            if(index < rooms.Count) {
                Debug.Log("Getting Live Info on room: " + rooms[index]);
                WarpClient.GetInstance().GetLiveRoomInfo(rooms[index]);
            }
            else {
                Debug.Log("No rooms were availible, create a room");
                WarpClient.GetInstance().CreateTurnRoom(username, username, 2, data, 100);
            }
        }
        else {
            Debug.Log("OnRoomsInRangeOccured: connection failed!");
        }
    }

    private void OnCreateRoomOccured(bool _IsSuccess, string _RoomId) {
        Debug.Log("OnCreateRoom " + _IsSuccess + " " + _RoomId);
        if(_IsSuccess) {
            GameView.SetText("StatusTxt", "Created Room!");
            WarpClient.GetInstance().JoinRoom(_RoomId);
            WarpClient.GetInstance().SubscribeRoom(_RoomId); //so i can get events when other users join my room
        }
    }

    private void OnGetLiveRoomInfoOccured(LiveRoomInfoEvent eventObj) {
        Debug.Log("OnGetLiveRoomInfo " + eventObj.getData().getId() + " " + eventObj.getResult() + " " + eventObj.getJoinedUsers().Length);
        GameView.SetText("StatusTxt", "Room Information: " + eventObj.getData().getId() + " " + eventObj.getJoinedUsers().Length);
        Dictionary<string, object> _temp = eventObj.getProperties();
        Debug.Log(_temp.Count + " " + _temp["Password"] + " " + data["Password"].ToString());

        if(eventObj.getResult() == 0 && eventObj.getJoinedUsers().Length == 1 &&
            _temp["Password"].ToString() == data["Password"].ToString()) {

            WarpClient.GetInstance().JoinRoom(eventObj.getData().getId());
            WarpClient.GetInstance().SubscribeRoom(eventObj.getData().getId());

            data["AwayPlayer"] = MiniJSON.Json.Serialize(GetLocalSoldiers().ToArray());

            WarpClient.GetInstance().UpdateRoomProperties(eventObj.getData().getId(), data, null);

            GameView.SetText("StatusTxt", "Joining Room...");
        }
        else {
            index++;
            if(index < rooms.Count) {
                Debug.Log("Getting Live Info on room: " + rooms[index]);
                WarpClient.GetInstance().GetLiveRoomInfo(rooms[index]);
            }
            else {
                Debug.Log("No rooms were availible, create a room");

                WarpClient.GetInstance().CreateTurnRoom("Room Name", username, 2, null, 30);

                data["HomePlayer" + username] = MiniJSON.Json.Serialize(GetLocalSoldiers().ToArray());
                WarpClient.GetInstance().UpdateRoomProperties(rooms[index], data, null);
            }
        }
    }

    private void OnUserJoinRoomOccured(RoomData eventObj, string _UserName) {
        Debug.Log("OnUserJoinRoom " + " " + _UserName);
        GameView.SetText("StatusTxt", "User " + ParseUsername(_UserName) + " Joined Room!");
        if(_UserName != eventObj.getRoomOwner()) {
            //int randTurn = Random.Range(0, 2);
            //string firstPlayer = string.Empty;
            //firstPlayer = randTurn == 0 ? username : _UserName;
            //WarpClient.GetInstance().startGame(true, firstPlayer);
            realEnemyUsername = ParseUsername(_UserName);
            WarpClient.GetInstance().startGame();
        }
    }

    private void SwitchToGameScene() {
        SoundManager.Instance.Music.clip = SoundManager.Instance.InGameMusic;
        SoundManager.Instance.Music.Play();
        Initiate.Fade("Game_Scene", GameView.transitionColor, 2f);
    }

    public void OnGameStartedOccured(string _Sender, string _RoomId, string _NextTurn) {

        Debug.Log("OnGameStartedOccured: " + _Sender + " " + _RoomId + " " + _NextTurn);
        if(_Sender != username) {
            realEnemyUsername = ParseUsername(_Sender);
        }

        currentUsernameTurn = _NextTurn;
        ParseUsername(_NextTurn);
        GameManager.CURRENT_TURN = GameSide.LeftSide;
        if(currentUsernameTurn == username) {
            playerSide = GameSide.LeftSide;
            isMyTurn = true;
            var awaySoldiers = MiniJSON.Json.Deserialize(data["AwayPlayer"].ToString()) as List<object>;
            SoldierManager.Instance.InitEnemyBoard(awaySoldiers);
        }
        else {
            playerSide = GameSide.RightSide;
            isMyTurn = false;
            var homeSoldiers = MiniJSON.Json.Deserialize(data["HomePlayer" + currentUsernameTurn].ToString()) as List<object>;
            SoldierManager.Instance.InitEnemyBoard(homeSoldiers);
            SoldierManager.Instance.FlipSide();
        }
        SoldierManager.Instance.HideAllSoldiers();
        SwitchToGameScene();
    }

    public void SendMove(Tile oldTile, Tile newTile) {
        //Debug.LogError("Con State : " + WarpClient.GetInstance().GetConnectionState());
        if(isMyTurn) {
            isMyTurn = false;
            Dictionary<string, object> toSend = new Dictionary<string, object> {
                { "UserName", username },
                { "oldTileRow", oldTile.Row },
                { "oldTileColumn", oldTile.Column },
                { "newTileRow", newTile.Row },
                { "newTileColumn", newTile.Column }
            };
            string jsonToSend = MiniJSON.Json.Serialize(toSend);
            Debug.Log(jsonToSend);
            WarpClient.GetInstance().sendMove(jsonToSend);
        }
    }

    public void SendGameQuit(string msg) {
        Dictionary<string, object> toSend = new Dictionary<string, object> {
                { "GameQuit", msg },
            };
        string jsonToSend = MiniJSON.Json.Serialize(toSend);
        WarpClient.GetInstance().sendMove(jsonToSend);
    }

    public void OnMoveCompletedOccured(MoveEvent _Move) {
        //Debug.LogError("OnMoveCompleted " + _Move.getMoveData() + " " + _Move.getNextTurn() + " " + _Move.getSender());
        if(_Move.getSender() != username && _Move.getMoveData() != null) {
            Dictionary<string, object> recievedData = MiniJSON.Json.Deserialize(_Move.getMoveData()) as Dictionary<string, object>;
            if(recievedData != null && !GameManager.Instance.IsGameOver) {
                if(recievedData.ContainsKey("oldTileRow")) {
                    var matrixTile = TileManager.Instance.MatrixTiles;
                    Tile oldTile = matrixTile[int.Parse(recievedData["oldTileRow"].ToString()), int.Parse(recievedData["oldTileColumn"].ToString())];
                    Tile newTile = matrixTile[int.Parse(recievedData["newTileRow"].ToString()), int.Parse(recievedData["newTileColumn"].ToString())];
                    var zombie = oldTile.Soldier as Zombie;
                    SoldierManager.Instance.MakeEnemyMove(zombie, newTile);

                    currentUsernameTurn = _Move.getNextTurn();
                    isMyTurn = (_Move.getNextTurn() == username);
                    StartCoroutine(GameManager.Instance.CountTime());
                }
                else if(recievedData.ContainsKey("GameQuit")) {
                    GameManager.Instance.WinGame(PlayerSide, recievedData["GameQuit"].ToString());
                }
            }
        }
    }

    public void OnGameStoppedOccured(string _Sender, string _RoomId) {
        Debug.Log(_Sender + " " + _RoomId);
    }

    public void Disconnect() {
        WarpClient.GetInstance().Disconnect();
        rooms.Clear();
        index = 0;
        Globals.Instance.UnityObjects["StatusConnectionWindow"].SetActive(false);
    }

    private List<string> GetLocalSoldiers() {
        var localSoldiers = SoldierManager.Instance.LocalPlayerList;
        List<string> listOfSoldiers = new List<string>();
        foreach(var soldier in localSoldiers) {
            listOfSoldiers.Add(soldier.CurrentTile.Row + "," + soldier.CurrentTile.Column + "," + Regex.Match(soldier.name, @"^[a-zA-Z0-9]*").Value);
        }
        return listOfSoldiers;
    }

    public static string ParseUsername(string username) {
        return username.Remove(username.IndexOf('#'));
    }

    private void OnApplicationQuit() {
        //if(Globals.IS_IN_GAME && !Globals.IS_SINGLE_PLAYER) {
        //    SendGameQuit(realUsername + " gave up");
        //}
        WarpClient.GetInstance().Disconnect();
    }

    private void OnDestroy() {
        WarpClient.GetInstance().Disconnect();
        Destroy(WarpClient.GetInstance());
    }
}