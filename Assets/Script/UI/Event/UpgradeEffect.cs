using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeEffect : MonoBehaviour
{

    [Header("온오프 오브젝트")]
    public GameObject slotBack;
    public Image slotImage;
    [Header("패널 색")]
    public Gradient gradientColor;
    public AnimationCurve curve;
    [Header("텍스트 크기")]
    [SerializeField, Range(0.2f, 1.0f)] float sizeFilter = 0.5f;
    [Header("ActionText")]
    public TextMeshProUGUI[] actionTexts;
    private float[] baseFontSizes;
    public float colorDuration = 1f;
    public float textDuration = 0.5f;

    WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
    public bool isPlayUpgrade = false;

    private void Awake()
    {
        baseFontSizes = new float[actionTexts.Length];
        for (int i = 0; i < actionTexts.Length; i++)
        {
            baseFontSizes[i] = actionTexts[i].fontSize;
        }
    }

    public IEnumerator ClickEffectProcess()
    {
        slotBack.SetActive(true);

        yield return null;
        float t = 0;
        float colorClampValue = 0;
        float textClampValue = 0;

        while (colorClampValue <= 1 || textClampValue <= 1)
        {
            t += Time.deltaTime;
            colorClampValue = t / colorDuration;
            textClampValue = t / textDuration;

            if(colorClampValue <= 1)
            {
                slotImage.color = gradientColor.Evaluate(colorClampValue);
            }

            if (textClampValue <= 1)
            {
                float upsize = (curve.Evaluate(textClampValue) * sizeFilter) + 1;

                for (int i = 0; i < actionTexts.Length; i++)
                {
                    actionTexts[i].fontSize = baseFontSizes[i] * upsize;
                }
            }


            yield return waitFrame;
        }

        yield return null;

        //Return
        for (int i = 0; i < actionTexts.Length; i++)
        {
            actionTexts[i].fontSize = baseFontSizes[i];
        }
        slotBack.SetActive(false);
    }
}
