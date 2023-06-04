using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class UtilityMethod
{
    public static T GetJsonParseData<T>(TextAsset jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData.text);
    }

    public static void SetBtnApplyEvent(Button btn, UnityAction action)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            action.Invoke();
        });
    }
}