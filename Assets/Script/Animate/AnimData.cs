using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimData : MonoBehaviour
{
    public string dataName;
    public AnimCurveType animCurveType;
    public float animDuration = 0.5f;
    public float value;
    public float animTime;
    public float animValue;
    public float animStartTime;

    public void ResetAnimData()
    {
        value = 0;
        animTime = 0f;
        animValue = 0f;
        animStartTime = Time.time;
    }

}