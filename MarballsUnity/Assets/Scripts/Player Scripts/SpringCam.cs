/// <summary>
/// SpringCam.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 24, 2015
/// Last Revision: Apr. 25, 2015
/// 
/// Class that provides a third approach to camera behavior. Relies mainly on rigidbody and springs.
/// 
/// NOTES: - This class is currently crudely put together as proof of concept.
/// 	   - As decreed by Chris, this camera is probably not suited for this game as it stands.
/// 
/// TO DO: - Refine and tweak until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SpringCam : MonoBehaviour/*, ICamera*/ {	// If using in Marballs, re-add the ICamera.

	// Variables
	#region Variables
	GameMaster gm;						// Reference to GameMaster.
	Transform myTransform;				// Cached reference to transform.
	//Rigidbody myBody;					// Cached reference to rigidbody.
	Transform marble;					// Reference to marble.
	CameraController.ControlMode mode;	// Which input device to use for control.
	float vertOffset;					// Preferred vertical distance between cam and marble.

	public float sensitivity = 3;		// How quickly the player can move the camera.
	public Vector3 startPos;			// Camera's starting position.

	#endregion

	void Awake() {
		gm = GameMaster.CreateGM();
		myTransform = transform;
		gm.cam = myTransform;
		//myBody = GetComponent<Rigidbody>();
		startPos = myTransform.position;
	}

	// OnEnable - Called when script is enabled.
	void OnEnable() {
		Marble.respawn += ResetPosition;
	}
	
	// OnDisable - Called when script is disabled.
	void OnDisable() {
		Marble.respawn -= ResetPosition;
	}

	// Use this for initialization
	void Start () {
		marble = gm.marble.marform;
		vertOffset = myTransform.position.y - marble.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.position = new Vector3(myTransform.position.x, marble.position.y + vertOffset, myTransform.position.z);
	}

	// v OBLIGATORY CAM FUNCTIONS v //

	public void ResetPosition() {
		myTransform.position = startPos;
	}

	public void MoveUp() {
		myTransform.RotateAround(marble.position, myTransform.right, sensitivity);
		vertOffset++;
	}

	public void MoveDown() {
		myTransform.RotateAround(marble.position, myTransform.right, -sensitivity);
		vertOffset--;
	}

	public void MoveLeft() {
		myTransform.RotateAround(marble.position, Vector3.up, sensitivity);
	}

	public void MoveRight() {
		myTransform.RotateAround(marble.position, Vector3.up, -sensitivity);
	}

	public void ToggleControlMode() {
		mode = CameraController.ControlMode.Keyboard;
	}

	public CameraController.ControlMode Mode {
		get { return mode; }
	}
}
