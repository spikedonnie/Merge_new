using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class MergeManager : MonoBehaviour
{

    //총 고용 가능한 용병 수
    private int currentBattleMercenary = 0;

    //추가 대기 용명 수
    private int currentWaitMercenary;

    private bool bWaitFull = false;

    BattleSaveData battleSaveData;




    public IEnumerator SetMergeManager()
    {
        battleSaveData = GameDataManager.instance.GetSaveData.battleSaveData;
        CurrentWaitMercenary = battleSaveData.watingHero;
        yield return null;
    }

    public void CreateEffect(Vector3 pos)
    {
        ObjectPool.instance.GetObject(EPoolName.MergeEffect).transform.position = pos;
    }

    //출전 가능한 최대 용병 수
    public int GetMaxBattleMercenary
    {
        get
        {
            //Debug.Log("출전 가능한 기본 용병수:"+ defaultCollectMercenary+"추가 용병수:" + (int)GameController.instance.abilityManager.TotalAbilityData(AbilityType.MaxCollect));
            return (int)GameController.instance.abilityManager.TotalAbilityData(AbilityType.MaxCollect);
        }
    }

    //현재 출전중인 용병 수
    public int CurrentBattleMercenary
    {
        get
        {
            return currentBattleMercenary;
        }
        set
        {
            currentBattleMercenary = value;

            if (currentBattleMercenary < 0) currentBattleMercenary = 0;
            UIController.instance.UpdateReserveUI();
        }
    }

    //최대로 대기할 수 있는 용병 수
    public int GetMaxWaitMercenary
    {
        get
        {
            //Debug.Log("대기 가능한 기본 용병수:" + defaultWaitMercenary + "추가 용병수:" + (int)GameController.instance.abilityManager.TotalAbilityData(AbilityType.MaxSupply));
            return (int)GameController.instance.abilityManager.TotalAbilityData(AbilityType.MaxSupply);
        }
    }

    //현재 대기중인 용병 수
    public int CurrentWaitMercenary
    {
        get
        {
            return currentWaitMercenary;
        }
        set
        {
            currentWaitMercenary = value;

            if (currentWaitMercenary >= GetMaxWaitMercenary)
            {
                currentWaitMercenary = GetMaxWaitMercenary;
                bWaitFull = true;
            }
            else
            {
                bWaitFull = false;
            }

            if (currentWaitMercenary < 0) currentWaitMercenary = 0;



            battleSaveData.watingHero = currentWaitMercenary;
            UIController.instance.UpdateSupplyUI();
        }
    }

    public void WaitMercenaryMAX()
    {
        CurrentWaitMercenary = GetMaxWaitMercenary;
    }

    public bool IsWaitFull()
    {
        return bWaitFull;
    }

    public bool IsBattleHeroFull()
    {
        if (CurrentBattleMercenary < GetMaxBattleMercenary)
        {

            return false;
        }

        UIController.instance.SendPopupMessage(AlarmTYPE.FULL_BATTLE);
        UIController.instance.UpdateReserveUI();
        return true;
    }





}