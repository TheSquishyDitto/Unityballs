/// <summary>
/// GameMaster.cs
/// Authors: Kyle Dawson, Charlie Sun, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  ???    , 2015
/// Last Revision: Feb. 11, 2015
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
	public static GameMaster GM;	// Reference to singleton.

	public Transform marble;		// Reference to currently active marble.
	public Transform cam;			// Reference to camera.
	public LevelGUI gui;			// Reference to current level's GUI. 

	public GameState state;			// Current state of game.
	public bool paused;				// True if game is paused, false otherwise.
	public float timer;				// How much time has elapsed since the start of a level.

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
		DontDestroyOnLoad(this); // GameMaster should exist forever.
		GM = this; // Failsafe in case there was a condition in which the GM was not already set to be this.
	}

	// Use this for initialization
	void Start () {
		name = "Game Master";
		timer = 0;
		state = (Application.loadedLevel == 0)? GameState.Menu : GameState.Playing; // DEBUG
	}
	
	// Update is called once per frame
	void Update () {
		if(state == GameState.Playing && !paused) {
			timer += Time.deltaTime;
		}
	}

	// Pause - Toggles game paused state.
	public void TogglePause() {
		paused = !paused;
		Time.timeScale = (paused)? 0 : 1; // When paused, physics simulation speed is set to 0.
	}
	
	// ResetVariables - Clears and sets variables to their initial states.
	void ResetVariables() {
		state = GameState.Menu;
		timer = 0;
		if (paused) { TogglePause(); }
		marble = null;
		cam = null;
		gui = null;
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
		if (level != 0) {
			state = GameState.Start;
		}
	}
	
	public void OnWin() {
		state = GameState.Win;
		Time.timeScale = 0.5f;
		//marble.GetComponent<Marble>().Brake();
	}
}
