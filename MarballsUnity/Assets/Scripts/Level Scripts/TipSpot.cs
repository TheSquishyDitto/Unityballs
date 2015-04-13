/// <summary>
/// TipSpot.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  6, 2015
/// Last Revision: Apr.  6, 2015
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

	#endregion

	// Awake - Called before anything else.	
	void Awake () {
		gm = GameMaster.CreateGM();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	// OnTriggerEnter - Display tip.
	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Marble>()) {
			if (!(showOnce && displayCount > 0)) {
				CancelInvoke("HideTip");
				gm.hud.tipBox.color = backColor;
				gm.hud.tipMessage.text = Substitute(tip);
				gm.hud.tipMessage.color = textColor;
				gm.hud.tipWindow.SetActive(true);

				if (sound) AudioSource.PlayClipAtPoint(sound, gm.cam.position);

				displayCount++;
				Invoke("HideTip", duration);
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

	// HideTip - Hides the tip window.
	void HideTip() {
		gm.hud.tipWindow.SetActive(false);
	}

	// Substitute - Replaces certain words and phrases with their intended meaning.
	// NOTE: Most likely not the most efficient way to do this, but it doesn't lag the game (yet).
	string Substitute(string tipText) {
		string result;

		result = tipText.Replace("&use", gm.input.use.ToString());
		result = result.Replace("&pause", gm.input.pause.ToString());
		result = result.Replace("&jump", gm.input.jump.ToString());
		result = result.Replace("&forward", gm.input.forward.ToString());
		result = result.Replace("&back", gm.input.backward.ToString());
		result = result.Replace("&left", gm.input.left.ToString());
		result = result.Replace("&right", gm.input.right.ToString());
		result = result.Replace("&camleft", gm.input.camLeft.ToString());
		result = result.Replace("&camright", gm.input.camRight.ToString());
		result = result.Replace("&camup", gm.input.camUp.ToString());
		result = result.Replace("&camdown", gm.input.camDown.ToString());
		result = result.Replace("&camtoggle", gm.input.camToggle.ToString());
		result = result.Replace("&guide", gm.input.levelHelp.ToString());

		return result;
	}
}
