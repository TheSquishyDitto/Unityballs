/// <summary>
/// BeaniePropeller.cs
/// Authors: Kyle Dawson
/// Date Created:  May   1, 2015
/// Last Revision: May   1, 2015
/// 
/// Class for spinning the beanie based on marble's speed.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BeaniePropeller : MonoBehaviour {

	GameMaster gm;				// Reference to Game Master.
	Transform myTransform;		// Cached reference to transform.
	Rigidbody marbody;			// Reference to marble's rigidbody.

	float speedMultiplier = 5;	// Affects how fast beanie spins.

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		marbody = gm.marble.marbody;
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.Rotate(Vector3.up, marbody.velocity.sqrMagnitude * Time.deltaTime * speedMultiplier);
	}
}
