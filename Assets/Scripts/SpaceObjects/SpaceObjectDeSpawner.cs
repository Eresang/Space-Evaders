using UnityEngine;
using System.Collections;

public class SpaceObjectDeSpawner : MonoBehaviour
{
	public GameObject despawnPrefab;
	public bool spawnScrap = false;

	public void DeSpawn ()
	{
		if ((spawnScrap) && (despawnPrefab)) {
			(Instantiate (despawnPrefab, transform.position, transform.rotation) as GameObject).transform.parent = transform.parent;
			Instantiate (Global.defaultConversion, transform.position, transform.rotation);
		}
		Destroy (this.gameObject);
	}
}
