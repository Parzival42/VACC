using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class Sucked : MonoBehaviour {

    public float speedCoefficient = 1f;
    public float maxSpeed = 10f;
    public Vector3 finalPosition;
    public Transform suckDirection;

    Vector3 direction;
    Rigidbody body;
    Material material;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
        direction = (finalPosition - transform.position).normalized;
        material = GetComponent<Renderer>().material;
    }
	
	// Update is called once per frame
	void Update () {

        material.SetVector("_MeltPosition", suckDirection.position);

        float distance = Vector3.Distance(finalPosition, transform.position);
        if (distance <= 0)
            Destroy(gameObject);
        float speed = Mathf.Clamp(distance, 0f, maxSpeed) * speedCoefficient;
        body.velocity = direction * speed;
    }
}
