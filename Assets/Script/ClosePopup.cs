using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EventManager;

public class ClosePopup : MonoBehaviour
{
    [Range(1f, 5f)]
    public float delayTime= 0;
    WaitForSeconds seconds;
    UnityAction callBackEvent;

    private void Awake()
    {
        seconds = new WaitForSeconds(delayTime);
    }

    public IEnumerator Close(UnityAction callBack = null)
    {
        if(callBack != null)
        {
            callBackEvent = callBack;
        }

        yield return seconds;

        if (callBackEvent != null)
        {
            callBackEvent.Invoke();
        }
    }


}
