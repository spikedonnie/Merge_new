using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyBase : MonoBehaviour, Unit
{
    public Enemy enemyData;
    private Vector3 hpPosition;
    public Animator baseEnemyAnimator;
    private HpBar mHpBar;
    private GameObject mHpBarPrefab;
    private SortingGroup mSortingGroup;
    private AnimData animInData;
    private AnimData animOutData;
    public MonsterInOutAnimator inOutAnimator;

    private float attackDelayTime = 1f;
    private float damage = 1f;

    public float enemyCurrentHp;
    public int stageIndex;
    public int enemyCount;
    public int sortingOderIndex;
    public bool isArriveHitPosition = false;
    public bool isDie = false;
    public int returnImageID;
    public int bgID;
    public float CurrentHp
    {
        get { return enemyCurrentHp; }
        set
        {
            enemyCurrentHp = value;
        }
    }

    private void Awake()
    {
        baseEnemyAnimator = GetComponent<Animator>();
        mSortingGroup = GetComponent<SortingGroup>();
        inOutAnimator = GetComponent<MonsterInOutAnimator>();
        animInData = GetComponent<AnimData>();
        inOutAnimator.enemyBase = this;
        inOutAnimator.animDataIn = animInData;
    }

    private void Update()
    {
        if (mHpBar != null)
        {
            mHpBar.UpdateHpBar(HpPosition());
        }
    }

    public void ReduceHealth(float reduce)
    {
        if (isDie) return;

        mHpBar.UpdateUIHealth(reduce, CurrentHp);
        CurrentHp -= reduce;
    }

    public void InitEnemy()
    {
        isArriveHitPosition = false;

        //�� ü�� ����
        enemyCurrentHp = enemyData.HP;
        //���� ������ �������� �̵�
        StartCoroutine(inOutAnimator.AnimEnemyIn(() => isArriveHitPosition = true));
        //3�ʸ��� ����
        InvokeRepeating("AttackAnimation", 1f, attackDelayTime);
        //Hp Bar UI ����
        SetHpBar();

        if (enemyData.EnemyTYPE.Equals(EnemyTYPE.Common))
        {
            transform.localScale = Vector3.one;
        }
        else if (enemyData.EnemyTYPE.Equals(EnemyTYPE.Elite))
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

    }

    private void AttackAnimation()
    {
        if (GameController.instance.IsGameStop) return;
        inOutAnimator.PlayAttackAnimation();
    }

    private Vector3 HpPosition()
    {
        hpPosition = transform.position;
        hpPosition.y = hpPosition.y - 0.3f;
        return hpPosition;
    }

    private void SetHpBar()
    {
        mHpBarPrefab = ObjectPool.instance.GetObject(EUIPoolName.HpBar);

        mHpBar = mHpBarPrefab.GetComponent<HpBar>();

        mHpBar.SettingHpBar(transform, enemyData.HP);
    }

    public void ReturnHpBar()
    {
        if (mHpBarPrefab != null)
        {
            ObjectPool.instance.ReturnObject(mHpBarPrefab, EUIPoolName.HpBar);
            mHpBar = null;
        }
    }

    public void TakeDamage()
    {
        List<Character> list = GameController.instance.mergeController.GetHeroList();

        if (list.Count > 0 && !GameController.instance.IsGameStop)
        {
            list[Random.Range(0, list.Count)].Hit(damage);
        }
        else
        {
            Debug.Log("������ �÷��̾ ����");
        }
    }

    public virtual void Die()
    {
        CancelInvoke("AttackAnimation");

        CurrentHp = 0;

        isArriveHitPosition = false;
        isDie = true;

        if (enemyData.EnemyTYPE.Equals(EnemyTYPE.Elite) || enemyData.EnemyTYPE.Equals(EnemyTYPE.Unique))
            GameController.instance.BattleManager.IsBoss = false;

        ReturnHpBar();

        //�� ��� �ִϸ��̼� ����
        StartCoroutine(inOutAnimator.AnimEnemyOut());

    }

    public void ForceCommonEliteKill()
    {
        CancelInvoke("AttackAnimation");
        isArriveHitPosition = false;
        isDie = true;
        ReturnHpBar();
        ResetScale();
        ObjectPool.instance.ReturnNormalMonster(returnImageID);
    }

    public void ForceUniqueKill()
    {
        CancelInvoke("AttackAnimation");
        isArriveHitPosition = false;
        isDie = true;
        ReturnHpBar();
        ResetScale();
        ObjectPool.instance.ReturnUniqueMonster(returnImageID);
    }

    public void ResetScale()
    {
        if (enemyData.EnemyTYPE.Equals(EnemyTYPE.Common))
        {
            transform.localScale = Vector3.one;
        }
        else if (enemyData.EnemyTYPE.Equals(EnemyTYPE.Elite))
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }
}