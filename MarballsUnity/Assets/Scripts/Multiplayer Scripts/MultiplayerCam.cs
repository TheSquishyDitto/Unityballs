/// <summary>
/// MultiplayerCam.cs
/// Authors: Kyle Dawson
/// Date Created:  May   6, 2015
/// Last Revision: May   6, 2015
/// 
/// Class for networked instances of the player camera.
/// 
/// NOTES: - Currently only supports basic lateral movement and zooming via keyboard.
/// 
/// TO DO: - Re-add offline features/integrate networked features into actual camera.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class MultiplayerCam : MonoBehaviour {
	
	Transform myTransform;			// Cached reference to transform.
	Transform parent;				// Reference to parent object that is being viewed.

	public float sensitivity = 3;	// Speed of rotation.

	// Awake - Called before anything else.
	void Awake() {
		myTransform = transform;
		parent = myTransform.parent;
	}

	// Start - Use this for initialization.
	void Start() {
		myTransform.position = parent.position.normalized * (parent.position.magnitude + 10);
	}
	
	// Update - Called once per frame.
	void Update () {
		MoveControls();
	}

	// LateUpdate - Called after update.
	void LateUpdate() {
		myTransform.LookAt(parent.position);
	}

	// MoveControls - Hackish input for multiplayer.
	void MoveControls() {
		if (Input.GetKey(KeyCode.LeftArrow)) {
			myTransform.RotateAround(parent.position, Vector3.up, sensitivity * Time.deltaTime * 50);
		}

		if (Input.GetKey(KeyCode.RightArrow)) {
			myTransform.RotateAround(parent.position, Vector3.up, -sensitivity * Time.deltaTime * 50);
		}

		if(Input.GetAxis("Mouse ScrollWheel") != 0)
			myTransform.position += (-myTransform.position + parent.position).normalized * Input.GetAxis("Mouse ScrollWheel");
	}
}
