using System.Collections;
using UnityEngine;

public class CarLightStarter : MonoBehaviour
{
	private void Start ()
    {
        StartCoroutine(PlayCarLight());
	}

    private IEnumerator PlayCarLight()
    {
        yield return new WaitForSeconds(10f);
        AnimationManager.Instance.PlayCarLight();
    }
}