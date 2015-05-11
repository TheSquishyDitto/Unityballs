/// <summary>
/// NetworkMaster.cs
/// Authors: Kyle Dawson
/// Date Created:  May   5, 2015
/// Last Revision: May  10, 2015
/// 
/// Class for managing network behavior.
/// 
/// NOTES: - Currently very limited. Needs more support from UI?
/// 
/// TO DO: - Give user more control over connections.
/// 	   - Give user feedback.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkMaster : MonoBehaviour {

	// Variables
	#region Variables
	[Header("Server Data")]
	public string gameType = "MarballsSumo";	// Unique name of this game.
	public int maxPlayers = 8;					// Maximum players per room.
	public int port = 7777;

	string roomName = string.Empty;				// Used when creating a room's name.
	string roomDesc = string.Empty;				// Used when leaving a comment about the room.
	string username = string.Empty;				// Used to create a username.
	HostData[] hostList;						// List of hosts, which might mean servers/rooms?

	[Header("References")]
	public GameObject netWindow;				// Reference to connection window.
	public GameObject playerPrefab;				// Reference to player prefab to spawn.
	public GameObject roomButtonPrefab;			// Reference to button prefab to spawn.
	public GameObject panCam;					// Reference to panning camera.
	public Transform roomList;					// Reference to room list container.
	public Transform[] spawnPoints;				// Array of spawn points.

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		Application.targetFrameRate = 60;

		if (GameMaster.GM != null) {
			Destroy(GameMaster.GM);
		}

		StartCoroutine("AutoRefresh");
	}

	// StartServer - Starts the server.
	public void StartServer() {

		if (roomName == string.Empty) roomName = "Marble Sumo";
		if (roomDesc == string.Empty) roomDesc = "Knock other players off the platform!";

		Network.InitializeServer(maxPlayers, port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameType, roomName, roomDesc);
		//MasterServer.ipAddress = "127.0.0.1";

		netWindow.SetActive(false);
	}

	// CreateJoin - Starts a server and joins it.
	public void CreateJoin() {
		StartServer();
		SpawnPlayer();
	}

	// SpawnPlayer - Creates a player instance across all clients.
	void SpawnPlayer() {
		panCam.SetActive(false);
		netWindow.SetActive(false);

		int spawnIndex = (Network.connections.Length)/*MultiplayerMarble.quantity*/ % spawnPoints.Length;
		//Debug.Log("Quantity: " + MultiplayerMarble.quantity);
		GameObject obj = (GameObject)Network.Instantiate(playerPrefab, spawnPoints[spawnIndex].position + (Vector3.up * 3), spawnPoints[spawnIndex].rotation, 0);
		MultiplayerMarble marble = obj.GetComponentInChildren<MultiplayerMarble>();
		marble.SetUsername(username);

		Vector3 color = new Vector3(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f));
		marble.ChangeColor(color);
	}

	// RefreshHostList - Requests the most up-to-date host list from the Master Server.
	public void RefreshHostList() {
		MasterServer.RequestHostList(gameType);
	}

	// OnMasterServerEvent - Called when Master Server does something. Used to retrieve server list.
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();

		GenerateRoomList();
	}

	// GenerateRoomList - Generates the list of available rooms to play in.
	void GenerateRoomList() {
		for (int i = 0; i < roomList.childCount; i++)
			Destroy(roomList.GetChild(i).gameObject);

		// Generate UI buttons for each room.
		if (hostList != null) {
			for (int i = 0; i < hostList.Length; i++) {
				// Creates a button and sets up its appearance.
				GameObject button = (GameObject)Instantiate(roomButtonPrefab);
				Text[] textParts = button.GetComponentsInChildren<Text>();
				textParts[0].text = hostList[i].gameName;
				textParts[1].text = hostList[i].connectedPlayers + " online";
				textParts[2].text = hostList[i].comment;

				// Adds button functionality and puts it in the list.
				HostData host = hostList[i];
				button.GetComponent<Button>().onClick.AddListener(() => {JoinServer(host);});
				button.transform.SetParent(roomList);
			}
		}
	}

	// JoinServer - Joins a server.
	void JoinServer(HostData hostData) {
		Network.Connect(hostData);
	}

	// OnServerInitialized - Called when server starts. Generally only called by the server host.
	void OnServerInitialized() {
		Debug.Log("Server started!");
		//SpawnPlayer();
	}

	// OnConnectedToServer - Called when a player connects to the server.
	void OnConnectedToServer() {
		Debug.Log("Server joined!");
		SpawnPlayer();
	}

	// OnDisconnectedFromServer - Called basically everywhere when server goes down.
	void OnDisconnectedFromServer() {
		Application.LoadLevel(0);
	}

	// OnPlayerDisconnected - Called when a player disconnects.
	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	// Update - Called every frame.
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Network.isClient || Network.isServer)
				Network.Disconnect();
			else
				Application.LoadLevel(0);
		}
	}

	// AutoRefresh - Refreshes the host list every few seconds automatically.
	IEnumerator AutoRefresh() {
		while (!Network.isClient && !Network.isServer) {
			yield return new WaitForSeconds(2f);
			RefreshHostList();
		}
	}

	#region UI Setter Functions
	public void UpdateUsername(string name) {
		username = name;
	}
	
	public void UpdateRoomName(string name) {
		roomName = name;
	}
	
	public void UpdateRoomDesc(string desc) {
		roomDesc = desc;
	}
	
	public void UpdatePort(string num) {
		port = int.Parse(num);
	}
	#endregion

}
