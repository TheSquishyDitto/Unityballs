/// <summary>
/// ObjectSpawner.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 29, 2015
/// Last Revision: May  12, 2015
/// 
/// Class that spawns objects using an object pool.
/// 
/// NOTES: - This implementation spawns things based on time.
/// 
/// TO DO: - Tweak until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider), typeof(ObjectPooler))]
public class ObjectSpawner : MonoBehaviour {

	// Variables
	#region Variables
	protected Transform myTransform;		// Cached reference to transform.
	protected BoxCollider spawnZone;		// Area to spawn objects within.
	protected ObjectPooler pool;			// Pool of objects to be drawn from.
	protected GameObject focus;				// The object currently being spawned.
	protected float spawnCountdown = 0;		// Time remaining until next spawn.
	
	public Vector2 frequency = new Vector2(1, 1);	// How quickly objects should spawn.

	#endregion

	// Start - Use this for initialization.
	void Start () {
		myTransform = transform;
		pool = GetComponent<ObjectPooler>();
		spawnZone = GetComponent<BoxCollider>();
		spawnZone.isTrigger = true;
	}
	
	// Update - Called once per frame.
	protected virtual void Update () {

		spawnCountdown -= Time.deltaTime; // Counts down until next spawn.

		// Once the countdown reaches zero, spawn and reset the timer.
		if (spawnCountdown <= 0) {
			Spawn();
			ResetCountdown();
		}
	}

	// Spawn - Grabs pooled object and reuses it.
	// NOTE: This spawns an object anywhere randomly inside the bounding box.
	protected virtual void Spawn() {
		focus = pool.GetObject();
		if (focus != null) {
			focus.transform.position = new Vector3(Random.Range(-spawnZone.bounds.extents.x, spawnZone.bounds.extents.x),
			                                       Random.Range(-spawnZone.bounds.extents.y, spawnZone.bounds.extents.y),
			                                       Random.Range(-spawnZone.bounds.extents.z, spawnZone.bounds.extents.z)) + myTransform.position;
			
			focus.SetActive(true);
		}
	}

	// ResetCountdown - Sets the spawning countdown up again.
	protected virtual void ResetCountdown() {
		spawnCountdown = Mathf.Clamp(Random.Range(frequency.x, frequency.y), 0, frequency.y);
	}

	// OnDrawGizmos - Allows you to see the spawn zone in scene view.
	void OnDrawGizmos() {
		spawnZone = GetComponent<BoxCollider>();
		Gizmos.color = new Color(1, .5f, 0, .5f);
		Gizmos.DrawCube(transform.position, spawnZone.bounds.size);
	}
}
