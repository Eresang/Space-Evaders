using UnityEngine;
using System.Collections;

public class ParticleHandleScript : MonoBehaviour {

	float emissionRate = 20;
	private ParticleSystem thisSystem;
	[SerializeField] private Movement parentsMovement;

	void Awake(){
		thisSystem = GetComponent<ParticleSystem>();
	}

	void LateUpdate(){
		if (parentsMovement.stopOnTarget && parentsMovement.TargetReached () && emissionRate != 0f){
			emissionRate = 0f;
		} else if (parentsMovement.stopOnTarget && !parentsMovement.TargetReached () && emissionRate == 0){
			emissionRate = 20;
		}
		if (thisSystem.emissionRate != emissionRate){
			thisSystem.emissionRate = emissionRate;
		}
	}
}
