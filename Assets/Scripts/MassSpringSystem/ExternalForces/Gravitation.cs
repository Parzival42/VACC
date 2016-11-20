using UnityEngine;
using System.Collections;
using System;

public class Gravitation : MonoBehaviour, Force
{
    [SerializeField]
    [Range(0.0f,2.0f)]
    private float modifier = 1.0f;

    private Vector3 gravitation = new Vector3(0, -9.81f, 0);

    public Vector3 getForce()
    {
        return gravitation * modifier;
    }
}
