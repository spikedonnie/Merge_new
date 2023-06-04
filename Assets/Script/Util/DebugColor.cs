using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugColor
{

    public static void DebugMsg(object msg1, object msg2)
    {
        Debug.Log($"<color=lime>{msg1}</color> / <color=red>{msg2}</color>");
    }
    public static void DebugMsg(object msg1, object msg2, object msg3)
    {
        Debug.Log($"<color=lime>{msg1}</color> / <color=red>{msg2}</color> / <color=white>{msg3}</color>");
    }


}
