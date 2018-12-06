using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreHUDControl : MonoBehaviour
{
	public Text score, time;
	
	void FixedUpdate ()
	{
		if (score != null) {
			score.text = Mathf.FloorToInt (Highscore.GetScore ()).ToString ();
		}
		if (SpaceObjectContainer.lastSpaceObjectContainer) {
			if (time != null) {
				time.text = SpaceObjectContainer.lastSpaceObjectContainer.GetTimeInfo ();
			}
		} else if (time != null) {
			time.text = "";
		}
	}
}
