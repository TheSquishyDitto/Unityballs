/// <summary>
/// StatUpdater.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 27, 2015
/// Last Revision: July 22, 2015
/// 
/// Class that updates status display.
/// 
/// NOTES: - Handles HP, MP, XP, time, speed, and related stat displays.
/// 	   - Does not handle buff, ability, or tip boxes.
/// 
/// TO DO: - Add support for minor text components as they gain functionality.
/// 	   - Hide the statbar when actually doing things. Show when idle.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatUpdater : MonoBehaviour {

	// Variables
	#region Variables
	GameMaster gm;

	// Major Text Components
	public Text health;
	public Text marpower;
	public Text exp;
	public Text timer;
	public Text speed;

	// Minor Text Components
	public Text defense;
	public Text discount;
	public Text surplus;
	public Text best;
	public Text rps;

	// Routine References
	Coroutine HPBlink;
	Coroutine MPBlink;

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
	}

	// OnEnable - Called when object becomes active.
	void OnEnable() {
		Messenger<int, int>.AddListener("UpdateHealth", UpdateHealth);
		Messenger<int, int>.AddListener("UpdateMarpower", UpdateMarpower);
		Messenger<int>.AddListener("UpdateExp", UpdateExp);
		Messenger<int>.AddListener("UpdateDefense", UpdateDefense);
	}

	// OnDisable - Called when object is deactivated.
	void OnDisable() {
		Messenger<int, int>.RemoveListener("UpdateHealth", UpdateHealth);
		Messenger<int, int>.RemoveListener("UpdateMarpower", UpdateMarpower);
		Messenger<int>.RemoveListener("UpdateExp", UpdateExp);
		Messenger<int>.RemoveListener("UpdateDefense", UpdateDefense);
	}

	// Start - Use this for initialization.
	void Start() {
		if (gm.levelData && gm.levelData.bestTimes.Count > 0)
			best.text = "Best: " + gm.levelData.bestTimes[0].ToString("F1") + " s";
		else
			best.text = string.Empty;
	}

	// Update - Called once per frame.
	void Update () {
		// Update timer.
		timer.text = gm.timer.ToString("F1");
		timer.text += " s";

		// Speed gauge.
		if (gm.marble) {
			speed.text = Mathf.Round(gm.marble.Speed) + " m/s";
			rps.text = Mathf.Round(gm.marble.RPS) + " rps";
		}
	}

	// UpdateHealth - Does what it says.
	void UpdateHealth(int hp, int maxHP) {
		if (HPBlink != null) StopCoroutine(HPBlink);
		health.text = hp + "/" + maxHP;
		HPBlink = StartCoroutine("Blink", health.gameObject);
	}

	// UpdateMarpower - Does what it says.
	void UpdateMarpower(int mp, int maxMP) {
		if (MPBlink != null) StopCoroutine(MPBlink);
		marpower.text = mp + "/" + maxMP;
		MPBlink = StartCoroutine("Blink", marpower.gameObject);
	}

	// UpdateExp - Does what it says.
	void UpdateExp(int xp) {
		exp.text = xp.ToString();
	}

	// UpdateDefense - Makes defense show up if there is any. Hides otherwise.
	void UpdateDefense(int def) {
		defense.transform.parent.gameObject.SetActive((def != 0));

		if (def != 0) {
			defense.text = (def > 0)? "+" + def : def.ToString();
			StartCoroutine("Blink", defense.gameObject);
		}
	}

	// Blink - Makes changes more noticeable.
	public IEnumerator Blink(GameObject blinkee) {
		for (int i = 0; i < 6; i++) {
			blinkee.SetActive(!blinkee.activeSelf);
			yield return new WaitForSeconds(0.2f);
		}

		blinkee.SetActive(true); // Just in case it accidentally ends up off.
	}
}
