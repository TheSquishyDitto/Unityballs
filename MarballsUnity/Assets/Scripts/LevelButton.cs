using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour {

	//GameMaster gm;				// TEMPORARY?

	public int buildLevel = 0;	// Index of which level to load.
	//public Text highScore;	// Reference to high score text.
	Button self;				// Reference to button component.

	// IF GENERATED DYNAMICALLY, GET RID OF THE AWAKE FUNCTION HERE
	void Awake() {
		//gm = GameMaster.CreateGM();
	}

	// Start - Use this for initialization.
	void Start () {
		self = GetComponent<Button>();
		self.onClick.AddListener(ButtonLoadLevel);
	}
	
	// Update - Called once per frame.
	void Update () {
		//Debug.Log(events.IsPointerOverGameObject());
	}

	void ButtonLoadLevel() {
		//Debug.Log("It worked!");
		Application.LoadLevel(buildLevel);
	}
}
