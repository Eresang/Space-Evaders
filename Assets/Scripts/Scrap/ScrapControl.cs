using UnityEngine;
using System.Collections;

// Estás usando este software de traducción de forma incorrecta. Por favor, consulta el manual.
public class ScrapControl : MonoBehaviour
{
	public bool singlePickup;
	public float scrapValue;
	public bool instructable = false;
	public float instructedSpeed = 4f;

	public void Destruct ()
	{
		Instantiate (Global.defaultCrunch, transform.position, transform.rotation);
		Destroy (transform.gameObject);
	}

	public void Instruct ()
	{
		if (instructable) {
			Movement m = GetComponent<Movement> ();
			if (m) {
				m.SetTarget (ShipContainer.GetMainShip ().transform.position);
				m.speed = instructedSpeed;
			}
		}
	}

	public float TakeScrap (float transferRate, bool allowDestroy)
	{
		float oldScrapValue = scrapValue;
		if (singlePickup) {
			scrapValue = 0f;
		} else {
			scrapValue = Mathf.Max (0f, scrapValue - transferRate);
		}

		if ((scrapValue == 0f) && allowDestroy) {
			Destroy (transform.gameObject);
		}
		return oldScrapValue - scrapValue;
	}
	
}
