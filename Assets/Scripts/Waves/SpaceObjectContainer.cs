using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Wave state.
/// </summary>
public enum WaveState
{
	Waiting,
	Running,
	Ended,
	Other
}

/// <summary>
/// Space object container.
/// </summary>
public class SpaceObjectContainer : MonoBehaviour
{
	// The current wave
	private Wave wave;

	public int initialSeed;
	public float initialAngle;
	public float fullCircleRotationTime;
	
	// Wait for all spawned objects to be destroyed before calling the wave finished
	public bool waitForObjectsToBeDeleted = true;

	public SpaceObject[] pipelines;
	public Wave[] waves;
	public float inBetweenWaveDelay;
	public GameObject inBetweenWave;
	private int waveIndex = 0;
	private float timer;
	private GameObject inBetween;
	
	private WaveState state;
	
	private static LinkedList<GameObject> spaceobjects = new LinkedList<GameObject> ();
	public static SpaceObjectContainer lastSpaceObjectContainer;

	public static LinkedList<GameObject> GetSpaceobjects ()
	{
		return spaceobjects;
	}

	public int GetWaveIndex ()
	{
		return waveIndex;
	}

	/// <summary>
	/// Adds the space ship.
	/// </summary>
	/// <param name="newSpaceship">New spaceship.</param>
	public static void AddSpaceObject (GameObject newSpaceobject)
	{
		spaceobjects.AddFirst (newSpaceobject);
	}
	
	/// <summary>
	/// Gets the space ship.
	/// </summary>
	/// <returns>The space ship.</returns>
	/// <param name="index">Index.</param>
	public static GameObject GetSpaceObject (int index)
	{
		if (spaceobjects.Count > 0) {
			int realIndex = index % spaceobjects.Count;
			LinkedListNode<GameObject> sobject = spaceobjects.Last;
			for (int i = 0; i < realIndex; i++) {
				sobject = sobject.Previous;
			}
			return sobject.Value;
		} else {
			return null;
		}
	}
	
	/// <summary>
	/// Gets the spaceship count.
	/// </summary>
	/// <returns>The spaceship count.</returns>
	public static int GetSpaceObjectCount ()
	{
		return spaceobjects.Count;
	}
	
	/// <summary>
	/// Removes the space ship.
	/// </summary>
	/// <returns>The space ship.</returns>
	/// <param name="spaceship">Spaceship.</param>
	public static int RemoveSpaceObject (GameObject spaceobject)
	{
		spaceobjects.Remove (spaceobject);
		return spaceobjects.Count;
	}

	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
	
	// Use this for internal initialization
	void Awake ()
	{
		// Make FixedUpdate select the next wave (or generate it)
		state = WaveState.Waiting;
		SpaceObjectDirection.SetAngle (initialAngle);
		SpaceObjectDirection.SetAngularSpeed (fullCircleRotationTime);
		SeededRandom.SetSeed (initialSeed);
		lastSpaceObjectContainer = this;
		waveIndex = Mathf.Min (waves.Length - 1, Global.startWave);
	}
	
	// Should we wait for the children to be deleted?
	bool WaitForChildren ()
	{
		return waitForObjectsToBeDeleted && (GetSpaceObjectCount () > 0);
	}

	public string GetTimeInfo ()
	{
		switch (state) {
		case WaveState.Running:
			return wave.GetWaveTime ();
		case WaveState.Other:
			if (timer >= inBetweenWaveDelay) {
				return "";
			} else {
				return Mathf.RoundToInt (inBetweenWaveDelay - timer).ToString ();
			}
		default:
			return "";
		}
	}
	
	// Update is called once per physics frame
	void FixedUpdate ()
	{
		switch (state) {
		// Wave is waiting to start up
		case WaveState.Waiting:
			if (wave == null && waves != null) {
				// Select wave
				if (waves.Length > 0 && waveIndex < waves.Length) {
					wave = waves [waveIndex];
					wave.StartWave ();
				} else {
					// No valid waves left >----------------------------------------------------------------------------
					//if (Global.currentGlobalControl) {
					//	state = WaveState.Ended;
					//	Global.currentGlobalControl.ShowInfo (new string[2]{" ", "Out of Waves!"});
					//}
					//                     >----------------------------------------------------------------------------
					wave = waves [waves.Length - 1];
					wave.StartWave ();
				}
				waveIndex++;
			} else {
				// Move to running state when the wave delay has passed
				if (wave.GetWaveStarted ()) {
					state = WaveState.Running;
					OnWaveStart ();
				}
			}
			break;
			
		// Wave is currently happening; generate objects
		case WaveState.Running:
			if (wave.GetWaveFinished () && !WaitForChildren ()) {	
				wave = null;
				state = WaveState.Other;
				if (inBetweenWave != null) {
					SpaceObjectDeSpawner[] objects = GetComponentsInChildren<SpaceObjectDeSpawner> ();
					for (int i = 0; i < objects.Length; i++) {
						objects [i].DeSpawn ();
					}
					timer = 0f;
					OnInBetweenStart ();
					/*GameObject[] comets = GameObject.FindGameObjectsWithTag ("SpaceObject");
						for (int i = 0; i < comets.Length -1; i++) {
							Destroy (comets [i]);
						}*/
				}
			} else {
				if (!Global.paused) {
					float difficulty = wave.GetDifficulty ();
					for (int i = 0; i < pipelines.Length; i++) {
						pipelines [i].TrySpawn (this.transform, difficulty);
					}
				} else {
					wave.Delay ();
				}
			}
			break;

		// Enough with the waves already
		case WaveState.Ended:

			break;

		// Allow a special wave to run in between normal waves and always wait for the children to have gone
		case WaveState.Other:
			if (timer < inBetweenWaveDelay) {
				timer += Time.fixedDeltaTime;
				if (timer >= inBetweenWaveDelay) {
					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++) {
						Destroy (transform.GetChild (i).gameObject);
					}

					inBetween = Instantiate (inBetweenWave) as GameObject;
					inBetween.transform.SetParent (transform);
				}
			} else if (inBetweenWave == null || (inBetweenWave != null && inBetween == null)) {
				state = WaveState.Waiting;
				OnWaveEnd ();
			}
			break;
		}
	}

	void OnInBetweenStart ()
	{
		if (Global.currentGlobalControl) {
			Global.currentGlobalControl.ShowInfo (new string[2]{"", "Bonus"});
		}
	}

	/// <summary>
	/// Raises the wave end event.
	/// </summary>
	void OnWaveEnd ()
	{
		if (Global.currentGlobalControl) {
			//Global.currentGlobalControl.ShowInfo (new string[2]{"defeated", "Wave " + waveIndex.ToString ()});
		}
	}

	/// <summary>
	/// Raises the wave start event.
	/// </summary>
	void OnWaveStart ()
	{
		if (Global.currentGlobalControl) {
			Global.currentGlobalControl.ShowInfo (new string[2]{"incoming", "Wave " + waveIndex.ToString ()});
		}
	}
}