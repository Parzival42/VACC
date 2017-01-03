using UnityEngine;

/// <summary>
/// Forces the gameobject to sleep (physics) on start.
/// So it won't fall of the wall for example.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PhysicsSleepOnStart : MonoBehaviour
{
    private Rigidbody rigid;

	private void Start ()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.Sleep();
	}
}