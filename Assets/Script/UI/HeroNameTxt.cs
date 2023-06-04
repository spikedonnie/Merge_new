using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroNameTxt : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public void SetHeroName(MercenaryType hero)
    {
        Player player = GameDataManager.instance.SheetJsonLoader.GetPlayerData(hero);
        textMeshProUGUI.text = string.Format("{0}.{1}", (int)hero + 1, player.Name_KR);
    }

    public void UpdatePosition(Vector3 pos)
    {
        transform.position = Camera.main.WorldToScreenPoint(pos);
    }
}