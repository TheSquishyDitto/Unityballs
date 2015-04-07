/// <summary>
/// GhostSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr.  6, 2015
/// 
/// Class for ghost granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 	   - Consider tinting the screen a spooky color while ghosting.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class GhostSource : BuffSource {

	// Variables
	#region Variables
	public delegate void GhostAction();
	public delegate void UnghostAction();

	public static GhostAction Ghosting;		// A subscription list of all things that need to happen when ghosting.
	public static UnghostAction Unghosting;	// Likewise for unghosting.

	Color marbleColor;	// Reference to marble's original color.

	#endregion
	
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

		if (Ghosting != null) Ghosting();
	}
	
	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		marble.GetComponent<Renderer>().material.color = marbleColor; // Restores opacity.

		if (Unghosting != null) Unghosting();
	}
}