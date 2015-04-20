using UnityEngine;
using System.Collections;

public class SlidingTexture : MonoBehaviour {

	public float speedModifier = 0.1f;				// How fast texture should move.
	public Vector2 direction = new Vector2(0, 1);	// Direction the texture should move.
	Renderer appearance;							// Cached reference to renderer.

	// Use this for initialization
	void Start () {
		appearance = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		appearance.material.mainTextureOffset = direction * Time.time * speedModifier;
	}
}
