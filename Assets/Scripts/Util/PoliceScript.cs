using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceScript : MonoBehaviour {

    public bool active = true;

    public GameObject light1;
    public GameObject light2;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (true)
        {
            Vector3 newPosition = this.transform.position;
            newPosition.z -= Time.deltaTime * 6;
            this.transform.position = newPosition;
        }
	}
}
