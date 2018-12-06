using UnityEngine;
using System.Collections;

public class ShopSpawner : MonoBehaviour
{
	public GameObject shopPrefab;
	private GameObject shop;
	// Use this for initialization
	void Awake ()
	{
		if (shopPrefab) {
			shop = Instantiate (shopPrefab) as GameObject;
			shop.transform.SetParent (Camera.main.transform, false);
		}
	}

	void Update ()
	{
		if (!(shop)) {
			Destroy (this.gameObject);
		}
	}
}
