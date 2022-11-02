﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowingObjects : MonoBehaviour
{
	
	[Header("Settings")]
	
	public Transform attackPoint;
	public GameObject objectToThrow;
	
	[Header("Settings")]
	public int totalThrows;
	public int throwCoolDown;
	public float throwForce;
	public float throwupwardForce;
	
	
	public KeyCode throwKey = KeyCode.G;
	
	bool readyTothrow;
	
	private void Start(){
		totalThrows++;
		GameManager.instance.LethalCount.text = totalThrows.ToString("F0");
		readyTothrow = true;
	}
	
	private void Update(){
		
		if (totalThrows > 0	)
		{
			if (readyTothrow == true)
            {
                if (Input.GetKeyDown(throwKey))
                {
					Throw();
                }
            }
		}
	}
	
	private void Throw()
	{
		
		readyTothrow = false;
		
		// We are instantiate object throw
		GameObject projectile = Instantiate(objectToThrow, attackPoint.position,transform.rotation);
		
		//we get rigidbody
		Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
		
		//caculate direction
		Vector3 forceDirection = transform.forward;
		
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
