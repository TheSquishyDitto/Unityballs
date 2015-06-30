/// <summary>
/// CharmButton.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 28, 2015
/// 
/// Class that handles some of the behavior/manages the data for the charm buttons.
/// 
/// NOTES: - Handles only the buttons, and mainly only stores references.
/// 
/// TO DO: - ???
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CharmButton : MonoBehaviour {
	public Image charmIcon;
	public Text charmName;
	public Text charmCost;
	public Image equippedIcon;
}
