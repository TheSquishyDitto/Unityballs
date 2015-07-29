/// <summary>
/// GameMenu.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class that handles the in-game menu that displays marble stats, progress, and charms.
/// 
/// NOTES: - Only tab currently implemented is charm tab.
/// 
/// TO DO: - Status tab.
/// 	   - Polishing charm tab: need a way to gray out charms when there aren't enough points to equip them.
/// 	   		+ Can darken most button features, just need to refresh list to find out which buttons are weeded out.
/// 			+ Allow rearranging of charms. Either click and drag or some manner of sorting options.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GameMenu : MonoBehaviour {
	// Variables
	#region Variables
	GameMaster gm;						// Reference to game master.
	Canvas gameMenu;					// Reference to the canvas this container is on.

	public Text pointsRemaining;		// How many charm points are left.
	public GameObject charmContainer;	// The game object the charm buttons are parented to.
	public GameObject charmButton;		// The prefab used to instantiate the buttons.
	public Text description;			// Description of charm.

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
		gameMenu = GetComponent<Canvas>();
	}

	// OnEnable - Called when object is enabled.
	void OnEnable() {
		Messenger.AddListener("GameMenu", ToggleGameMenu);
	}

	// OnDisable - Called when object is disabled.
	void OnDisable() {
		Messenger.RemoveListener("GameMenu", ToggleGameMenu);
	}

	// Start - Use this for initialization.
	void Start() {
		gameMenu.enabled = false;
		PopulateCharmList();
	}

	// Update - Called every frame.
	void Update() {
		// Refreshes available charm points.
		// NOTE: THIS CAN BE OPTIMIZED BY ONLY DOING IT ON CHARM EQUIP/DEQUIP
		if (pointsRemaining != null && gm.marble != null)
			pointsRemaining.text = "Charm Capacity: " + gm.marble.charmCapacity;
	}

	// ToggleGameMenu - Enables/disables in-game menu.
	void ToggleGameMenu() {
		gameMenu.enabled = !gameMenu.enabled;
		//Time.timeScale = (gameMenu.enabled)? 0 : 1; // Not sure whether to pause with the menu open or not.
		// Pausing currently makes blinking coroutines look funny.
	}

	// PopulateCharmList - Generates the list of charms the player currently owns.
	public void PopulateCharmList() {

		// For every charm the player has...
		for (int i = 0; i < gm.marble.charms.Count; i++) {
			// Instantiate the button and put it where it belongs,
			CharmButton newButton = Instantiate(charmButton).GetComponent<CharmButton>();
			newButton.transform.SetParent(charmContainer.transform);

			// Update the button's info,
			newButton.charmIcon.sprite = gm.marble.charms[i].data.icon;
			newButton.charmIcon.color = gm.marble.charms[i].data.tint;
			newButton.charmName.text = gm.marble.charms[i].type;
			newButton.charmCost.text = gm.marble.charms[i].data.cost.ToString();
			newButton.equippedIcon.gameObject.SetActive(gm.marble.charms[i].equipped);

			// Make the button do things when clicked,
			newButton.GetComponent<Button>().onClick.AddListener(gm.marble.charms[i].ToggleEquip);
			int i2 = i;	// This is necessary to make the next line function properly.
			newButton.GetComponent<Button>().onClick.AddListener(() => {newButton.equippedIcon.gameObject.SetActive(gm.marble.charms[i2].equipped);});

			// Make the buttons show descriptions when hovered over by using code I don't fully understand,
			EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
			EventTrigger.Entry entry = new EventTrigger.Entry();

			trigger.AddListener((eventData) => { description.text = gm.marble.charms[i2].data.description; });

			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback = trigger;

			newButton.GetComponent<EventTrigger>().triggers.Add(entry);


			// ... and set the scale back to one? Why this is necessary I don't know, but it is.
			newButton.transform.localScale = Vector3.one;
		}
	}
}
