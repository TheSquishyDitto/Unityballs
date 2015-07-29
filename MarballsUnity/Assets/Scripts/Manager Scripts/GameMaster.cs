/// <summary>
/// GameMaster.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 11, 2015
/// Last Revision: July 23, 2015
/// 
/// Unifying class that controls game conditions and allows some inter-object communications.
/// 
/// NOTES: - This is a singleton class so only one of it should ever exist, if you need a reference to it, call GameMaster.CreateGM()
/// 	   - This class is mainly for handling game flow (level start events, level timer, etc.)
/// 
/// TO DO: - Split saving/loading and level loading from this class.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class GameMaster : MonoBehaviour {

	// Enum for state of game.
	/*public enum GameState {
		Menu,		// State between or before levels.
		Prestart,	// State where the camera pans around the level.
		Start,		// The state immediately before the timer begins.
		Playing,	// The part of the game where mechanics matter.
		Win,		// State immediately after player wins a level.
		//Sumo		// Multiplayer mode.
	}*/

	// Variables
	#region Variables
	public static GameMaster GM;		// Reference to singleton.

	[Header("References")]
	public Settings settings;			// Reference to most game settings.
	public Marble marble;				// Reference to currently active marble.
	public InputManager input;			// Reference to input manager.
	public ControlScript controlMenu;	// Reference to control menu.
	public LevelDataObject levelData;	// Reference to information about current level.

	public Transform guiContainer;		// Reference to parent of GUI objects.

	[Header("Gameplay Variables")] 	// Some of these variables should be moved into getters/setters probably.
	//public GameState state;		// Current state of game.
	public bool paused;				// True if game is paused, false otherwise.
	public float timer = 0;			// How much time has elapsed since the start of a level.
	public int buildLevelCap = 9;	// The last level accessible through standard progression.

	public bool levelSelect = false;// Done for win screen
	
	//public static event UnityAction pan;	// Container for actions before the game actually starts.
	//public static event UnityAction start;	// Container for actions when game starts. 
	//public static event UnityAction play;	// Container for actions when gameplay begins.
	//public static event UnityAction win;	// Container for actions when winning.

	public static Sequence sequence = new Sequence();	// Container for actions to perform at level start.

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

	// Monobehaviour Functions
	#region Monobehaviour Functions

	// Awake - Called before anything else.
	void Awake () {
		if (GM != null) {
			Destroy (gameObject);	// If a GameMaster already exists, destroys this instance.
		} else {
			GM = this;  // Failsafe in case there was a condition in which the GM was not already set to be this.
			DontDestroyOnLoad(this); // GameMaster should exist forever.
		}

		timer = 0;
		settings = LoadSettings();
		guiContainer = GameObject.FindGameObjectWithTag("GUI").transform;
		settings.freezeTimer = true;
		//state = (Application.loadedLevel == 0)? GameState.Menu : state; // DEBUG
		LoadLevelData();
	}

	// OnEnable - Called when enabled. Generally used to subscribe to events.
	void OnEnable() {
		Messenger.AddListener("Pause", TogglePause);
		Messenger.AddListener("BeginTimer", BeginTimer);
	}

	// OnDisable - Called when disabled. Used to unsubscribe from events to prevent memory leaks.
	void OnDisable() {
		Messenger.RemoveListener("Pause", TogglePause);
		Messenger.RemoveListener("BeginTimer", BeginTimer);
	}

	// Start - Use this for initialization.
	void Start () {
		name = "Game Master";

		Debug.Log("Save File Path: " + Application.persistentDataPath); // DEBUG - This is where data is saved to.
	}
	
	// Update - Called once per frame.
	void Update () {
		if (!settings.freezeTimer) {	// If the timer isn't frozen, increase it steadily.
			timer += Time.deltaTime;
		}
	}

	#endregion

	// BeginTimer - Starts up timer.
	void BeginTimer() {
		timer = 0;
		settings.freezeTimer = false;
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
		//state = GameState.Menu;
		timer = 0;
		if (paused) { TogglePause(); }
		marble = null;
		
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

		guiContainer = GameObject.FindGameObjectWithTag("GUI").transform;
		settings.freezeTimer = true;

		if(level == 0)	{
			if(levelSelect)
			{
				//mainMenu.ToggleSelectLevel();
				levelSelect = false;
			}	
		}
	}

	// Saving/Loading Functions - Functions relevant to persisting game data.
	#region Saving/Loading Functions

	// LoadSettings - Easily loads game settings.
	public static Settings LoadSettings() {
		return Resources.Load("Data/GameSettings") as Settings;
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
	public void Save() {
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

	#endregion

	// State Changers - Functions that change the game's conditions.
	#region State Changers
	// OnPreStart - Called before level starts.
	/*public void OnPreStart(){
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
	}*/

	#endregion
}

/*[Serializable]
// This class should be used to save and load marble's data.
// NOTE: Any data that needs to persist across game sessions should be stored here.
public class MarbleProfile {
	int hp;		// Current health.
	int maxHP;	// Max health.
	int mp;		// Current marble power.
	int maxMP;	// Maximum marble power.
	int cc;		// Current charm capacity.
	int maxCC;	// Maximum charm capacity.
	int xp;		// Unspent gathered balloons.
	int level;	// How many times balloons have been used to level up.

	int defense;			// How much the marble resists damage.
	float damageThreshold;	// Min. velocity for offense.
	float damageInterval;	// Rate at which offense scales with velocity.
	
	//serializable list of gathered charms and whether they're equipped

	// Constructor that rips all data from marble and stores it.
	public MarbleProfile(Marble marble) {

		// charms and their states should be saved here
		// temporarily unequip all charms before saving data? not sure if that step would be necessary

		hp = marble.HP;
		maxHP = marble.maxHP;
		mp = marble.MP;
		maxMP = marble.maxMP;
		cc = marble.CC;
		maxCC = marble.maxCC;
		xp = marble.XP;
		level = marble.Level;

		defense = marble.defense; // USE GETTER/SETTERS WHEN AVAILABLE
		damageThreshold = marble.damageThreshold;
		damageInterval = marble.damageInterval;

		// re-equip charms if they were unequipped for saving
	}




}*/
