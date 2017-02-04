using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static AnimationManager animationManager = null;

    #region Inspector variables
    [SerializeField]
    [Tooltip("Should contain animation controller with corresponding animation.")]
    private GameObject carLightPrefab;
    #endregion

    public static AnimationManager Instance
    {
        get
        {
            animationManager = FindObjectOfType<AnimationManager>();

            if (animationManager == null)
                animationManager = new GameObject("_AnimationManager").AddComponent<AnimationManager>();
            return animationManager;
        }
    }

    public void PlayCarLight()
    {
        GameObject carLight = Instantiate(carLightPrefab);
        Animator anim = GetAnimatorOf(carLight);
        anim.Play("huber");
    }

    private Animator GetAnimatorOf(GameObject g)
    {
        Animator anim = g.GetComponent<Animator>();
        if (anim == null)
            Debug.LogError("No animator in prefab <b>" + g.name + "</b>!");
        return anim;
    }
}