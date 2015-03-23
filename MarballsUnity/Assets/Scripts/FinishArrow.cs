using UnityEngine;
using System.Collections;

public class FinishArrow : MonoBehaviour {
	public GameMaster gm;

	void Awake () {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = gm.cam.rotation;
		transform.position = gm.cam.position;
	}
}
