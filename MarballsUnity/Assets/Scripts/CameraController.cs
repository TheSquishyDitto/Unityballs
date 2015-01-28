using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform ball;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.LookAt (ball);
	}
}
