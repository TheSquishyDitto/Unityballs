using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlScript : MonoBehaviour {
	public GameMaster gm;				// Reference to the Game Master.
	
	public Button fButton;
	public Button bButton;
	public Button lButton;
	public Button rButton;
	public Button jButton;
	
	public Button camUp;
	public Button camDown;
	public Button camLeft;
	public Button camRight;
	public Button camToggle;
	
	public Button useButton;
	public Button breakButton;
	public Button respawnButton;
	public Button helperButton;
	public Button pauseButton;
	
	public GameObject popUp;
	bool commandVisible;
	int id;
	Event e;
	KeyCode k;

	void Awake() {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
		gm.controlMenu = this;
		
		commandVisible = false;
		
	}
	
	void Update() {
		popUp.SetActive(commandVisible);
		
		fButton.GetComponentInChildren<Text>().text = gm.input.forward.ToString();
		bButton.GetComponentInChildren<Text>().text = gm.input.backward.ToString();
		lButton.GetComponentInChildren<Text>().text = gm.input.left.ToString();
		rButton.GetComponentInChildren<Text>().text = gm.input.right.ToString();
		jButton.GetComponentInChildren<Text>().text = gm.input.jump.ToString();
		camUp.GetComponentInChildren<Text>().text = gm.input.camUp.ToString();
		camDown.GetComponentInChildren<Text>().text = gm.input.camDown.ToString();
		camLeft.GetComponentInChildren<Text>().text = gm.input.camLeft.ToString();
		camRight.GetComponentInChildren<Text>().text = gm.input.camRight.ToString();
		camToggle.GetComponentInChildren<Text>().text = gm.input.camToggle.ToString();
		useButton.GetComponentInChildren<Text>().text = gm.input.use.ToString();
		breakButton.GetComponentInChildren<Text>().text = gm.input.brake.ToString();
		respawnButton.GetComponentInChildren<Text>().text = gm.input.respawn.ToString();
		helperButton.GetComponentInChildren<Text>().text = gm.input.levelHelp.ToString();
		pauseButton.GetComponentInChildren<Text>().text = gm.input.pause.ToString();
	}
	
	/*
	string CleanUp(string name) {
	}
	*/


	// OnGUI - The only input class allowed in here are events; Input class does not work within here.
	public void OnGUI(){
		e = Event.current;
		commandVisible = !commandVisible;
		
		if(commandVisible){
			if (e != null && (e.isKey || e.shift)/* && Input.anyKeyDown*/){
				if (Input.GetKey(KeyCode.LeftShift))
					k = KeyCode.LeftShift;
				else if (Input.GetKey(KeyCode.RightShift))
					k = KeyCode.RightShift;
				else
					k = e.keyCode;	// Shift keys do not have a keycode yet apparently.				
				ChangeKey();
			}
			else if (e != null && e.isMouse/* && Input.GetMouseButtonDown(0)*/){
				k = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse" + e.button, true);//KeyCode.Mouse0;
				ChangeKey();
			}		
		}		
	}
	
	public void ButtonID(int num){
		id = num;	
	}
	
	public void Default(){
		gm.input.ResetDefault();
	}
	
	void ChangeKey(){
		commandVisible = !commandVisible;
		switch (id){
			case 0:
				gm.input.forward = k;
				break;
			case 1:
				gm.input.backward = k;
				break;
			case 2:
				gm.input.left = k;
				break;
			case 3:
				gm.input.right = k;
				break;
			case 4:
				gm.input.jump = k;
				break;
			case 5:
				gm.input.camUp = k;
				break;
			case 6:
				gm.input.camDown = k;
				break;
			case 7:
				gm.input.camLeft = k;
				break;
			case 8:
				gm.input.camRight = k;
				break;
			case 9:
				gm.input.camToggle = k;
				break;
			case 10:
				gm.input.use = k;
				break;
			case 11:
				gm.input.brake = k;
				break;
			case 12:
				gm.input.respawn = k;
				break;
			case 13:
				gm.input.levelHelp = k;
				break;
			case 14:
				gm.input.pause = k;
				break;
		}	
	}
}
