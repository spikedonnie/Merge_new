using UnityEngine;

using UnityEngine.Pool;
using System.Collections.Generic;
using DamageNumbersPro;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.TextCore.Text;
using System.Collections;
using AssetKits.ParticleImage;
using Spine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private DropTable dropTable;
    [SerializeField] private RecordBattle recordBattle;
    public DamageNumber damageNumber;
    public DamageNumber criticalDamageNumber;

    public bool isArriveHitPosition = false;
    public bool isAutoProgressStage = true;
    public bool IsBoss { get; set; }

    public bool IsUseDungeonKey { get; set; }
    private int step = 0;
    public Animator warningAnim;

    [SerializeField] private EnemyBase enemy;
    //[SerializeField] private Queue<int> normalMonsterQueue = new Queue<int>();
    private Vector3 enemyNormalPosition = new Vector3(3, 1, 0);
    private Vector3 enemyCriticalPosition = new Vector3(3, 2, 0);
    public ParticleImage particleImage;

    [SerializeField] BattleSaveData battleSaveData;

    public int CurrentStage {get;set;}

    public EnemyBase enemyBase
    {
        get
        {
            return enemy;
        }
        set
        {
            enemy = value;
        }
    }

    //TestCode//
    public void SetStepMax(int st)
    {
        step = st;
    }
    //TestCode//
    public void SetStage(int st)
    {
        battleSaveData.currentStage = st;
        GameController.instance.BattleManager.enemyBase.Die();
    }




    private void Start()
    {
        AddEvent();
    }

    private void AddEvent()
    {
        EventManager.Instance.AddCallBackEvent<MercenaryType, Vector3>(CallBackEventType.TYPES.OnHitEnemy, OnHitEnemy);
        EventManager.Instance.AddCallBackEvent<EnemyTYPE>(CallBackEventType.TYPES.OnKillEnemy, OnKillEnemy);
        EventManager.Instance.AddCallBackEvent<EnemyTYPE>(CallBackEventType.TYPES.OnReward, OnReward);
        particleImage.onStop.AddListener(ShotCoin);
        particleImage.onFirstParticleFinish.AddListener(() => AudioManager.Instance.PlayEffect(ESoundEffect.Coins));
    }

    public IEnumerator Init()
    {
        //????��?�� ?��?��?���? �??��????��
        this.battleSaveData = GameDataManager.instance.GetSaveData.battleSaveData;

        if (battleSaveData.currentStage >= Define.MAX_STAGE) battleSaveData.currentStage = Define.MAX_STAGE;

        SpawnEnemy(EnemyTYPE.Common);

        //?��?�� ?���? ?��?��
        for (int i = 0; i < this.battleSaveData.heroList.Count; i++)
        {
            if (battleSaveData.heroList[i] >= (int)MercenaryType.DemonMagic3)
                battleSaveData.heroList[i] = (int)MercenaryType.DemonMagic3;
            
            GameController.instance.mergeController.GetNewHeroByObjectPool.CreateCharacter(battleSaveData.heroList[i]);
        }



        damageNumber.PrewarmPool();

        criticalDamageNumber.PrewarmPool();

        yield return null;

    }

    public int GetStage()
    {
        return battleSaveData.currentStage;
    }

    #region ?�� ?��?�� 

    public EnemyBase CreateMonster(EnemyTYPE enemyType, int stageIndex)
    {
        EnemyBase enemyInstance = null;

        Enemy enemyData = GameDataManager.instance.SheetJsonLoader.GetEnemyData(enemyType, stageIndex);

        switch (enemyType)
        {
            case EnemyTYPE.Common:
                int random = UnityEngine.Random.Range(0, enemyData.MONSTER_IMAGE_ID);
                enemyInstance = ObjectPool.instance.GetNormalMonster(random).GetComponent<EnemyBase>();
                enemyInstance.returnImageID = random;
                break;

            case EnemyTYPE.Elite:
                enemyInstance = ObjectPool.instance.GetNormalMonster(enemyData.MONSTER_IMAGE_ID).GetComponent<EnemyBase>();
                enemyInstance.returnImageID = enemyData.MONSTER_IMAGE_ID;
                break;

            case EnemyTYPE.Unique:
                enemyInstance = ObjectPool.instance.GetUniqueMonster(enemyData.MONSTER_IMAGE_ID).GetComponent<EnemyBase>();
                enemyInstance.returnImageID = enemyData.MONSTER_IMAGE_ID;
                break;

            case EnemyTYPE.Max:
                break;
            default:
                break;
        }

        enemyInstance.enemyData.ID = enemyData.ID;
        enemyInstance.enemyData.HP = enemyData.HP;
        enemyInstance.enemyData.GOLD = enemyData.GOLD;
        enemyInstance.enemyData.MONSTER_IMAGE_ID = enemyData.MONSTER_IMAGE_ID;
        enemyInstance.enemyData.EnemyTYPE = enemyData.EnemyTYPE;
        enemyInstance.enemyData.NAME = enemyData.NAME;
        enemyInstance.enemyData.DIAMOND = enemyData.DIAMOND;
        enemyInstance.enemyData.STONE = enemyData.STONE;
        enemyInstance.sortingOderIndex = 0;
        enemyInstance.stageIndex = stageIndex;
        enemyInstance.bgID = enemyInstance.enemyData.BG_ID;

        return enemyInstance;
    }

    //?�� ?��?��
    public void SpawnEnemy(EnemyTYPE enemyType)
    {
        switch (enemyType)
        {
            case EnemyTYPE.Common: SpawnCommonEnemy(); break;
            case EnemyTYPE.Elite: SpawnEliteEnemy(); break;
            case EnemyTYPE.Unique: SpawnUniqueEnemy(); break;
            case EnemyTYPE.Max: break;
            default: break;
        }
    }
    //?���? ?�� ?��?��
    private void SpawnCommonEnemy()
    {

        //보스 카운?��?�� ?��?��?���? 바로 보스?�� ?��?��
        if (isAutoProgressStage && IsAchiveEliteTargetCount())
        {
            isAutoProgressStage = false;
            StartCoroutine("ShowWarningText");
            return;
        }

        enemyBase = CreateMonster(EnemyTYPE.Common, battleSaveData.currentStage);

        GameController.instance.SetBackground(enemyBase.bgID);

        //?�� 체력 감소 ?���? ?��?��
        var reduce = enemyBase.enemyData.HP * GameController.instance.abilityManager.TotalAbilityData(AbilityType.EnemyHP);
        //Debug.Log("?�� 체력:"+ enemyBase.enemyData.HP+" ?���?:" + GameController.instance.abilityManager.TotalAbilityData(AbilityType.EnemyHP)*100 + "% 체력감소" + reduce );
        enemyBase.enemyData.HP = enemyBase.enemyData.HP - reduce;
        //Debug.Log("최종 체력:"+ enemyBase.enemyData.HP);
        enemyBase.InitEnemy();

        UIController.instance.UpdateProgressLeaderCountUI(battleSaveData.currentStage, step);

        UIController.instance.SetChallangeBattleUI(false);


        UIController.instance.SetUI(enemyBase.enemyData.EnemyTYPE, battleSaveData.currentStage);//UI?��?��?��?��

    }

    IEnumerator ShowWarningText()
    {
        yield return null;
        GameController.instance.BattleManager.IsBoss = true;
        GameController.instance.BattleManager.SpawnEnemy(EnemyTYPE.Elite); 
        warningAnim.SetTrigger("Play");
    }

    //?��리트 ?�� ?��?��
    private void SpawnEliteEnemy()
    {

        if (enemyBase != null)
        {
            enemyBase.ForceCommonEliteKill();

            enemyBase = null;
        }

        enemyBase = CreateMonster(EnemyTYPE.Elite, battleSaveData.currentStage);

        //?�� 체력 감소 ?���? ?��?��
        var reduce = enemyBase.enemyData.HP * GameController.instance.abilityManager.TotalAbilityData(AbilityType.EnemyHP);
        //Debug.Log("?�� 체력:"+ enemyBase.enemyData.HP+" ?���?:" + GameController.instance.abilityManager.TotalAbilityData(AbilityType.EnemyHP)*100 + "% 체력감소" + reduce );
        enemyBase.enemyData.HP = enemyBase.enemyData.HP - reduce;
        //Debug.Log("최종 체력:"+ enemyBase.enemyData.HP);

        enemyBase.InitEnemy();
        UIController.instance.EliteChallangeButton(false);
        UIController.instance.SetChallangeBattleUI(true);
        UIController.instance.SetUI(enemyBase.enemyData.EnemyTYPE, battleSaveData.currentStage);//UI?��?��?��?��

    }
    //?��?��?�� ?�� ?��?��
    private void SpawnUniqueEnemy()
    {

        //?��?��?�� ?��?�� ?�� 처치
        if(enemyBase != null)
        {
            enemyBase.ForceCommonEliteKill();

            enemyBase = null;

        }
        
        step--;
        
        if(step < 0)
        {
            step = 0;
        }

        isAutoProgressStage = true;

        if (CurrentStage > Define.MAX_UNIQUE_INDEX)
        {
            CurrentStage = Define.MAX_UNIQUE_INDEX;
        }

        //?�� ?��?��
        enemyBase = CreateMonster(EnemyTYPE.Unique, CurrentStage);//?��곳에?�� ?��?��?��보스 처리?��줘야?��

        GameController.instance.SetBackground(enemyBase.bgID);

        //?�� 체력 감소 ?���? ?��?��
        var reduce = enemyBase.enemyData.HP * GameController.instance.abilityManager.TotalAbilityData(AbilityType.EnemyHP);
        //Debug.Log("?�� 체력:"+ enemyBase.enemyData.HP+" ?���?:" + GameController.instance.abilityManager.TotalAbilityData(AbilityType.EnemyHP)*100 + "% 체력감소" + reduce );
        enemyBase.enemyData.HP = enemyBase.enemyData.HP - reduce;
        //Debug.Log("최종 체력:"+ enemyBase.enemyData.HP);
        enemyBase.InitEnemy();
        UIController.instance.SetChallangeBattleUI(true);//보스모드 UI ?��?��?��
        UIController.instance.EliteChallangeButton(false);//?��?�� 버튼 비활?��?��
        UIController.instance.SetUI(enemyBase.enemyData.EnemyTYPE, CurrentStage + 1);//stage말고보스 ?��?��?�� 보내줘야?�� UI?��?��?��?���?�?

    }

    

    //?�� ?���?
    public void OnHitEnemy(MercenaryType type, Vector3 pos)
    {

        //?�� 불러?���?
        EnemyBase enemy = enemyBase;

        if(enemy == null)
        {
            Debug.Log("공격 �??��?�� ?��?�� ?��?��");
            return;
        }

        if (enemy.isDie) return;

        GetPoolEffect(type);

        //?��?��?��?�� ?��미�?? 받아?���?
        float playerDamage = GameController.instance.abilityManager.FinalTotalDamage(type, enemy.enemyData.EnemyTYPE);


        bool cri = GameController.instance.abilityManager.IsCritical();

        playerDamage = cri ? GameController.instance.abilityManager.CalcCriticalDamage(playerDamage) : playerDamage;
        //Damage Floating Text

        if (cri)
        {
            criticalDamageNumber.Spawn(enemyCriticalPosition, string.Format("{0:F0}", NumberToSymbol.ChangeNumber(playerDamage)));
        }
        else
        {
            damageNumber.Spawn(enemyNormalPosition, string.Format("{0:F0}", NumberToSymbol.ChangeNumber(playerDamage)));
        }

        //?��?���? ?��미�?? 처리
        enemy.ReduceHealth(playerDamage);

        //?�� 체력 ?��?��
        if (enemy.CurrentHp <= 0)
        {
            enemy.Die();

            return;
        }

        //?�� ?���? ?��?��메이?�� 처리
        enemy.inOutAnimator.PlayHitAnimation();
    }

    //?�� 죽음
    private void OnKillEnemy(EnemyTYPE type)
    {
        GameController.instance.IsGameStop = true;

        switch (type)
        {
            case EnemyTYPE.Common: DieCommonEnemy(); break;
            case EnemyTYPE.Elite: DieEliteEnemy(); break;
            case EnemyTYPE.Unique: DieUniqueEnemy(); break;
            case EnemyTYPE.Max: break;
            default: break;
        }
    }
    // ?���? 몬스?�� ?��망시
    public void DieCommonEnemy()
    {
        var leaderStage = GameDataManager.instance.SheetJsonLoader.GetEnemyData(EnemyTYPE.Common, battleSaveData.currentStage).LEADER_STAGE;

        step++;
        //보스 ?��?��?�� ?��?��?���?
        if (step >= leaderStage)
        {
            step = leaderStage;
        }

        //?���? 몬스?��?�� ?��망하?��마자 ?��?��?��?��
        SpawnEnemy(EnemyTYPE.Common);

    }
    //?��리트 몬스?�� ?��망시
    public void DieEliteEnemy()
    {
        step = 0;

        battleSaveData.currentStage++;

        if(battleSaveData.currentStage > Define.MAX_STAGE)
        {
            battleSaveData.currentStage = Define.MAX_STAGE;
        }

        isAutoProgressStage = true;

        UIController.instance.UpdateProgressLeaderCountUI(battleSaveData.currentStage, step);
        //?��리스 몬스?��?�� 보상 ?��?���? ?��?��?�� ?���? 몬스?�� ?��?��?��
        UIController.instance.EliteChallangeButton(false);

        GameDataManager.instance.SaveData();

    }
    //보스 몬스?�� ?��망시
    public void DieUniqueEnemy()
    {
        if(IsUseDungeonKey)
        {
            return;
        }

        battleSaveData.uniqueMonsterIndex++;
        
        if (battleSaveData.uniqueMonsterIndex > Define.MAX_UNIQUE_INDEX)
        {
            battleSaveData.uniqueMonsterIndex = Define.MAX_UNIQUE_INDEX;
        }
    }

    #endregion

    //EnemyBase -> Die -> OutAnim -> Event Reward
    private void OnReward(EnemyTYPE type)
    {
        switch (type)
        {
            case EnemyTYPE.Common:
                particleImage.Play();
                break;

            case EnemyTYPE.Elite:

                // //보상 리스?���? 만들�?
                // List<RewardInfoData> rewards = new List<RewardInfoData>();
                // rewards.Add(new RewardInfoData(RewardTYPE.Gold, enemyBase.enemyData.GOLD));
                // //?��?�� ?��보�?? 만들?��?�� ?��?��
                // PopupRewardInfoData infoData = new PopupRewardInfoData();
                // infoData.SetTitle("CLEAR");
                // infoData.SetRewardInfoList(rewards);
                // infoData.SetCallBack();
                // PopupController.instance.SetupPopupInfo(infoData);



                ShowEliteBossReward(SetDropTable(enemyBase));
                break;

            case EnemyTYPE.Unique:
                ShowRaidBossReward(SetDropTable(enemyBase));
                break;

            case EnemyTYPE.Max:
                break;

            default:
                break;
        }
    }
    
    void ShowRaidBossReward(DropTable table)
    {

        //죽었?��?�� ?��?���? ?��?��?��?���? ?��?��?���? 체크
        GameController.instance.IsGameStop = true;
        GameController.instance.AudioManager.PlayEffect(ESoundEffect.BattleWin);
        PopupRewardInfoData data = new PopupRewardInfoData();
        data.SetTitle("REWARD");
        data.SetEffectType(EEfectGameObjectTYPE.BossReward);
        data.SetCallBack(RaidCallBack);

        List<RewardInfoData> rewards = new List<RewardInfoData>();


        var stone = GameDataManager.instance.CalcSpiritStoneBonus(dropTable.typeValue_2);

        if(IsUseDungeonKey)
        {
            rewards.Add(new RewardInfoData(table.item2, stone));

        }
        else
        {
            rewards.Add(new RewardInfoData(table.item1, table.typeValue_1));
            rewards.Add(new RewardInfoData(table.item2, stone));
        }
 

        data.SetRewardInfoList(rewards);

        PopupController.instance.SetupPopupInfo(data);

    }
    
    void RaidCallBack()
    {
        IsUseDungeonKey = false;

        GameController.instance.IsGameStop = false;
        UIController.instance.dungeonUI.OpenUI();
        GameController.instance.BattleManager.SpawnEnemy(EnemyTYPE.Common);
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_TRAINING)) return;
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_RAID_MENU)) return;
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.GUIDE_AUTO_SKILL)) return;
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_AD_MENU)) return;
        TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_SHOP);
    }

    void ShowEliteBossReward(DropTable table)
    {
        GameController.instance.IsGameStop = true;
        GameController.instance.AudioManager.PlayEffect(ESoundEffect.BattleWin);
        PopupRewardInfoData data = new PopupRewardInfoData();
        data.SetTitle("REWARD");
        data.SetEffectType(EEfectGameObjectTYPE.BossReward);
        data.SetCallBack(ElieteCallBack);

        List<RewardInfoData> rewards = new List<RewardInfoData>();
        rewards.Add(new RewardInfoData(table.item1, table.typeValue_1));
        data.SetRewardInfoList(rewards);

        PopupController.instance.SetupPopupInfo(data);
        
    }

    void ElieteCallBack()
    {
        GameController.instance.IsGameStop = false;
        GameController.instance.BattleManager.SpawnEnemy(EnemyTYPE.Common);

        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_TRAINING)) return;
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_RAID_MENU)) return;
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.GUIDE_AUTO_SKILL)) return;
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_AD_MENU)) return;
        TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_SHOP);
    }













    
    void ShotCoin()
    {
        particleImage.Stop();

        var rewardGold = SetDropTable(enemyBase).typeValue_1;

        rewardGold = CalcDoubleGold(rewardGold);

        GameDataManager.instance.AddGoldData(rewardGold);

        TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_HERO_MENU);

    }

    void GetPoolEffect(MercenaryType type)
    {

        EPoolName effect = (EPoolName)Enum.Parse(typeof(EPoolName), GameDataManager.instance.SheetJsonLoader.GetPlayerData(type).EffectType);
        var efx = ObjectPool.instance.GetObject(effect);
        efx.transform.position = RandomTargetPosition();
    }

    public Vector3 RandomTargetPosition()
    {
        return new Vector3(UnityEngine.Random.Range(enemyNormalPosition.x - 0.2f, enemyNormalPosition.x + 0.2f), UnityEngine.Random.Range(enemyNormalPosition.y, enemyNormalPosition.y + 1f), 0);
    }

    float CalcDoubleGold(float gold)
    {
        var ran = UnityEngine.Random.Range(0f, 100f);

        var per = GameController.instance.abilityManager.TotalAbilityData(AbilityType.DoubleGoldBonus);

        if(ran < per)
        {
            return gold * 2;
        }

        return gold;
    }

    public bool IsAchiveEliteTargetCount()
    {
        var data = GameDataManager.instance.SheetJsonLoader.GetEnemyData(EnemyTYPE.Common, battleSaveData.currentStage).LEADER_STAGE;

        if (step >= data)
        {
            step = data;

            return true;
        }

        return false;
    }

    private DropTable SetDropTable(EnemyBase enemy)
    {
        switch (enemy.enemyData.EnemyTYPE)
        {
            case EnemyTYPE.Common:

                dropTable.item1 = RewardTYPE.Gold;
                dropTable.item2 = RewardTYPE.Gold;
                dropTable.typeValue_1 = enemy.enemyData.GOLD;
                dropTable.typeValue_2 = 0;

                return dropTable;

            case EnemyTYPE.Elite:

                dropTable.item1 = RewardTYPE.Gold;
                dropTable.typeValue_1 = enemy.enemyData.GOLD;

                return dropTable;

            case EnemyTYPE.Unique:

                dropTable.item1 = RewardTYPE.Diamond;
                dropTable.item2 = RewardTYPE.SpiritStone;
                dropTable.typeValue_1 = enemy.enemyData.DIAMOND;
                dropTable.typeValue_2 = enemy.enemyData.STONE;

                return dropTable;

            default:
                break;
        }
        return null;
    }

    //    private int[] GetRandomInt(int length, int min, int max)
    //    {
    //        int[] randArray = new int[length];
    //        bool bSame;

    //        for (int i = 0; i < length; ++i)
    //        {
    //            while (true)
    //            {
    //                randArray[i] = UnityEngine.Random.Range(min, max)
    //;
    //                bSame = false;

    //                for (int j = 0; j < i; ++j)
    //                {
    //                    if (randArray[j] == randArray[i])
    //                    {
    //                        bSame = true;
    //                        break;
    //                    }
    //                }
    //                if (!bSame) break;
    //            }
    //        }

    //        return randArray;
    //    }

    //중복?���? ?��?�� ?��?�� ?��?��
    //private int GetShakingMonsterIndex()
    //{
    //    int monsterIndex;
    //    int[] randomMonsterIndex;

    //    //Monster Queue�? 비어 ?��?���?
    //    if (CheckMakeMonsterQueue())
    //    {
    //        normalMonsterQueue.Clear();
    //        //0�??�� 17(몬스?��?�� 종류)까�???�� 중복?���? ?��?�� ?���? Shake?��?�� ?��?���?
    //        randomMonsterIndex = GetRandomInt(monsterGrade * maxMonsterCount, 0, monsterGrade * maxMonsterCount);

    //        for (int i = 0; i < maxMonsterCount; i++)
    //        {
    //            normalMonsterQueue.Enqueue(randomMonsterIndex[i]);
    //        }
    //    }

    //    monsterIndex = normalMonsterQueue.Dequeue();

    //    return monsterIndex + 1;
    //}

    //리스?��?�� ?���? 추�??

    //private bool CheckMakeMonsterQueue()
    //{
    //    if (normalMonsterQueue.Count <= 0)
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    //보통,?��리트 ?�� ?��?��




}