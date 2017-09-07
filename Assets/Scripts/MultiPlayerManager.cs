
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerManager : Singleton<MultiPlayerManager> {

    private Dictionary<string, object> data;
    

    private Listener listener;
    private string username = string.Empty;
    private List<string> rooms;
    private int index = 0;

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

        username = System.DateTime.UtcNow.Ticks.ToString();
    }

    public void SetUsername(string newUsername) {
        username = newUsername;
        GameView.SetText("Txt_Username", "Welcome " + username + "!");
    }

    public void ConnectGame() {
        GameView.SetText("StatusTxt", "Connecting...");
        WarpClient.GetInstance().Connect(username);
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
    }

    private void OnEnable() {
        Listener.OnConnect += OnConnectOccured;
        Listener.OnRoomsInRange += OnRoomsInRangeOccured;
        Listener.OnCreateRoom += OnCreateRoomOccured;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfoOccured;
        Listener.OnUserJoinRoom += OnUserJoinRoomOccured;
        Listener.OnGameStarted += OnGameStartedOccured;
    }

    private void OnDisable() {
        Listener.OnConnect -= OnConnectOccured;
        Listener.OnRoomsInRange -= OnRoomsInRangeOccured;
        Listener.OnCreateRoom -= OnCreateRoomOccured;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfoOccured;
        Listener.OnUserJoinRoom -= OnUserJoinRoomOccured;
        Listener.OnGameStarted -= OnGameStartedOccured;
    }

    private void OnConnectOccured(bool _IsSuccess) {
        Debug.Log("OnConnect: " + _IsSuccess);
        if(_IsSuccess) {

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
                WarpClient.GetInstance().CreateTurnRoom("Room Name", username, 2, data, 60);
            }
        }
        else {
            //GameObject.Find("Btn_Play").GetComponent<Button>().interactable = true;
            Debug.Log("OnRoomsInRangeOccured: connection failed!");
        }
    }

    private void OnCreateRoomOccured(bool _IsSuccess, string _RoomId) {
        Debug.Log("OnCreateRoom " + _IsSuccess + " " + _RoomId);
        if(_IsSuccess) {
            GameView.SetText("StatusTxt","Created Room!");
            WarpClient.GetInstance().JoinRoom(_RoomId);

            //so i can get events when other users join my room
            WarpClient.GetInstance().SubscribeRoom(_RoomId);
        }
    }

    private void OnGetLiveRoomInfoOccured(LiveRoomInfoEvent eventObj) {
        Debug.Log("OnGetLiveRoomInfo " + eventObj.getData().getId() + " " + eventObj.getResult() + " " + eventObj.getJoinedUsers().Length);
        GameView.SetText("StatusTxt","Room Information: " + eventObj.getData().getId() + " " + eventObj.getJoinedUsers().Length);

        Dictionary<string, object> _temp = eventObj.getProperties();
        Debug.Log(_temp.Count + " " + _temp["Password"] + " " + data["Password"].ToString());

        if(eventObj.getResult() == 0 && eventObj.getJoinedUsers().Length == 1 &&
            _temp["Password"].ToString() == data["Password"].ToString()) {
            WarpClient.GetInstance().JoinRoom(eventObj.getData().getId());
            WarpClient.GetInstance().SubscribeRoom(eventObj.getData().getId());
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
                WarpClient.GetInstance().CreateTurnRoom("Room Name", username, 2, null, 60);
            }
        }
    }

    private void OnUserJoinRoomOccured(RoomData eventObj, string _UserName) {
        Debug.Log("OnUserJoinRoom " + " " + _UserName);
        GameView.SetText("StatusTxt", "User " + _UserName + " Joined Room!");
        //SC_MenuView.Instance.SetInfoText("OnUserJoinRoom " + " " + _UserName);
        if(_UserName != eventObj.getRoomOwner()) {
            WarpClient.GetInstance().startGame();
        }
    }

    private void OnGameStartedOccured(string _Sender, string _RoomId, string _NextTurn) {
        Debug.Log("SC_MenuLogic: " + _Sender + " " + _RoomId + " " + _NextTurn);
        Debug.Log("Game is starting...");

        SoundManager.Instance.Music.clip = SoundManager.Instance.InGameMusic;
        SoundManager.Instance.Music.Play();
        //SceneManager.LoadSceneAsync("Game_Scene");
        Initiate.Fade("Game_Scene", GameView.transitionColor, 2f);
        //SC_MenuView.Instance.SetInfoText("OnGameStarted " + _Sender + " " + _RoomId + " " + _NextTurn);
        //SC_MenuGlobals.Instance.unityObjects["Screen_Menu"].SetActive(false);
        //SC_MenuGlobals.Instance.unityObjects["Screen_Game"].SetActive(true);
    }

    public void Disconnect() {
        WarpClient.GetInstance().Disconnect();
        rooms.Clear();
        index = 0;
        Globals.Instance.UnityObjects["StatusConnectionWindow"].SetActive(false);
    }
}
