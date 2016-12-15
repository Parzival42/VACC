using UnityEngine;
using System.Collections;

public class TubeConnection : MonoBehaviour {

    private Transform transform1;

    [SerializeField]
    private Transform transform2;

    private bool connect = false;

	// Use this for initialization
	void Start () {

        transform1 = gameObject.transform;
        if(transform1!=null && transform2 != null)
        {
            connect = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (connect)
        {
            transform2.position = transform1.position;
        }
	}
}
