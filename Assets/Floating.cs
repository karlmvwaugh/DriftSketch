using UnityEngine;
using System;
using System.Collections;

public class Floating : MonoBehaviour {
	private float dx;
	private float dy; 
	private bool started = false;
	private DateTime lastTime; 
	private DateTime initTime;
	// Use this for initialization
	void Start () {
	}

	public void Init(float x, float y){
		dx = x;
		dy = y;
		lastTime = DateTime.Now;
		initTime = DateTime.Now;
		started = true;
	}


	// Update is called once per frame
	void Update () {
		if (!started) return; 

		var difference = GetDifference();
		updatePosition(difference);



		toKillOrNotToKill();
	}

	void toKillOrNotToKill(){
		if ((DateTime.Now - initTime).TotalMilliseconds > 30000){
			Destroy (this.gameObject); 
		}
	}


	void updatePosition(float timePassed){
		var old = transform.position;
		transform.position = new Vector3(old.x + dx*timePassed/1000f, old.y + dy*timePassed/1000f, old.z);
	}

	float GetDifference(){
		var now = DateTime.Now;
		var difference = (float)(now - lastTime).TotalMilliseconds;
		lastTime = now;
		return difference;
	}
}
