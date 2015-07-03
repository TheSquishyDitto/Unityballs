/// <summary>
/// GhostSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Jun. 26, 2015
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
using UnityEngine.Events;
using System.Collections;

public class GhostSource : BuffSource {

	// Variables
	#region Variables
	public static event UnityAction Ghosting;	// A subscription list of all things that need to happen when ghosting.
	public static event UnityAction Unghosting;	// Likewise for unghosting.

	Color marbleColor;	// Reference to marble's original color.

	#endregion

	// BuffFunction - Applies the buff to the marble.
	protected override void BuffFunction() {
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