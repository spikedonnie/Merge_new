using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MergeController : MonoBehaviour
{
    [SerializeField] MergeModel mergeModel;
    [SerializeField] MergeView mergeView;
    [SerializeField] List<Character> heroList = new List<Character>();


    [SerializeField] HeroSorting heroSorting;

    bool isMerging = false;
    private float spawnX = -5f;
    private float maxMoveX = 8;
    private float maxMinX = 2f;
    private float moveMaxY = -6f;
    private float moveMinY = -2f;
    IEnumerator coroutineMoveToTarget;


    private float mergeSpeed;

    /// <summary>
    /// TEST CODE
    /// </summary>
    public void UpHeroLv()
    {
        for (int i = 0; i < heroList.Count; i++)
        {
            heroList[i].TestLvUp();
        }
    }

    public IEnumerator Init()
    {
        yield return null;
        var fingerLevel = GameDataManager.instance.GetSaveData.godFingerLevel;
        var godFingerModel = GameDataManager.instance.SheetJsonLoader.GetGodFingerData(fingerLevel);
        var sortingTime =  GameDataManager.instance.GetSaveData.sortingCoolTime;

        mergeView.Init(godFingerModel,sortingTime);
        mergeModel.SetGoldFingerDataModel(new GodFingerModel(godFingerModel.duration, godFingerModel.count, godFingerModel.cost));
        mergeView.SetGoldFingerPopUpOpenButtonEvent(mergeModel.GetGodFingerModel());
        mergeView.SetGoldFingerUseButtonEvent(() => StartGodFinger());
        mergeView.SetSortingButtonEvent(() => PlaySorting());
        mergeView.OpenGodFingerPenel(false);



    }

    public Vector3 SpawnPosition 
    {
        get { return new Vector3(spawnX, UnityEngine.Random.Range(moveMinY, moveMaxY), 0); }
    }
    public Vector3 TargetPosition
    { 
        get { return new Vector3(spawnX + UnityEngine.Random.Range(maxMinX, maxMoveX), UnityEngine.Random.Range(moveMinY, moveMaxY), 0); } 
    }
    public Character GetNewHeroByObjectPool
    {
        get { return ObjectPool.instance.GetObject(EPoolName.Character).GetComponent<Character>(); }    
    }
    public List<Character> GetHeroList()
    {
        return heroList;
    }
    public float GetSortingTime()
    {
        return mergeView.CurrentCooldownTime;
    }

    void PlaySorting()
    {

        StartCoroutine(mergeView.SortingMercenary());

        heroSorting.MoveToNextPosition(heroList);
    }

    public void StartGodFinger()
    {
        var godFingerModel = mergeModel.GetGodFingerModel();
        var gemValue= GameDataManager.instance.GetSaveData.currency[(int)RewardTYPE.Diamond];

        if(gemValue < godFingerModel.cost)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_DIAMOND);
            return;
        }

        GameDataManager.instance.SetCurrency(RewardTYPE.Diamond, -godFingerModel.cost);

        mergeView.OpenGodFingerPenel(false);
        mergeView.GodFingerButtonClick(false);
        godFingerModel.SetCount(1);  
        godFingerModel = GameDataManager.instance.SheetJsonLoader.GetGodFingerData(godFingerModel.count);
        mergeModel.SetGoldFingerDataModel(godFingerModel);

        GameDataManager.instance.GetSaveData.godFingerLevel = godFingerModel.count;

        GameDataManager.instance.SaveData();

        var duration = godFingerModel.duration + GameController.instance.abilityManager.TotalAbilityData(AbilityType.GoldFingerDuration);

        StartCoroutine("FingerTimeCoroutine", duration);
        StartCoroutine("UltroMergeCoroutine");

    }
    IEnumerator FingerTimeCoroutine(float time)
    {
        float timeRemaining = time;

        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            mergeView.godFingerTimeText.text = string.Format("{0:F1}s", timeRemaining);

            GameController.instance.MergeManager.WaitMercenaryMAX(); 


            yield return null;
        }

        mergeView.GodFingerButtonClick(true);
        StopCoroutine("UltroMergeCoroutine");
    }

    IEnumerator UltroMergeCoroutine()
    {

        while (true)
        {
            var heroType = FindEqualHeroGrade(heroList);

            var newHeroList = GetChractersByType(heroType);

            if (HeroSameLevel(heroType) || newHeroList.Count < 2)
            {
                //Debug.Log("");
                yield return null;

            }
            else
            {
                var one = FindAutoMergeHero(newHeroList);

                if (one != null)
                {
                    one.isAutoSelected = true;

                    var two = FindAutoMergeHero(newHeroList);

                    if (two != null)
                    {
                        two.isAutoSelected = true;
                        yield return StartCoroutine(MoveToTargetHeroPosition(one, two));
                    }
                    else
                    {
                        one.isAutoSelected = false;
                        one = null;
                    }
                }

                yield return null;

            }

        }

    }
    private bool CanMergeHeroes(List<Character> heroList)
    {
        var heroType = FindEqualHeroGrade(heroList);
        var newHeroList = GetChractersByType(heroType);
        return !HeroSameLevel(heroType) && newHeroList.Count >= 2;
    }



    bool HeroSameLevel(MercenaryType type)
    {

        if (type == MercenaryType.Max) return true;
        return false;

    }


    public void Merging()
    {
        if (isMerging) return;

        //bool flag = false;
        isMerging = true;

        //Character self;
        //Character target;

        List<Character> hList = heroList;

        //List?????? ???? ??????? ?????? 2?? ????? [????? ???]
        MercenaryType heroType = FindEqualHeroGrade(hList);
        //Debug.Log($"???? ??????? ?????? : {heroType} ");
        if(HeroSameLevel(heroType))
        {
            isMerging = false;
            return;
        } 
        //if (heroType == MercenaryType.Max-1) return;

        // [???? ????? ??????? ????]
        List<Character> newHeroList = GetChractersByType(heroType);
        //Debug.Log($"???? ????? ?????? : {newHeroList.Count} ??");

        // ???? ??????? ��?????? ?????? ???? ???? ��???? ????? ????????
        Character one = FindAutoMergeHero(newHeroList);
        //Debug.Log($"???? ???????? ?????? : {one} ");

        if (one == null)
        {
            Debug.Log($"NULL");
            isMerging = false;
            return;
        }
        one.isAutoSelected = true;


        // ????? ??????? ��????? ?????? ??
        Character two = FindAutoMergeHero(newHeroList);

        if (two == null)
        {
            Debug.Log($"NULL");
            isMerging = false;
            one.isAutoSelected = false;
            return;
        }
        two.isAutoSelected = true;
        
        coroutineMoveToTarget = MoveToTargetHeroPosition(one, two);
        StartCoroutine(coroutineMoveToTarget);
        
    }


    float GetMergeSpeed() 
    {
        var speed = GameController.instance.abilityManager.TotalAbilityData(AbilityType.MergeSpeed);

        var defaultSpeed = Define.DEFAULT_MERGE_SPEED;

        return defaultSpeed + speed;

    }

    // IEnumerator MoveToTargetHeroPosition(Character s, Character e)
    // {

    //     Character sHero = s;
    //     Character eHero = e;
    //     Vector3 newSpawnPos = TargetPosition;
        
    //     var speed = GetMergeSpeed();
    //     //Debug.Log($"MERGE SPEED : {speed}");
    //     float sDis = Vector3.Distance(sHero.CurrentPosition, newSpawnPos) * speed;
    //     float eDis = Vector3.Distance(eHero.CurrentPosition, newSpawnPos) * speed;

    //     while (true)
    //     {
    //         sHero.CurrentPosition = Vector3.MoveTowards(sHero.CurrentPosition, newSpawnPos, Time.deltaTime * sDis);
    //         eHero.CurrentPosition = Vector3.MoveTowards(eHero.CurrentPosition, newSpawnPos, Time.deltaTime * eDis);

    //         if (Vector3.Distance(sHero.CurrentPosition,newSpawnPos) <= 0.1f && Vector3.Distance(eHero.CurrentPosition, newSpawnPos) <= 0.1f)
    //         {
    //             sHero.LevelUpHero();
    //             eHero.DisableHero();
    //             RemoveCharacterList(eHero);
    //             isMerging = false;

    //             yield break;
    //         }

    //         yield return null;
    //     }
    // }
    IEnumerator MoveToTargetHeroPosition(Character s, Character e)
    {
        Character sHero = s;
        Character eHero = e;
        Vector3 newSpawnPos = TargetPosition;

        var speed = GetMergeSpeed();
        //Debug.Log($"MERGE SPEED : {speed}");
        float sDis = Vector3.Distance(sHero.CurrentPosition, newSpawnPos) * speed;
        float eDis = Vector3.Distance(eHero.CurrentPosition, newSpawnPos) * speed;

        float journeyLengthS = Vector3.Distance(sHero.CurrentPosition, newSpawnPos);
        float journeyLengthE = Vector3.Distance(eHero.CurrentPosition, newSpawnPos);

        float startTime = Time.time;

        while (true)
        {
            float distCoveredS = (Time.time - startTime) * sDis;
            float fracJourneyS = Mathf.Min(distCoveredS / journeyLengthS, 1f);

            float distCoveredE = (Time.time - startTime) * eDis;
            float fracJourneyE = Mathf.Min(distCoveredE / journeyLengthE, 1f);

            sHero.CurrentPosition = Vector3.Lerp(sHero.CurrentPosition, newSpawnPos, fracJourneyS);
            eHero.CurrentPosition = Vector3.Lerp(eHero.CurrentPosition, newSpawnPos, fracJourneyE);

            if (fracJourneyS >= 0.9f && fracJourneyE >= 0.9f)
            {
                sHero.LevelUpHero();
                eHero.DisableHero();
                RemoveCharacterList(eHero);
                isMerging = false;

                yield break;
            }

            yield return null;
        }
    }
    public int GetHighestHeroLevel()
    {
        //현재 고용중인 최고 등급의 영웅 레벨 가져오기
        List<Character> chaList = GetHeroList();

        if(chaList.Count > 0)
        {
           
            int temp = chaList[chaList.Count - 1].heroLv;

            for (int i = 0; i < chaList.Count; i++)
            {
                if (chaList[i].heroLv > temp)
                    temp = chaList[i].heroLv;
            }

            return temp; 
        }
        else
        {
            return 0;
        }

    }

    public bool CheckEqualLevel(Character self, Character target)
    {
        if (self.heroLv == target.heroLv)
        {
            return true;
        }

        return false;
    }

    public bool CheckHeroMaxLevel(int level)
    {
        if (level >= Define.MAX_HERO_LEVEL - 1)
        {
            return true;
        }

        return false;

    }

    public void AddCharacterList(Character character)
    {
        heroList.Add(character);
        GameController.instance.MergeManager.CurrentBattleMercenary = heroList.Count;

    }

    //??????? ?�� ????
    public void RemoveCharacterList(Character character)
    {
        if (heroList.Contains(character))
        {
            heroList.Remove(character);
            GameController.instance.MergeManager.CurrentBattleMercenary = heroList.Count;
        }

    }

    //??????? ?????? ????
    public void ChangeHeroStartLevel(int level)
    {
        for (int i = 0; i < heroList.Count; i++)
        {
            if (heroList[i].heroLv < level)
            {
                heroList[i].ChangeStartLevel(level);
            }
        }

    }

    public void SaveHeroList(List<int> hList)
    {
        hList.Clear();

        for (int i = 0; i < heroList.Count; i++)
        {
            hList.Add(heroList[i].heroLv);
        }

    }

    //?????????? ?????? 2?? ????? ????? ???
    MercenaryType FindEqualHeroGrade(List<Character> list)
    {
        if (list == null || list.Count == 0)
        {
            return 0;
        }

        Dictionary<MercenaryType, int> levelCounts = new Dictionary<MercenaryType, int>();

        foreach (Character hero in list)
        {
            if (levelCounts.ContainsKey(hero.heroType))
            {
                levelCounts[hero.heroType]++;
            }
            else
            {
                levelCounts.Add(hero.heroType, 1);
                //levelCounts[hero.heroType] = 1;
            }
        }
        

        MercenaryType levelWithMultipleHeroes = MercenaryType.Max;
        foreach (MercenaryType level in levelCounts.Keys)
        {

            //???? ?????? ????? ???��???? ???
            if (GameController.instance.CardManager.GetHeroLockState(level) || CheckHeroMaxLevel((int)level))
            {
                //Debug.Log(level + " : ?????? ????? ??????");
                //Debug.Log($"Max LEVEL");
                continue;
            }

            if (levelCounts[level] > 1 && level < levelWithMultipleHeroes)
            {
                levelWithMultipleHeroes = level;
            }
        }

        //Debug.Log(levelWithMultipleHeroes+"???? ???? ???");
        return levelWithMultipleHeroes;
    }

    //???? ??????? ?????? ?? ??? ??????? ????
    public List<Character> GetChractersByType(MercenaryType grade)
    {
        return heroList.Where(w => w.heroType == grade).ToList();
    }
   
    //???? ??????? ��???????? ?? ?????? ???? ???? ��???? ????? ????????
    Character FindAutoMergeHero(List<Character> chracters)
    {
        Character select = null;

        foreach (Character hero in chracters)
        {
            if (!hero.isAutoSelected && hero.isStopSpawnMove)
            {
                select = hero;
                break;
            }
        }

        return select;
    }

    public void ClearHeroList()
    {
        heroList.Clear();
    }






    //???? ??
    private bool isSkillInUse = false;
    private float skillDuration = 30f;
    private float skillCooldown = 0f;
    private float skillCooldownDuration = 30f;

    public void UseSkill()
    {
        if (!isSkillInUse && Time.time >= skillCooldown)
        {
            isSkillInUse = true;
            skillCooldown = Time.time + skillCooldownDuration;

            StartCoroutine(ActivateSkill());
        }
    }

    private IEnumerator ActivateSkill()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        // ??? ???? ?? ???? ????
        while (elapsedTime < skillDuration)
        {
            elapsedTime = Time.time - startTime;
            // ??? ???? ?? ???

            yield return null;
        }

        isSkillInUse = false;
        skillCooldown = Time.time + skillCooldownDuration;
    }






}
