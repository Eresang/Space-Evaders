using UnityEngine;
using System.Collections;

[System.Serializable]
public static class SeededRandom
{
	private static int seed;

	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

	public static void SetSeed (int InitialSeed)
	{
		seed = InitialSeed;
	}

	public static float Value ()
	{
		Random.seed = seed;
		float result = Random.value;
		seed = Random.seed;
		return result;
	}

	public static bool Decision ()
	{
		return Value () < 0.5f;
	}

	public static int Sign ()
	{
		return Global.signs [Range (0, 2)];
	}

	public static int Range (int min, int max)
	{
		Random.seed = seed;
		int result = Random.Range (min, max);
		seed = Random.seed;
		return result;
	}

	public static float Range (float min, float max)
	{
		Random.seed = seed;
		float result = Random.Range (min, max);
		seed = Random.seed;
		return result;
	}
}
