using UnityEngine;
using System.Collections;

public class VolumeManager : MonoBehaviour {
	
	public float SFXVolume = 1;
	public float BackgroundVolume = 1;

	[SerializeField]private AudioSource[] backgroundAudio;
	[SerializeField]private AudioSource[] sfxAudio;

	void Update(){
		SFXVolume = (PlayerPrefs.GetFloat ("MasterVolume") * PlayerPrefs.GetFloat ("SFXVolume"));
		BackgroundVolume = (PlayerPrefs.GetFloat ("MasterVolume") * PlayerPrefs.GetFloat ("BackgroundVolume"));
		sfxAudio = FindObjectsOfType<AudioSource>();
		for(int i = 0; i < sfxAudio.Length; i ++){
			sfxAudio[i].volume = SFXVolume;
		}
		for (int i = 0; i < backgroundAudio.Length; i ++){
			backgroundAudio[i].volume = BackgroundVolume;
		}
	}
}
