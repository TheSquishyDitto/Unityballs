using UnityEngine;
using System.Collections;

public class JitterCam : MonoBehaviour {

	public float intensity = 1;
	public float interval = 1.5f;

	Transform myTransform;
	Vector3 startPos;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		startPos = myTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time % interval < 1)
			myTransform.position = startPos + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0);
		else
			myTransform.position = startPos;
	}
}
