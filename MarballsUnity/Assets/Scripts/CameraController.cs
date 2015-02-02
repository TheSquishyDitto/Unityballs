/// <summary>
/// CameraController.cs
/// Authors: Kyle Dawson, Chris Viqueira, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan. 28, 2015
/// Last Revision: Feb.  2, 2015
/// 
/// Class that controls camera movement.
/// 
/// NOTES: - Current camera controls are C, and either arrow keys or mouse depending on mode.
/// 
/// TO DO: - Tweak movement until desired.
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
	public Transform ball; // Reference to coordinates of marble

	public float theta;  // Radians around y-axis (horizontal).
	public float psy;	 // Radians around x-axis (vertical).
	public float radius; // Distance from marble.

	public const float PSYMAX = (Mathf.PI / 2) - 0.1f; // Maximum value for psy. Camera inverts at Pi/2+.
	public const float PSYMIN = 0;					   // Minimum value for psy.

	public float smooth = 1; // Used to adjust how smoothly the camera adjusts position between frames (Lerp)

	public ControlMode mode;		 // Which control mode camera is using.
	public float sensitivity = 0.2f; // Mouse sensitivity.

	#endregion

	// Start - Use this for initialization
	void Start () {
		ball = GameObject.FindGameObjectWithTag("Marble").transform;
		radius = Vector3.Distance(transform.position, ball.position);
		mode = ControlMode.Keyboard;
	}
	
	// Update - Called once per frame
	void Update () {
		// Keyboard mode controls
		if (mode == ControlMode.Keyboard) { 
			if (Input.GetKey (KeyCode.UpArrow)) {
				psy = Mathf.Clamp(psy + .01f, PSYMIN, PSYMAX);
			}

			if (Input.GetKey (KeyCode.DownArrow)) {
				psy = Mathf.Clamp(psy - .01f, PSYMIN, PSYMAX);
			}

			if (Input.GetKey (KeyCode.LeftArrow)) {
				theta -= .01f;
			}

			if (Input.GetKey (KeyCode.RightArrow)) {
				theta += .01f;	
			}

		// Mouse mode controls
		} else if (mode == ControlMode.Mouse) {
			if (Input.GetMouseButton(0)) { // 0 is left mouse button.
				Screen.lockCursor = true; // While true, cursor is hidden and constantly put in middle of screen.
				psy = Mathf.Clamp(psy + (Input.GetAxis("Mouse Y") * sensitivity), PSYMIN, PSYMAX);
				theta += Input.GetAxis("Mouse X") * sensitivity;
			}

			if (Input.GetMouseButtonUp(0)) { // 0 is left mouse button.
				Screen.lockCursor = false;
			}

			radius -= Input.GetAxis("Mouse ScrollWheel");
		
		// Unspecified case controls
		} else {
				Debug.LogWarning("(CameraController.cs) Control mode not specified!");
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			ToggleControlMode();
		}

	}

	// LateUpdate - Called directly after everything updates
	void LateUpdate () {
		// Attempted to use Lerp to fix mild jitter, but couldn't get it to work.
		//gameObject.transform.position = Vector3.Lerp(transform.position, GetSphericalPosition(), Time.deltaTime * smooth);
		gameObject.transform.position = GetSphericalPosition();
		gameObject.transform.LookAt(ball.position);
	}


	// GetSphericalPosition - Return spherical coordinate of camera
	Vector3 GetSphericalPosition() {
		Vector3 retPos = new Vector3();

		// Turns out radians were needed after all.
		retPos.x = radius * Mathf.Cos (psy) * Mathf.Cos (theta) + ball.position.x;
		retPos.y = radius * Mathf.Sin (psy) + ball.position.y;
		retPos.z = radius * Mathf.Cos (psy) * Mathf.Sin (theta) + ball.position.z;

		return retPos;
	}

	// ToggleControlMode - Changes camera control style.
	void ToggleControlMode() {
		if (mode == ControlMode.Keyboard)
			mode = ControlMode.Mouse;
		else if (mode == ControlMode.Mouse)
			mode = ControlMode.Keyboard;
	}
}
