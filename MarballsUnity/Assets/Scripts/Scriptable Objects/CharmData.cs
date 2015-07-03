using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class CharmData : ScriptableObject {
	public string type;				// The name of this type of charm.
	public int cost = 0; 			// The CP cost of equipping this type of charm.
	public bool visible = true;		// Whether this charm shows up to the player.
	public Sprite icon;				// The icon displayed in the charm management window/ability box.
	public string description;		// A description of what this charm does/offers.
	public Color tint;				// The color to tint the icon.
}
