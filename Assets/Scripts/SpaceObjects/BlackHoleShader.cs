using UnityEngine;
using System.Collections;

public class BlackHoleShader : MonoBehaviour
{

	[SerializeField]
	Material
		material;
	
	void Update ()
	{
		material.SetVector ("_ObjectPosition", new Vector4 (transform.position.x, transform.position.y, transform.position.z, 1f));
	}
}
