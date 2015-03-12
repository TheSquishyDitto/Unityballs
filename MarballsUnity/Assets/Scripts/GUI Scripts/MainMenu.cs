/// <summary>
/// MainMenu.cs
/// Authors: Kyle Dawson, Charlie Sun, Brenton Brown [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 11, 2015
/// Last Revision: Mar.  9, 2015
/// 
/// Class that displays the main menu and gives it function.
/// 
/// NOTES: - Currently very basic, plenty to be done!
/// 
/// TO DO: - Make it pretty.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public GameMaster gm;			// Reference to Game Master.
	public GameObject mainSet;		// Reference to main set of buttons.
	public GameObject optionSet;	// Reference to option submenu buttons.
	public GameObject levelSet;		// Reference to select level submenu buttons.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.mainMenu = this;
	}

	// LoadLevel - Tells the GameMaster to load a level.
	public void LoadLevel(string name)
	{
		// [ If we wanna add loading text we should do that here. ]
		gm.LoadLevel(name);
	}

	// ToggleOptions - Toggles the option submenu.
	public void ToggleOptions()
	{
		mainSet.SetActive (!mainSet.activeSelf);
		optionSet.SetActive (!optionSet.activeSelf);
	}

	// ToggleSelectLevel - Toggles the level selection submenu.
	public void ToggleSelectLevel()
	{
		mainSet.SetActive (!mainSet.activeSelf);
		levelSet.SetActive (!levelSet.activeSelf);
	}

	// QuitRequest - Quits the game.
	public void QuitRequest()
	{
		Application.Quit ();
	}
	
	/*
	// Enum for main menu state.
	public enum MenuState {
		Start,			// Typical title screen state.
		MainMenu,		// Menu for options, level selection, exiting, etc.
		LevelSelect,	// Menu for choosing level to go to.
		Loading			// Loading screen.
	}
	
	public MenuState state;	// What state the main menu is in.

	// OnGUI - Used for GUIs. This is a draw call so order, presence, or absence of code can be fairly important.
	// NOTE: Currently using a whole lotta magic numbers, feel free to change those to constants or public variables and experiment.
	void OnGUI () {
		// Title Screen
		if (state == MenuState.Start) {
			GUI.Label (new Rect(50, 50, 200, 25), "Press any key to continue.");

		// Main Menu
		} else if (state == MenuState.MainMenu) {
			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 40, 100, 30), "Level Select")) {
				state = MenuState.LevelSelect;
			}

			GUI.enabled = false; // TEMPORARY
			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 10, 100, 30), "Options")) {
				//[insert options menu]
			}

			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 + 20, 100, 30), "Quit")) {
				//Application.Quit; // Does nothing until executable is built.
			}
			GUI.enabled = true;	// TEMPORARY

		// Level Selection
		} else if (state == MenuState.LevelSelect) {
			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 40, 100, 30), "Level 1")) {
				state = MenuState.Loading;
				gm.LoadLevel("Level1");
			}

			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 10, 100, 30), "Test Level")) {
				state = MenuState.Loading;
				gm.LoadLevel("TestLevel");
			}
			
			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 + 20, 100, 30), "Back")) {
				state = MenuState.MainMenu;
			}

		// Loading Screen
		} else if (state == MenuState.Loading) {
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "LOADING...");
		}
	}
	
	// Update - Called once per frame.
	void Update () {
		if (state == MenuState.Start && Input.anyKey) {
			state = MenuState.MainMenu;
		}
	}
	
	*/
}
