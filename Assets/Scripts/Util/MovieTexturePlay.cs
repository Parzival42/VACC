using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieTexturePlay : MonoBehaviour {

    public MovieTexture movTexture;

    void Start () {
        if(GetComponent<Renderer>().materials[1].mainTexture is MovieTexture){
            movTexture = (MovieTexture)GetComponent<Renderer>().materials[1].mainTexture;
            movTexture.loop = true;
            movTexture.Play();
        }
    }
}
