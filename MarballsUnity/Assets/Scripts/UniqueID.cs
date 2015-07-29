// Note: See UniqueIdentifierAttribute.cs in the Editor folder for credits.
// This class CAN be used as a component, but the [UniqueIdentifier] tag can be used anywhere for the same effect.

using UnityEngine;
using System.Collections;

// Placeholder for UniqueIdDrawer script
public class UniqueIdentifierAttribute : PropertyAttribute { }

public class UniqueID : MonoBehaviour {
	[UniqueIdentifier]
	public string uniqueID;
}
