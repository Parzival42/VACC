using UnityEngine;
using System.Collections;


class VacuumCollisionSounds : MonoBehaviour
{
    void OnCollisionStay(Collision col)
    {
        float magnitude = col.relativeVelocity.magnitude;
    }
}

