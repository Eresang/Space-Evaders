using UnityEngine;
using System.Collections;

public static class Highscore
{
	private static float currentScore = 0f;

	/// <summary>
	/// Adds the score.
	/// </summary>
	/// <returns>The score.</returns>
	/// <param name="score">Score.</param>
	public static float AddScore (float score)
	{
		currentScore += score;
		return currentScore;
	}

	/// <summary>
	/// Removes the score.
	/// </summary>
	/// <returns>The score.</returns>
	/// <param name="score">Score.</param>
	public static float RemoveScore (float score)
	{
		currentScore -= score;
		return currentScore;
	}

	public static bool TryRemoveScore (float score)
	{
		if (currentScore >= score) {
			currentScore -= score;
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Gets the score.
	/// </summary>
	/// <returns>The score.</returns>
	public static float GetScore ()
	{
		return currentScore;
	}

	/// <summary>
	/// Resets the score.
	/// </summary>
	public static void ResetScore ()
	{
		//currentScore = 0f;
	}
}
