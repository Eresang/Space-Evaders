using UnityEngine;
using System.Collections;

public class EndShop : MonoBehaviour
{
	public GameObject shop;
	
	public void ExitShop ()
	{
		Destroy (shop);
	}
}
