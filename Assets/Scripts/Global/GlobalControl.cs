using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Collection of global variables
public static class Global
{
	public static bool paused = false;
	public static int[] signs = new int[2]{-1, 1};
	public static GlobalControl currentGlobalControl;
	public static GameObject currentMarkerContainer;
	public static GameObject defaultExplosion;
	public static GameObject defaultScrapShipExplosion;
	public static GameObject defaultCrunch;
	public static GameObject defaultConversion;
	public static int maxWaves = 15;
	public static int startWave = 0;
}

// For setting up Global and Screen in a scene
public class GlobalControl : MonoBehaviour
{
	public GameObject markerContainer;
	public GameObject defaultExplosion;
	public GameObject defaultScrapShipExplosion;
	public GameObject defaultCrunch;
	public GameObject defaultConversion;
	public GameObject gameOverPrefab;
	public GameObject infoTextParent;
	public float infoTextDuration;
	[Range(0f, 1f)]
	public float
		infoTextAlphaPeak;
	public Color infoTextColor;
	
	private float timer = Mathf.Infinity;
	private Text[] labels;
	private GameObject pause;
	private float unpauseTimer = 0f;
	private float gameOverTimer = 0f;

	public void GameOver (float delay)
	{
		gameOverTimer = delay;
		Global.paused = true;
	}

	public void Unpause (float delay)
	{
		if (gameOverTimer <= 0f) {
			unpauseTimer = delay;
		}
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		Screen.Update ();
		Highscore.ResetScore ();
		Global.paused = false;
		Global.currentGlobalControl = this;
		Global.currentMarkerContainer = markerContainer;
		Global.defaultExplosion = defaultExplosion;
		Global.defaultScrapShipExplosion = defaultScrapShipExplosion;
		Global.defaultCrunch = defaultCrunch;
		Global.defaultConversion = defaultConversion;
	}

	public void SetStartWave (int number)
	{
		Global.startWave = Mathf.Max (number, Global.maxWaves);
	}

	public int GetMaxWaves ()
	{
		return Global.maxWaves;
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		if (infoTextParent) {
			// Make sure the parent is active before retrieving Text[]
			bool oldActive = infoTextParent.activeSelf;
			infoTextParent.SetActive (true);
			labels = infoTextParent.GetComponentsInChildren<Text> ();
			infoTextParent.SetActive (oldActive);
		}
	}

	/// <summary>
	/// Shows the info.
	/// </summary>
	/// <param name="wave">Wave.</param>
	/// <param name="texts">Texts.</param>
	public void ShowInfo (string[] texts)
	{
		if (labels != null) {
			infoTextParent.SetActive (true);
			for (int i = 0; i < Mathf.Min (labels.Length, texts.Length); i++) {
				labels [i].text = texts [i];
			}
		}
		timer = 0f;
	}
	
	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void FixedUpdate ()
	{
		if (!Global.paused) {
			if (infoTextDuration > 0f && timer <= infoTextDuration) {
				// Peak alpha at 1/2 duration
				Color c = infoTextColor;
				if (infoTextAlphaPeak > 0f) {
					float a = timer / (infoTextDuration * infoTextAlphaPeak);
					if (a >= 1f) {
						a = (infoTextDuration - timer) / (infoTextDuration * (1f - infoTextAlphaPeak));
					}
					c.a = a;
				} else {
					c.a = 1f;
				}
				for (int i = 0; i < labels.Length; i++) {
					labels [i].color = c;
				}
			
				timer += Time.fixedDeltaTime;
			} else if (timer != Mathf.Infinity) {
				Color c = infoTextColor;
				c.a = 0f;
				for (int i = 0; i < labels.Length; i++) {
					labels [i].color = c;
				}
				infoTextParent.SetActive (false);
				timer = Mathf.Infinity;
			}
		} else {
			if (gameOverTimer > 0f) {
				gameOverTimer -= Time.fixedDeltaTime;
				if ((gameOverTimer <= 0f) && (gameOverPrefab)) {
					Instantiate (gameOverPrefab);
					Destroy (SpaceObjectContainer.lastSpaceObjectContainer.gameObject);
					Destroy (ShipContainer.lastContainer);
				}
			} else if (unpauseTimer > 0f) {
				unpauseTimer -= Time.fixedDeltaTime;
				if (unpauseTimer <= 0f) {
					Global.paused = false;
				}
			}
		}
	}

}