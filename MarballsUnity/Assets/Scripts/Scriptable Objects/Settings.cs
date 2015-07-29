/// <summary>
/// Settings.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 25, 2015
/// Last Revision: July 22, 2015
/// 
/// Class for creating universal game settings assets.
/// 
/// NOTES: - One has already been created. If lost, create another from the asset menu after uncommenting [CreateAssetMenu].
/// 
/// TO DO: - Make actual use of settings saving/loading.
/// 	   - Get InputManager to save key bindings as well.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

//[CreateAssetMenu]
public class Settings : ScriptableObject {

	[Header("Developer Settings")]
	public string version;			// Current game version.
	public bool debug;				// Whether the game is in debug mode or not.
	public bool freezeTimer;		// Whether timer will tick.
	public float countdownLength;	// How long player must wait to start the level.
	public int highScoreCount;		// How many high scores should be saved per level.

	[Header("Game Settings")]
	[Range(1, 120)]
	public int targetFPS;			// Preferred framerate. Useless if VSync is active.
	public bool simpleAnim;			// Whether most animations should be simplified/skipped.
	public bool gravityFinish;		// Whether the victory animation should be a black hole or not.
	public bool guideArrows;		// Whether the arrow guides are enabled or not.
	public bool useOnGrab;			// Whether powerups should be used on contact or not. May interfere with level completion.

	[Header("Video Settings")]
	[Range(30, 150)]
	public int fov;					// Field of view for player's camera.

	[Header("Audio Settings")] // There is a built-in audio manager in Unity, but it doesn't differentiate music and sound effects.
	[Range(0, 1)]
	public float masterVolume;		// Volume control that modifies the volume of all game sounds.
	[Range(0, 1)]
	public float musicVolume;		// Volume control for music only.
	[Range(0, 1)]
	public float fxVolume;			// Volume control for sound effects only.

	// Getters/Setters
	public float MusicScaler {
		get { return musicVolume * masterVolume; }
	}

	public float FXScaler {
		get { return fxVolume * masterVolume; }
	}

	// SaveSettings - Saves game settings from a settings asset.
	public static void SaveSettings(Settings settings) {
		PlayerPrefs.SetString("Version", settings.version);
		PlayerPrefs.SetInt("Target_FPS", settings.targetFPS);
		PlayerPrefs.SetInt("Simple_Animations", (settings.simpleAnim)? 1 : 0);
		PlayerPrefs.SetInt("Gravity_Finish", (settings.gravityFinish)? 1 : 0);
		PlayerPrefs.SetInt("Guide_Arrows", (settings.guideArrows)? 1 : 0);
		PlayerPrefs.SetInt("Use_Buffs_On_Grab", (settings.useOnGrab)? 1 : 0);
		PlayerPrefs.SetFloat("Master_Volume", settings.masterVolume);
		PlayerPrefs.SetFloat("Music_Volume", settings.musicVolume);
		PlayerPrefs.SetFloat("FX_Volume", settings.fxVolume);
	}

	// LoadSettings - Loads game settings into an existing settings asset.
	public static void LoadSettings(Settings settings) {
		settings.targetFPS = PlayerPrefs.GetInt("Target_FPS");
		settings.simpleAnim = (PlayerPrefs.GetInt("Simple_Animations") == 1);
		settings.gravityFinish = (PlayerPrefs.GetInt("Gravity_Finish") == 1);
		settings.guideArrows = (PlayerPrefs.GetInt("Guide_Arrows") == 1);
		settings.useOnGrab = (PlayerPrefs.GetInt("Use_Buffs_On_Grab") == 1);
		settings.masterVolume = PlayerPrefs.GetFloat("Master_Volume");
		settings.musicVolume = PlayerPrefs.GetFloat("Music_Volume");
		settings.fxVolume = PlayerPrefs.GetFloat("FX_Volume");
	}
}
