/// <summary>
/// ObjectPooler.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 29, 2015
/// Last Revision: Apr. 29, 2015
/// 
/// Class that allows recycling cloned instances of objects without constant instantiation.
/// 
/// NOTES: - This implementation is heavily based on Mike Geig's from the live training tutorial on Object Pooling.
/// 
/// TO DO: - Tweak for efficiency/flexibility?
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {

	// Variables
	#region Variables
	public GameObject pooledObject;		// What object will be pooled.
	public int poolSize = 20;			// Preferred/starting size of pool.
	public bool allowGrowth = true;		// Allow pool to grow if necessary.

	List<GameObject> pool;				// The actual pool of objects to use.

	#endregion

	// Start - Use this for initialization.
	void Start () {
		pool = new List<GameObject>();

		// Sets up initial pool of objects.
		for (int i = 0; i < poolSize; i++) {
			pool.Add(CreateObject());
		}
	}

	// GetObject - Draws an object from the pool.
	public GameObject GetObject() {
		// Finds an unused object in the pool and returns it.
		for (int i = 0; i < pool.Count; i++) {
			if (!pool[i].activeInHierarchy) {
				return pool[i];
			}
		}

		// If there were no unused objects, but growth is enabled, just makes another.
		if (allowGrowth) {
			pool.Add(CreateObject());
			return pool[pool.Count - 1];
		}

		// If there were no objects available and growth is disabled, complain about it.
		Debug.LogWarning("(ObjectPooler.cs) Tried to obtain object but none available!");
		return null;
	}

	// CreateObject - Creates a pooled object.
	GameObject CreateObject() {
		GameObject obj = (GameObject)Instantiate(pooledObject);
		obj.transform.parent = transform;
		obj.SetActive(false);
		return obj;
	}
}
