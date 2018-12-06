using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ContinueOptions
{
	public GameObject associatedPanel;
	public Sprite buttonText;
}

public class GarageMenuContinueControl : MonoBehaviour
{
	public ContinueOptions[] continueOptions;
	public Image buttonImage;
	private int currentOption = -1;
	public bool destroyOnOutOfOptions;
	public GameObject shop;

	void SetOptions (bool active)
	{
		if ((currentOption >= 0) && (currentOption < continueOptions.Length)) {
			if (continueOptions [currentOption].associatedPanel)
				continueOptions [currentOption].associatedPanel.SetActive (active);
			if (active)
				buttonImage.sprite = continueOptions [currentOption].buttonText;
		}
	}

	public void NextOption ()
	{
		if ((currentOption + 1 >= continueOptions.Length) && destroyOnOutOfOptions) {
			Destroy (shop);
		} else {
			SetOptions (false);
			currentOption++;
			SetOptions (true);
		}
	}

	// Use this for initialization
	void Start ()
	{
		NextOption ();
	}
}
