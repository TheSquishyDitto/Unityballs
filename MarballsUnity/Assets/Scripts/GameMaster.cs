/// <summary>
/// GameMaster.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 11, 2015
/// Last Revision: Apr. 16, 2015
/// 
/// Unifying class that controls game conditions and allows some inter-object communications.
/// 
/// NOTES: - This is a singleton class so only one of it should ever exist, if you need a reference to it, call GameMaster.CreateGM()
/// 
/// TO DO: - Add game conditions.
/// 	   - Add more events to subscribe to?
/// 
/// </summary>

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

public class GameMaster : MonoBehaviour {

	// Enum for state of game.
	public enum GameState {
		Menu,		// State between or before levels.
		//Prestart,	// State where the camera pans around the level.
		Start,		// The state immediately before the timer begins.
		Playing,	// The part of the game where mechanics matter.
		Win			// State immediately after player wins a level.
	}

	public delegate void StartAction();
	public delegate void PlayAction();
	public delegate void DieAction();
	public delegate void WinAction();

	// Variables
	#region Variables
	// public string version = "[insert fancy decimals here]"; // Which version of Marballs is currently running.

	public static GameMaster GM;	// Reference to singleton.

	public Marble marble;			// Reference to currently active marble.
	public Transform cam;			// Reference to camera.
	public SpawnArea respawn;		// Reference to level's respawn point.
	public Transform finishLine;	// Reference to finish line.
	public InputManager input;		// Reference to input manager.
	public PauseMenu pauseMenu; 	// Reference to pause menu.
	public MainMenu mainMenu;		// Reference to main menu.
	public ControlScript controlMenu;	// Reference to control menu.
	public MainHUD hud;				// Reference to HUD.
	public LevelDataObject[] data;	// Reference to information about all levels.

	public GameState state;			// Current state of game.
	public bool paused;				// True if game is paused, false otherwise.
	public float timer;				// How much time has elapsed since the start of a level.
	public float countdownLength;	// How long timer should countdown in the starting phase.
	public bool simpleAnim = false;	// Whether the victory animation should be excessive or not.
	public bool useOnGrab = false;	// If true, picked up powerups are used immediately and automatically.
	public bool guides = false;		// If true, arrows pop up to help the player.

	public bool debug = true;		// If true, game is currently in general debug mode. DEBUG
	public bool freezeTimer = false;// If true, timer will not change.
	
	public bool levelSelect = false;// Done for win screen

