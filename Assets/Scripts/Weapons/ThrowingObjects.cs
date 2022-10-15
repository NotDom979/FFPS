﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingObjects : MonoBehaviour
{
	[Header("Settings")]
	public Transform cam;
	public Transform attackPoint;
	public GameObject objectToThrow;
	
	[Header("Settings Part2")]
	public int totalThrows;
	public int throwCoolDown;
	public float throwForce;
	public float throwupwardForce;
	
	
	public KeyCode throwKey = KeyCode.L;
	
	bool readyTothrow;
	
	private void Start(){
		GameManager.instance.LethalCount.text = totalThrows.ToString("F0");
		readyTothrow = true;
		
	}
	
	private void Update(){
		
		if (Input.GetKeyDown(throwKey) && readyTothrow && totalThrows > 0	)
		{
			Throw();
		}
	}
	
	private void Throw()
	{
		
		readyTothrow = false;
		
		// We are instantiate object throw
		GameObject projectile = Instantiate(objectToThrow, attackPoint.position,cam.rotation);
		
		//we get rigidbody
		Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
		
		//caculate direction
		Vector3 forceDirection = cam.transform.forward;
		RaycastHit hit;
		
		if (Physics.Raycast(cam.position, cam.forward, out hit,100f))
		{
			forceDirection = (hit.point - attackPoint.position).normalized;
		}
		
		//Add Force
		Vector3 forceToAdd = forceDirection *throwForce + transform.up * throwupwardForce;
		
		projectileRB.AddForce(forceToAdd, ForceMode.Impulse);
		totalThrows--;
		GameManager.instance.LethalCount.text = totalThrows.ToString("F0");
		
		//cooldown
		Invoke(nameof(ResetThrow), throwCoolDown);
	}
	
	private void ResetThrow()
	{
		readyTothrow = true;
	}
}
