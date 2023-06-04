using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SummonUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button smallButton;
    [SerializeField] private Button commonButton;
    [SerializeField] private Button largeButton;
    [SerializeField] private SummonSlot[] summonSlots;
    private int[] gradeLevel = new int[4] { 30, 20, 10, 0 };
    public Sprite[] boxSprites;

    private WaitForSeconds wait = new WaitForSeconds(0.05f);
    private WaitForSeconds load = new WaitForSeconds(1f);
    private Summon summon;

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        closeButton.onClick.AddListener(CloseUI);
        smallButton.onClick.AddListener(SmallPurchase);
        commonButton.onClick.AddListener(CommonPurchase);
        largeButton.onClick.AddListener(LargePurchase);
    }

    private void Start()
    {
        if (closeButton != null)
            CloseUI();
    }

    public void Init(Summon parent)
    {
        summon = parent;
    }

    public void SummonRelic(int count)
    {
        OpenUI();
        HideUI();
        StartCoroutine(CorSummonRelic(count));
    }

    private IEnumerator CorSummonRelic(int count)
    {
        yield return load;

        for (int i = 0; i < count; i++)
        {
            summonSlots[i].gameObject.SetActive(true);
            AudioManager.Instance.PlaySound(PoolAudio.RELIC);
            summonSlots[i].SendSummonInfo(ShakeSummon());

            if (summon.CheckRelicAllMaxLevel())
            {
                break;
            }

            yield return wait;
        }

        ShowUI();

        yield break;
    }



    private SummonData ShakeSummon()
    {
        SummonData data = new SummonData();
        int relic;

        if (summon.CheckRelicAllMaxLevel())
        {
            return null;
        }



        do
        {
            relic = Random.Range(0, (int)RelicType.MAX);
            //Debug.Log((RelicType)relic);
        } while (GameController.instance.RelicManager.IsCheckMaxLevel((RelicType)relic));

        data.itemIndex = relic;

        data.icon = Utils.GetRelicSpriteByName(((RelicType)data.itemIndex).ToString());

        data.box = Utils.GetUiSprite("RelicBox").uiSprite;

        Relic relicData = GameDataManager.instance.SheetJsonLoader.GetRelicData((RelicType)data.itemIndex);


        data.infoText = relicData.Name_KR;

        GameController.instance.RelicManager.AddRelic((RelicType)data.itemIndex);

        return data;
    }

    private int CheckGrade(int rnd)
    {
        int rank = 0;

        for (int i = 0; i < gradeLevel.Length; i++)
        {
            if (rnd <= gradeLevel[i])
            {
                continue;
            }
            else
            {
                rank = i;
                break;
            }
        }
        return rank;
    }

    private void OpenUI()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.4f);
    }

    public void CloseUI()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.4f);

        for (int i = 0; i < summonSlots.Length; i++)
        {
            summonSlots[i].gameObject.SetActive(false);
        }
    }

    private void ShowUI()
    {
        closeButton.enabled = true;
        smallButton.enabled = true;
        commonButton.enabled = true;
        largeButton.enabled = true;
    }

    private void HideUI()
    {
        for (int i = 0; i < summonSlots.Length; i++)
        {
            summonSlots[i].gameObject.SetActive(false);
        }

        closeButton.enabled = false;
        smallButton.enabled = false;
        commonButton.enabled = false;
        largeButton.enabled = false;
    }

    private void SmallPurchase()
    {
        summon.PurchaseSmallSummonRelic();
    }

    private void CommonPurchase()
    {
        summon.PurchaseCommonSummonRelic();
    }

    private void LargePurchase()
    {
        summon.PurchaseLargeSummonRelic();
    }
}