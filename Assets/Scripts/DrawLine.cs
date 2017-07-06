using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour {
	
	public List<Transform > waypoints = new List <Transform > ();
	public float rate = 1;
	public int currentWaypoint = 1;
	public void MoveToWaypoint(){
		print ("public void MoveToWaypoint():" + waypoints.Count);
		StartCoroutine ("move");
	}
	public void ClearPath(){
		waypoints.Clear ();
		//print ("path.Clear();");
	}
	IEnumerator move(){
		for (int i = 0;i < waypoints .Count ;i++){
			iTween.MoveTo (this.gameObject, waypoints [i].position, rate);
			//print ("new id:" + i);
			yield return new WaitForSeconds (rate);
		}
		waypoints.Clear ();
	}
}
