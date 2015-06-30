/// <summary>
/// MarbleData.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 25, 2015
/// Last Revision: Jun. 26, 2015
/// 
/// Class for creating assets that store default values for each marble form.
/// 
/// NOTES: - These do not set the marble's values as runtime (for the most part). The marble will try to default to these values.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class MarbleData : ScriptableObject {

	[Header("Identifying Info")]
	public string form;				// Name of this marble form.

	[Header("RPG Stats")]
	public int defense;				// How much defense this marble type has.
	//public int maxHeldBuffs;		// How many buffs the marble can hold.

	[Header("Movement Stats")]
	public float size;				// Default marble size.
	public float maxAngVelocity;	// Marble's maximum rotational speed. Affects top speed.
	public float speedMultiplier;	// Modifier for marble's velocity.
	public float revSpeed;			// Modifer for marble's acceleration.
	public float brakeSpeed;		// Modifer for marble's stopping speed.
	public float shackle;			// Limiting constant for speed.
	public float jumpHeight;		// Modifier for marble's jumping height.
}
