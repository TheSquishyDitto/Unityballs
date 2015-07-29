/// <summary>
/// LevelLoadTrigger.cs (formerly SecretLevel.cs)
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Mar. 22, 2015
/// Last Revision: July 24, 2015
/// 
/// Class that loads another level upon touching a trigger.
/// 
/// NOTES: - Currently loads levels synchronously and only by trigger.
/// 
/// TO DO: - Use enum to make editor spiffy (name/index options should not be visible simultaneously)
/// 	   - Split level loading functions into a dedicated level loading wrapper.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class LevelLoadTrigger : MonoBehaviour {

	public enum TypeChoice {
		ByName,
		ByIndex
	}

	public TypeChoice choice = TypeChoice.ByName;	// Whether to use the index or the name.
	public string levelName;						// Name of level to load.
	public int levelIndex;							// Index of level to load.

	//public bool additively = false;				// Whether to load the level additively or not.
	//public bool async = false;					// Whether level should be loaded in the background.
	//public bool active = true;					// Whether trigger is active or not.

	// OnTriggerEnter - Called when an object hits the trigger.
	void OnTriggerEnter (Collider other) {
		if (other.GetComponent<Marble>()) {
			if (choice == TypeChoice.ByName)
				GameMaster.CreateGM().LoadLevel(levelName);
			else if (choice == TypeChoice.ByIndex)
				GameMaster.CreateGM().LoadLevel(levelIndex);
		}
	}
}
