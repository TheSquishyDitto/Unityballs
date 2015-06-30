/// <summary>
/// CameraController2.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 24, 2015
/// Last Revision: Jun. 26, 2015
/// 
/// Class that controls camera movement in a different way.
/// 
/// NOTES: - This class currently does not support vertical mouse movement.
/// 	   - This class offers rotating the camera up when in close quarters.
/// 
/// TO DO: - Tweak movement until desired.
/// 	   - Fix vertical mouse movement to prevent gimbal lock.
/// 	   - Fix cursor lock behavior with menus and such.
/// 	   - Fix miscellaneous bizarre behavior.
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
	float ampSense = 50;					// Amplifies sensitivity for framerate independence.
	CameraController.ControlMode mode;		// Which control scheme the camera is using.
	bool frozen = false;					// Whether camera should stop moving or not.

	Vector3 velocity = Vector3.zero;		// Variable used exclusively for smooth damping.
	public float smoothDamp = 0.1f;			// How quickly positions should be smoothed.

	public float radius = 15;				// Preferred distance away from marble.
	public float sensitivity = 3f;			// How quickly the camera can move.
	public bool autoRotate = true;			// Whether camera should rotate on its own in certain conditions.
	public Vector3 defOffset = new Vector3(0, 5.5f, 14);	// Default/starting offset.

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
		myTransform = transform;
		//gm.cam = transform;
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

		Messenger.AddListener("CamUp", MoveUp);
		Messenger.AddListener("CamDown", MoveDown);
		Messenger.AddListener("CamLeft", MoveLeft);
		Messenger.AddListener("CamRight", MoveRight);
		Messenger.AddListener("CamModeToggle", ToggleControlMode);
		Messenger<float>.AddListener("CameraShake", DoCameraShake);
	}
	
	// OnDisable - Called when script is disabled.
	void OnDisable() {
		Marble.respawn -= ResetPosition;

		Messenger.RemoveListener("CamUp", MoveUp);
		Messenger.RemoveListener("CamDown", MoveDown);
		Messenger.RemoveListener("CamLeft", MoveLeft);
		Messenger.RemoveListener("CamRight", MoveRight);
		Messenger.RemoveListener("CamModeToggle", ToggleControlMode);
		Messenger<float>.RemoveListener("CameraShake", DoCameraShake);
	}
	
	// Update - Called once per frame.
	void Update () {
		// DEBUG - Forgot this was here. Probably safe to remove.
		//if (Input.GetKeyDown(KeyCode.Z)) {
		//	Freeze(!frozen);
		//}

		if (!frozen) {
			// Allow zooming in and out.
			if(Input.GetAxis("Mouse ScrollWheel") != 0)
				radius = Mathf.Clamp(radius - Input.GetAxis("Mouse ScrollWheel"), 3, 15);

			// Moving the mouse moves the camera in Mouse control mode.
			// NOTE: Commented out vertical implementation due to issues with avoiding Gimbal Lock.
			if (mode == CameraController.ControlMode.Mouse) {
				offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, Vector3.up) * offset;
			/*	if (offSetOffset == Vector3.zero)
					offset = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * sensitivity / 2, myTransform.right) * offset;

				float angle = Vector3.Angle(Vector3.forward, offset);
				if (angle < 10 || angle > 80)
					offset = Quaternion.AngleAxis(-angle, myTransform.right) * offset;*/
				//Debug.LogWarning("CameraExperiment.cs) Mouse Mode not currently supported!");
			}

			// Recalculate ray and offset.
			ray = new Ray(marble.position, offset.normalized);
			offset = ray.GetPoint(radius) - marble.position;

			Debug.DrawLine(marble.position, marble.position + offset, Color.blue);

			// Casts ray to detect obstructions.
			if (Physics.Raycast(ray, out hit, radius + 1)) {
				// If obstructed, cut the ray short and move the camera up based on how close obstruction is.
				offSetOffset = ray.GetPoint(radius + 1) - hit.point;

				// Prevent offSetOffset from being large enough to make the camera go through the marble.
				if (offSetOffset.sqrMagnitude > offset.sqrMagnitude)
					offSetOffset -= offSetOffset.normalized;

				// Rotate camera upwards if automatic rotation is enabled.
				if (autoRotate) offSetOffset -= Vector3.up * (Vector3.Distance(ray.GetPoint(radius + 1), hit.point) / 2f);
			} else {
				// Otherwise, everything proceeds normally.
				offSetOffset = Vector3.zero;
			}
		}
	}

	// LateUpdate - Called after update.
	void LateUpdate() {
		if (!frozen) {
			// Apply changes to camera.
			//myTransform.position = marble.position + offset - offSetOffset; // Uncomment this out if never using SmoothDamp.
			myTransform.position = Vector3.SmoothDamp(myTransform.position, marble.position + offset - offSetOffset, ref velocity, smoothDamp);
			myTransform.LookAt(marble.position);
		}
	}

	// ResetPosition - Returns camera to original position.
	public void ResetPosition() {
		offset = defOffset;
		myTransform.position = gm.marble.marform.position + defOffset;
		myTransform.LookAt(gm.marble.marform.position);
	}

	// Freeze - Stops camera from moving or updating variables.
	public void Freeze(bool freeze) {
		frozen = freeze;
	}

	// RecalculatePosition - Manually updates important position variables.
	public void RecalculatePosition() {
		offset = myTransform.position - marble.position;
		defOffset = offset;	// Not sure if this belongs here but it's needed at the moment.
	}

	// DoCameraShake - Initiates camera shaking coroutine.
	public void DoCameraShake(float intensity) {
		StopCoroutine("CameraShake");
		StartCoroutine("CameraShake", intensity);
	}

	// CameraShake - Jostles the camera a little bit.
	public IEnumerator CameraShake(float intensity) {
		int duration = 25;

		for (int i = 0; i < duration; i++) {
			myTransform.position += new Vector3(Random.Range(-intensity, intensity), 
			                                    Random.Range(-intensity, intensity), 
			                                    Random.Range(-intensity, intensity));

			yield return new WaitForFixedUpdate();
		}
	}

	// Control Functions - Rotates the offset around and/or changes camera mode.
	#region Control Functions

	// Moves camera up.
	public void MoveUp() {
		if (mode == CameraController.ControlMode.Keyboard && myTransform.eulerAngles.x < 90 - sensitivity/* && !(autoRotate && offSetOffset != Vector3.zero)*/) {
			offset = Quaternion.AngleAxis(sensitivity * Time.deltaTime * ampSense, myTransform.right) * offset;
			//Debug.Log("Before: " + offset);
			if (offset.y > radius - 3) {
				//offset.y = radius - 1;//radius - 1;
				offset = new Vector3(offset.x, radius - 3, offset.z);
			}
			//Debug.Log("After: " + offset);
		}
	}
	
	// Moves camera down.
	public void MoveDown() {
		if (mode == CameraController.ControlMode.Keyboard && myTransform.eulerAngles.x > sensitivity/* && !(autoRotate && offSetOffset != Vector3.zero)*/) {
			offset = Quaternion.AngleAxis(-sensitivity * Time.deltaTime * ampSense, myTransform.right) * offset;
			if (offset.y < 0) offset.y = 0;
		}
	}
	
	// Moves camera left.
	public void MoveLeft() {
		if (mode == CameraController.ControlMode.Keyboard)
			offset = Quaternion.AngleAxis(sensitivity * Time.deltaTime * ampSense, Vector3.up) * offset;
	}
	
	// Moves camera right.
	public void MoveRight() {
		if (mode == CameraController.ControlMode.Keyboard)
			offset = Quaternion.AngleAxis(-sensitivity * Time.deltaTime * ampSense, Vector3.up) * offset;
	}
	
	// ToggleControlMode - Changes camera control style.
	public void ToggleControlMode() {
		if (mode == CameraController.ControlMode.Keyboard) {
			mode = CameraController.ControlMode.Mouse;
			Cursor.lockState = CursorLockMode.Locked;	// Hides and locks cursor in the center.
			Cursor.visible = false;
		} else if (mode == CameraController.ControlMode.Mouse) {
			mode = CameraController.ControlMode.Keyboard;
			Cursor.lockState = CursorLockMode.None;	// Undoes lock. Lock always undone by Escape due to Unity implementation.
			Cursor.visible = true;
		}

		//mode = CameraController.ControlMode.Keyboard;
	}
	
	#endregion

	// Getters and Setters
	#region Getters and Setters

	public CameraController.ControlMode Mode {
		get { return mode; }
	}
	#endregion
}
