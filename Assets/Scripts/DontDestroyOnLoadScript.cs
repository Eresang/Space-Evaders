using UnityEngine;
using System.Collections;

public class DontDestroyOnLoadScript : MonoBehaviour
{

	private  static Hashtable h = new Hashtable ();

	/// <summary>
	/// creates a reference to this object
	/// </summary>
	void Awake ()
	{
		if (h.Contains (this.gameObject.name)) {
			Destroy (this.gameObject);
		} else {
			h.Add (this.gameObject.name, 0);
		}
		DontDestroyOnLoad (this.gameObject);
	}
}
