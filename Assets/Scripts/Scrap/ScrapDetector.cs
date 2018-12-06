using UnityEngine;
using System.Collections;

public class ScrapDetector : MonoBehaviour
{
	public float transferSpeed;
	public float animationSpeed;
	
	private GameObject locatedScrap;
	private LineRenderer line;
	private Material material;
	private ScrapControl scrap;
	
	private float timer;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		line = GetComponent<LineRenderer> ();
		material = line.material;
	}
	
	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void FixedUpdate ()
	{
		if ((locatedScrap) || (timer > 0f)) {
			timer += Time.fixedDeltaTime;
			if (timer > animationSpeed) {
				timer = 0f;
			}

			if (locatedScrap) {
				line.SetPosition (1, locatedScrap.transform.position);
				line.SetPosition (0, transform.position);
				
				Highscore.AddScore (scrap.TakeScrap (transferSpeed * Time.fixedDeltaTime, timer == 0f));
			}
			
			material.mainTextureOffset = new Vector2 (-1f + (2f * timer / animationSpeed), 0f);
		}
	}
	
	/// <summary>
	/// Raises the trigger enter2d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter2D (Collider2D other)
	{
		if (((locatedScrap != other.gameObject) && ((other.gameObject.tag == "Scrap") || (other.gameObject.tag == "Satellite"))) && ((locatedScrap == null) ||
			((locatedScrap != null) && ((locatedScrap.transform.position - transform.position).sqrMagnitude > (other.gameObject.transform.position - transform.position).sqrMagnitude)))) {
			if (other.gameObject.tag == "Scrap") {
				locatedScrap = other.gameObject;
			} else {
				locatedScrap = other.transform.parent.gameObject;
			}
			scrap = locatedScrap.GetComponent<ScrapControl> ();
			if (!(scrap)) {
				locatedScrap = null;
			}

			if (locatedScrap) {
				line.SetVertexCount (2);
				line.SetPosition (1, locatedScrap.transform.position);
				line.SetPosition (0, transform.position);
			}
		}
	}

	/// <summary>
	/// Raises the trigger stay2d event.
	/// </summary>
	void OnTriggerStay2D (Collider2D other)
	{
		OnTriggerEnter2D (other);
	}

	/// <summary>
	/// Raises the trigger exit2d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject == locatedScrap) {
			locatedScrap = null;
			line.SetVertexCount (0);
		}
	}
}
