using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RelicSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ability1_Txt = null;
    [SerializeField] private TextMeshProUGUI ability2_Txt = null;
    [SerializeField] private TextMeshProUGUI relicNameTxt = null;
    [SerializeField] private TextMeshProUGUI relicHaveCountTxt = null;

    [SerializeField]
    private UISlot slot = new UISlot();

    [SerializeField] private RelicModel relicData;
   
    public void Init(RelicModel data)
    {
        relicData = data;
        slot.iconImage.sprite = Utils.GetRelicSpriteByName(relicData.relicTypeName.ToString());
        relicNameTxt.text = I2.Loc.LocalizationManager.GetTranslation(relicData.name_KR);
        RefreshUI();
    }

    public RelicModel GetRelicModel()
    {
        return relicData;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);

    }
    public void RefreshUI()
    {

        relicHaveCountTxt.text = string.Format("<color=#39FF9D>{0}</color>/{1}", relicData.haveCount, relicData.relicMaxHaveCount);
        ability1_Txt.text = string.Format("{0}:{1}%", I2.Loc.LocalizationManager.GetTranslation(relicData.TYPE_1_NAME), relicData.type1_Value * (relicData.haveCount));
        ability2_Txt.text = string.Format("{0}:{1:0.#}{2}", I2.Loc.LocalizationManager.GetTranslation(relicData.TYPE_2_NAME), relicData.type2_Value * (relicData.haveCount),relicData.UNIT_NAME);
        relicNameTxt.text = I2.Loc.LocalizationManager.GetTranslation(relicData.name_KR);

    }

    public bool IsCheckMaxLevel()
    {
        if (relicData.haveCount < relicData.relicMaxHaveCount)
        {
            return false;

        }
        return true;
    }

}
