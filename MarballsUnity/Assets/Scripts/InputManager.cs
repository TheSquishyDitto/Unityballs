/// <summary>
/// InputManager.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 11, 2015
/// Last Revision: Apr. 18, 2015
/// 
/// Class that handles all game input.
/// 
/// NOTES: - Should probably be attached to GameMaster object.
/// 
/// TO DO: - Allow customizable keys.
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
		Pause
	}

	//public delegate void InputEvent();	// Datatype for input function containers.

	int numKeys = System.Enum.GetValues(typeof(Keys)).Length;
	public List<KeyCode> keyBindings = new List<KeyCode>();

/*
	public static event InputEvent[] pressed;	// Containers for functions that occur when key is pressed.
	// Main advantage to this is that the InputManager doesn't need to know about the functions it's calling.


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
		keyBindings[(int)Keys.Respawn] = KeyCode.R; // Disable for final product
	}
	
	// Update - Called once per frame.
	void Update () {
		if (allowInput) {
			CameraControls();
	
			// These controls are active during gameplay.
			if (gm.state == GameMaster.GameState.Playing || gm.state == GameMaster.GameState.Start) {
				if (!gm.paused) {
					MarbleSpecialControls();
				}
				MenuControls();
			}
	
			// These controls are suited to experimentation and have their own conditions.
			DebugControls();

			/*
			// Event-based code would put most of the checking tasks on the objects with the actual functions.
			for(int i; i < pressed.Length; i++) {
				if (pressed[i] != null)
					pressed[i]();
			}

			 */ 
		}
	}

	// FixedUpdate - Called at a fixed interval around every physics calculation.
	void FixedUpdate() {
		if (allowInput) {
			// As long as the game isn't paused, these controls handle marble movement and positioning.
			if (gm.state == GameMaster.GameState.Playing || gm.state == GameMaster.GameState.Start) {
				if (!gm.paused && gm.marble) {
					MarbleMoveControls();
				}
			}
		}
	}

	// MarbleMoveControls - Controls for moving the marble.
	void MarbleMoveControls() {
		// Forward movement.
		if (Input.GetKey(keyBindings[(int)Keys.Forward])) {
			gm.marble.Forward();
		}
		// Backward movement.
		if (Input.GetKey(keyBindings[(int)Keys.Backward])) {
			gm.marble.Backward();
		}
		// Leftwards movement.
		if (Input.GetKey(keyBindings[(int)Keys.Left])) {
			gm.marble.Left();
		}
		// Rightwards movement.
		if (Input.GetKey(keyBindings[(int)Keys.Right])) {
			gm.marble.Right();
		}
		// Jumping.
		if (Input.GetKey(keyBindings[(int)Keys.Jump])) {
			gm.marble.Jump();
		}
		/*// Use a held buff.
		if (Input.GetKeyDown(use)) {
			gm.marble.UseBuff();
		}
		// Braking. Current implementation is better considered a debug control.
		if (Input.GetKey(brake)) {
			gm.marble.Brake();
		}
		// Respawning. May be better to consider it a debug control instead.
		if (Input.GetKeyDown (respawn)) {
			gm.marble.Respawn();
		}*/
	}

	// MarbleSpecialControls - Controls that aren't related to directional movement.
	void MarbleSpecialControls() {
		// Use a held buff.
		if (Input.GetKeyDown(keyBindings[(int)Keys.Use])) {
			gm.marble.UseBuff();
		}
		// Braking. Current implementation is better considered a debug control.
		if (Input.GetKey(keyBindings[(int)Keys.Brake])) {
			gm.marble.Brake();
		}
		// Respawning. May be better to consider it a debug control instead.
		if (Input.GetKeyDown (keyBindings[(int)Keys.Respawn])) {
			gm.marble.Respawn();
		}
	}
	
	// CameraControls - Controls for the camera.
	void CameraControls() {
		if (gm.cam) {
			// These controls are relevant to moving the camera when the camera is in keyboard mode.
			if (gm.cam.GetComponent<CameraController>().mode == CameraController.ControlMode.Keyboard) {
				// Move up.
				if (Input.GetKey(keyBindings[(int)Keys.CamUp])) {
					gm.cam.GetComponent<CameraController>().MoveUp();
				}
				// Move down.
				if (Input.GetKey(keyBindings[(int)Keys.CamDown])) {
					gm.cam.GetComponent<CameraController>().MoveDown();
				}
				// Move left.
				if (Input.GetKey(keyBindings[(int)Keys.CamLeft])) {
					gm.cam.GetComponent<CameraController>().MoveLeft();
				}
				// Move right.
				if (Input.GetKey(keyBindings[(int)Keys.CamRight])) {
					gm.cam.GetComponent<CameraController>().MoveRight();
				}
			}
			// Toggle whether the keyboard or mouse control the camera.
			if (Input.GetKeyDown(keyBindings[(int)Keys.CamToggle])) {
				gm.cam.GetComponent<CameraController>().ToggleControlMode();
			}
		}
	}

	// MenuControls - Controls for bringing up or closing menus (namely pausing right now).
	void MenuControls() {
		// Toggles the game being paused.
		if (Input.GetKeyDown(keyBindings[(int)Keys.Pause])) {
			gm.TogglePause();
		}

		// Toggles level guides.
		if (Input.GetKeyDown(keyBindings[(int)Keys.LevelHelp])) {
			gm.ToggleGuides();
		}
	}

	// DebugControls - Controls that are only active in debug mode.
	void DebugControls() {
		if (gm.debug) {
			// Any button presses that you want to use for experimentation can go here.
		}
	}
	
}
