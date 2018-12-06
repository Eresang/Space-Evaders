using UnityEngine;
using System.Collections;

/// <summary>
/// Spawn offsets.
/// </summary>
[System.Serializable]
public class MinMaxOffsets
{
	public Vector3 minOffset, maxOffset;
}

/// <summary>
/// Space object spawn type.
/// </summary>
public enum SpaceObjectSpawnType
{
	Parallel,
	Predefined
}

/// <summary>
/// Space object trajectory type.
/// </summary>
public enum SpaceObjectTrajectoryType
{
	Targeted,
	TargetZero,
	TargetOpposite,
	TargetOppositeX,
	TargetOppositeY,
	Predefined
}

/// <summary>
/// Space object spawner.
/// </summary>
public class SpaceObjectSpawner : MonoBehaviour
{
	public float minSpawnTime, maxSpawnTime;
	[Range(0f, 1f)]
	public float
		spawnChance;

	public GameObject prefab;
	
	public float spawnTime;
	[Range(1, 20)]
	public int
		staggeredSpawn, multiSpawn;
	[Range(0f, Mathf.PI * 2f)]
	public float
		angularSeparation;
	public Vector3 gridSeparation;

	public SpaceObjectSpawnType spawnType;
	public MinMaxOffsets spawnOffsets;
	public Vector3 spawnAlignment;

	public SpaceObjectTrajectoryType trajectoryType;
	public Vector3 predefinedTrajectory;

	private float timer = 0f;
	private int totalSpawns = 0;

	/// <summary>
	/// Spawn the specified offset and baseAngle.
	/// </summary>
	/// <param name="offset">Offset. The index of the multispawn at which this spawn is.</param>
	/// <param name="baseAngle">Base angle. The angle all multispawn objects will be based on.</param>
	void Spawn (int offset, float baseAngle)
	{
		Vector3 oldPosition = transform.position;

		float corrector;
		Vector3 corrected = transform.position;
		if (spawnAlignment.x != 0f) {
			corrector = Mathf.Ceil (transform.position.x / spawnAlignment.x);
			corrected.x = corrector * spawnAlignment.x;
		}
		if (spawnAlignment.y != 0f) {
			corrector = Mathf.Ceil (transform.position.y / spawnAlignment.y);
			corrected.y = corrector * spawnAlignment.y;
		}
		if (spawnAlignment.z != 0f) {
			corrector = Mathf.Ceil (transform.position.z / spawnAlignment.z);
			corrected.z = corrector * spawnAlignment.z;
		}
		transform.position = corrected;

		// Get the angle at which to spawn
		int adapter = Mathf.FloorToInt ((offset + 1) / 2f) * Global.signs [offset % 2];
		float angle = (angularSeparation) * adapter + baseAngle;
		
		// Pre rotate so transform.up becomes usable
		transform.eulerAngles = new Vector3 (0f, 0f, (baseAngle + (Mathf.PI * 0.5f)) * Mathf.Rad2Deg);
		
		// Decide starting position
		// SpaceObjectSpawnType.Predefined not needed in switch
		switch (spawnType) {
		case SpaceObjectSpawnType.Parallel:
			transform.position = Screen.GetRadiusEdge (angle);
			break;
		}

		corrected = adapter * gridSeparation;
		transform.position += corrected;
		
		// Add random offset to start position
		transform.position = new Vector3 (transform.position.x - SeededRandom.Range (spawnOffsets.minOffset.x, spawnOffsets.maxOffset.x), 
		                                  transform.position.y - SeededRandom.Range (spawnOffsets.minOffset.y, spawnOffsets.maxOffset.y), 
		                                  transform.position.z - SeededRandom.Range (spawnOffsets.minOffset.z, spawnOffsets.maxOffset.z));
		
		Vector3 targetPoint = predefinedTrajectory; 
		// SpaceObjectTrajectoryType.Predefined no needed in switch
		switch (trajectoryType) {
		case SpaceObjectTrajectoryType.Targeted:
			// Pick a random target, if none exists, use predefined targetPoint
			GameObject target = ShipContainer.GetSpaceShip (SeededRandom.Range (0, 25));
			if (target != null) {
				targetPoint = target.transform.position;
			}
			break;

		case SpaceObjectTrajectoryType.TargetOpposite:
			// Set target to the opposite of the screen, multispawns will move parallel
			targetPoint = Screen.GetScreenEdge (transform.position, transform.up);
			break;

		case SpaceObjectTrajectoryType.TargetOppositeX:
			targetPoint = transform.position;
			targetPoint.x *= -1f;
			break;

		case SpaceObjectTrajectoryType.TargetOppositeY:
			targetPoint = transform.position;
			targetPoint.y *= -1f;
			break;

		case SpaceObjectTrajectoryType.TargetZero:
			targetPoint = Vector3.zero;
			break;
		}

		GameObject newObject = Instantiate (prefab) as GameObject;
		Movement movement = newObject.GetComponent<Movement> ();
		if (movement) {
			movement.SetTarget (targetPoint);
		}
		
		targetPoint -= transform.position;
		transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (targetPoint.y, targetPoint.x) * Mathf.Rad2Deg + 270f);
		
		newObject.transform.parent = transform.parent;
		newObject.transform.position = transform.position;
		newObject.transform.rotation = transform.rotation;

		transform.position = oldPosition;
	}
	
	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void FixedUpdate ()
	{
		if (timer > spawnTime) {
			Destroy (transform.gameObject);
		} else {
			timer += Time.fixedDeltaTime;
			float angle = SpaceObjectDirection.GetAngle ();
			int allowedSpawns;
			if (spawnTime != 0f) {
				allowedSpawns = Mathf.RoundToInt (timer * staggeredSpawn / spawnTime) - totalSpawns;
			} else {
				allowedSpawns = staggeredSpawn;
			}
			// Spawn for all multispawns
			for (int i = 0; i < allowedSpawns; i++) {
				for (int j = 0; j < multiSpawn; j++) {
					Spawn (j, angle);
				}
				totalSpawns += 1;
			}
		}
	}
}
