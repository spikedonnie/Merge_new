using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using System;
using UnityEngine.Events;

public class NewHero : UIManager
{
    [SerializeField] private TextMeshProUGUI diamondTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI damageTxt;
    [SerializeField] private TextMeshProUGUI adDamageTxt;
    [SerializeField] private TextMeshProUGUI closeTxt;
    [SerializeField] private Image heroIcon;
    [SerializeField] private Image damageIcon;

    [SerializeField] private Image diamondIcon;
    [SerializeField] private Image adDamageIcon;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Button btn;

    private DropTable dropTable;
    private MercenaryType heroType;
    bool isClose = false;
    bool isRewardComplete = false;
    float savedDiamond = 0;

    private void Start()
    {
        base.Init();
        btn.onClick.AddListener(GetReward);
    }

    public void SetRewardData(DropTable table, MercenaryType type)
    {
        if (!isRewardComplete && dropTable != null)
        {
            savedDiamond += dropTable.typeValue_1;
        }

        isClose = false;
        isRewardComplete = false;
        SettingData(type);
        dropTable = table;

        damageTxt.enabled = true;
        diamondTxt.enabled = true;

        diamondIcon.enabled = true;
        damageIcon.enabled = true;

        adDamageIcon.enabled = false;
        adDamageTxt.enabled = false;
        diamondTxt.text = string.Format("{0}", dropTable.typeValue_1);
        StartCoroutine(DelayClose(GetReward));

    }


    private void SettingData(MercenaryType type)
    {
        isClose = false;
        GameController.instance.IsGameStop = true;
        OpenUI();
        particle.Stop(true);
        particle.Play(true);
        heroType = type;
        Player player = GameDataManager.instance.SheetJsonLoader.GetPlayerData(type);

        nameTxt.text = player.Name_KR;
        heroIcon.sprite = Utils.GetDocumentHeroSprite(type.ToString()).uiSprite;
        
        damageTxt.text = NumberToSymbol.ChangeNumber(GameController.instance.DamageManager.BaseDamageByPlayer(heroType));

        StartCoroutine(DelayClose(GetReward));

    }

    private IEnumerator DelayClose(UnityAction action)
    {
        btn.interactable = false;
        closeTxt.enabled = false;
        yield return new WaitForSeconds(2f);
        closeTxt.enabled = true;
        btn.interactable = true;
        yield return new WaitForSeconds(3f);
        if (action != null) action.Invoke();
    }

    public void GetReward()
    {
        if (isClose) return;
        StopAllCoroutines();
        isClose = true;
        GameController.instance.IsGameStop = false;
        GameDataManager.instance.AddDiamondData(dropTable.typeValue_1 + savedDiamond);
        savedDiamond = 0;
        isRewardComplete = true;
        CloseUI();
    }


    public override void CloseUI()
    {
        base.CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();

        AudioManager.Instance.PlayEffect(ESoundEffect.NewHero);
    }
}