using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	public float timer;

	// Use this for initialization
	void Start () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
	}
}
