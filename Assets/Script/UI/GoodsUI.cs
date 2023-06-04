using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoodsUI : MonoBehaviour
{
    public TextMeshProUGUI goodsValueText;

    public void UpdateGoodsValue(string value)
    {
        goodsValueText.text = value;

    }
}