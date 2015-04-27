/// <summary>
/// CameraController2.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 24, 2015
/// Last Revision: Apr. 25, 2015
/// 
/// Class that controls camera movement in a different way.
/// 
/// NOTES: - This class currently only supports keyboard movement.
/// 	   - This class offers rotating the camera up when in close quarters.
/// 
/// TO DO: - Tweak movement until desired.
/// 	   - Fix mouse movement to prevent gimbal lock.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class CameraController2 : MonoBehaviour, ICamera {

	// Variables
	#region Variables
	GameMaster gm;							// Reference to GameMaster.
	Transform marble;						// Reference to marble's transform.
	Transform myTransform;					// Cached reference to own transform.

	Ray ray;								// Ray used for raycasting.
	RaycastHit hit;							// Hit information from raycast.
	Vector3 offset;							// Camera position/direction offset from marble.
	Vector3 offSetOffset = Vector3.zero;	// The offset from the offset when handling obstructions.
	//Vector3 velocity = Vector3.zero;		// Variable used exclusively for smooth damping.
	float ampSense = 50;					// Amplifies sensitivity for framerate independence.
	CameraController.ControlMode mode;		// Which control scheme the camera is using.
	
	public float radius = 15;				// Preferred distance away from marble.
	public float sensitivity = 3f;			// How quickly the camera can move.
	//public float smoothDamp = 0.01f;		// How quickly positions should be smoothed.
	public bool autoRotate = true;			// Whether camera should rotate on its own in certain conditions.

	public Vector3 defOffset = new Vector3(0, 5.5f, 14);	// Default/starting offset.

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
		myTransform = transform;
		gm.cam = this.transform;
	}

	// Start - Use this for initialization.
	void Start () {
		marble = gm.marble.marform;
		ResetPosition();
		offset = myTransform.position - marble.position;
	}

	// OnEnable - Called when script is enabled.
	void OnEnable() {
		Marble.respawn += ResetPosition;
	}
	
	// OnDisable - Called when script is disabled.
	void OnDisable() {
		Marble.respawn -= ResetPosition;
	}
	
	// Update - Called once per frame.
	void Update () {
		// Allow zooming in and out.
		if(Input.GetAxis("Mouse ScrollWheel") != 0)
			radius = Mathf.Clamp(radius - Input.GetAxis("Mouse ScrollWheel"), 3, 15);

		// Moving the mouse moves the camera in Mouse control mode.
		// NOTE: Commented out implementation due to issues with avoiding Gimbal Lock.
		if (mode == CameraController.ControlMode.Mouse) {
		/*	offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, Vector3.up) * offset;
			if (offSetOffset == Vector3.zero)
				offset = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * sensitivity / 2, myTransform.right) * offset;

			float angle = Vector3.Angle(Vector3.forward, offset);
			if (angle < 10 || angle > 80)
				offset = Quaternion.AngleAxis(-angle, myTransform.right) * offset;*/
			Debug.LogWarning("CameraExperiment.cs) Mouse Mode not currently supported!");
		}

		// Recalculate ray and offset.
		ray = new Ray(marble.position, offset.normalized);
		offset = ray.GetPoint(radius) - marble.position;

		// Casts ray to detect obstructions.
		if (Physics.Raycast(ray, out hit, radius)) {
			// If obstructed, cut the ray short and move the camera up based on how close obstruction is.
			offSetOffset = ray.GetPoint(radius) - hit.point;
			if (autoRotate) offSetOffset -= Vector3.up * Vector3.Distance(ray.GetPoint(radius), hit.point);
		} else {
			// Otherwise, everything proceeds normally.
			offSetOffset = Vector3.zero;
		}
	}

	// LateUpdate - Called after update.
	void LateUpdate() {
		// Apply changes to camera.
		myTransform.position = marble.position + offset - offSetOffset; // Comment this out if using SmoothDamp.
		//myTransform.position = Vector3.SmoothDamp(myTransform.position, marble.position + offset - offSetOffset, ref velocity, smoothDamp);
		transform.LookAt(marble.position);
	}

	// ResetPosition - Returns camera to original position.
	public void ResetPosition() {
		offset = defOffset;
		myTransform.position = gm.marble.marform.position + defOffset;
	}

	// Control Functions - Rotates the offset around and/or changes camera mode.
	#region Control Functions
	// Moves camera up.
	public void MoveUp() {
		if (myTransform.eulerAngles.x < 90 - sensitivity && offSetOffset == Vector3.zero)
			offset = Quaternion.AngleAxis(sensitivity * Time.deltaTime * ampSense, myTransform.right) * offset;
	}
	
	// Moves camera down.
	public void MoveDown() {
		if (myTransform.eulerAngles.x > 0 + sensitivity && offSetOffset == Vector3.zero)
			offset = Quaternion.AngleAxis(-sensitivity * Time.deltaTime * ampSense, myTransform.right) * offset;
	}
	
	// Moves camera left.
	public void MoveLeft() {
		offset = Quaternion.AngleAxis(sensitivity * Time.deltaTime * ampSense, Vector3.up) * offset;
	}
	
	// Moves camera right.
	public void MoveRight() {
		offset = Quaternion.AngleAxis(-sensitivity * Time.deltaTime * ampSense, Vector3.up) * offset;
	}
	
	// ToggleControlMode - Changes camera control style.
	public void ToggleControlMode() {
		/*if (mode == CameraController.ControlMode.Keyboard) {
			mode = CameraController.ControlMode.Mouse;
			Cursor.lockState = CursorLockMode.Locked;	// When true, cursor is hidden and constantly centered.
		} else if (mode == CameraController.ControlMode.Mouse) {
			mode = CameraController.ControlMode.Keyboard;
			Cursor.lockState = CursorLockMode.None;	// Undoes lock. Lock always undone by Escape due to Unity implementation.
		}*/

		mode = CameraController.ControlMode.Keyboard;
	}
	
	#endregion

	// Getters and Setters
	#region Getters and Setters

	public CameraController.ControlMode Mode {
		get { return mode; }
	}
	#endregion
}
