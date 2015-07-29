/// <summary>
/// InputManager.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 11, 2015
/// Last Revision: Jun. 24, 2015
/// 
/// Class that handles all game input.
/// 
/// NOTES: - Should probably be attached to GameMaster object.
/// 
/// TO DO: - Use new event system to promote independence!
/// 	   - Save player's key rebindings! This can use PlayerPrefs instead of serialization.
/// 	   - REFACTOR ONCE THE ABOVE IS COMPLETED.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	// Variables
	#region Variables
	GameMaster gm;				// Reference to Game Master.
	
	public bool allowInput;		// Whether the game is currently accepting input for standard purposes.

	public enum Keys {
		Forward,
		Backward,
		Left,
		Right,
		Jump,
		CamUp,
		CamDown,
		CamLeft,
		CamRight,
		CamToggle,
		Use,
		Brake,
		Respawn,
		LevelHelp,
		Pause,
		Special,
		ScrollLeft,
		ScrollRight,
		Menu
	}

	//public delegate void InputEvent();	// Datatype for input function containers.

	int numKeys = System.Enum.GetValues(typeof(Keys)).Length;
	public List<KeyCode> keyBindings = new List<KeyCode>();

/*
	public KeyCode forward;		// Which key moves the marble forward.
	public KeyCode backward;	// Which key moves the marble backward.
	public KeyCode left;		// Which key moves the marble left.
	public KeyCode right;		// Which key moves the marble right.
	public KeyCode jump;		// Which key makes the marble jump.

	public KeyCode camUp;		// Which key moves the camera upwards.
	public KeyCode camDown;		// Which key moves the camera downwards.
	public KeyCode camLeft;		// Which key moves the camera left.
	public KeyCode camRight;	// Which key moves the camera right.
	public KeyCode camToggle;	// Which key toggles the camera control mode.

	public KeyCode use;			// Which key uses buffs.
	public KeyCode levelHelp;	// Which key toggles level helpers.
	public KeyCode pause;		// Which key pauses the game.
	public KeyCode brake;		// Which key brakes the marble.
	public KeyCode respawn;		// Which key respawns the marble.
*/
	#endregion

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.input = this;
	}

	// OnEnable - Called when object is enabled.
	void OnEnable() {
		Messenger<bool>.AddListener("AllowInput", AllowInput);
	}

	// OnDisable - Called when object is disabled.
	void OnDisable() {
		Messenger<bool>.RemoveListener("AllowInput", AllowInput);
	}

	// Start - Use this for initialization
	void Start () {

		allowInput = true;
		
		for(int i = 0; i < numKeys; i++)
			keyBindings.Add (KeyCode.None);
		
		ResetDefault ();
	}
	
	public void ResetDefault(){
		// DEFAULT CONTROLS - SHOULD BE READ EXTERNALLY OR SOMETHING LATER MAYBE
		keyBindings[(int)Keys.Forward] = KeyCode.W;	
		keyBindings[(int)Keys.Backward] = KeyCode.S;
		keyBindings[(int)Keys.Left] = KeyCode.A;
		keyBindings[(int)Keys.Right] = KeyCode.D;
		keyBindings[(int)Keys.Jump] = KeyCode.Space;
		keyBindings[(int)Keys.CamUp] = KeyCode.UpArrow;
		keyBindings[(int)Keys.CamDown] = KeyCode.DownArrow;
		keyBindings[(int)Keys.CamLeft] = KeyCode.LeftArrow;
		keyBindings[(int)Keys.CamRight] = KeyCode.RightArrow;
		keyBindings[(int)Keys.CamToggle] = KeyCode.C;
		keyBindings[(int)Keys.Use] = KeyCode.F;
		keyBindings[(int)Keys.LevelHelp] = KeyCode.LeftShift;
		keyBindings[(int)Keys.Pause] = KeyCode.Escape;
		keyBindings[(int)Keys.Brake] = KeyCode.B;
		keyBindings[(int)Keys.Respawn] = KeyCode.R; // Disable this one for final product
		keyBindings[(int)Keys.Special] = KeyCode.Tab;
		keyBindings[(int)Keys.ScrollLeft] = KeyCode.Q;
		keyBindings[(int)Keys.ScrollRight] = KeyCode.E;
		keyBindings[(int)Keys.Menu] = KeyCode.V;
	}

	// AllowInput - Changes whether input is currently accepted or not.
	void AllowInput(bool allow) { allowInput = allow; }
	
	// Update - Called once per frame.
	void Update () {
		if (allowInput) {
			if (!gm.paused)
				CameraControls();
	
			// These controls are active during gameplay.
			//if (gm.state == GameMaster.GameState.Playing || gm.state == GameMaster.GameState.Start) {
				if (!gm.paused) {
					MarbleSpecialControls();
				}
				MenuControls();
			//}
	
			// These controls are suited to experimentation and have their own conditions.
			DebugControls();
		}
	}

	// FixedUpdate - Called at a fixed interval around every physics calculation.
	void FixedUpdate() {
		if (allowInput) {
			// As long as the game isn't paused, these controls handle marble movement and positioning.
			//if (gm.state == GameMaster.GameState.Playing || gm.state == GameMaster.GameState.Start) {
				if (!gm.paused && gm.marble) {
					MarbleMoveControls();
				}
			//}
		}
	}

	// MarbleMoveControls - Controls for moving the marble.
	void MarbleMoveControls() {
		// Forward movement.
		if (Input.GetKey(keyBindings[(int)Keys.Forward])) {
			Messenger.Broadcast("MarbleForward");
		}
		// Backward movement.
		if (Input.GetKey(keyBindings[(int)Keys.Backward])) {
			Messenger.Broadcast("MarbleBackward");
		}
		// Leftwards movement.
		if (Input.GetKey(keyBindings[(int)Keys.Left])) {
			Messenger.Broadcast("MarbleLeft");
		}
		// Rightwards movement.
		if (Input.GetKey(keyBindings[(int)Keys.Right])) {
			Messenger.Broadcast("MarbleRight");
		}
		// Jumping.
		if (Input.GetKey(keyBindings[(int)Keys.Jump])) {
			Messenger.Broadcast("MarbleJump");
		}
	}

	// MarbleSpecialControls - Controls that aren't related to directional movement.
	void MarbleSpecialControls() {
		// Use a held buff.
		if (Input.GetKeyDown(keyBindings[(int)Keys.Use])) {
			Messenger.Broadcast("UseBuff");
		}
		// Braking. Current implementation is better considered a debug control.
		if (Input.GetKey(keyBindings[(int)Keys.Brake])) {
			Messenger.Broadcast("MarbleBrake");
		}
		// Use an active ability.
		if (Input.GetKeyDown(keyBindings[(int)Keys.Special])) {
			Messenger.Broadcast("UseAbility");
		}
		// Scrolls ability box to the left.
		if (Input.GetKeyDown(keyBindings[(int)Keys.ScrollLeft])) {
			Messenger.Broadcast("ScrollLeft");
		}
		// Scrolls ability box to the right.
		if (Input.GetKeyDown(keyBindings[(int)Keys.ScrollRight])) {
			Messenger.Broadcast("ScrollRight");
		}
		// Open in-game menu.
		if (Input.GetKeyDown(keyBindings[(int)Keys.Menu])) {
			Messenger.Broadcast("GameMenu");
		}
		// Respawning. May be better to consider it a debug control instead.
		//if (Input.GetKeyDown (keyBindings[(int)Keys.Respawn]) && gm.debug) {
			//gm.marble.Die();//Respawn();
		//}
	}
	
	// CameraControls - Controls for the camera.
	void CameraControls() {
		// Move up.
		if (Input.GetKey(keyBindings[(int)Keys.CamUp])) {
			Messenger.Broadcast("CamUp", MessengerMode.DONT_REQUIRE_LISTENER);
		}
		// Move down.
		if (Input.GetKey(keyBindings[(int)Keys.CamDown])) {
			Messenger.Broadcast("CamDown", MessengerMode.DONT_REQUIRE_LISTENER);
		}
		// Move left.
		if (Input.GetKey(keyBindings[(int)Keys.CamLeft])) {
			Messenger.Broadcast("CamLeft", MessengerMode.DONT_REQUIRE_LISTENER);
		}
		// Move right.
		if (Input.GetKey(keyBindings[(int)Keys.CamRight])) {
			Messenger.Broadcast("CamRight", MessengerMode.DONT_REQUIRE_LISTENER);
		}

		// Toggle whether the keyboard or mouse control the camera.
		if (Input.GetKeyDown(keyBindings[(int)Keys.CamToggle])) {
			Messenger.Broadcast("CamModeToggle", MessengerMode.DONT_REQUIRE_LISTENER);
		}
	}

	// MenuControls - Controls for bringing up or closing menus (namely pausing right now).
	void MenuControls() {
		// Toggles the game being paused.
		if (Input.GetKeyDown(keyBindings[(int)Keys.Pause])) {
			Messenger.Broadcast("Pause");
		}

		// Toggles level guides.
		if (Input.GetKeyDown(keyBindings[(int)Keys.LevelHelp])) {
			Messenger.Broadcast("ToggleArrows");
		}
	}

	// DebugControls - Controls that are only active in debug mode.
	void DebugControls() {
		if (gm.settings.debug) {
			// Any button presses that you want to use for experimentation can go here.
		}
	}
	
}
