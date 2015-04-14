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
///		   - Dynamically populate menu with buttons and such. Check KanbanFlow for more stuff.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public GameMaster gm;			// Reference to Game Master.
	public GameObject mainSet;		// Reference to main set of buttons.
	public GameObject optionSet;	// Reference to option submenu buttons.
	public GameObject levelSet;		// Reference to select level submenu buttons.
	public GameObject controlSet;	// Reference to select controls submenu buttons.
	public GameObject props;
	public GameObject title;

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
	
	// ToggleSelectControls - Toggles the control submenu
	public void ToggleSelectControls()
	{
		optionSet.SetActive (!optionSet.activeSelf);
		controlSet.SetActive (!controlSet.activeSelf);
		props.SetActive (!props.activeSelf);
		title.SetActive (!title.activeSelf);
	}

	// QuitRequest - Quits the game.
	public void QuitRequest()
	{
		Application.Quit ();
	}
}