	// Events
	public static event StartAction start;	// Container for actions when game starts. 
	public static event PlayAction play;	// Container for actions when gameplay begins.
	public static event DieAction die;		// Container for actions when player dies.
	public static event WinAction win;		// Container for actions when winning.

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
	}

	// Start - Use this for initialization.
	void Start () {
		name = "Game Master";
		timer = 0;
		state = (Application.loadedLevel == 0)? GameState.Menu : state; // DEBUG

		Debug.Log("Save File Path: " + Application.persistentDataPath); // DEBUG - This is where data is saved to.
		Load(); // Loads game data.
	}
	
	// Update - Called once per frame.
	void Update () {
		if (!freezeTimer) {	// If the timer isn't frozen,
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
	}

	// TogglePause - Toggles game paused state.
	public void TogglePause() {
		paused = !paused;
		input.allowInput = !(input.allowInput);
		Time.timeScale = (paused)? 0 : 1; // When paused, physics simulation speed is set to 0.
		if (pauseMenu) {
			pauseMenu.gameObject.SetActive(paused);
			// [ code to reset pause menu buttons to initial state ]
		} else
			Debug.LogWarning("(GameMaster.cs) No pause menu found!");
	}

	// ToggleGuides - Toggles helpful level guides such as finish line arrows.
	public void ToggleGuides() {
		guides = !guides;
	}

	// CancelCoroutines - Stops various animations and clears what they've done.
	// TODO REFACTOR THIS
	public void CancelCoroutines() {
		hud.StopCoroutine("OnDeath");
		finishLine.GetComponent<FinishLine>().StopCoroutine("SwirlFinish");
		hud.StopCoroutine("OnVictory");
		hud.winOptions.SetActive(false);
		marble.transform.localScale = new Vector3(marble.defSize, marble.defSize, marble.defSize);
		hud.deathScreen.color = new Color(hud.deathScreen.color.r, hud.deathScreen.color.g, hud.deathScreen.color.b, 0);
		hud.deathMessage.color = new Color(hud.deathMessage.color.r, hud.deathMessage.color.g, hud.deathMessage.color.b, 0);
		hud.winScreen.color = new Color(hud.winScreen.color.r, hud.winScreen.color.g, hud.winScreen.color.b, 0);
		hud.winMessage.color = new Color(hud.winMessage.color.r, hud.winMessage.color.g, hud.winMessage.color.b, 0);
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
		cam = null;
		respawn = null;
		//gui = null;
		finishLine = null;
		pauseMenu = null;
		hud = null;
		
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

	// OnLevelWasLoaded - Triggers every time a level loads.
	void OnLevelWasLoaded(int level) {

		if(level == 0)	{
			if(levelSelect)
			{
				mainMenu.ToggleSelectLevel();
				levelSelect = false;
			}	
		}
	}

	// Save - Saves the player's time.
	void Save() {
		// Creates/overwrites file.
		BinaryFormatter converter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/PlayerTimes.dat");

		// Writes level data to player record.
		PlayerRecord record = new PlayerRecord();
		for (int i = 0; i < data.Length; i++) {
			if (data[i] != null) {
				record.bestTimes.Add(data[i].bestTime);
				record.levelNames.Add(data[i].levelName);
			}
		}

		// Saves data and closes file.
		converter.Serialize(file, record);
		file.Close();

		Debug.Log("(GameMaster.cs) Saved file!");
	}

	// Load - Loads the player's time.
	void Load() {
		// Checks if file exists.
		if (File.Exists(Application.persistentDataPath + "/PlayerTimes.dat")) {
			// Opens file.
			BinaryFormatter converter = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/PlayerTimes.dat", FileMode.Open);

			// Reads what data is there and closes file.
			PlayerRecord records = (PlayerRecord)converter.Deserialize(file);
			file.Close();

			// Loads data into level data.
			if (records.bestTimes.Count == data.Length) {
				for (int i = 0; i < data.Length; i++) {
					Debug.Log("LOADED DATA FOR LEVEL " + i + ": " + records.bestTimes[i]); // DEBUG

					// Checks to make sure saved data and active data share formatting.
					if (data[i] != null && records.bestTimes != null && records.levelNames != null && data[i].levelName == records.levelNames[i])
						data[i].bestTime = records.bestTimes[i];
					else
						Debug.LogWarning("(GameMaster.cs) Saved data and active data are mismatched!");
				}
			} else {
				Debug.LogWarning("(GameMaster.cs) Saved data and active data sets are different lengths!");
			}
		}
	}

	// State Changers - Functions that change the game's conditions.
	#region State Changers
	// OnStart - Called when a level is to be started.
	public void OnStart() {
		Time.timeScale = 1;
		timer = countdownLength;
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

	// OnDeath - Called when the player dies.
	public void OnDeath() {
		if (die != null) die();	// Should subscribe the HUD's OnDeath functionality to this.
	}

	// OnWin - Called when a level is won.
	public void OnWin() {
		state = GameState.Win;
		Time.timeScale = 0.5f; // Slowmo victory!

		if (win != null) win(); // Should subscribe the various victory functions to this.

		// If the player got a better time than the current best, saves their data.
		if (Application.loadedLevel > 0 && data[Application.loadedLevel - 1].bestTime > timer) {
			data[Application.loadedLevel - 1].bestTime = timer;
			Save();
		}
	}

	#endregion
}
