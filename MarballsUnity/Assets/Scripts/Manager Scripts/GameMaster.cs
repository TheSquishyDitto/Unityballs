/// <summary>
/// GameMaster.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 11, 2015
/// Last Revision: Jun. 25, 2015
/// 
/// Unifying class that controls game conditions and allows some inter-object communications.
/// 
/// NOTES: - This is a singleton class so only one of it should ever exist, if you need a reference to it, call GameMaster.CreateGM()
/// 
/// TO DO: - Add more events to subscribe to?
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

public class GameMaster : MonoBehaviour {

	// Enum for state of game.
	public enum GameState {
		Menu,		// State between or before levels.
		Prestart,	// State where the camera pans around the level.
		Start,		// The state immediately before the timer begins.
		Playing,	// The part of the game where mechanics matter.
		Win,		// State immediately after player wins a level.
		//Sumo		// Multiplayer mode.
	}

	// Variables
	#region Variables
	[Header("References")]
	public static GameMaster GM;		// Reference to singleton.
	public Settings settings;			// Reference to most game settings.
	
	public Marble marble;				// Reference to currently active marble.
	public InputManager input;			// Reference to input manager.
	public ControlScript controlMenu;	// Reference to control menu.
	public LevelDataObject levelData;	// Reference to information about current level.
	public Camera panCam;				// Reference to pan cam.

	[Header("Gameplay Variables")] 	// Some of these variables should be moved into getters/setters probably.
	public GameState state;			// Current state of game.
	public bool paused;				// True if game is paused, false otherwise.
	public float timer = 0;			// How much time has elapsed since the start of a level.
	public int buildLevelCap = 9;	// The last level accessible through standard progression.
	
	public bool levelSelect = false;// Done for win screen

	// Events
	public static event UnityAction pan;	// Container for actions before the game actually starts.
	public static event UnityAction start;	// Container for actions when game starts. 
	public static event UnityAction play;	// Container for actions when gameplay begins.
	public static event UnityAction win;	// Container for actions when winning.

	#endregion

	// CreateGM - Refers to existing GameMaster, or creates and returns one if one doesn't currently exist.
	public static GameMaster CreateGM() {
		if (GM == null) {
			GM = ((GameObject)Instantiate(Resources.Load ("Prefabs/GameMaster"))).GetComponent<GameMaster>();
			if (GM == null)
				Debug.LogWarning("(GameMaster.cs) Failed to create Game Master!");
		}
		return GM;
	}

	// Awake - Called before anything else.
	void Awake () {
		if (GM != null) {
			Destroy (gameObject);	// If a GameMaster already exists, destroys this instance.
		} else {
			GM = this;  // Failsafe in case there was a condition in which the GM was not already set to be this.
			DontDestroyOnLoad(this); // GameMaster should exist forever.
		}

		settings = LoadSettings();
		state = (Application.loadedLevel == 0)? GameState.Menu : state; // DEBUG
		LoadLevelData();
	}

	// OnEnable - Called when enabled. Generally used to subscribe to events.
	void OnEnable() {
		Messenger.AddListener("Pause", TogglePause);
	}

	// OnDisable - Called when disabled. Used to unsubscribe from events to prevent memory leaks.
	void OnDisable() {
		Messenger.RemoveListener("Pause", TogglePause);
	}

	// Start - Use this for initialization.
	void Start () {
		name = "Game Master";

		Debug.Log("Save File Path: " + Application.persistentDataPath); // DEBUG - This is where data is saved to.
	}
	
	// Update - Called once per frame.
	void Update () {
		if (!settings.freezeTimer) {	// If the timer isn't frozen,
			if (!paused) {	// and the game isn't paused,
				if (state == GameState.Start && timer > 0) { // in the starting phase,
					timer -= Time.deltaTime;	// the timer counts down to 0,
					if (timer <= 0)	{ // and when it reaches 0, the gameplay begins.
						OnPlay();
					}

				} else if(state == GameState.Playing) { // Here the timer serves a different purpose,
					timer += Time.deltaTime;	// tracking the player's time since starting.
				}
			}
		}

		// TODO Change keys to player's actual keys and/or let InputManager handle this.
		if (state == GameMaster.GameState.Prestart) {
			if(Input.GetKeyDown(KeyCode.Space)) {
				OnStart();
			}
			
			if (Input.GetKeyDown(KeyCode.Escape)) {
				LoadLevel(0);
			}
		}
	}

	// TogglePause - Toggles game paused state.
	public void TogglePause() {
		paused = !paused;
		//input.allowInput = !(input.allowInput);
		Time.timeScale = (paused)? 0 : 1; // When paused, physics simulation speed is set to 0.

		Messenger<bool>.Broadcast("SetPauseActive", paused);
	}

	// CancelCoroutines - Stops various animations and clears what they've done.
	// TODO REFACTOR THIS
	public void CancelCoroutines() {
		//References.hud.StopCoroutine("OnDeath");
		//finishLine.GetComponent<FinishLine>().StopCoroutine("SwirlFinish");
		//References.hud.StopCoroutine("OnVictory");
		Messenger.Broadcast("ClearHUDTint");

		marble.ResetState();
	}

