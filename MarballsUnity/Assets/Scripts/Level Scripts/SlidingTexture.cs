using UnityEngine;
using System.Collections;

public class SlidingTexture : MonoBehaviour {

	public float speedModifier = 0.1f;
	Renderer appearance;

	// Use this for initialization
	void Start () {
		appearance = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		appearance.material.mainTextureOffset = new Vector2(0, Time.time * speedModifier);
	}
}
