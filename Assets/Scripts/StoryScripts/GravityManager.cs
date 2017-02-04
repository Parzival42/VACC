using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour {

    #region variables
    [SerializeField]
    private List<Rigidbody> affectedRigidbodies;

    [SerializeField]
    private float duration = 5.0f;

    [SerializeField]
    private float forceMultiplier = 1.0f;

    [SerializeField]
    private float drag = 0.02f;

    [SerializeField]
    private float torqueMultiplier = 1.0f;

    [SerializeField]
    private float angularDrag = 0.02f;

    [SerializeField]
    private bool test = false;

    [SerializeField]
    private bool activateable = true;

    [SerializeField]
    private float maxMass = 1.0f;

    private List<float> rigidBodyMasses;
    #endregion

    #region methods
    // Use this for initialization
    void Start () {
        affectedRigidbodies = new List<Rigidbody>();
        rigidBodyMasses = new List<float>();
        for (int i = 0; i < Enum.GetNames(typeof(NOS)).Length; i++)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Stage"+(i+1));
            for (int j = 0; j < gameObjects.Length; j++)
            {
                affectedRigidbodies.Add(gameObjects[j].GetComponent<Rigidbody>());
                if (affectedRigidbodies[affectedRigidbodies.Count - 1] != null)
                {
                    rigidBodyMasses.Add(affectedRigidbodies[affectedRigidbodies.Count - 1].mass);
                }else
                {
                    rigidBodyMasses.Add(0.0f);
                }
            }
        }
    }
	
    void Update()
    {
        if (test)
        {
            test = false;
            if (activateable)
            {
                StartCoroutine(GravityFalls(duration));
            }
        }
    }


    public void StabGravityInTheBack(float duration)
    {
        StartCoroutine(GravityFalls(duration));
    }

    private IEnumerator GravityFalls(float duration)
    {
        activateable = false;
        ReverseGravity();
        yield return new WaitForSeconds(duration);
        AntiReverseGravity();
        activateable = true;
    }

    private void ReverseGravity()
    {
        for(int i = 0; i < affectedRigidbodies.Count; i++)
        {
            if(affectedRigidbodies[i] != null)
            {
                affectedRigidbodies[i].useGravity = false;
               // affectedRigidbodies[i].isKinematic = false;
                if(affectedRigidbodies[i].mass > maxMass)
                {
                    affectedRigidbodies[i].mass = maxMass;
                }
                affectedRigidbodies[i].AddForce(Vector3.up * forceMultiplier);
                affectedRigidbodies[i].AddTorque(new Vector3(UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)).normalized * torqueMultiplier);
            }
        }
    }

    private void AntiReverseGravity()
    {
        for (int i = 0; i < affectedRigidbodies.Count; i++)
        {
            if (affectedRigidbodies[i] != null)
            {
                affectedRigidbodies[i].mass = rigidBodyMasses[i];
                affectedRigidbodies[i].useGravity = true;
            }
        }
    }


    #endregion
}
