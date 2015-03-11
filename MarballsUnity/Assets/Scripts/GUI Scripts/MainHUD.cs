using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour {
	public GameMaster gm;	// Reference to Game Master.
	public Transform marble;	// Reference to currently active marble.
	public Text timer;
	public Text speed;
	
	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
	}
	
	// Use this for initialization
	void Start () {
		marble = gm.marble;
	}
	
	// Update is called once per frame
	void Update () {
		if (!gm.paused)
			timer.text = (Mathf.Round(gm.timer * 10) / 10.0) + " s";
		
		if (marble != null) {
			// Speed gauge.
			speed.text = Mathf.Round(marble.GetComponent<Rigidbody>().velocity.magnitude) + " m/s";
		}	
	}
	
	// Debug buttons
	public void StartButton (){
		gm.OnStart();
	}
	
	public void PlayButton (){
		gm.OnPlay();
	}
}
