using UnityEngine;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using com.shephertz.app42.gaming.multiplayer.client.command;

using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class Listener : ConnectionRequestListener, LobbyRequestListener, ZoneRequestListener, RoomRequestListener, ChatRequestListener, UpdateRequestListener, NotifyListener, TurnBasedRoomListener
	{						
		public delegate void ConnectHandler(bool _IsSuccess);
		public static event ConnectHandler OnConnect;

		public delegate void DisconnectHandler(bool _IsSuccess);
		public static event DisconnectHandler OnDisconnect;

		public delegate void CreateRoomHandler(bool _IsSuccess,string _RoomId);
		public static event CreateRoomHandler OnCreateRoom;

		public delegate void RoomsInRangeHandler(bool _IsSuccess,MatchedRoomsEvent eventObj);
		public static event RoomsInRangeHandler OnRoomsInRange;

		public delegate void GetLiveRoomInfoHandler(LiveRoomInfoEvent eventObj);
		public static event GetLiveRoomInfoHandler OnGetLiveRoomInfo;
		
		public delegate void JoinRoomHandler(bool _IsSuccess,string _RoomId);
		public static event JoinRoomHandler OnJoinRoom;

		public delegate void UserJoinRoomHandler(RoomData eventObj, string _UserName);
		public static event UserJoinRoomHandler OnUserJoinRoom;

		public delegate void StartGameAckHandler();
		public static event StartGameAckHandler OnStartGameAck;

		public delegate void GameStartedHandler(string _Sender, string _RoomId, string _NextTurn);
		public static event GameStartedHandler OnGameStarted;

		public delegate void StopGameAckHandler();
		public static event StopGameAckHandler OnStopGameAck;

		public delegate void GameStoppedHandler(string _Sender, string _RoomId);
		public static event GameStoppedHandler OnGameStopped;

		public delegate void SendMoveAckHandler();
		public static event SendMoveAckHandler OnSendMoveAck;

		public delegate void MoveCompletedHandler(MoveEvent _Move);
		public static event MoveCompletedHandler OnMoveCompleted;

		int state = 0;
		public void sendMsg(string msg)
		{
			if(state == 1)
			{
				WarpClient.GetInstance().SendChat(msg);
			}
		}

		//ConnectionRequestListener
		#region ConnectionRequestListener
		public void onConnectDone(ConnectEvent eventObj)
		{
			if (eventObj.getResult () == 0) 
			{
				if (OnConnect != null)
					OnConnect (true);
			} 
			else 
			{
				if (OnConnect != null)
					OnConnect (false);
			}
		}

		public void onInitUDPDone(byte res)
		{
		}
		
		public void onDisconnectDone(ConnectEvent eventObj)
		{
			Debug.Log("onDisconnectDone : " + eventObj.getResult());
			if (eventObj.getResult() == 0 && OnDisconnect != null)
				OnDisconnect (true);
			else if (OnDisconnect != null)
				OnDisconnect (false);
		}

		#endregion
		
		//LobbyRequestListener
		#region LobbyRequestListener
		public void onJoinLobbyDone (LobbyEvent eventObj)
		{
			Debug.Log ("onJoinLobbyDone : " + eventObj.getResult());
			if(eventObj.getResult() == 0)
			{
				state = 1;
			}
		}
		
		public void onLeaveLobbyDone (LobbyEvent eventObj)
		{
			Debug.Log ("onLeaveLobbyDone : " + eventObj.getResult());
		}
		
		public void onSubscribeLobbyDone (LobbyEvent eventObj)
		{
			Debug.Log ("onSubscribeLobbyDone : " + eventObj.getResult());
			if(eventObj.getResult() == 0)
			{
				WarpClient.GetInstance().JoinLobby();
			}
		}
		
		public void onUnSubscribeLobbyDone (LobbyEvent eventObj)
		{
			Debug.Log ("onUnSubscribeLobbyDone : " + eventObj.getResult());
		}
		
		public void onGetLiveLobbyInfoDone (LiveRoomInfoEvent eventObj)
		{
			Debug.Log ("onGetLiveLobbyInfoDone : " + eventObj.getResult());
		}
		#endregion
		
		//ZoneRequestListener
		#region ZoneRequestListener
		public void onDeleteRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onDeleteRoomDone : " + eventObj.getResult());
		}
		
		public void onGetAllRoomsDone (AllRoomsEvent eventObj)
		{
			Debug.Log ("onGetAllRoomsDone : " + eventObj.getResult());
			for(int i=0; i< eventObj.getRoomIds().Length; ++i)
			{
				Debug.Log ("Room ID : " + eventObj.getRoomIds()[i]);
			}
		}
		
		public void onCreateRoomDone (RoomEvent eventObj)
		{
			if (eventObj.getResult() == 0 && OnCreateRoom != null)
				OnCreateRoom (true,eventObj.getData().getId());
			else OnCreateRoom (false,string.Empty);
		}
		
		public void onGetOnlineUsersDone (AllUsersEvent eventObj)
		{
			Debug.Log ("onGetOnlineUsersDone : " + eventObj.getResult());
		}
		
		public void onGetLiveUserInfoDone (LiveUserInfoEvent eventObj)
		{
			Debug.Log ("onGetLiveUserInfoDone : " + eventObj.getResult());
		}
		
		public void onSetCustomUserDataDone (LiveUserInfoEvent eventObj)
		{
			Debug.Log ("onSetCustomUserDataDone : " + eventObj.getResult());
		}
		
        public void onGetMatchedRoomsDone(MatchedRoomsEvent eventObj)
		{
			if (eventObj.getResult () == WarpResponseResultCode.SUCCESS && OnRoomsInRange != null)
				OnRoomsInRange (true,eventObj);
			else if (OnRoomsInRange != null)
				OnRoomsInRange (false,null);
		}		
		#endregion

		//RoomRequestListener
		#region RoomRequestListener
		public void onSubscribeRoomDone (RoomEvent eventObj)
		{
			if(eventObj.getResult() == 0)
			{
				/*string json = "{\"start\":\""+id+"\"}";
				WarpClient.GetInstance().SendChat(json);
				state = 1;*/
				//WarpClient.GetInstance().JoinRoom(appwarp.roomid);
			}
			
			Debug.Log ("onSubscribeRoomDone : " + eventObj.getResult());
		}
		
		public void onUnSubscribeRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onUnSubscribeRoomDone : " + eventObj.getResult());
		}
		
		public void onJoinRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onJoinRoomDone : " + eventObj.getResult());

			if (eventObj.getResult () == 0 && OnJoinRoom != null)
				OnJoinRoom (true, eventObj.getData ().getId());
			else if(eventObj.getResult() == 0 && OnJoinRoom != null)
				OnJoinRoom(false,string.Empty);
		}
		
		public void onLockPropertiesDone(byte result)
		{
			Debug.Log ("onLockPropertiesDone : " + result);
		}
		
		public void onUnlockPropertiesDone(byte result)
		{
			Debug.Log ("onUnlockPropertiesDone : " + result);
		}
		
		public void onLeaveRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onLeaveRoomDone : " + eventObj.getResult());
		}
		
		public void onGetLiveRoomInfoDone (LiveRoomInfoEvent eventObj)
		{
			if (OnGetLiveRoomInfo != null)
				OnGetLiveRoomInfo (eventObj);
		}
		
		public void onSetCustomRoomDataDone (LiveRoomInfoEvent eventObj)
		{
			Debug.Log ("onSetCustomRoomDataDone : " + eventObj.getResult());
		}
		
		public void onUpdatePropertyDone(LiveRoomInfoEvent eventObj)
        {
            if (WarpResponseResultCode.SUCCESS == eventObj.getResult())
            {
				Debug.Log ("UpdateProperty event received with success status");
            }
            else
            {
				Debug.Log("Update Propert event received with fail status. Status is :" + eventObj.getResult().ToString());
            }
        }
		#endregion
		
		//ChatRequestListener
		#region ChatRequestListener
		public void onSendChatDone (byte result)
		{
			Debug.Log ("onSendChatDone result : " + result);
			
		}
		
		public void onSendPrivateChatDone(byte result)
		{
			Debug.Log ("onSendPrivateChatDone : " + result);
		}
		#endregion
		
		//UpdateRequestListener
		#region UpdateRequestListener
		public void onSendUpdateDone (byte result)
		{
		}
		public void onSendPrivateUpdateDone (byte result)
		{
			Debug.Log ("onSendPrivateUpdateDone : " + result);
		}
		#endregion

		//NotifyListener
		#region NotifyListener
		public void onRoomCreated (RoomData eventObj)
		{
			Debug.Log ("onRoomCreated");
		}
		public void onPrivateUpdateReceived (string sender, byte[] update, bool fromUdp)
		{
			Debug.Log ("onPrivateUpdate");
		}
		public void onRoomDestroyed (RoomData eventObj)
		{
			Debug.Log ("onRoomDestroyed");
		}
		
		public void onUserLeftRoom (RoomData eventObj, string username)
		{
			Debug.Log ("onUserLeftRoom : " + username);
		}
		
		public void onUserJoinedRoom (RoomData eventObj, string username)
		{
			if (OnUserJoinRoom != null)
				OnUserJoinRoom(eventObj, username);
		}
		
		public void onUserLeftLobby (LobbyData eventObj, string username)
		{
			Debug.Log ("onUserLeftLobby : " + username);
		}
		
		public void onUserJoinedLobby (LobbyData eventObj, string username)
		{
			Debug.Log ("onUserJoinedLobby : " + username);
		}
		
		public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<string, object> properties, Dictionary<string, string> lockedPropertiesTable)
		{

			Debug.Log ("onUserChangeRoomProperty : " + sender);
            MultiPlayerManager.Instance.Data = properties;
        }
			
		public void onPrivateChatReceived(string sender, string message)
		{
			Debug.Log ("onPrivateChatReceived : " + sender);
		}
		
		public void onMoveCompleted(MoveEvent move)
		{
			if (OnMoveCompleted != null)
				OnMoveCompleted(move);
		}
		
		public void onChatReceived (ChatEvent eventObj)
		{
			Debug.Log(eventObj.getSender() + " sended " + eventObj.getMessage());
//			com.shephertz.app42.gaming.multiplayer.client.SimpleJSON.JSONNode msg =  com.shephertz.app42.gaming.multiplayer.client.SimpleJSON.JSON.Parse(eventObj.getMessage());
			//msg[0] 
//			if(eventObj.getSender() != appwarp.username)
//			{
//				//Log(msg["x"].ToString()+" "+msg["y"].ToString()+" "+msg["z"].ToString());
//			}
		}
		
		public void onUpdatePeersReceived (UpdateEvent eventObj)
		{
			Debug.Log ("onUpdatePeersReceived");
		}
		
		public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<String, System.Object> properties)
        {
			Debug.Log("Notification for User Changed Room Propert received");
			Debug.Log(roomData.getId());
			Debug.Log(sender);
            foreach (KeyValuePair<String, System.Object> entry in properties)
            {
				Debug.Log("KEY:" + entry.Key);
				Debug.Log("VALUE:" + entry.Value.ToString());
            }
        }

		
		public void onUserPaused(String locid, Boolean isLobby, String username)
		{
			Debug.Log("onUserPaused");
		}
		
		public void onUserResumed(String locid, Boolean isLobby, String username)
		{
			Debug.Log("onUserResumed");
		}
		
		public void onGameStarted(string sender, string roomId, string nextTurn)
		{
			if (OnGameStarted != null)
				OnGameStarted(sender, roomId, nextTurn);
		}
		
		public void onGameStopped(string sender, string roomId)
		{
			if (OnGameStopped != null)
				OnGameStopped(sender, roomId);
		}

		public void onNextTurnRequest (string lastTurn)
		{
			Debug.Log("onNextTurnRequest");
		}
		#endregion

		//TurnBasedRoomListener
		#region TurnBasedRoomListener
		public void onSendMoveDone(byte result)
		{
            //Debug.LogError("result = " + (int) result);
			if (OnSendMoveAck != null)
				OnSendMoveAck();
		}
		
		public void onStartGameDone(byte result)
		{        
			if (OnStartGameAck != null)
				OnStartGameAck();
		}
		
		public void onStopGameDone(byte result)
		{
			if (OnStopGameAck != null)
				OnStopGameAck();
		}
		
		public void onSetNextTurnDone(byte result)
		{
		}
		
		public void onGetMoveHistoryDone(byte result, MoveEvent[] moves)
		{
		}
		#endregion
	}
}

