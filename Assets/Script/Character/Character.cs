using DG.Tweening;
using SolClovser.EasyMultipleHealthbar;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static EventManager;

[System.Serializable]
public class Character : MonoBehaviour, Unit
{
    private float maxHp;
    private float waitAttackingCurrentTime;
    private float currentHp;
    private float minAttackSpeed = -0.3f; 
    private float maxAttackSpeed = 0.3f; 
    
    public int heroLv;

    [SerializeField] private AttackType attackType;
    public MercenaryType heroType; 
    
    public CharacterSpriteInfo mCharacterSpriteInfo;
    public GameObject projectile_Magic_Prefab;
    public GameObject projectilePrefab;

    private Animator animator;
    private Vector3 currentPosition;
    private Vector3 mAttackPoint;
    private Vector3 hitPosition; 
    private SpriteRenderer[] attackSprites;
    private SpriteRenderer[] dieSprites;
    private SpriteRenderer[] hitSprites;
    private SpriteRenderer[] idleSprites;
    private SpriteRenderer[] walkSprites;

    public BoxCollider2D col2D = null;
    public SortingGroup sortingLayer;
    public bool IsDie;
    public bool IsGoingMerge;
    public bool isStopSpawnMove;

    public bool isSelected;
    public bool isAutoSelected;

    WaitForSeconds waitForSecond;
    float totalWakeTime;

    public Vector3 CurrentPosition
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    //test code
    public void TestLvUp()
    {
        if (GameController.instance.mergeController.CheckHeroMaxLevel(heroLv)) return;
        if (GameDataManager.instance.GetSaveData.heroLock[heroLv]) return;
        heroLv++;
        heroType = (MercenaryType)heroLv;
        attackType = GameController.instance.DamageManager.GetAttackTypeByPlayerType(heroType);
        mCharacterSpriteInfo = Utils.GetItemVisualByName(((MercenaryType)heroLv).ToString());
        gameObject.name = heroType.ToString();
        SpriteSetting();
        sortingLayer.sortingOrder = heroLv;
        if (CheckNewHero(heroType))
        {
            UIController.instance.OpenNewHeroPopUp(heroType);
        }
    }

    private void Awake()
    {
        idleSprites = transform.Find("Idle").GetComponentsInChildren<SpriteRenderer>();
        walkSprites = transform.Find("Walk").GetComponentsInChildren<SpriteRenderer>();
        hitSprites = transform.Find("Hit").GetComponentsInChildren<SpriteRenderer>();
        dieSprites = transform.Find("Die").GetComponentsInChildren<SpriteRenderer>();
        attackSprites = transform.Find("Attack").GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col2D = GetComponent<BoxCollider2D>();
        hitPosition = new Vector3(0, +0.5f, 0);
        maxHp = 2;
        currentHp = maxHp;
    }

