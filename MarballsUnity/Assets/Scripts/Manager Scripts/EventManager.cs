/// <summary>
/// EventManager.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 22, 2015
/// Last Revision: Jun. 24, 2015
/// 
/// Class that handles messages between game objects.
/// 
/// NOTES: - This implementation is heavily based on Adam Buckner's from the live training tutorial on Event Systems.
/// 	   - MESSENGER.CS IS HIGHLY SUPERIOR, USE IT INSTEAD.
/// 
/// TO DO: - Tweak for efficiency/flexibility/customization?
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {

	// Variables
	#region Variables
	static EventManager eventManager;	// Singleton(-esque) reference to the EventManager.
	Dictionary <string, UnityEvent> eventDictionary;	// Dictionary of events.

	static UnityEvent outEvent = null;	// Holds an event and prevents garbage collector clutter.

	#endregion

	// Singleton(-esque) Getter
	// REVAMP TO BE AN ACTUAL SINGLETON?
	public static EventManager instance {
		get {
			if (!eventManager) {
				eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;
				
				if (!eventManager) {
					Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
				} else {
					eventManager.Init (); 
				}
			}
			
			return eventManager;
		}
	}

	// Init - Initializes the event dictionary.
	void Init () {
		if (eventDictionary == null) {
			eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}

	// StartListening - Adds an event to the dictionary if it isn't already there.
	public static void StartListening (string eventName, UnityAction listener) {
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
			thisEvent.AddListener (listener);
		} else {
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventName, thisEvent);
		}
	}

	// StopListening - Removes an event from the dictionary.
	public static void StopListening (string eventName, UnityAction listener) {
		if (eventManager == null) return;
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
			thisEvent.RemoveListener (listener);
		}
	}

	// TriggerEvent - Triggers an event in the event dictionary, if it exists.
	public static void TriggerEvent (string eventName) {
		outEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out outEvent)) {
			outEvent.Invoke ();
		}
	}
}