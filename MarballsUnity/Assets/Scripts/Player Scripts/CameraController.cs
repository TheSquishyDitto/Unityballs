/// <summary>
/// CameraController.cs
/// Authors: Kyle Dawson, Charlie Sun, Chris Viqueira
/// Date Created:  Jan. 28, 2015
/// Last Revision: Apr. 13, 2015
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
	#region Variables
	public GameMaster gm;					// Reference to Game Master.
	public Transform marble; 				// Reference to coordinates of marble

	public float defTheta = Mathf.PI / 2;   // Default value of theta.
	public float defPsy = 0.5f;				// Default value of psy.
	public float defRadius = 15;			// Default radius from marble.

	public float theta;  					// Radians around y-axis (horizontal).
	public float psy;	 					// Radians around x-axis (vertical).
	public float radius; 					// Distance from marble.
	public float playerRadius;				// Player preferred distance from marble.
	public float playerPsy;					// Player preferred psy. Currently not adjustable.
	
	public const float PSYMAX = (Mathf.PI / 2) - 0.1f; // Maximum value for psy. Camera inverts at Pi/2+.
	public const float PSYMIN = 0;					   // Minimum value for psy.
	public const float RADMIN = 3;					   // Minimum distance from marble
	
	//public float smooth = 1; // Used to adjust how smoothly the camera adjusts position between frames (Lerp)
	
	public ControlMode mode;		 	// Which control mode camera is using.
	public float mouseSensitivity; 		// Mouse sensitivity.
	public float keyboardSensitivity; 	// Keyboard sensitivity.
	public bool autoRotate = true;		// Whether camera should automatically position itself as player moves.
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
		ResetPosition();
		radius = defRadius;
		playerRadius = radius;
		playerPsy = psy;
		marble = gm.marble.transform;
		//radius = Vector3.Distance(transform.position, ball.position);
		mode = ControlMode.Keyboard;
		
		mouseSensitivity = 0.05f;
		keyboardSensitivity = 3f;
	}
	
	// Update - Called once per frame
	void Update () {
		// Keyboard mode controls
		if (mode == ControlMode.Keyboard) { 
			// Moved to control functions, handled by input manager.
			
		// Mouse mode controls
		} else if (mode == ControlMode.Mouse) {
			// Moving the mouse moves the camera.
			psy = Mathf.Clamp(psy + (Input.GetAxis("Mouse Y") * mouseSensitivity), PSYMIN, PSYMAX);
			theta -= Input.GetAxis("Mouse X") * mouseSensitivity;
			
			
		// Unspecified case controls
		} else {
			Debug.LogWarning("(CameraController.cs) Control mode not specified!");
		}
		
		// Allows zooming in and out.
		if(Input.GetAxis("Mouse ScrollWheel") != 0) {
			radius -= Input.GetAxis("Mouse ScrollWheel");
			playerRadius = radius;	// Changes player preferred radius
		} 

		radius = playerRadius;	// Attempts to set radius at the player preferred distance.

		// If radius is too small, pushes it out, and up if autorotate is enabled.
		//if (radius < RADMIN) {
		//	MoveUp();
		//	Debug.Log("Movin' on up!");
		//	radius = RADMIN;	} 

		// Checks if marble can "see" the camera currently.
		Debug.DrawRay(marble.position, transform.position - marble.position, Color.blue); // DEBUG
		RaycastHit hit;
		// If marble cannot "see" the camera, moves the camera to a point on the radius that it CAN be seen.
		if (Physics.Raycast(marble.position, (transform.position - marble.position).normalized, out hit, radius)) {
			transform.position = hit.point - (transform.position - marble.position).normalized;
			radius = Vector3.Distance(transform.position, marble.position); // Currently ignores minimum radius.

			// Moves camera up if stuck against wall, otherwise settles it back down.
			// NOTE: Currently causes jitter!
			if (autoRotate) {
				if (radius < RADMIN)
					MoveUp();
				else if (psy > playerPsy && radius > RADMIN + 3)
					MoveDown();
			}
		}
	}
	
	// LateUpdate - Called directly after everything updates
	void LateUpdate () {

		// Experimental autorotation.
		/*Vector3 direction = transform.position - marble.position;

		if (autoRotate && marble.GetComponent<Rigidbody>().velocity.magnitude > 31) {
			if (direction.x != marble.GetComponent<Rigidbody>().velocity.x || direction.z != marble.GetComponent<Rigidbody>().velocity.z)
				MoveRight();
		}*/

		// Consider attempting to Lerp again if there's any jitter.
		transform.position = GetSphericalPosition();
		transform.LookAt(marble.position);
	}
	
	
	// GetSphericalPosition - Return spherical coordinate of camera
	Vector3 GetSphericalPosition() {
		Vector3 retPos = new Vector3();
		
		// These are all using radians.
		retPos.x = radius * Mathf.Cos (psy) * Mathf.Cos (theta) + marble.position.x;
		retPos.y = radius * Mathf.Sin (psy) + marble.position.y;
		retPos.z = radius * Mathf.Cos (psy) * Mathf.Sin (theta) + marble.position.z;
		
		return retPos;
	}

	// ResetPosition - Returns the camera to its initial position.
	public void ResetPosition() {
		theta = defTheta;
		psy = defPsy;
	}
	
	// Control Functions
	#region Control Functions
	// Moves camera up.
	public void MoveUp() {
		psy = Mathf.Clamp(psy + (keyboardSensitivity * Time.deltaTime), PSYMIN, PSYMAX);
	}
	
	// Moves camera down.
	public void MoveDown() {
		psy = Mathf.Clamp(psy - (keyboardSensitivity * Time.deltaTime), PSYMIN, PSYMAX);
		
	}
	
	// Moves camera left.
	public void MoveLeft() {
		theta -= keyboardSensitivity * Time.deltaTime;
		
	}
	
	// Moves camera right.
	public void MoveRight() {
		theta += keyboardSensitivity * Time.deltaTime;

		
	}
	
	// ToggleControlMode - Changes camera control style.
	public void ToggleControlMode() {
		if (mode == ControlMode.Keyboard) {
			mode = ControlMode.Mouse;
			//Screen.lockCursor = true;	// When true, cursor is hidden and constantly centered.
		} else if (mode == ControlMode.Mouse) {
			mode = ControlMode.Keyboard;
			//Screen.lockCursor = false;	// Undoes lock. Lock always undone by Escape due to Unity implementation.
		}
	}
	
	#endregion
}