    private void Start()
    {
        currentPosition = transform.position;
        mAttackPoint = transform.position;
        mAttackPoint.x = mAttackPoint.x + 2;
        IsGoingMerge = false;
        isAutoSelected = false;
        isSelected = false;
        waitAttackingCurrentTime = Define.DEFAULT_MERCENARY_ATTACK_SPEED;
        waitAttackingCurrentTime = waitAttackingCurrentTime + UnityEngine.Random.Range(waitAttackingCurrentTime + minAttackSpeed, waitAttackingCurrentTime + maxAttackSpeed);
        waitForSecond = new WaitForSeconds(waitAttackingCurrentTime);
    }

    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }
    //잡고 있는중
    public void GrabAndMove(Vector3 position)
    {
        transform.position = position;
    }

    public void SetEnableCollider(bool flag) 
    {
        col2D.enabled = flag;
        IsGoingMerge = !flag;
    }


    public void CreateCharacter(int lv)
    {
        isAutoSelected = false;
        isSelected = false; 
        isStopSpawnMove = false;
        IsGoingMerge = false;
        //애니메이션 세팅
        animator.ResetTrigger("Die");
        IsDie = false;

        //사운드 재생
        AudioManager.Instance.PlaySound(PoolAudio.CREATEHERO);

        //영웅 레벨 및 타입 세팅
        heroLv = lv;
        heroType = (MercenaryType)heroLv;
        attackType = GameController.instance.DamageManager.GetAttackTypeByPlayerType(heroType);
        gameObject.name = heroType.ToString();

        //생성 위치 및 타겟 위치 세팅
        transform.position = GameController.instance.mergeController.SpawnPosition;
        //스프라이트 및 레이어 세팅
        mCharacterSpriteInfo = Utils.GetItemVisualByName(((MercenaryType)heroLv).ToString());
        SpriteSetting();
        sortingLayer.sortingOrder = heroLv;
        
        GameController.instance.mergeController.AddCharacterList(this);
        StartCoroutine("MoveDestination");
        StartCoroutine("CheckAttackDelayTime");
    }

    IEnumerator MoveDestination()
    {
        Vector3 destination = GameController.instance.mergeController.TargetPosition;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 10 * Time.deltaTime);

            if (Vector3.Distance(transform.position, destination)  <= 0.1f)
            {
                isStopSpawnMove = true;
                yield break;
            }

            yield return null;
        }

    }

    public void LevelUpHero()
    {
        if (GameController.instance.mergeController.CheckHeroMaxLevel(heroLv)) return;

        //애니메이션 세팅
        animator.ResetTrigger("Die");

        //사운드 재생

        //Debug.Log($"Before Level : {heroLv}");
        heroLv = CalcMergeLevelJump(heroLv);

        //영웅 레벨 및 타입 세팅
        heroLv++;
        //Debug.Log($"After Level : {heroLv}");

        heroType = (MercenaryType)heroLv;
        attackType = GameController.instance.DamageManager.GetAttackTypeByPlayerType(heroType);
        gameObject.name = heroType.ToString();
        //생성 위치 및 타겟 위치 세팅
        //transform.position = startPos;
        //스프라이트 및 레이어 세팅
        mCharacterSpriteInfo = Utils.GetItemVisualByName(((MercenaryType)heroLv).ToString());
        SpriteSetting();
        sortingLayer.sortingOrder = heroLv;
        GameController.instance.AudioManager.PlayEffect(ESoundEffect.Merge);
        GameController.instance.MergeManager.CreateEffect(transform.position + hitPosition);

        IsDie = false;

        if (CheckNewHero(heroType))
        {
            UIController.instance.OpenNewHeroPopUp(heroType);
        }

        isStopSpawnMove = true;
        isAutoSelected = false;
        isSelected = false;

        StartCoroutine("CheckAttackDelayTime");
    }

    int CalcMergeLevelJump(int level)
    {
        var bMerge = GameController.instance.abilityManager.TotalAbilityData(AbilityType.MergeJump);

        //현재 고용중인 최고 등급의 영웅 레벨 가져오기
        int highest = GameController.instance.mergeController.GetHighestHeroLevel();

        if(level >= highest)
        {
            return level;
        }

        if (bMerge > 0)
        {
            var merge = UnityEngine.Random.Range(0, 100);

            if (merge < bMerge)
            {
                ObjectPool.instance.GetObject(EPoolName.JumpEffect).transform.position = transform.position;

                return level + 1;
            }
        }

        return level;

    }


    public void ReturnLayerOder()
    {
        sortingLayer.sortingOrder = heroLv;

    }

    public MercenaryType GetHeroType()
    {
        return heroType;
    }

    public void Hit(float dmg)
    {
        if (IsDie || GameController.instance.IsGameStop) return;
        ObjectPool.instance.GetObject(EPoolName.HeroHit).transform.position = transform.position + hitPosition;

        currentHp -= dmg;
        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    public void DisableHero()
    {
        StopAllCoroutines();
        ObjectPool.instance.ReturnObject(gameObject, EPoolName.Character);

    }

    public void ChangeStartLevel(int level)
    {
        heroLv = level;
        heroType = (MercenaryType)heroLv;
        attackType = GameController.instance.DamageManager.GetAttackTypeByPlayerType(heroType);
        mCharacterSpriteInfo = Utils.GetItemVisualByName(((MercenaryType)heroLv).ToString());
        SpriteSetting();
        sortingLayer.sortingOrder = heroLv;

        if (CheckNewHero(heroType))
        {
            UIController.instance.OpenNewHeroPopUp(heroType);
        }
    }

    public void TakeDamage()
    {
        if (IsDie || GameController.instance.IsGameStop) return;

        switch (attackType)
        {
            case AttackType.WARRIOR:
                //공격 이펙트

                //데미지 처리
                EventManager.Instance.RunEvent(CallBackEventType.TYPES.OnHitEnemy, heroType, transform.position);
                GameController.instance.AudioManager.PlaySound(PoolAudio.MELEE);
                break;

            case AttackType.RANGER:
                //궁수면 투사체 발사
                GameController.instance.AudioManager.PlaySound(PoolAudio.RANGE);
                GetPoolProjectile();
                break;

            case AttackType.MAGIC:
                //법사면 투사체 발사
                GameController.instance.AudioManager.PlaySound(PoolAudio.MAGIC);
                GetPoolProjectile();
                break;

            default:
                break;
        }


        //if ((GameController.instance.UnitManager.Enemy != null
        //    && GameController.instance.UnitManager.Enemy.isArriveHitPosition)
        //    || GameController.instance.DungeonManager.Enemy != null)
    }

    void GetPoolProjectile()
    {

        var arrow = ObjectPool.instance.GetObject(EPoolName.Projectile);
        arrow.GetComponent<Projectile>().Init(GameController.instance.BattleManager.RandomTargetPosition(), transform.position, heroType);
    }

    private IEnumerator CheckAttackDelayTime()
    {
        while (true) 
        {

            yield return null;

            //영웅이 죽거나 게임이 스탑일때 루프 돌면서 대기
            while (IsDie || GameController.instance.IsGameStop || GameController.instance.BattleManager.enemyBase == null)
            {
                //Debug.Log("영웅이 죽거나 게임이 스탑일때 루프 돌면서 대기");
                yield return null;
            }

            yield return waitForSecond;

            //적이 아직 공격가능한 범위에 오지 않았거나 적이 죽었다면 계속 대기
            while (!GameController.instance.BattleManager.enemyBase.isArriveHitPosition || GameController.instance.BattleManager.enemyBase.isDie)
            {
                //Debug.Log("적이 아직 공격가능한 범위에 오지 않았거나 적이 죽었다면 계속 대기");
                yield return null;
            }


            if (!IsDie)
            {
                animator.ResetTrigger("Attack");
                SetaActionState(ActionStateType.Attack);
            }

        }

    }


    private bool CheckNewHero(MercenaryType type)
    {
        var maxIndex = GameDataManager.instance.GetSaveData.heroTypeMaxIndex;

        if((int)type > maxIndex)
        {
            GameDataManager.instance.GetSaveData.heroTypeMaxIndex++;
            return true;
        }

        return false;
    }

    private void Die()
    {
        //RequestAHealthbar();
        SetaActionState(ActionStateType.Die);
        IsDie = true;
        StartCoroutine(WakeUp());
    }

    private void SetaActionState(ActionStateType state)
    {
        animator.SetTrigger(state.ToString());
    }

    private void SpriteSetting()
    {
        for (int i = 0; i < idleSprites.Length; i++)
        {
            idleSprites[i].sprite = mCharacterSpriteInfo.idleSprite[i];
        }
        for (int i = 0; i < walkSprites.Length; i++)
        {
            walkSprites[i].sprite = mCharacterSpriteInfo.WalkSprite[i];
        }
        for (int i = 0; i < hitSprites.Length; i++)
        {
            hitSprites[i].sprite = mCharacterSpriteInfo.hurtSprite[i];
        }
        for (int i = 0; i < attackSprites.Length; i++)
        {
            attackSprites[i].sprite = mCharacterSpriteInfo.AttackSprite[i];
        }
        for (int i = 0; i < dieSprites.Length; i++)
        {
            dieSprites[i].sprite = mCharacterSpriteInfo.dieSprite[i];
        }
    }

    private IEnumerator WakeUp()
    {
        yield return null;
        totalWakeTime = GameController.instance.abilityManager.TotalAbilityData(AbilityType.RecoveryHeroHP);
        yield return new WaitForSeconds(totalWakeTime);
        currentHp = maxHp;
        IsDie = false;
    }


    public void ForceMove(Vector3 targetPos)
    {
        if(isAutoSelected) return;
        isStopSpawnMove = true;
        transform.position = targetPos;
    }

}