using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffData
{
    public BuffType type;
    public int buffCount;
    public float buffDuration;
    public bool isActive = false;
    public BuffData(BuffType type, int buffCount, float buffDuration)
    {
        this.type = type;
        this.buffCount = buffCount;
        this.buffDuration = buffDuration;

        if(this.buffDuration > 1)
        {
            isActive = true;
        }
    }
    public BuffData()
    {
        this.isActive = false;
    }
}