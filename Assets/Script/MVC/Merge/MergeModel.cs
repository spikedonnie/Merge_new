using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeModel : MonoBehaviour
{
    [SerializeField] GodFingerModel godFingerModel;


    public GodFingerModel GetGodFingerModel()
    {
        return godFingerModel;
    }

    public void SetGoldFingerDataModel(GodFingerModel model)
    {
        godFingerModel.duration = model.duration;
        godFingerModel.count = model.count;
        godFingerModel.cost = model.cost;
        godFingerModel.onChange?.Invoke(godFingerModel);

    }

}
[System.Serializable]
public class GodFingerModel
{
    public float duration = 0;
    public int count = 0;   
    public int cost = 0;
    public int maxCount { get { return 10; } }
    public System.Action<GodFingerModel> onChange;
    public GodFingerModel(float d, int l, int c)
    {
        duration = d;
        count = l;
        cost = c;
    }

    public void SetCount(int c)
    {
        count = count + c;

        if(count >= maxCount)
        {
            count = maxCount;
        }

    }

}