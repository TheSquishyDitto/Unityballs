using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

	public Transform staree;	// Reference to transform to stare at.
	Transform myTransform;		// Reference to gameobject's transform.

	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (staree) myTransform.LookAt(staree);
	}
}
