/// <summary>
/// FollowMarble.cs (formerly BaseParticle.cs)
/// Authors: Kyle Dawson
/// Date Created:  Feb. 11, 2015
/// Last Revision: Apr. 18, 2015
/// 
/// Class that simply follows the marble.
/// 
/// NOTES: - Use this for anything not attached to the marble that doesn't know where the marble is.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class FollowMarble : FollowObject {

	protected GameMaster gm;		// Reference to Game Master.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();
	}

	// Start - Use this for initialization
	protected override void Start () {
		base.Start();
		trackingObject = gm.marble.marform;
	}
}
