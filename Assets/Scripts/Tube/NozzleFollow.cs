﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NozzleFollow : MonoBehaviour {


    [SerializeField]
    private Transform target;

    [SerializeField]
    private float minDistance = 1.0f;

    [SerializeField]
    private float maxDistance = 3.0f;

    [SerializeField]
    private float forceMultiplier = 1.0f;

    [SerializeField]
    private float turnSpeed = 2.0f;

    [SerializeField]
    private float brakeForce = 0.9f;

    private Vector3 direction = new Vector3();
    private float distance = 0.0f;

    private Vector3 lookAtVector = new Vector3();
	
	// Update is called once per frame
	void Update () {
        distance = Vector3.Distance(this.transform.position, target.position);
        direction = target.position - this.transform.position;
        lookAtVector.Set(target.position.x, transform.position.y, target.position.z);

        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed  );

        if ( distance > minDistance && distance < maxDistance)
        {
            


        }else if(distance < minDistance)
        {
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.velocity = rigid.velocity * 0.9f;

        }
        else if(distance > maxDistance)
        {
            //transform.LookAt(lookAtVector);
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.AddForce(direction.normalized * forceMultiplier);

          
        }


	}




    //private void RotationTween()
}