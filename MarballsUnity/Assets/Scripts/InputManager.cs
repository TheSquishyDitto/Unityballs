/// <summary>
/// InputManager.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 11, 2015
/// Last Revision: Mar. 22, 2015
/// 
/// Class that handles all game input.
/// 
/// NOTES: - Should probably be attached to GameMaster object.
/// 
/// TO DO: - Allow customizable keys.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	// Variables
	#region Variables
	public GameMaster gm;		// Reference to Game Master.
	
	public bool allowInput;		// Whether the game is currently accepting input for standard purposes.

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

	public KeyCode pause;		// Which key pauses the game.
	public KeyCode brake;		// Which key brakes the marble.
	public KeyCode respawn;		// Which key respawns the marble.

	#endregion

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.input = this;
	}

	// Start - Use this for initialization
	void Start () {

		allowInput = true;

		// DEFAULT CONTROLS - SHOULD BE READ EXTERNALLY OR SOMETHING LATER
		forward = KeyCode.W;
		backward = KeyCode.S;
		left = KeyCode.A;
		right = KeyCode.D;
		jump = KeyCode.Space;
		camUp = KeyCode.UpArrow;
		camDown = KeyCode.DownArrow;
		camLeft = KeyCode.LeftArrow;
		camRight = KeyCode.RightArrow;
		camToggle = KeyCode.C;
		pause = KeyCode.Escape;
		brake = KeyCode.B;
		respawn = KeyCode.R;
	}
	
	// Update - Called once per frame.
	void Update () {
		if (allowInput) {
			CameraControls();
	
			// These controls are active during gameplay.
			if (gm.state == GameMaster.GameState.Playing || gm.state == GameMaster.GameState.Start) {
			
				MenuControls();
			}
	
			// These controls are suited to experimentation and have their own conditions.
			DebugControls();
		}
	}

	// FixedUpdate - Called at a fixed interval around every physics calculation.
	void FixedUpdate() {
		if (allowInput) {
			// As long as the game isn't paused, these controls handle marble movement and positioning.
			if (gm.state == GameMaster.GameState.Playing || gm.state == GameMaster.GameState.Start) {
				if (!gm.paused) {
					MarbleControls();
				}
			}
		}
	}

	// MarbleControls - Controls for the marble.
	void MarbleControls() {
		if (gm.marble) {
			// Forward movement.
			if (Input.GetKey(forward)) {
				gm.marble.Forward();
			}
			// Backward movement.
			if (Input.GetKey(backward)) {
				gm.marble.Backward();
			}
			// Leftwards movement.
			if (Input.GetKey(left)) {
				gm.marble.Left();
			}
			// Rightwards movement.
			if (Input.GetKey(right)) {
				gm.marble.Right();
			}
			// Jumping.
			if (Input.GetKeyDown(jump)) {
				gm.marble.Jump();
			}
			// Braking. Current implementation is better considered a debug control.
			if (Input.GetKey(brake)) {
				gm.marble.Brake();
			}
			// Respawning. May be better to consider it a debug control instead.
			if (Input.GetKeyDown (respawn)) {
				gm.marble.Respawn();
			}
		}
	}

	// CameraControls - Controls for the camera.
	void CameraControls() {
		if (gm.cam) {
			// These controls are relevant to moving the camera when the camera is in keyboard mode.
			if (gm.cam.GetComponent<CameraController>().mode == CameraController.ControlMode.Keyboard) {
				// Move up.
				if (Input.GetKey(camUp)) {
					gm.cam.GetComponent<CameraController>().MoveUp();
				}
				// Move down.
				if (Input.GetKey(camDown)) {
					gm.cam.GetComponent<CameraController>().MoveDown();
				}
				// Move left.
				if (Input.GetKey(camLeft)) {
					gm.cam.GetComponent<CameraController>().MoveLeft();
				}
				// Move right.
				if (Input.GetKey(camRight)) {
					gm.cam.GetComponent<CameraController>().MoveRight();
				}
			}
			// Toggle whether the keyboard or mouse control the camera.
			if (Input.GetKeyDown(camToggle)) {
				gm.cam.GetComponent<CameraController>().ToggleControlMode();
			}
		}
	}

	// MenuControls - Controls for bringing up or closing menus (namely pausing right now).
	void MenuControls() {
		// Toggles the game being paused.
		if (Input.GetKeyDown(pause)) {
			gm.TogglePause();
		}
	}

	// DebugControls - Controls that are only active in debug mode.
	void DebugControls() {
		if (gm.debug) {
			// Any button presses that you want to use for experimentation can go here.
		}
	}
}
