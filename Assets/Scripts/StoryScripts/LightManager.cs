using UnityEngine;
using System.Collections;

public class LightManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] lightObjects;

    [SerializeField]
    private ReflectionProbe reflectionProbe;

	// Use this for initialization
	void Awake () {
        MainKillSwitch.MainPowerChanged += OnMainPowerChanged;		
	}

    private void OnMainPowerChanged(bool powerOn)
    {
        for(int i = 0; i < lightObjects.Length; i++)
        {
            lightObjects[i].SetActive(powerOn);
        }
        //reflectionProbe.RenderProbe();
        //reflectionProbe.gameObject.SetActive(powerOn);
        //StartCoroutine(ResetReflectionProbe());   
    }

    private IEnumerator ResetReflectionProbe()
    {
        //reflectionProbe.gameObject.SetActive(false);

        for(int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(0.15f);
       
            reflectionProbe.RenderProbe();
        }
        //reflectionProbe.gameObject.SetActive(true);

    }
}
