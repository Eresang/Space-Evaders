using UnityEngine;
using System.Collections;

public class SelfDestructScript : MonoBehaviour {

	[Tooltip("How much time will pass before the game object will be destroyed")]
	[Range(0f, 15f)]
	[SerializeField]private float DestroyTime = 1f;

	private float timePassed;

	void FixedUpdate () {
		timePassed += Time.fixedDeltaTime;

		//destroys this gameobject when the time set is reached
		if(timePassed >= DestroyTime){
			Destroy(this.gameObject);
		}
	}
}
