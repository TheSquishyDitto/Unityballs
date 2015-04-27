/// <summary>
/// Balloon.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 26, 2015
/// Last Revision: Apr. 26, 2015
/// 
/// Class for collectable balloons.
/// 
/// NOTES: - Currently completely aesthetic.
/// 
/// TO DO: - Tweak until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Balloon : MonoBehaviour {

	Transform myTransform;
	float startHeight;

	public bool collectable = true;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		startHeight = myTransform.position.y + Random.Range(-2.0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.position = new Vector3(myTransform.position.x, 
		                                   startHeight,
		                                   myTransform.position.z) + (Vector3.up * Mathf.Sin(Time.time));
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Marble")) {
			AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/Balloon"), Vector3.zero);
			if (collectable) gameObject.SetActive(false);
		}
	}
}
