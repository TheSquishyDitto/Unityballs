/// <summary>
/// Balloon.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 26, 2015
/// Last Revision: Apr. 27, 2015
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
	Vector3 startPos;
	//float startHeight;
	float timeOffset;

	public bool collectable = true;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		startPos = myTransform.position;
		//startHeight = myTransform.position.y;
		timeOffset = Random.Range(0, 10.0f);
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.position = startPos + (Vector3.up * Mathf.Sin(Time.time + timeOffset));
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Marble")) {
			AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/Balloon"), Vector3.zero);
			if (collectable) gameObject.SetActive(false);
		}
	}
}
