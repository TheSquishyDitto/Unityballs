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
	bool commandVisible = false;
	int id;
	Event e;
	KeyCode k;

	void Awake() {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
		gm.controlMenu = this;
	}
	
	void Update() {
		popUp.SetActive(commandVisible);
		
		fButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[0].ToString());
		bButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[1].ToString());
		lButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[2].ToString());
		rButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[3].ToString());
		jButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[4].ToString());
		camUp.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[5].ToString());
		camDown.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[6].ToString());
		camLeft.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[7].ToString());
		camRight.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[8].ToString());
		camToggle.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[9].ToString());
		useButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[10].ToString());
		breakButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[11].ToString());
		//respawnButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[12].ToString());
		helperButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[13].ToString());
		pauseButton.GetComponentInChildren<Text>().text = CleanUp(gm.input.keyBindings[14].ToString());
	}
	
	
	string CleanUp(string name) {
		string newName;
		
		if(name.Contains("Left")){
			name = name.Replace("Left","");
			newName = "Left"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Right")){
			name = name.Replace("Right","");
			newName = "Right"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Page")){
			name = name.Replace("Page","");
			newName = "Page"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Up")){
			name = name.Replace("Up","");
			newName = "Up"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Down")){
			name = name.Replace("Down","");
			newName = "Down"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Keypad")){
			name = name.Replace("Keypad","");
			newName = "Keypad"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Scroll")){
			name = name.Replace("Scroll","");
			newName = "Scroll"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Back")){
			name = name.Replace("Back","");
			newName = "Back"+System.Environment.NewLine+name;
		}
		else if(name.Contains("Mouse0")){
			newName = "Primary"+System.Environment.NewLine+"Mouse";
		}
		else if(name.Contains("Mouse1")){
			newName = "Second"+System.Environment.NewLine+"Mouse";
		}
		else if(name.Contains("Mouse2")){
			newName = "Third"+System.Environment.NewLine+"Mouse";
		}
		if(name.Contains("None")){
			newName = "";
		}
		else 
			newName = name;
		
		return newName;
	}
	


	// OnGUI - The only input class allowed in here are events; Input class does not work within here.
	public void OnGUI(){
		e = Event.current;
		
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
		commandVisible = !commandVisible;
		Debug.Log (commandVisible);
	}
	
	public void Default(){
		gm.input.ResetDefault();
	}
	
	void ChangeKey(){
		commandVisible = !commandVisible;
		gm.input.keyBindings[id] = k;
		
		for(int i = 0; i < gm.input.keyBindings.Count; i++){
			if(i != id && gm.input.keyBindings[i] == k) {
				gm.input.keyBindings[i] = KeyCode.None;
			}	
		}
		
	}
}
