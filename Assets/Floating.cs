using UnityEngine;
using System;
using System.Collections;

public class Floating : MonoBehaviour {
	private float dx;
	private float dy; 
	private bool started = false;
	private DateTime lastTime; 
	private DateTime initTime;
	private float timeTilDeath = 30000;
	// Use this for initialization
	void Start () {
	}

	public void Init(float x, float y, float death){
		dx = x;
		dy = y;
		timeTilDeath = death;
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
		if ((DateTime.Now - initTime).TotalMilliseconds > timeTilDeath){
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
