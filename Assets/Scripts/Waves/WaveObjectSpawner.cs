using UnityEngine;
using System.Collections;

/// <summary>
/// Spawn type.
/// </summary>
public enum SpawnType
{
	FixedSpawnTime,
	SpacedSpawnTime
}

/// <summary>
/// Angle of entry type.
/// </summary>
public enum AngleOfEntryType
{
	FixedPosition,
	RandomPosition,
	FixedAngle,
	WaveDirection,
	RandomAngle
}

/// <summary>
/// Target type.
/// </summary>
public enum TargetType
{
	FixedTarget,
	Opposite,
	HorizontalOpposite,
	VerticalOpposite
}

/// <summary>
/// Wave object spawner.
/// </summary>
public class WaveObjectSpawner : MonoBehaviour
{
	// ---------------------------------------------------------------
	// ---[               User definable variables                ]---
	// ---------------------------------------------------------------
	[Tooltip("The spawner will create (multiple of) this object at every spawn opportunity")]
	public GameObject
		objectPrefab;

	// ---[ Multispawn ]---

	[Header("Multispawn Settings")]
	[Tooltip("Number of bundled spawn opportunities")]
	[Range(1, 20)]
	public int
		staggeredSpawns = 1;

	[Tooltip("Minimum number of spawns per bundled spawn")]
	[Range(1, 20)]
	public int
		minMultiSpawns = 1;
	[Tooltip("Maximum number of spawns per bundled spawn")]
	[Range(1, 20)]
	public int
		maxMultiSpawns = 1;

	[Tooltip("All possible spawns from this spawner will occur within this timespan")]
	public float
		spawnDuration;

	[Tooltip("Angular distance between every consecutive multispawn spawn\n" +
			 "[0, pi]")]
	[Range(0f, Mathf.PI)]
	public float
		angularSeparation;

	// ---[ Start ]---

	[Header("Start Position")]
	[Tooltip("All of angle of entry types use offset variables\n\n"+
	         "[FixedAngle] use angle of entry variable\n\n" +
			 "[WaveDirection] use current wave direction\n\n" +
			 "[RandomAngle] randomly choose an angle")]
	public AngleOfEntryType
		angleOfEntryType;

	[Tooltip("Start position for use when Angle Of Entry Type is set to FixedPosition")]
	public Vector3
		fixedPosition;

	[Tooltip("Angle of entry of the object, in radians\n" +
			 "[0, pi*2]")]
	[Range(0f, Mathf.PI * 2f)]
	public float
		angleOfEntry;

	[Tooltip("Minimum offset from angle of entry\n" +
			 "[0, pi*2]")]
	[Range(0f, Mathf.PI * 2f)]
	public float
		minAngleOfEntryOffset;

	[Tooltip("Maximum offset from angle of entry\n" +
			 "[0, pi*2]")]
	[Range(0f, Mathf.PI * 2f)]
	public float
		maxAngleOfEntryOffset;

	// ---[ Target ]---

	[Header("Target Position")]
	[Tooltip("All of target types use offset variables\n\n"+
	         "[FixedTarget] object will move towards the target position variable\n\n" +
	         "[Opposite] object will move towards its mirrored coordinates\n\n" +
	         "[HorizontalOpposite] object will move towards the vertical edge of the screen that's the furthest away\n\n" +
	         "[VerticalOpposite] object will move towards the horizontal edge of the screen that's the furthest away")]
	public TargetType
		targetType = TargetType.Opposite;
	
	[Tooltip("Target position for use when Target Type is set to FixedTarget")]
	public Vector3
		targetPosition;

	[Tooltip("If true all multispawns will move parallel to each other, but uses individual offset variables as well")]
	public bool
		parallel = true;

	[Tooltip("Minimum offset from direction to target\n" +
			 "[0, pi/2]")]
	[Range(0f, Mathf.PI /2f)]
	public float
		minTargetOffset;
	
	[Tooltip("Maximum offset from direction to target\n" +
			 "[0, pi/2]")]
	[Range(0f, Mathf.PI / 2f)]
	public float
		maxTargetOffset;

	// ---------------------------------------------------------------
	// ---[                  Internal variables                   ]---
	// ---------------------------------------------------------------

	// Keep track of alive time
	private float timer = 0f;
	// Time between staggers
	private float staggerTime = 0f;
	// The upcoming stagger index
	private int currentStagger = 0;
	
	// ---------------------------------------------------------------
	// ---[                Functions & procedures                 ]---
	// ---------------------------------------------------------------

