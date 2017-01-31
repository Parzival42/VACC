using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothRip : MonoBehaviour {

    #region variables

    [SerializeField]
    private bool ripCloth = false;

    private MSDCurtain msdCurtain;
    private bool rippable = true;
    #endregion

    #region properties
    public MSDCurtain MSDCurtain
    {
        set { msdCurtain = value; }
    }
    #endregion

    #region methods
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (ripCloth && rippable)
        {
            Debug.Log("ripped");
            rippable = false;
            ripCloth = false;
        }	
	}

    #endregion
}
