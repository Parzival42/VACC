using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Tag for the collision checks")]
    private string tagString;

    public delegate void TriggerEntered();
    public delegate void TriggerExited();

    public event TriggerEntered OnTriggerEntered;
    public event TriggerExited OnTriggerExited;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == tagString)
        {
            if (OnTriggerEntered != null)
                OnTriggerEntered();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == tagString)
        {
            if (OnTriggerExited != null)
                OnTriggerExited();
        }
    }

    void OnDestroy(){
        OnTriggerEntered = null;
        OnTriggerExited = null;
    }
}
