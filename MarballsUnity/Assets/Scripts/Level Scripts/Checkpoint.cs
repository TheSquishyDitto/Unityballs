/// <summary>
/// Checkpoint.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 27, 2015
/// Last Revision: Apr. 30, 2015
/// 
/// Class that handles passing checkpoints.
/// 
/// TODO: - Add fancy animation and/or indicator of currently active checkpoint?
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	//GameMaster gm;				// Reference to GameMaster.
	public GameObject sfx;			// Reference to sparkly effects.
	public bool pristine = true;	// Whether or not the checkpoint has been touched.

	// Awake - Called before anything else.
	void Awake() {
		//gm = GameMaster.CreateGM();
	}
	
	// OnTriggerEnter - Called when player touches the checkpoint trigger.
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Marble") && pristine) {
			//gm.checkpoint = transform;
			other.GetComponent<Marble>().spawnPoint = transform;
			pristine = false;
			if (sfx) sfx.SetActive(false);
			AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/collectSound"), Vector3.zero, 0.5f);
		}
	}
}
