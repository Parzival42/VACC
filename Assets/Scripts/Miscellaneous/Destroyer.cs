using UnityEngine;

/// <summary>
/// Helping destroy util for animations.
/// </summary>
public class Destroyer : MonoBehaviour
{
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}