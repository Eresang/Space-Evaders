using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour
{

	private float masterVolume, backgroundVolume, SFXVolume;
	private int audioType;
	[SerializeField]
	private Slider
		audioSlider;
	public Slider musicSlider, sFXSlider, speechSlider;
	[SerializeField]
	private GameObject
		clickParticle, bgplane;
	private float alpha = 1;
	private bool playClicked = false;
	private bool loading = false;
	[SerializeField]
	private Image
		masterVolumeLight, bgVolumeLight, SFXVolumeLight;
	[SerializeField]
	private Sprite
		greenLight, redLight;

	public Text startWaveText;

	void Start ()
	{
		//make sure all settings have a base value;
		if (!PlayerPrefs.HasKey ("MasterVolume")) {
			PlayerPrefs.SetFloat ("MasterVolume", 1f);
			PlayerPrefs.SetFloat ("BackgroundVolume", 0.5f);
			PlayerPrefs.SetFloat ("SFXVolume", 0.5f);
			PlayerPrefs.SetString ("Difficulty", "Easy");
		}
		masterVolume = PlayerPrefs.GetFloat ("MasterVolume");
		speechSlider.value = masterVolume;
		backgroundVolume = PlayerPrefs.GetFloat ("BackgroundVolume");
		musicSlider.value = backgroundVolume;
		SFXVolume = PlayerPrefs.GetFloat ("SFXVolume");
		sFXSlider.value = SFXVolume;

		TryChangeStartLevel (0);
	}

	void Update ()
	{
		//fade out menu on play
		if (playClicked) {
			alpha -= Time.deltaTime;
			GetComponent<CanvasGroup> ().alpha = alpha;
		}

		//start game when menu is done fading out
		if (alpha <= 0 && !loading) {
			Application.LoadLevelAsync ("Game");
			loading = true;
		}

		if (Input.GetMouseButtonDown (0)) {
			//instantiates clickparticle;
			InstantiateClickParticle ();
		}
	}

	public void TryChangeStartLevel (int change)
	{
		Global.startWave = Mathf.Clamp (Mathf.FloorToInt ((Global.startWave + change) / 5f) * 5, 0, Global.maxWaves);
		if (startWaveText) {
			startWaveText.text = Global.startWave.ToString ();
		}
	}

	// make menu fade out
	public void FadeMenuToBlack ()
	{
		playClicked = true;
	}

	/// <summary>
	/// Setting the slider to modify the right type of audio
	/// </summary>
	/// <param name="audiotype">Audiotype.</param>
	public void SelectSound (int audiotype)
	{
		audioType = audiotype;
		switch (audioType) {
		case 0:
			audioSlider.value = masterVolume;
			masterVolumeLight.sprite = greenLight;
			bgVolumeLight.sprite = redLight;
			SFXVolumeLight.sprite = redLight;
			break;
		case 1:
			audioSlider.value = backgroundVolume;
			masterVolumeLight.sprite = redLight;
			bgVolumeLight.sprite = greenLight;
			SFXVolumeLight.sprite = redLight;
			break;
		case 2: 
			audioSlider.value = SFXVolume;
			masterVolumeLight.sprite = redLight;
			bgVolumeLight.sprite = redLight;
			SFXVolumeLight.sprite = greenLight;
			break;
		}
	}

	/// <summary>
	/// Changes the sound level based on the position of the sliderhandle
	/// </summary>
	public void ChangeSoundLevel ()
	{
		switch (audioType) {
		case 0:
			masterVolume = audioSlider.value;
			PlayerPrefs.SetFloat ("MasterVolume", masterVolume);
			break;
		case 1:
			backgroundVolume = audioSlider.value;
			PlayerPrefs.SetFloat ("BackgroundVolume", backgroundVolume);
			break;
		case 2:
			SFXVolume = audioSlider.value;
			PlayerPrefs.SetFloat ("SFXVolume", SFXVolume);
			break;
		case 3:
			return;
		}
	}

	public void ChangeSoundLevel (int audioCategory)
	{
		switch (audioCategory) {
		case 0:
			masterVolume = speechSlider.value;
			PlayerPrefs.SetFloat ("MasterVolume", masterVolume);
			break;
		case 1:
			backgroundVolume = musicSlider.value;
			PlayerPrefs.SetFloat ("BackgroundVolume", backgroundVolume);
			break;
		case 2:
			SFXVolume = sFXSlider.value;
			PlayerPrefs.SetFloat ("SFXVolume", SFXVolume);
			break;
		case 3:
			return;
		}
	}

	/// <summary>
	/// Changes the difficulty.
	/// </summary>
	/// <param name="difficulty">Difficulty.</param>
	public void ChangeDifficulty (string difficulty)
	{
		PlayerPrefs.SetString ("Difficulty", difficulty);
	}

	/// <summary>
	/// Instantiates the click particle.
	/// </summary>
	void InstantiateClickParticle ()
	{
		if (EventSystem.current.IsPointerOverGameObject ()) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				GameObject.Instantiate (clickParticle, hit.point, hit.transform.rotation);
			}
		}
	}
}
