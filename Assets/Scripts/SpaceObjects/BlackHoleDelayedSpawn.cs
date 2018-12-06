using UnityEngine;
using System.Collections;

public class BlackHoleDelayedSpawn : MonoBehaviour
{
	public GameObject blackHolePrefab;
	public float fadeTime;
	private Renderer r;
	private float timer;

	void Start ()
	{
		r = GetComponentInChildren<Renderer> ();
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (timer < fadeTime) {
			timer += Time.fixedDeltaTime;
			Color c = r.material.color;
			c.a = timer / fadeTime;
		} else {
			if (blackHolePrefab) {
				Instantiate (blackHolePrefab, transform.position, transform.rotation);
			}
			Destroy (this.gameObject);
		}
	}
}
