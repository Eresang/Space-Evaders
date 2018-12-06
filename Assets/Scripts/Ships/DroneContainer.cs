using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DroneContainer : BaseUpgrade
{
	private int maxDrones = 3;
	private int lastCount = 0;
	public GameObject dronePrefab;
	public GameObject ignoreCollider;

	public GameObject[] brachia = new GameObject [6];
	private GameObject[] drones = new GameObject [6];

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			Upgrade ();
		}
	}

	public override void Upgrade ()
	{
		AddDrone ();
	}

	public override int UpgradeCount ()
	{
		int r = 0;
		for (int i = 0; i < drones.Length; i++) {
			if (drones [i] != null) {
				r++;
			}
		}
		return r;
	}

	public override int MaxUpgrades ()
	{
		return maxDrones;
	}

	public override string GetUpgradeText ()
	{
		return MaxUpgrades () == UpgradeCount () ? "Fully upgraded" : "Buy drone";
	}

	public void ReReposition ()
	{
		for (int i = 0; i < drones.Length; i++) {
			if (drones [i] != null) {
				drones [i].transform.position = brachia [i].transform.position;
			}
		}
	}

	public void AddDrone ()
	{
		if (UpgradeCount () < maxDrones && (dronePrefab)) {
			GameObject newDrone = GameObject.Instantiate (dronePrefab) as GameObject;
			newDrone.transform.parent = ShipContainer.lastContainer.transform;
			DroneControl drone = newDrone.GetComponent<DroneControl> ();
			drone.container = this;
			if (UpgradeCount () == 0) {
				drones [0] = newDrone;
			} else {
				drones [Find (null)] = newDrone;
			}
			RepositionDrones ();
			SpawnAll ();
			drone.Spawn (brachia [Find (newDrone)]);
		}
		lastCount = UpgradeCount ();
	}

	int FindOpenSpot ()
	{
		int i = FindFirstNonNull ();


		return i;
	}

	int Find (GameObject query)
	{
		int i = 0;
		while (drones[i] != query && i < drones.Length) {
			i++;
		}
		if (drones [i] != query) {
			i = -1;
		}
		return i;
	}

	int FindFirstNonNull ()
	{
		int i = 0;
		while (drones[i] == null && i < drones.Length) {
			i++;
		}
		if (drones [i] == null) {
			i = -1;
		}
		return i;
	}

	int FindFirstAfter (int index)
	{
		int i = index + 1;
		while (i < drones.Length && drones[i] == null) {
			i++;
		}
		if (i >= drones.Length) {
			i = -1;
		}
		return i;
	}
	
	GameObject FindClosest (Vector3 closestTo)
	{
		GameObject r = null;
		float d = 0f;
		for (int i = 0; i < drones.Length; i++) {
			if ((drones [i] != null) && ((r == null) || (d > (closestTo - drones [i].transform.position).sqrMagnitude))) {
				r = drones [i];
				d = (closestTo - drones [i].transform.position).sqrMagnitude;

			}
		}
		return r;
	}

	void AttachAll ()
	{
		for (int i = 0; i < drones.Length; i++) {
			if (drones [i] != null) {
				drones [i].GetComponent<DroneControl> ().Attach (brachia [i]);
			}
		}
	}

	void SpawnAll ()
	{
		for (int i = 0; i < drones.Length; i++) {
			if (drones [i] != null) {
				drones [i].transform.position = brachia [i].transform.position;
			}
		}
	}

	void RepositionDrones ()
	{
		int c = UpgradeCount ();
		if (UpgradeCount () > 1) {
			int spacing = Mathf.FloorToInt ((drones.Length - c) / c) + 1;
			int start = FindFirstNonNull ();
			int index = FindFirstAfter (-1);
			GameObject[] n = new GameObject[6];
			while (index != -1) {
				n [start] = drones [index];
				start = (start + spacing) % 6;
				index = FindFirstAfter (index);
			}
			drones = n;
			AttachAll ();
		}
	}

	public void RemoveDrone (GameObject drone)
	{
		for (int i = 0; i < drones.Length; i++) {
			if (drones [i] == drone) {
				drones [i] = null;
			}
		}
		lastCount = UpgradeCount ();
		if (lastCount > 0) {
			RepositionDrones ();
		}
	}

	void FixedUpdate ()
	{
		if (lastCount != UpgradeCount ()) {
			RepositionDrones ();
		}
	}

	void OnDestroy ()
	{
		for (int i = 0; i < drones.Length; i++) {
			if (drones [i] != null) {
				Destroy (drones [i]);
			}
		}
	}
}
