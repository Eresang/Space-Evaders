using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class Wave
{
	private float creationTime;

	// Wait time before spawning, amount of time before maximum difficulty is reached and duration of the wave itself
	public float waveDelay, waveTime, maxDifficultyTime;
	[Range(0f, 1f)]
	public float
		minDifficulty, maxDifficulty;

	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

	// Constructor
	public Wave ()
	{
		creationTime = Mathf.Infinity;
	}

	public string GetWaveTime ()
	{
		return Mathf.RoundToInt (waveTime - Mathf.Max (0f, Mathf.Min (waveTime, Time.time - (creationTime + waveDelay)))).ToString ();
	}

	// Because the constructor isn't called in the main thread
	public void StartWave ()
	{
		creationTime = Time.time;
	}

	public bool GetWaveStarted ()
	{
		return Time.time - waveDelay >= creationTime;
	}

	// Does the wave still have time left?
	public bool GetWaveFinished ()
	{
		return creationTime + waveTime <= Time.time - waveDelay;
	}

	public float GetDifficulty ()
	{
		return maxDifficultyTime != 0f ? Mathf.Clamp01 (Mathf.Lerp (minDifficulty, maxDifficulty, (Time.time - creationTime - waveDelay) / maxDifficultyTime)) : maxDifficulty;
	}

	public void Delay ()
	{
		creationTime += Time.fixedDeltaTime;
	}
}
