using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	public GameMaster gm;

	// Use this for initialization
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.pauseMenu = this;
		gameObject.SetActive(false);
	}
	
	public void Resume(){
		gm.TogglePause();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
