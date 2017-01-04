using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour {

    [Range(1, 600)]
    public float speed = 220f;

	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }
}
