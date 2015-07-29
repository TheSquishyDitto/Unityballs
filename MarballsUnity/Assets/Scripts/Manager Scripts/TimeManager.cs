/// <summary>
/// TimeManager.cs
/// Authors: Kyle Dawson
/// Date Created:  July 18, 2015
/// Last Revision: July 29, 2015
/// 
/// Class that manages any time-related functionality not already covered by Unity.
/// 
/// NOTES: - Currently only contains a stopwatch feature.
/// 	   - TimeEvent class implemented at bottom of this file.
/// 
/// TO DO: - Can probably make this class completely static by making it invoke the coroutine on the caller.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimeManager : MonoBehaviour {

	public static TimeManager Timer;	// Reference to timer. 

	// CreateTimer - Used to grab the singleton instance.
	public static TimeManager CreateTimer() {
		if (Timer == null) {
			Timer = ((GameObject)Instantiate(Resources.Load ("Prefabs/GameMaster"))).GetComponent<TimeManager>();

			if (Timer == null)
				Debug.LogWarning("(TimeManager.cs) Failed to load Time Manager!");
		}

		return Timer;
	}

	// Awake - Called before anything else.
	public void Awake() {
		Timer = this;
	}

	// StartStopwatch - Starts the Stopwatch coroutine.
	public void StartStopwatch(TimeEvent timer) {
		timer.routine = StartCoroutine(Stopwatch(timer));
	}

	// Stopwatch - Coroutine that executes an action after time has passed.
	public IEnumerator Stopwatch(TimeEvent timer) {
		float initialTime = timer.duration;

		// Counts down timer.
		while (timer.duration > 0) {
			timer.duration -= (!timer.paused)? Time.deltaTime : 0;
			timer.completion = 1 - (timer.duration / initialTime);
			yield return null;
		}

		// Executes action when timer has finished.
		if (timer.action != null)
			timer.action();
	}
}

[System.Serializable]
public class TimeEvent {
	public float duration;		// Length of time before executing action.
	public UnityAction action;	// Action to take when time runs out.
	public bool paused = false;	// Whether timer should stop or not.
	public Coroutine routine;	// Coroutine affecting this instance.
	public float completion;	// Percentage of how completed the timer is.

	// Constructors
	public TimeEvent(float duration) {
		this.duration = duration;
	}

	public TimeEvent(float duration, UnityAction action) {
		this.duration = duration;
		this.action = action;
	}
}
