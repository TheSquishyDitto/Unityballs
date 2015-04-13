/// <summary>
/// TeleportZone.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 11, 2015
/// Last Revision: Apr. 11, 2015
/// 
/// Class for zones that teleport an object to another location.
///  
/// NOTES: - Currently only supports BoxCollider triggers.
/// 
/// TO DO: - Add support for other collider types.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TeleportZone : MonoBehaviour {

	public Transform destination;	// Place to teleport object to.
	BoxCollider zone;				// Reference to zone's trigger.
	
	// Start - Use this for initialization.
	void Start() {
		zone = GetComponent<BoxCollider>();

		if (!zone.isTrigger) zone.isTrigger = true;
	}

	// OnTriggerEnter - Teleports object to destination.
	void OnTriggerEnter(Collider other) {
		other.transform.position = destination.position;
	}

	// OnDrawGizmosSelected - Shows teleport zone in scene view.
	void OnDrawGizmosSelected() {
		Gizmos.color = new Color(0, 1, 1, 0.5f);

		Gizmos.DrawCube(transform.position, GetComponent<BoxCollider>().size);
	}
}
