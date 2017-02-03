using UnityEngine;

public class LightManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] lightObjects; 

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
    }
}
