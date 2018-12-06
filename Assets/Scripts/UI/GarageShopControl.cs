using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class OptionButtonSettings
{
	public bool optionEnabled;
	public string[] texts = new string[2];
	public Sprite[] images = new Sprite[2];
	public Color color;
	public int cost;
	public AudioClip[] voiceOver;
}

[System.Serializable]
public class OptionSettings
{
	public OptionButtonSettings[] options = new OptionButtonSettings[2];
}

public class GarageShopControl : MonoBehaviour
{
	public AudioManager audioManager;

	public GameObject pigeonPrefab;
	public GameObject uFOPrefab;

	public Text scrapText;

	public Vector3[] resetPositions;

	public ShopShipPanel[] shipControls;
	public Sprite[] shipImages;
	private ShopShipPanel currentSelection;

	private ShipShopOptions options;
	public OptionSettings[] optionSettings;

	public Sprite[] selectionSprites;

	public GarageShopOptionControl option1, option2;

	public void TryOption (int optionID)
	{
		switch (options) {
		case ShipShopOptions.Ships:
			if (optionID == 0) {
				// Enough scrap? Buy Pigeon
				if (Highscore.TryRemoveScore (optionSettings [(int)options].options [0].cost)) {
					Instantiate (pigeonPrefab);
					int audioID = Random.Range (0, optionSettings [(int)options].options [0].voiceOver.Length);
					audioManager.Play (optionSettings [(int)options].options [0].voiceOver [audioID]);
				}
			} else {
				// Enough scrap? Buy UFO
				if (Highscore.TryRemoveScore (optionSettings [(int)options].options [1].cost)) {
					Instantiate (uFOPrefab);
					int audioID = Random.Range (0, optionSettings [(int)options].options [1].voiceOver.Length);
					audioManager.Play (optionSettings [(int)options].options [1].voiceOver [audioID]);
				}
			}
			break;

		case ShipShopOptions.Drones:
			if (optionID == 0) {
				// Enough scrap and still upgradable? Buy Drone
				if (currentSelection.UpgradeAvailable () && (Highscore.TryRemoveScore (optionSettings [(int)options].options [0].cost))) {
					currentSelection.BuyUpgrade ();
					int audioID = Random.Range (0, optionSettings [(int)options].options [0].voiceOver.Length);
					audioManager.Play (optionSettings [(int)options].options [0].voiceOver [audioID]);
				}
			} // No second option needed
			break;

		case ShipShopOptions.Boost:
			if (optionID == 0) {
				// Enough scrap and still upgradable? Buy Boost
				if (currentSelection.UpgradeAvailable () && (Highscore.TryRemoveScore (optionSettings [(int)options].options [0].cost))) {
					currentSelection.BuyUpgrade ();
					int audioID = Random.Range (0, optionSettings [(int)options].options [0].voiceOver.Length);
					audioManager.Play (optionSettings [(int)options].options [0].voiceOver [audioID]);
				}
			} else {
				// Destroy ship and regain scrap
				Highscore.AddScore (optionSettings [(int)options].options [1].cost);
				currentSelection.DestroyShip ();
				int audioID = Random.Range (0, optionSettings [(int)options].options [1].voiceOver.Length);
				audioManager.Play (optionSettings [(int)options].options [1].voiceOver [audioID]);
			}
			break;

		case ShipShopOptions.Waypoint:
			if (optionID == 0) {
				// Enough scrap and still upgradable? Buy Waypoint
				if (currentSelection.UpgradeAvailable () && (Highscore.TryRemoveScore (optionSettings [(int)options].options [0].cost))) {
					currentSelection.BuyUpgrade ();
					int audioID = Random.Range (0, optionSettings [(int)options].options [0].voiceOver.Length);
					audioManager.Play (optionSettings [(int)options].options [0].voiceOver [audioID]);
				}
			} else {
				// Destroy ship and regain scrap
				Highscore.AddScore (optionSettings [(int)options].options [1].cost);
				currentSelection.DestroyShip ();
				int audioID = Random.Range (0, optionSettings [(int)options].options [1].voiceOver.Length);
				audioManager.Play (optionSettings [(int)options].options [1].voiceOver [audioID]);
			}
			break;
		}
	}

	public void ChangeShips ()
	{
		int m = ShipContainer.GetSpaceshipCount ();
		for (int i = 0; i < shipControls.Length; i++) {
			if (shipControls [i]) {
				GameObject ship = ShipContainer.GetSpaceShip (i);
				ShipControl sc = null;
				int image = 0;
				if (ship) {
					ship.transform.position = resetPositions [i];
					ship.transform.rotation = Quaternion.identity;

					sc = ship.GetComponent<ShipControl> ();
					image = (int)sc.shipType;
					sc.Reset ();
				}
				shipControls [i].SetShip (sc, shipImages [image], i <= m);
			}
		}
		if (scrapText)
			scrapText.text = Mathf.FloorToInt (Highscore.GetScore ()).ToString ();
	}

	public void SetCurrentSelection (int selectionID)
	{
		currentSelection = shipControls [selectionID];

		for (int i = 0; i < shipControls.Length; i++) {
			shipControls [i].SetSelected (selectionSprites [i == selectionID ? 1 : 0]);
		}
	}

	public void OnOptionChange ()
	{
		options = currentSelection.GetOptions ();
		option1.SetOptions (optionSettings [(int)options].options [0]);
		option2.SetOptions (optionSettings [(int)options].options [1]);
		option1.gameObject.SetActive (optionSettings [(int)options].options [0].optionEnabled && currentSelection.UpgradeAvailable ());
		option2.gameObject.SetActive (optionSettings [(int)options].options [1].optionEnabled);
	}

	// Use this for initialization
	void Start ()
	{
		ChangeShips ();
	}
}
