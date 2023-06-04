using System;
using UnityEngine;

public class PopupCanvasUI : MonoBehaviour
{
    private static PopupCanvasUI instance = null;

    public static PopupCanvasUI Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public TwoButtonPopup twoButtonPopup;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public void ShowTwoButtonPopupp(string content, Action yes, Action no)
    {
        twoButtonPopup.SetUp(content, yes, no);
    }

    
    
    

}