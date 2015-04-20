using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// LEVELDATAOBJECT - Holds data specific to each level and aids indirectly in saving/loading.
[System.Serializable]
public class LevelDataObject : ScriptableObject {

	public enum MessageMode {
		Off,		// This level should not use any specialized messages.
		Replace,	// This level will replace the standard messages with its own.
		Append		// This level will add more messages to the standard pool.
	}

	public string levelName = "Generic Level";				// Name of level.
	public int difficulty = 0;								// Scale of 1-5 or 1-10 of how hard the level is.
	public AudioClip music;									// The BGM for the level.
	
	public List<float> bestTimes = new List<float>();		// Player's best times on the level.

	// CAN ADD MORE EASILY REDEFINABLE LEVEL PROPERTIES HERE, LIKE GRAVITY AND SUCH

	public MessageMode messageMode;		// Whether this level should use custom messages, and if so, how.
	public List<string> deathMessages = new List<string>();	// Death messages for this level.
	public List<string> winMessages = new List<string>();	// Win messages for this level.
}

// PLAYERRECORD - Class for holding player's save data before writing it.
[System.Serializable]
public class PlayerRecord {
	//public List<string> initials = new List<string>(); // Initials to match the scores.
	public List<float> bestTimes = new List<float>();	 // The 5 best scores the player has achieved.

	// UNLOCKABLES, PLAYBACKS, AND ACHIEVEMENTS CAN RESIDE IN THIS CLASS
}