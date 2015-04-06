/// <summary>
/// GhostSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr.  3, 2015
/// 
/// Class for ghost granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class GhostSource : BuffSource {

	Color marbleColor;	// Reference to marble's original color.
	
	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = Ghost; // Basically gives the marble the buff function to use.
		marble.heldBuff = Marble.PowerUp.Ghost;
	}
	
	// Ghost - Makes the marble intangible for a while.
	public void Ghost(float intensity, float duration) {
		marble.buff = Marble.PowerUp.Ghost;

		// Makes the marble translucent.
		marbleColor = marble.GetComponent<Renderer>().material.color;
		marble.GetComponent<Renderer>().material.color = new Color(marbleColor.r, marbleColor.g, marbleColor.b, 0.5f);
	}
	
	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		marble.GetComponent<Renderer>().material.color = marbleColor; // Restores opacity.
	}
}