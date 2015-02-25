/// <summary>
/// CameraController.cs
/// Authors: Kyle Dawson, Chris Viqueira, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan. 28, 2015
/// Last Revision: Feb. 23, 2015
/// 
/// Class that controls camera movement.
/// 
/// NOTES: - Current camera controls are C, and either arrow keys or mouse depending on mode.
/// 
/// TO DO: - Tweak movement until desired.
/// 	   - Make cursor lock work properly with pause screen.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	// Enum for determining how to control camera.
	public enum ControlMode {
		Keyboard,
		Mouse
	}
	
	// Variables
	// (Regions don't do anything functionally, they just help organize code and can be collapsed)
	#region Variables
	public GameMaster gm;					// Reference to Game Master.
	public Transform ball; 					// Reference to coordinates of marble
	
	public float theta = Mathf.PI / 2;  	// Radians around y-axis (horizontal).
	public float psy = 0.5f;	 			// Radians around x-axis (vertical).
	public float radius = 15; 				// Distance from marble.
	public float playerRadius;
	
	public const float PSYMAX = (Mathf.PI / 2) - 0.1f; // Maximum value for psy. Camera inverts at Pi/2+.
	public const float PSYMIN = 0;					   // Minimum value for psy.
	public const float RADMIN = 3;						// Minimum distance from marble
	
	//public float smooth = 1; // Used to adjust how smoothly the camera adjusts position between frames (Lerp)
	
	public ControlMode mode;		 	// Which control mode camera is using.
	public float sensitivity = 0.05f; 	// Mouse sensitivity.
	//public bool invertX;				// Whether horizontal mouse controls should be inverted.
	//public bool invertY;				// Whether vertical mouse controls should be inverted.
	
	#endregion
	
	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.cam = this.transform;
	}
	
	// Start - Use this for initialization
	void Start () {
		playerRadius = radius;
		ball = gm.marble;//GameObject.FindGameObjectWithTag("Marble").transform;
		//radius = Vector3.Distance(transform.position, ball.position);
		mode = ControlMode.Keyboard;
	}
	
	// Update - Called once per frame
	void Update () {
		// Keyboard mode controls
		if (mode == ControlMode.Keyboard) { 
			// Moved to control functions, handled by input manager.
			
			// Mouse mode controls
		} else if (mode == ControlMode.Mouse) {
			// Moving the mouse moves the camera.
			psy = Mathf.Clamp(psy + (Input.GetAxis("Mouse Y") * sensitivity), PSYMIN, PSYMAX);
			theta -= Input.GetAxis("Mouse X") * sensitivity;
			
			
			// Unspecified case controls
		} else {
			Debug.LogWarning("(CameraController.cs) Control mode not specified!");
		}
		
		// Allows zooming in and out.
		if(Input.GetAxis("Mouse ScrollWheel") != 0) {
			radius -= Input.GetAxis("Mouse ScrollWheel");
			playerRadius = radius;
		} 
		
		if (radius < RADMIN) {
			radius = RADMIN;
		}
	}
	
	// LateUpdate - Called directly after everything updates
	void LateUpdate () {
		// Consider attempting to Lerp again if there's any jitter.
		gameObject.transform.position = GetSphericalPosition();
		gameObject.transform.LookAt(ball.position);
	}
	
	
	// GetSphericalPosition - Return spherical coordinate of camera
	Vector3 GetSphericalPosition() {
		Vector3 retPos = new Vector3();
		
		// These are all using radians.
		retPos.x = radius * Mathf.Cos (psy) * Mathf.Cos (theta) + ball.position.x;
		retPos.y = radius * Mathf.Sin (psy) + ball.position.y;
		retPos.z = radius * Mathf.Cos (psy) * Mathf.Sin (theta) + ball.position.z;
		
		return retPos;
	}
	
	// Control Functions
	#region Control Functions
	// Moves camera up.
	public void MoveUp() {
		if(!Physics.Raycast(transform.position, transform.up, 0.8f)){	// Checks if camera is close to another object
			psy = Mathf.Clamp(psy + .03f, PSYMIN, PSYMAX);
		}
	}
	
	// Moves camera down.
	public void MoveDown() {
		if(!Physics.Raycast(transform.position, -transform.up, 0.8f)){	// Checks if camera is close to another object
			psy = Mathf.Clamp(psy - .03f, PSYMIN, PSYMAX);
		}
		
	}
	
	// Moves camera left.
	public void MoveLeft() {
		if(!Physics.Raycast(transform.position, -transform.right, 0.8f)){	// Checks if camera is close to another object
			theta -= .03f;
		}
		
	}
	
	// Moves camera right.
	public void MoveRight() {
		if(!Physics.Raycast(transform.position, transform.right, 0.8f)){	// Checks if camera is close to another object
			theta += .03f;
		}
		
	}
	
	// ToggleControlMode - Changes camera control style.
	public void ToggleControlMode() {
		if (mode == ControlMode.Keyboard) {
			mode = ControlMode.Mouse;
			Screen.lockCursor = true;	// When true, cursor is hidden and constantly centered.
		} else if (mode == ControlMode.Mouse) {
			mode = ControlMode.Keyboard;
			Screen.lockCursor = false;	// Undoes lock. Lock always undone by Escape due to Unity implementation.
		}
	}
	
	public void checkCollision() {
		Debug.Log("Hit");
	}
	
	#endregion
}