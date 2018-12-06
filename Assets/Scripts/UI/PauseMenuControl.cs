using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenuControl : MonoBehaviour
{
	public bool pauseOnDeploy;
	public Text gameOverText;

	void Start ()
	{
		if (pauseOnDeploy)
			Global.paused = true;
		if (gameOverText)
			gameOverText.text = "died during wave " + SpaceObjectContainer.lastSpaceObjectContainer.GetWaveIndex ().ToString ();
	}

	void OnDestroy ()
	{
		Global.currentGlobalControl.Unpause (0.2f);
	}

	public void DoResume ()
	{
		Destroy (this.gameObject);
	}

	public void ToMenu ()
	{
		Application.LoadLevelAsync ("Menu");
	}
}
