using UnityEngine;
using System.Collections;

public class Sucked : MonoBehaviour {

    public float speedCoefficient = 1f;
    public float maxSpeed = 10f;
    public Vector3 finalPosition;

    Vector3 direction;
    Rigidbody body;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
        direction = (finalPosition - transform.position).normalized;
    }
	
	// Update is called once per frame
	void Update () {
        float distance = Vector3.Distance(finalPosition, transform.position);
        if (distance <= 0)
            Destroy(gameObject);
        float speed = Mathf.Clamp(distance * speedCoefficient, 0f, maxSpeed);
        body.velocity = direction * speed;
    }
}
