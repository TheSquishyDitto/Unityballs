using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// LEVELDATAOBJECT - Holds data specific to each level and aids in saving/loading.
[System.Serializable]
public class LevelDataObject : ScriptableObject {
	public string levelName = "Generic Level";	// Name of level.
	public int difficulty = 0;					// Scale of 1-5 or 1-10 of how hard the level is.
	public float bestTime = Mathf.Infinity;		// Player's best time on the level.

	// CAN ADD EASILY REDEFINABLE LEVEL PROPERTIES HERE, LIKE GRAVITY AND SUCH

	public bool customMessages = false;	// Whether this level should use its own set of win/death messages.
	public string[] deathMessages;		// Death messages for this level.
	public string[] winMessages;		// Win messages for this level.
}

// PLAYERRECORD - Class for holding player's save data before writing it.
[System.Serializable]
public class PlayerRecord {
	public List<float> bestTimes;
	public List<string> levelNames;	// Could be used to validate which time corresponds to which level.

	// UNLOCKABLES AND ACHIEVEMENTS CAN RESIDE IN THIS CLASS

	public PlayerRecord() {
		bestTimes = new List<float>();
		levelNames = new List<string>();
	}
}