/// <summary>
/// ControlScript.cs
/// Authors: Charlie Sun, Kyle Dawson
/// Date Created:  Apr.  8, 2015
/// Last Revision: Jun. 25, 2015
/// 
/// Class that allows dynamic key rebinding.
/// 
/// NOTES: - Does not allow joysticks, extra mouse buttons, or controllers.
/// 
/// TO DO: - Tweak and optimize.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlScript : MonoBehaviour {
	// Variables
	#region Variables
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

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
		gm.controlMenu = this;
	}

	// Start - Use this for initialization.
	void Start() {
		RefreshText();
	}

	// Update - Called once per frame.
	void Update() {
		popUp.SetActive(commandVisible);

	}
	
	// CleanUp - Makes key names more presentable.
	string CleanUp(string name) {
		string newName = name;
		
		if(name.Contains("Left")){
			newName = "Left"+System.Environment.NewLine+name.Replace("Left","");
		}
		else if(name.Contains("Right")){
			newName = "Right"+System.Environment.NewLine+name.Replace("Right","");
		}
		else if(name.Contains("Page")){
			newName = "Page"+System.Environment.NewLine+name.Replace("Page","");
		}
		else if(name.Contains("Up")){
			newName = "Up"+System.Environment.NewLine+name.Replace("Up","");
		}
		else if(name.Contains("Down")){
			newName = "Down"+System.Environment.NewLine+name.Replace("Down","");
		}
		else if(name.Contains("Keypad")){
			newName = "Keypad"+System.Environment.NewLine+name.Replace("Keypad","");
		}
		else if(name.Contains("Scroll")){
			newName = "Scroll"+System.Environment.NewLine+name.Replace("Scroll","");
		}
		else if(name.Contains("Back")){
			newName = "Back"+System.Environment.NewLine+name.Replace("Back","");
		}
		else if(name.Contains("Caps")){
			newName = "Caps"+System.Environment.NewLine+name.Replace("Caps","");
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
		else if(name.Contains("None")){
			newName = "";
		}

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

	// ButtonID - Gets ID of button.
	public void ButtonID(int num){
		id = num;
		commandVisible = !commandVisible;
		//Debug.Log (commandVisible);
	}

	// Default - Reverts to default key configuration.
	public void Default(){
		gm.input.ResetDefault();
		RefreshText();
	}

	// RefreshText - Updates display.
	void RefreshText() {
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

	// ChangeKey - Changes what key is assigned where.
	void ChangeKey(){
		commandVisible = !commandVisible;

		KeyCode temp = gm.input.keyBindings[id];
		gm.input.keyBindings[id] = k;
		
		for(int i = 0; i < gm.input.keyBindings.Count; i++){
			if(i != id && gm.input.keyBindings[i] == k) {
				gm.input.keyBindings[i] = temp;
			}	
		}

		RefreshText();
		
	}
}
