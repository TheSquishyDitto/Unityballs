using UnityEngine;
using System.Collections;

public class SpawnArea : MonoBehaviour {

	public GameMaster gm;
	
	void Awake () {
		gm = GameMaster.CreateGM ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// OnTriggerStay - As long as another object is within the collision zone.
	void OnTriggerStay(Collider other) {
		// If the other object has a rigidbody, boost it along.
		// Currently always boosts based on transform of physical panel.
		if (other.attachedRigidbody)
			other.attachedRigidbody.useGravity = (gm.state == GameMaster.GameState.Start) ? false : true;
		
	}
}
