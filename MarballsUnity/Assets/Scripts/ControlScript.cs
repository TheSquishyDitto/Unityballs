using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlScript : MonoBehaviour {
	public GameMaster gm;				// Reference to the Game Master.
	public Button button;
	public GameObject test;
	bool t = false;
	Event e;
	
	/*
	public KeyCode forward;		// Which key moves the marble forward.
	public KeyCode backward;	// Which key moves the marble backward.
	public KeyCode left;		// Which key moves the marble left.
	public KeyCode right;		// Which key moves the marble right.
	public KeyCode jump;		// Which key makes the marble jump.
	
	public KeyCode camUp;		// Which key moves the camera upwards.
	public KeyCode camDown;		// Which key moves the camera downwards.
	public KeyCode camLeft;		// Which key moves the camera left.
	public KeyCode camRight;	// Which key moves the camera right.
	public KeyCode camToggle;	// Which key toggles the camera control mode.
	
	public KeyCode use;			// Which key uses buffs.
	public KeyCode levelHelp;	// Which key toggles level helpers.
	public KeyCode pause;		// Which key pauses the game.
	public KeyCode brake;		// Which key brakes the marble.
	public KeyCode respawn;		// Which key respawns the marble.
	*/
	
	KeyCode k;

	void Awake() {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
	}
	
	void Update() {
		test.SetActive(t);
		button.GetComponentInChildren<Text>().text = gm.input.forward.ToString();
		
	}

	public void OnGUI(){
		e = Event.current;
		t = !t;	
		
		if(t){			
			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				gm.input.forward = KeyCode.LeftShift;
				t = !t;
			}
			else if (Input.GetKeyDown(KeyCode.RightShift))
			{
				gm.input.forward = KeyCode.RightShift;
				t = !t;
			}
			else if (e != null && e.isKey && Input.anyKeyDown){
				k = e.keyCode;
				Debug.Log (k);
				gm.input.forward = k;
				
				t = !t;
				Debug.Log (gm.input.forward);
			}
			
		}
		
	}
}
