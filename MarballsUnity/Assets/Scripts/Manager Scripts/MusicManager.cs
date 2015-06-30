using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	AudioSource boombox;		// Reference to audio source on this object.
	Settings settings;			// Reference to game settings.

	public AudioClip[] music;	// List of music to be played on each level.
								// NOT SURE IF LEVELMUSIC IN LEVELDATA SHOULD EVEN BE USED?
	
	// Awake - Called before anything else.
	void Awake() {
		boombox = GetComponent<AudioSource>();
		settings = GameMaster.LoadSettings();
	}

	// Start - Use this for initialization.
	void Start () {
		PlayMusic(Application.loadedLevel);
	}
	
	// Update - Called once per frame.
	void Update () {
		boombox.volume = settings.MusicScaler;
	}

	// OnLevelWasLoaded - Called when a level is loaded.
	void OnLevelWasLoaded(int level) {
		PlayMusic(level);
	}

	// PlayMusic - Plays the designated music for the current level.
	public void PlayMusic(int level) {
		boombox.Stop();

		if (music[level] != null)
			boombox.clip = music[level];

		boombox.Play();
	}
}
