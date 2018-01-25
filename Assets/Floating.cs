using UnityEngine;
using System;
using System.Collections;


public class Floating : MonoBehaviour {
	private float dx;
	private float dy; 
	private bool started = false;
	private DateTime lastTime; 
	private DateTime countDownTime;
	private bool countDownBegun = false;
	private float timeTilDeath = 30000;
	// Use this for initialization
	void Start () {
	}

	public void Init(float x, float y, float death, float timeDiff){
		dx = x;
		dy = y;
		timeTilDeath = death + timeDiff;
		var rot = randomRotation();
		transform.Rotate(Vector3.forward * rot); 
		lastTime = DateTime.Now;
		started = true;
	}

	public void StartCountDown(){
		countDownBegun = true;
		countDownTime = DateTime.Now;
	}

	// Update is called once per frame
	void Update () {
		if (!started) return; 

		var difference = GetDifference();
		updatePosition(difference);



		toKillOrNotToKill();
	}

	void toKillOrNotToKill(){
		if (countDownBegun && (DateTime.Now - countDownTime).TotalMilliseconds > timeTilDeath){
			Destroy (this.gameObject); 
		}
	}

	private int randomRotation(){
		var r = new System.Random();
		return r.Next(0, 360);
	}

	void updatePosition(float timePassed){
		var old = transform.position;
		transform.position = new Vector3(newX(old, timePassed), newY (old, timePassed), old.z);
	}

	float newX(Vector3 old, float timePassed){
		var newX = old.x + dx*timePassed/1000f;

		newX = wrapAroundAt(newX, 12.5f);

		return newX;
	}

	float newY(Vector3 old, float timePassed){
		var newY = old.y + dy*timePassed/1000f;
		newY = wrapAroundAt(newY, 6.25f);
		return newY;
	}

	float wrapAroundAt(float inValue, float wrapValue){
		if (inValue >= wrapValue){
			inValue = -1f*wrapValue + (inValue - wrapValue);
		}
		
		if (inValue <= -1f*wrapValue){
			inValue = wrapValue + (inValue + wrapValue);
		}

		return inValue;
	}

	float GetDifference(){
		var now = DateTime.Now;
		var difference = (float)(now - lastTime).TotalMilliseconds;
		lastTime = now;
		return difference;
	}
}
