using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveNumberDisplayControl : MonoBehaviour
{
	public Text display;

	// Use this for initialization
	void Start ()
	{
		if (display)
			display.text = SpaceObjectContainer.lastSpaceObjectContainer.GetWaveIndex ().ToString ();
	}
}
