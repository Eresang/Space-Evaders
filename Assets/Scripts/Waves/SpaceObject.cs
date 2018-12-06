using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpaceObject
{
	public GameObject prefab;
	public float interval;
	[Range(0f, 1f)]
	public float
		minDifficulty = 0f;
	[Range(0f, 1f)]
	public float
		maxDifficulty = 1f;
	[Range(0f, 1f)]
	public float
		chance = 1f;

	public string displayName;

	private float timer = 0f;

	public GameObject TrySpawn (Transform parent, float difficulty)
	{
		GameObject newSpawn = null;
		// Within allowable timespan?
		if (timer <= Time.time) {
			timer = Time.time + interval;
			// Within acceptable difficulty range? Chance factor?
			if (difficulty >= minDifficulty && difficulty <= maxDifficulty && (prefab) && SeededRandom.Value () <= chance) {
				newSpawn = GameObject.Instantiate (prefab) as GameObject;
				newSpawn.transform.SetParent (parent);
			}
		}
		return newSpawn;
	}
}
