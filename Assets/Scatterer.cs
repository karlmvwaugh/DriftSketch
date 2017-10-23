using UnityEngine;
using System;
using System.Collections;

public class Scatterer : MonoBehaviour {
	private Oscillator x1;
	private Oscillator y1;
	// Use this for initialization
	void Start () {	
		x1 = new Oscillator(){
			max = 0.15f,
			min = -0.15f,
			speed = 0.5f
		};

		y1 = new Oscillator(){
			max = 0.15f,
			min = -0.15f,
			speed = 0.45f
		};
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public float getX(){
		return x1.GetValue() * 0.5f;
	}

	public float getY(){
		return y1.GetValue() * 0.5f;
	}
}


public class Oscillator {
	public float max;
	public float min;
	public float speed;
	
	private float theta;
	private DateTime _lastTime;
	
	private float deltaMultiplier = 0.001f;
	// Use this for initialization
	public Oscillator () {
		theta = 0f;
		_lastTime = DateTime.Now;
	}
	
	public float GetValue(){
		Update ();
		return (Mathf.Sin(theta)*(max - min) + max + min ) / 2f;
	}
	
	// Update is called once per frame
	void Update () {
		var delta = GetDelta();
		theta += (delta * speed * deltaMultiplier);
		
		_lastTime = DateTime.Now;
	}
	
	float GetDelta(){
		var now = DateTime.Now;
		return (float)(now - _lastTime).TotalMilliseconds;
	}
}
