/// <summary>
/// BuffSource.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 23, 2015
/// Last Revision: Feb. 23, 2015
/// 
/// General class for granting/clearing buffs via trigger.
/// 
/// NOTES: - Can be modified easily via Unity inspector. The alternative to this class is a base buff class.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BuffSource : MonoBehaviour {

	public Marble.PowerUp buffType;	// What type of buff this source gives.
	public bool collectable;		// Whether this source disappears when collected.

	public float intensity;			// How strong the buff is. Acceptable values vary wildly by buff type.
	public int jumpCount;			// If granting multijump, how many jumps the marble should have.

	public float duration;			// How long the given buff should last.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Only grants buffs to marbles.
			Marble marble = other.GetComponent<Marble>();

			switch (buffType) {	// Grants selected buff based on enum.
			case Marble.PowerUp.None: marble.ClearBuffs(); break;
			case Marble.PowerUp.SpeedBoost: marble.SpeedBoost(intensity, duration); break;
			case Marble.PowerUp.MultiJump: marble.MultiJump(jumpCount); break;
			case Marble.PowerUp.SuperJump: marble.SuperJump(intensity); break;
			case Marble.PowerUp.SizeChange: marble.SizeChange(intensity, duration); break;
			default: Debug.LogWarning("(BuffSource.cs) Unsupported buff type!"); break;

			}

			if (collectable) gameObject.SetActive(false);	// Disappears if collectable.
		}	
	}
}
