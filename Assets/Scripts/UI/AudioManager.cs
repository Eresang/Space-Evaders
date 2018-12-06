using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public AudioClip[] voiceOvers;
	public AudioSource audioPlayer;
	public GameObject selectedShip;
	public int selection;
	public string thisString;

	
	public void SetString (string s)
	{
		thisString = s;
	}

	public void SetUpgradeClip ()
	{
		switch (selectedShip.name) {
		case "Mothership":
			if (selectedShip.GetComponent<ShipUpgrades> ().AmountOfUpgrades () == 1) {
				Play (1);
			} else {
				Play (2);
			}
			break;
		case "UFO(Clone)":
			Play (4);
			break;
		case "Pidgeon(Clone)":
			Play (0);
			break;
		}
	}

	public void SetBuyClip ()
	{
		switch (thisString) {
		case "BuyUFO":
			Play (5);
			break;
		case "BuyPigeon":
			Play (3);
			break;
		}
	}

	public void Play (int i)
	{
		if (!audioPlayer.isPlaying) {
			audioPlayer.clip = voiceOvers [i];
			audioPlayer.Play ();
		}
	}

	public void Play (AudioClip a)
	{
		if (!audioPlayer.isPlaying) {
			audioPlayer.clip = a;
			audioPlayer.Play ();
		}
	}
	
	void Update ()
	{
		//thisString = GetComponent<UpgradeControl>().audioString;
	}
}
