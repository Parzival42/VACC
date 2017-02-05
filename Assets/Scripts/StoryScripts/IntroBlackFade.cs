using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBlackFade : MonoBehaviour {

    private int depth = -1000;

    [SerializeField]
    private float fadeDuration = 0.3f;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeInOutQuad;

    [SerializeField]
    private Texture2D blackTexture;

    [SerializeField]
    private Texture2D whiteTexture;

    private int fadeDirection = -1;
    private float alphaValue = 1.0f;

    [SerializeField]
    private bool use = false;

    [SerializeField]
    private bool use2 = false;

    private bool useWhiteTexture = false;

    void Update()
    {
        if (use)
        {
            use = false;
            FadeIn();
        }

        if (use2)
        {
            use2 = false;
            FadeOut();
        }
    }

    void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, alphaValue);
        GUI.depth = depth;
        if (!useWhiteTexture)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
        }else
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), whiteTexture);
        }
    }

    public void FadeIn()
    {
        LeanTween.value(gameObject, alphaValue, 0.0f, fadeDuration + 0.2f)
        .setOnUpdate((float amount) =>
        {
            alphaValue = amount;
        })
        .setEase(easeType);
    }

    public void FadeOut()
    {
        useWhiteTexture = true;
        LeanTween.value(gameObject, alphaValue, 1.0f, fadeDuration)
          .setOnUpdate((float amount) =>
          {
              alphaValue = amount;
          })
          .setEase(easeType);
    }

    public void SetTransparent()
    {
        alphaValue = 0.0f;
    }
}