	// ResetVariables - Clears and sets variables to their initial states.
	// NOTES: - Should typically only be called when loading a level.
	// 		  - For references, objects that aren't destroyed should not be nulled out!
	void ResetVariables() {
		Time.timeScale = 1;
		state = GameState.Menu;
		timer = 0;
		if (paused) { TogglePause(); }
		marble = null;
		
	}

	// LoadSettings - Easily loads game settings.
	public static Settings LoadSettings() {
		return Resources.Load("Data/GameSettings") as Settings;
	}

	// LoadLevel - Loads another level using that level's index.
	public void LoadLevel(int levelIndex) {
		ResetVariables();
		Application.LoadLevel(levelIndex);
	}

	// LoadLevel - Loads another level using that level's name.
	public void LoadLevel(string levelName) {
		ResetVariables();
		Application.LoadLevel(levelName);
	}

	// OnLevelWasLoaded - Triggers every time a level loads and before the start functions of everything on the level.
	void OnLevelWasLoaded(int level) {
		LoadLevelData();

		if(level == 0)	{
			if(levelSelect)
			{
				//mainMenu.ToggleSelectLevel();
				levelSelect = false;
			}	
		}
	}

	// LoadLevelData - Loads the level-specific scriptable object for this level.
	void LoadLevelData() {
		levelData = (LevelDataObject)Resources.Load("Data/Level Data/" + Application.loadedLevelName + "Data");
		if (levelData != null) {
			Debug.Log("(GameMaster.cs) Obtained data for " + levelData.levelName); // DEBUG
			Load(); // Loads game data.
		} else {
			Debug.LogWarning("(GameMaster.cs) Could not find: " + "Data/Level Data/" + Application.loadedLevelName + "Data"); // DEBUG
		}
	}

	// Save - Saves the player's time.
	void Save() {
		// Creates/overwrites file.
		BinaryFormatter converter = new BinaryFormatter();
		FileStream file = File.Create(GetFilePath());

		// Writes level data to player record.
		PlayerRecord record = new PlayerRecord();
		for (int i = 0; i < levelData.bestTimes.Count; i++) {
			record.bestTimes.Add(levelData.bestTimes[i]);
		}

		// Saves data and closes file.
		converter.Serialize(file, record);
		file.Close();

		Debug.Log("(GameMaster.cs) Saved file!");
	}

	// Load - Loads the player's time.
	void Load() {
		// Checks if file exists.
		if (File.Exists(GetFilePath())) {
			// Opens file.
			BinaryFormatter converter = new BinaryFormatter();
			FileStream file = File.Open(GetFilePath(), FileMode.Open);

			// Reads what data is there and closes file.
			PlayerRecord records = (PlayerRecord)converter.Deserialize(file);
			file.Close();

			// Loads data into level data.
			levelData.bestTimes.Clear();
			for (int i = 0; i < records.bestTimes.Count; i++) {
				levelData.bestTimes.Add(records.bestTimes[i]);
			}
		}
	}

	// GetFilePath - Returns location of where the loaded level's save data is or will be.
	public string GetFilePath() {
		return Application.persistentDataPath + "/" + Application.loadedLevelName + "Save.dat";
	}

	// State Changers - Functions that change the game's conditions.
	#region State Changers
	// OnPreStart - Called before level starts.
	public void OnPreStart(){
		if (levelData != null) levelData.firstTime = settings.debug;	// If we aren't testing the game, panning shouldn't always happen.
		state = GameState.Prestart;

		if (pan != null) pan();
	}
	
	
	// OnStart - Called when a level is to be started.
	public void OnStart() {
		Time.timeScale = 1;
		timer = settings.countdownLength;
		state = GameState.Start;

		if (start != null) start();	// Activates any functions subscribed to the starting event.
	}

	// OnPlay - Called when the player is to actually play the level.
	public void OnPlay() {
		Time.timeScale = 1;
		timer = 0;
		state = GameState.Playing;

		if (play != null) play();	// Activates any functions subscribed to the gameplay event.
	}

	// OnWin - Called when a level is won.
	public void OnWin() {
		state = GameState.Win;
		Time.timeScale = 0.5f; // Slowmo victory!

		if (win != null) win(); // Should subscribe the various victory functions to this.

		// If this is a legitimate level...
		if (levelData != null) {
			// Keeps player's five best times.
			for (int i = 0; i < settings.highScoreCount; i++) {
				// Checks if there's a vacant slot or if the current slot is a worse time.
				if (levelData.bestTimes.Count == i || levelData.bestTimes[i] > timer) {
					levelData.bestTimes.Insert(i, timer);	// If so, shoves it in.

					// Shaves off any times that go beyond the specified place.
					if (levelData.bestTimes.Count > settings.highScoreCount) {
						levelData.bestTimes.RemoveRange(settings.highScoreCount, levelData.bestTimes.Count - settings.highScoreCount);
					}

					Save(); // Saves game.

					break; // Breaks out of for loop once the entry is added.
				}
			}
		}
	}

	#endregion
}
