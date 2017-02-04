using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowManager : MonoBehaviour {

    #region variables
    [SerializeField]
    private Rigidbody[] books;

    [SerializeField]
    private Rigidbody nosBottle;

    [SerializeField]
    private Vector3 ejectDirection;

    [SerializeField]
    private float forceMultiplier = 1.0f;

    private bool booksEjectable = true;

    private bool nosEjectable = true;
    #endregion

    #region variables
    public void EjectBooks()
    {
        if (booksEjectable)
        {
            booksEjectable = !booksEjectable;
            for(int i = 0; i < books.Length; i++)
            {
                books[i].AddForce(ejectDirection.normalized * forceMultiplier * Random.Range(1.0f, 1.4f), ForceMode.Impulse);
            }
        }
    }

    public void EjectNOS()
    {
        if (nosEjectable)
        {
            nosEjectable = !nosEjectable;
            
            nosBottle.isKinematic = false;
            nosBottle.AddForce(ejectDirection.normalized * forceMultiplier, ForceMode.Impulse);
            StartCoroutine(ActivateCollider());
        }
    }

    private IEnumerator ActivateCollider()
    {
        yield return new WaitForSeconds(0.05f);
        nosBottle.gameObject.GetComponent<Collider>().enabled = true;
    }

    #endregion
}
