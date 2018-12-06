using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradeControl : MonoBehaviour
{

	public GameObject[] ships = new GameObject[6];
	public Image[] shipUIImages;
	public Sprite[] shipSprites;
	public Sprite[] upgradeSprites;
	public Vector3[] spawnLocations = new Vector3[6];
	private Image selectedImage;
	public int selectedShip = 1;
	public string audioString;
	public Text priceText1, priceText2;

	public GameObject buyUpgradePanel, buyShipsPanel;

	public static UpgradeControl lastUpgradeControl;

	void Start ()
	{
		UpdateUI ();
		lastUpgradeControl = this;
	}

	public void SetShipPositions ()
	{
		int amountOfShips = 0;
		for (int i = 0; i < ships.Length; i ++) {
			if (ships [i] != null) {
				amountOfShips++;
			}
		}

		for (int i = 0; i < amountOfShips; i ++) {
			ships [i].transform.position = spawnLocations [i];
			ships [i].transform.rotation = Quaternion.identity;
			ShipControl m = ships [i].GetComponent<ShipControl> ();
			if (m) {
				m.Reset ();
			}
		}
	}

	public void UpdateUI ()
	{
		GetShips ();
		GetAvailableUpgrades ();
		SetUIImages ();
		SetUpgradeImages ();
		SetSelectedShip (shipUIImages [selectedShip - 1]);
	}

	public void UpgradeShip ()
	{
		ships [selectedShip - 1].GetComponent<ShipUpgrades> ().UpgradeThisShip ();
		if (ships [selectedShip - 1].GetComponent<ShipUpgrades> ().GetMaxUpgrades () - ships [selectedShip - 1].GetComponent<ShipUpgrades> ().AmountOfUpgrades () == 0) {
			SetButtonText ();
		}
	}

	public void SetSelectedShip (Image selectedImage)
	{
		string imageName = selectedImage.gameObject.name.ToString ();
		selectedShip = int.Parse ((imageName [4]).ToString ());
		SetButtonText ();
	}


	public void SetButtonText ()
	{
		if (selectedShip != 0) {
			GameObject ship = ships [selectedShip - 1];
			GetComponent<AudioManager> ().selectedShip = ship;
			if (ship != null) {
				buyUpgradePanel.SetActive (true);
				buyShipsPanel.SetActive (false);
				buyUpgradePanel.gameObject.transform.GetChild (0).GetComponent<Button> ().enabled = true;

				ShipUpgrades u = ship.GetComponent<ShipUpgrades> ();
				SetButtonActive (buyUpgradePanel.transform.GetChild (0).gameObject, u.UpgradeText ());
				if (u.AmountOfUpgrades () != u.GetMaxUpgrades ()) {
					priceText1.text = "" + 5; // u.GetUpgradeCost
					priceText2.text = "";
				} else {
					priceText1.text = "";
					priceText2.text = "";
				}

				/*if (ship.GetComponent<ShipUpgrades> ().GetMaxUpgrades () - ship.GetComponent<ShipUpgrades> ().AmountOfUpgrades () > 0) {
					SetButtonActive (buyUpgradePanel.transform.GetChild (0).gameObject, "Buy Upgrade");
					priceText1.text = "" + 5;
					priceText2.text = "";
				} else if (ship.GetComponent<ShipUpgrades> ().GetMaxUpgrades () == ship.GetComponent<ShipUpgrades> ().AmountOfUpgrades ()) {
					buyUpgradePanel.gameObject.transform.GetChild (0).GetComponent<Button> ().enabled = false;
					SetButtonActive (buyUpgradePanel.transform.GetChild (0).gameObject, "Ship Fully Upgraded");
					priceText1.text = "";
					priceText2.text = "";
				}*/
			} else {
				selectedShip = ShipContainer.GetSpaceshipCount () + 1;
				buyShipsPanel.SetActive (true);
				buyUpgradePanel.SetActive (false);
				SetButtonActive (buyShipsPanel.transform.GetChild (0).gameObject, "Buy Pidgeon");
				priceText1.text = "" + 5;
				priceText2.text = "" + 5;
				SetButtonActive (buyShipsPanel.transform.GetChild (1).gameObject, "Buy UFO");
			}
		} else {
			buyUpgradePanel.SetActive (false);
			buyShipsPanel.SetActive (false);
		}
	}

	void GetAvailableUpgrades ()
	{
		int activeShipCount = 0;
		for (int i = 0; i < ships.Length; i++) {
			if (ships [i] != null) {
				activeShipCount ++;
			}
		}
		for (int i = 0; i < activeShipCount; i++) {

			shipUIImages [i].gameObject.GetComponent<Button> ().interactable = true;
			Color c = shipUIImages [i].gameObject.GetComponent<Image> ().color;
			c.a = 1f;
			shipUIImages [i].gameObject.GetComponent<Image> ().color = c;
			//shipUIImages [i].gameObject.SetActive (true);

			int k = ships [i].GetComponent<ShipUpgrades> ().GetMaxUpgrades ();
			for (int j = 0; j < 3; j++) {
				shipUIImages [i].gameObject.transform.GetChild (j).gameObject.SetActive (k > j);
			}
		}
		for (int i = activeShipCount; i < ships.Length; i++) {

			shipUIImages [i].gameObject.GetComponent<Button> ().interactable = i <= activeShipCount;
			Color c = shipUIImages [i].gameObject.GetComponent<Image> ().color;
			if (i <= activeShipCount) {
				c.a = 1f;
			} else {
				c.a = 0f;
			}
			shipUIImages [i].gameObject.GetComponent<Image> ().color = c;
			//shipUIImages [i].gameObject.SetActive (i <= activeShipCount);

			for (int j = 0; j < 3; j++) {
				shipUIImages [i].gameObject.transform.GetChild (j).gameObject.SetActive (false);
			}
		}
	}

	void GetShips ()
	{
		for (int i = 0; i < ships.Length; i++) {
			if (ShipContainer.GetSpaceShip (i) != null) {
				ships [i] = ShipContainer.GetSpaceShip (i);
			} else {
				ships [i] = null;
			}
		}
	}

	void SetUIImages ()
	{
		for (int i = 0; i < ships.Length; i++) {
			if (ships [i] == null) {
				shipUIImages [i].sprite = shipSprites [0];
			} else if (ships [i].gameObject.name.Contains ("Mothership")) {
				shipUIImages [i].sprite = shipSprites [1];
			} else if (ships [i].gameObject.name.Contains ("Pidgeon")) {
				shipUIImages [i].sprite = shipSprites [2];
			} else if (ships [i].gameObject.name.Contains ("UFO")) {
				shipUIImages [i].sprite = shipSprites [3];
			} 
		}
	}

	void SetUpgradeImages ()
	{
		int amountOfShips = 0;
		for (int i = 0; i < ships.Length; i ++) {
			if (ships [i] != null) {
				amountOfShips++;
			}
		}
		for (int i = 0; i < amountOfShips; i++) {
			int k = ships [i].GetComponent<ShipUpgrades> ().AmountOfUpgrades ();
			for (int j = 0; j < 3; j++) {
				int boolInt = k > j ? 1 : 0;
				shipUIImages [i].gameObject.transform.GetChild (j).GetComponent<Image> ().sprite = upgradeSprites [boolInt];
			}

			/*switch (ships [i].GetComponent<ShipUpgrades> ().AmountOfUpgrades ()) {
			case 1:
				shipUIImages [i].gameObject.transform.GetChild (0).GetComponent<Image> ().sprite = upgradeSprites [1];
				shipUIImages [i].gameObject.transform.GetChild (1).GetComponent<Image> ().sprite = upgradeSprites [0];
				break;
			case 2:
				shipUIImages [i].gameObject.transform.GetChild (0).GetComponent<Image> ().sprite = upgradeSprites [1];
				shipUIImages [i].gameObject.transform.GetChild (1).GetComponent<Image> ().sprite = upgradeSprites [1];
				break;
			default:
				shipUIImages [i].gameObject.transform.GetChild (0).GetComponent<Image> ().sprite = upgradeSprites [0];
				shipUIImages [i].gameObject.transform.GetChild (1).GetComponent<Image> ().sprite = upgradeSprites [0];
				break;
			}*/
		}
	}

	public void SetButtonActive (GameObject button, string buttonText)
	{
		if (button) {
			button.gameObject.SetActive (true);
			//Debug.Log (buttonText);
			button.GetComponentInChildren<Text> ().text = buttonText;
		}
	}
}