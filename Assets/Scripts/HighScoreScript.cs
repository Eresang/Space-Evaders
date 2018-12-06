using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour {

	[SerializeField]private Text hS1, hS2, hS3, hS4, hS5;

	//checks if scores are already available
	void Start () {
		if(!PlayerPrefs.HasKey ("HighScore1")){
			PlayerPrefs.SetInt ("HighScore1", 0);
			PlayerPrefs.SetInt ("HighScore2", 0);
			PlayerPrefs.SetInt ("HighScore3", 0);
			PlayerPrefs.SetInt ("HighScore4", 0);
			PlayerPrefs.SetInt ("HighScore5", 0);
		}
		hS1.text = "" + PlayerPrefs.GetInt ("HighScore1");
		hS2.text = "" + PlayerPrefs.GetInt ("HighScore2");
		hS3.text = "" + PlayerPrefs.GetInt ("HighScore3");
		hS4.text = "" + PlayerPrefs.GetInt ("HighScore4");
		hS5.text = "" + PlayerPrefs.GetInt ("HighScore5");
	}
	

}
