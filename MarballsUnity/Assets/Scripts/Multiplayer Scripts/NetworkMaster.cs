/// <summary>
/// NetworkMaster.cs
/// Authors: Kyle Dawson
/// Date Created:  May   5, 2015
/// Last Revision: May   6, 2015
/// 
/// Class for managing network behavior.
/// 
/// NOTES: - Currently very limited. Needs support from UI system.
/// 
/// TO DO: - Give user more control over ports and connections.
/// 	   - Give user feedback.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class NetworkMaster : MonoBehaviour {

	// Variables
	#region Variables
	const string GAME_NAME = "MarballsSumo";	// Unique name of this game.
	const string ROOM_NAME = "COSC 426 Demo";	// Name of room.
	const int PORT = 7777;						// Port used for connection.
	const int MAX_PLAYERS = 2;					// Maximum players per room.
	
	HostData[] hostList;						// List of hosts, which might mean servers?
	
	public GameObject playerPrefab;				// Reference to player prefab to spawn.
	public GameObject panCam;					// Reference to panning camera.

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		if (GameMaster.GM != null) {
			Destroy(GameMaster.GM);
		}
	}

	// StartServer - Starts the server.
	void StartServer() {
		Network.InitializeServer(MAX_PLAYERS, PORT, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GAME_NAME, ROOM_NAME);
		//MasterServer.ipAddress = "127.0.0.1";
	}

	// SpawnPlayer - Creates a player instance across all clients.
	void SpawnPlayer() {
		panCam.SetActive(false);
		int spawnMod = (Network.connections.Length == 2)? 1 : -1;
		Network.Instantiate(playerPrefab, new Vector3(0, 20, 50 * spawnMod), Quaternion.identity, 0);
	}

	// RefreshHostList - Requests the most up-to-date host list from the Master Server.
	void RefreshHostList() {
		MasterServer.RequestHostList(GAME_NAME);
	}

	// OnMasterServerEvent - Called when Master Server does something. Used to retrieve server list.
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	// JoinServer - Joins a server.
	void JoinServer(HostData hostData) {
		Network.Connect(hostData);
	}

	// OnServerInitialized - Called when server starts.
	void OnServerInitialized() {
		Debug.Log("Server started!");
		SpawnPlayer();
	}

	// OnConnectedToServer - Called when a player connects to the server.
	void OnConnectedToServer() {
		Debug.Log("Server joined!");
		SpawnPlayer();
	}
	
	// OnGUI - Used for UI elements temporarily.
	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			// Start a server.
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			// Refresh list of ... hosts? servers?
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			// Show hosts and allow joining them.
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
}
