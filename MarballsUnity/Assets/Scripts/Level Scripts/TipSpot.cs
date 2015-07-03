/// <summary>
/// TipSpot.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  6, 2015
/// Last Revision: Jun. 25, 2015
/// 
/// Class for zones that should cause tips to pop up.
///
/// TO DO: - Tweak behavior until desired.
/// 	   - Determine exactly how tips should disappear.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class TipSpot : MonoBehaviour {

	// Variables
	#region Variables
	GameMaster gm;		// Reference to the Game Master.

	[TextArea(3,10)]	// <- Makes the tip show up with multiple lines in the inspector.
	public string tip;	// What will be written in the tip box.

	public bool showOnce = true;	// Whether tip should be displayed only once or every time.
	int displayCount = 0;			// How many times the tip has been displayed.

	public Color textColor = new Color(.88f, .88f, .2f, 1);		// What color the tip should be.
	public Color backColor = new Color(.03f, .157f, .03f, .4f);	// What color the tip's box should be.

	public float duration;	// How long the tip should be displayed.
	public AudioClip sound;	// The sound to be played when this tip pops up.
	public Collider zone;	// The trigger zone of this tip spot.

	TipBoxInfo info;

	#endregion

	// Awake - Called before anything else.	
	void Awake () {
		gm = GameMaster.CreateGM();
	}

	// Use this for initialization
	void Start () {
		info = new TipBoxInfo(tip, textColor, backColor, duration);
		info.text = Substitute(info.text);
	}

	// OnTriggerEnter - Display tip.
	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Marble>()) {
			if (!(showOnce && displayCount > 0)) {
				Messenger<TipBoxInfo>.Broadcast("DisplayTip", info);

				if (sound) AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);

				displayCount++;
			}
		}
	}

	// OnDrawGizmos - Allows the tip spot to be seen more easily in the scene view.
	void OnDrawGizmosSelected() {
		Gizmos.color = textColor;

		// Draws different gizmos if different colliders are used.
		if (zone.GetType() == typeof(SphereCollider)) {
			Gizmos.DrawWireSphere(transform.position, ((SphereCollider)zone).radius);
		}

		if (zone.GetType() == typeof(BoxCollider)) {
			Gizmos.DrawWireCube(transform.position, ((BoxCollider)zone).bounds.size);
		}
	}

	// Substitute - Replaces certain words and phrases with their intended meaning.
	// NOTE: Most likely not the most efficient way to do this, but it doesn't lag the game (yet).
	// REPLACE VARIOUS REFERENCES THROUGH GAMEMASTER BY MAKING INPUTMANAGER A SINGLETON.
	string Substitute(string tipText) {
		string result;

		result = tipText.Replace("&use", gm.input.keyBindings[10].ToString());
		result = result.Replace("&pause", gm.input.keyBindings[14].ToString());
		result = result.Replace("&jump", gm.input.keyBindings[4].ToString());
		result = result.Replace("&forward", gm.input.keyBindings[0].ToString());
		result = result.Replace("&back", gm.input.keyBindings[1].ToString());
		result = result.Replace("&left", gm.input.keyBindings[2].ToString());
		result = result.Replace("&right", gm.input.keyBindings[3].ToString());
		result = result.Replace("&camleft", gm.input.keyBindings[7].ToString());
		result = result.Replace("&camright", gm.input.keyBindings[8].ToString());
		result = result.Replace("&camup", gm.input.keyBindings[5].ToString());
		result = result.Replace("&camdown", gm.input.keyBindings[6].ToString());
		result = result.Replace("&camtoggle", gm.input.keyBindings[9].ToString());
		result = result.Replace("&guide", gm.input.keyBindings[13].ToString());

		return result;
	}
}

[System.Serializable]
public class TipBoxInfo {
	public string text;
	public Color textColor;
	public Color boxColor;
	public float duration;
	
	public TipBoxInfo(string tip, Color tC, Color bC, float time = 5) {
		text = tip;
		textColor = tC;
		boxColor = bC;
		duration = time;
	}
}
