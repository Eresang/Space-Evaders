using UnityEngine;
using System.Collections;

public class BlackHoleBehaviourScript : MonoBehaviour
{
	public Material material;
	private float timer = 0f;
	public float range, fade;
	public ParticleSystem ps;
	public float lifeTime = 10f;
	public float fadeTime = 1f;
	public GameObject parent;

	void Start ()
	{
		Material m = new Material (material);
		material = m;
		GetComponent<MeshRenderer> ().material = m;
	}

	void Update ()
	{
		timer += Time.deltaTime;
		if (timer < fadeTime) {
			ps.emissionRate = timer * lifeTime;
			material.SetFloat ("_Range", Mathf.Lerp (0, range, timer * timer));
			material.SetFloat ("_Fade", Mathf.Lerp (0, fade, timer * timer));
		}
		if (timer >= lifeTime && timer < lifeTime + fadeTime) {
			ps.emissionRate = 0;
			material.SetFloat ("_Range", Mathf.Lerp (0, range, Mathf.Pow (Mathf.Abs (timer - (lifeTime + fadeTime)), 2)));
			material.SetFloat ("_Fade", Mathf.Lerp (0, fade, Mathf.Pow (Mathf.Abs (timer - (lifeTime + fadeTime)), 2)));
		}
		//transform.localEulerAngles += new Vector3 (90 * Time.deltaTime, 0, 0);
		transform.Rotate (0, 90 * Time.deltaTime, 0);
		material.SetVector ("_ObjectPosition", new Vector4 (transform.position.x, transform.position.y, transform.position.z, 1));

		if (timer > lifeTime + fadeTime) {
			Destroy (parent);
		}
	}
}