	/// <summary>
	/// Spawns an object.
	/// </summary>
	void Spawn (Vector3 position, float targetAngle)
	{
		GameObject newObject = Instantiate (objectPrefab) as GameObject;

		transform.eulerAngles = new Vector3 (0f, 0f, targetAngle * Mathf.Rad2Deg);
		
		newObject.transform.parent = transform.parent;
		newObject.transform.rotation = transform.rotation;
		newObject.transform.position = position;

		Movement movement = newObject.GetComponent<Movement> ();
		if (movement) {
			movement.SetTarget (newObject.transform.up * Screen.GetDiameter () + position);
		}
	}

	/// <summary>
	/// Gets the target angle. Multispawn helper.
	/// </summary>
	/// <returns>The target angle.</returns>
	/// <param name="start">Start.</param>
	/// <param name="target">Target.</param>
	float GetTargetAngle (Vector3 start, Vector3 target)
	{
		Vector3 targetPoint = target - start;
		return Mathf.Atan2 (targetPoint.y, targetPoint.x) + (Mathf.PI * 1.5f);
	}

	/// <summary>
	/// Decides the target. Multispawn helper.
	/// </summary>
	/// <returns>The target.</returns>
	Vector3 DecideTarget (Vector3 start)
	{
		Vector3 target = targetPosition;
		switch (targetType) {
		// TargetType.FixedTarget not needed
		case TargetType.Opposite:
			target = start * -1f;
			break;
			
		case TargetType.HorizontalOpposite:
			target = start;
			target.x *= -1f;
			break;
			
		case TargetType.VerticalOpposite:
			target = start;
			target.y *= -1f;
			break;
		}
		return target;
	}

	/// <summary>
	/// Spawns object(s).
	/// </summary>
	void MultiSpawn ()
	{
		// Get base range
		float min = Mathf.Abs (maxAngleOfEntryOffset - minAngleOfEntryOffset) / 2f;
		float max = -min;

		// Get angular distance between object spawns
		float separation = Mathf.Abs (maxAngleOfEntryOffset - minAngleOfEntryOffset) + angularSeparation;

		// Get starting angle
		float baseAngle = angleOfEntry;

		switch (angleOfEntryType) {
		// AngleOfEntryType.FixedAngle not needed
		case AngleOfEntryType.WaveDirection:
			baseAngle = SpaceObjectDirection.GetAngle ();
			break;
			
		case AngleOfEntryType.RandomAngle:
			baseAngle = SeededRandom.Range (0f, Mathf.PI * 2f);
			break;
		}

		baseAngle += (Mathf.Abs (minAngleOfEntryOffset) + max) * SeededRandom.Sign ();
		float angle = baseAngle + SeededRandom.Range (min, max);

		// We have our start position
		Vector3 start = Screen.GetRadiusEdge (angle);
		if (angleOfEntryType == AngleOfEntryType.FixedPosition) {
			start = fixedPosition;
		} else if (angleOfEntryType == AngleOfEntryType.RandomPosition) {
			start = Screen.RandomPositionInView (4f);
		}

		// Decide on target
		Vector3 target = DecideTarget (start);
		float baseTargetAngle = GetTargetAngle (start, target);
		float targetAngle = SeededRandom.Range (minTargetOffset, maxTargetOffset) * SeededRandom.Sign () + baseTargetAngle;

		// Always spawn 1
		Spawn (start, targetAngle);

		// Loop through all spawns
		for (int i = 1; i < SeededRandom.Range(minMultiSpawns, maxMultiSpawns + 1); i++) {
			// Get positional modifier
			int modifier = (i + 1) / 2;
			if (i % 2 == 0) {
				modifier *= -1;
			}
			// Base angle for current spawn
			angle = (separation * modifier) + baseAngle + SeededRandom.Range (min, max);
			start = Screen.GetRadiusEdge (angle);

			// Do we need to pick another base angle?
			if (!parallel) {
				target = DecideTarget (start);
				baseTargetAngle = GetTargetAngle (start, target);
			}

			targetAngle = SeededRandom.Range (minTargetOffset, maxTargetOffset) * SeededRandom.Sign () + baseTargetAngle;

			Spawn (start, targetAngle);
		}
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		if (staggeredSpawns < 2) {
			staggerTime = spawnDuration / 2f;
			currentStagger = 1;
			staggeredSpawns = 2;
		} else {
			staggerTime = spawnDuration / (staggeredSpawns - 1);
		}
	}

	/// <summary>
	/// Performed for every physics update.
	/// </summary>
	void FixedUpdate ()
	{
		if (!Global.paused) {
			timer += Time.fixedDeltaTime;
			while (timer > staggerTime * currentStagger && currentStagger <= staggeredSpawns) {
				if (minMultiSpawns > 0) {
					MultiSpawn ();
				}
				currentStagger++;
			}
			if (currentStagger >= staggeredSpawns) {
				Destroy (this.gameObject);
			}
		}
	}
}