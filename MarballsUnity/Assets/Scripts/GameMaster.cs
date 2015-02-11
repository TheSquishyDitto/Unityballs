using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	public enum GameState {
		Start,
		Playing,
		Win
	}
	
	public float timer;
	public GameState state;
	

	// Use this for initialization
	void Start () {
		timer = 0;
		state = GameState.Playing;
	}
	
	// Update is called once per frame
	void Update () {
		if(state == GameState.Playing) {
			timer += Time.deltaTime;
		}
	}
}
