/// <summary>
/// GameMaster.cs
/// Authors: Kyle Dawson, Charlie Sun, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 11, 2015
/// Last Revision: Mar. 11, 2015
/// 
/// Unifying class that controls game conditions and allows some inter-object communications.
/// 
/// NOTES: - This is a singleton class so only one of it should ever exist, if you need a reference to it, call GameMaster.CreateGM()
/// 
/// TO DO: - Add game conditions.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	// Enum for state of game.
	public enum GameState {
		Menu,		// State between or before levels.
		Start,		// The state immediately before the timer begins.
		Playing,	// The part of the game where mechanics matter.
		Win			// State immediately after player wins a level.
	}

	// Variables
	#region Variables
	public static GameMaster GM = null;	// Reference to singleton.

	public Transform marble;		// Reference to currently active marble.
	public Transform cam;			// Reference to camera.
	public Transform respawn;		// Reference to level's respawn point.
	//public LevelGUI gui;			// Reference to current level's GUI. Pending deprecation.
	public Transform finishLine;	// Reference to finish line.
	public InputManager input;		// Reference to input manager.
	public PauseMenu pauseMenu; 	// Reference to pause menu.
	public MainMenu mainMenu;		// Reference to main menu.
	public MainHUD hud;				// Reference to HUD.

	public GameState state;			// Current state of game.
	public bool paused;				// True if game is paused, false otherwise.
	public float timer;				// How much time has elapsed since the start of a level.
	public float countdownLength;	// How long timer should countdown in the starting phase.

	public bool debug = true;		// If true, game is currently in general debug mode. DEBUG
	public bool freezeTimer = false;// If true, timer will not change.
	
	public bool levelSelect = false;// Done for win screen

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
			Destroy (gameObject);
		}
		else{
			GM = this;  // Failsafe in case there was a condition in which the GM was not already set to be this.
			DontDestroyOnLoad(this); // GameMaster should exist forever.
		}
	}

	// Use this for initialization
	void Start () {
		name = "Game Master";
		timer = 0;
		state = (Application.loadedLevel == 0)? GameState.Menu : state; // DEBUG
		Debug.Log ("start timer is " + timer);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("update timer is " + timer);
		if (!freezeTimer) {	// If the timer isn't frozen,
			//Debug.Log ("its cold");
			if (!paused) {	// and the game isn't paused,
				//Debug.Log ("am i pauseed");
				if (state == GameState.Start && timer > 0) { // in the starting phase,
					timer -= Time.deltaTime;	// the timer counts down to 0,
					//Debug.Log ("COUNTING DOWWWWWWN");
					if (timer <= 0)	{ // and when it reaches 0, the gameplay begins.
						OnPlay();
					}

				} else if(state == GameState.Playing) { // Here the timer serves a different purpose,
					timer += Time.deltaTime;	// tracking the player's time since starting.
				}
			}
		}
	}

	// Pause - Toggles game paused state.
	public void TogglePause() {
		paused = !paused;
		Time.timeScale = (paused)? 0 : 1; // When paused, physics simulation speed is set to 0.
		if (pauseMenu)
			pauseMenu.gameObject.SetActive(paused);
		else
			Debug.LogWarning("(GameMaster.cs) No pause menu found!");
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
		//Debug.Log ("RESETTING");
		
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
				levelSelect = !levelSelect;
			}	
		}
		
		//if (level != 0 && level != 1) {
			//state = GameState.OnStart();
			//OnStart();
		//}
	}

	// OnStart - Called when a level is to be started.
	public void OnStart() {
		Time.timeScale = 1;
		//Debug.Log ("start 'er up");
		timer = countdownLength + hud.goLength;
		state = GameState.Start;
		marble.GetComponent<Marble>().Respawn();
		marble.GetComponent<Rigidbody>().isKinematic = false;

		if (respawn) {
			respawn.GetComponent<SpawnArea>().sfx.SetActive(true);
		}

		hud.countdown.gameObject.SetActive(true);

		if (finishLine)	finishLine.GetComponent<FinishLine>().FlameOff();
		//Debug.Log ("ONSTART timer is " + timer);
	}

	// OnPlay - Called when the player is to actually play the level.
	public void OnPlay() {
		Time.timeScale = 1;
		timer = 0;
		marble.GetComponent<Rigidbody>().isKinematic = false;
		state = GameState.Playing;

		if (respawn) {
			respawn.GetComponent<SpawnArea>().sfx.SetActive(false);
		}

		hud.countdown.gameObject.SetActive(false);

		if (finishLine)	finishLine.GetComponent<FinishLine>().FlameOff();
	}

	// OnWin - Called when a level is won.
	public void OnWin() {
		state = GameState.Win;
		Debug.Log("(GameMaster.cs) You win!");
		Time.timeScale = 0.5f; // Slowmo victory!
		//marble.GetComponent<Marble>().Brake();
		
		if (finishLine) finishLine.GetComponent<FinishLine> ().FlameOn ();
	}
}
