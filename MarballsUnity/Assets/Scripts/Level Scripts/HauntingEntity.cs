/// <summary>
/// HauntingEntity.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 10, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class for Jerry. "I see."
///
/// TO DO: - Add more spooky behaviors, such as allowing Jerry to cause damage.
/// 	   - Finetune existing behavior maybe? The glitches are almost features though.
/// 	   - Adopt start sequence; start moving with a low priority.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class HauntingEntity : Ghostable {

	// Variables
	#region Variables

	Transform myTransform;			// Cached transform.
	Settings settings;				// Reference to game settings.
	Transform cam;					// Reference to the main camera.
	SphereCollider col;				// Reference to collider.
	Component[] appearances;		// Array of all renderer components of this object.

	public Transform destination;	// Where the object should move to.
	public bool useJumpScare = true;// Whether Jerry should make the player void their bowels.

	bool visible;					// Whether object's renderers as a whole are enabled or not.
	bool shouldTeleport;			// Whether object should teleport.

	[Header("Creepy Effect References")]
	public GameObject accomplice;				// Reference to Jerry model used for special effects.
	public Camera spookCam;						// Reference to scary cam.
	public JitterCam jitter;					// Reference to spasmatic effect on the scary cam.
	public NoiseAndGrain noise;					// Reference to visual noise effect.
	public MotionBlur blur;						// Reference to motion blur.
	public VignetteAndChromaticAberration vig;	// Reference to sorta blurry-ish effect with dark edges?

	#endregion

	// Start - Use this for initialization.
	protected override void Start () {
		myTransform = transform;
		appearances = GetComponentsInChildren<Renderer>();

		foreach (Renderer appearance in appearances)
			appearance.enabled = physical;

		visible = physical;

		marble = gm.marble;
		settings = GameMaster.LoadSettings();
		cam = Camera.main.transform;

	}
	
	// Update - Called once per frame.
	protected override void Update () {
		// If visible and distant enough away...
		if (/*gm.state == GameMaster.GameState.Playing && */ !((Renderer)appearances[0]).isVisible && Vector3.Distance(myTransform.position, destination.position) > 0) {
			myTransform.position = new Vector3(myTransform.position.x, destination.position.y + 2, myTransform.position.z); // Stay on player's y-level.
			myTransform.position += (destination.position - myTransform.position).normalized * Time.deltaTime * (gm.timer / 60); // Move towards player, quickly as time goes on.
			myTransform.LookAt(cam); // Face the camera.
			myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, 0, myTransform.eulerAngles.z); // Try to keep y-axis unrotated (doesn't work?)
		}

		// Used for testing.
		if (Input.GetKeyDown(KeyCode.G) && GameMaster.LoadSettings().debug) {
			StartCoroutine("JumpScare");
		}
	}

	// OnTriggerEnter - Called when object enters trigger radius.
	void OnTriggerEnter() {
		shouldTeleport = true;

		// If entity can be seen, fade away.
		if (visible) 
			StartCoroutine("FadeOut");

		// Otherwise, do something spooky.
		else {
			Teleport();
			int effect = Random.Range(0, 6);
			switch (effect) {
			case 0: AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/SpookyWind"), cam.position, settings.FXScaler); break;
			case 1: marble.marbody.AddForce(Random.insideUnitSphere.normalized * 2000); break;
			case 2: if (useJumpScare) StartCoroutine("JumpScare"); break;
			case 3: marble.ClearAllBuffs(); break;
			case 4: gm.timer++; break;
			case 5: TipBoxInfo scaryMessage = new TipBoxInfo("...I see you...", Color.white, Color.black, 3);//References.hud.tipBox.color = Color.black;
					Messenger<TipBoxInfo>.Broadcast("DisplayTip", scaryMessage); break;
					//References.hud.tipMessage.color = Color.white;
					//References.hud.tipMessage.text = "...I see you..."; 
					//References.hud.tipWindow.SetActive(true); break;
			default: break;
			}
		}
	}

	// GhostMode - Makes Jerry visible.
	protected override void GhostMode() {
		StopAllCoroutines();
		if (!physical) StartCoroutine("FadeIn");
	}
	
	// NormalMode - Makes Jerry invisible.
	protected override void NormalMode() {
		StopAllCoroutines();
		if (!physical && visible) {
			StartCoroutine("FadeOut");
			visible = false;
		}
	}

	// FadeIn - Makes the object slowly fade in to existence.
	protected override IEnumerator FadeIn() {
		visible = true;
		foreach (Renderer appearance in appearances)
			appearance.enabled = true;

		for (int i = 1; i <= fadeLength; i++) {
			foreach (Renderer appearance in appearances)
				appearance.material.SetColor("_Color", Color.Lerp(Color.clear, Color.white, ((float)i)/fadeLength));
			yield return new WaitForFixedUpdate();
		}
	}

	// FadeOut - Makes the object slowly fade out of existence.
	protected override IEnumerator FadeOut() {
		for (int i = 1; i <= fadeLength; i++) {
			foreach (Renderer appearance in appearances)
				appearance.material.SetColor("_Color", Color.Lerp(Color.white, Color.clear, ((float)i)/fadeLength));
			yield return new WaitForFixedUpdate();
		}

		foreach (Renderer appearance in appearances)
			appearance.enabled = false;

		if (shouldTeleport) Teleport();
	}

	// Teleport - Move to another location.
	void Teleport() {
		shouldTeleport = false;

		myTransform.position = new Vector3(Random.Range(-100, 100), 2, Random.Range(-100, 100)) + destination.position;

		if (visible) StartCoroutine("FadeIn");
	}

	// JumpScare - Makes Jerry's face menacingly pop up on screen.
	IEnumerator JumpScare() {
		accomplice.SetActive(true);
		spookCam.enabled = true;
		jitter.enabled = true;
		noise.enabled = true;
		blur.enabled = true;
		vig.enabled = true;

		noise.intensityMultiplier = 0;
		blur.blurAmount = 0.92f;
		vig.intensity = 0;

		for (int i = 0; i < 25; i++) {
			noise.intensityMultiplier += 0.4f;
			//blur.blurAmount += 0.01f;
			vig.intensity += 0.4f;

			yield return new WaitForFixedUpdate();
		}

		for (int i = 0; i < 25; i++) {
			noise.intensityMultiplier -= 0.4f;
			//blur.blurAmount -= 0.01f;
			vig.intensity -= 0.4f;
			
			yield return new WaitForFixedUpdate();
		}

		accomplice.SetActive(false);
		spookCam.enabled = false;
		jitter.enabled = false;
		noise.enabled = false;
		blur.enabled = false;
		vig.enabled = false;

	}
}
