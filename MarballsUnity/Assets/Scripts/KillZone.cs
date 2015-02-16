using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {
	
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
	
	void OnTriggerEnter() {
		gm.marble.GetComponent<Marble>().Respawn();
	}
}
