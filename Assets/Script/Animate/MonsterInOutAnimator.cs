using DG.Tweening;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterInOutAnimator : MonoBehaviour
{
    public EnemyBase enemyBase;
    public AnimData animDataIn;
    public Vector3 startPoint_MonsterIn;
    public Vector3 endPoint_MonsterIn;
    private bool isAnimPlay;

    public IEnumerator AnimEnemyIn(Action callBack)
    {
        enemyBase.isDie = false;
        var startPos = startPoint_MonsterIn;

        var enaPos = endPoint_MonsterIn;

        transform.position = startPos;

        enemyBase.baseEnemyAnimator.ResetTrigger("Die");

        enemyBase.baseEnemyAnimator.SetBool("Walk", true);

        animDataIn.ResetAnimData();

        while (animDataIn.animTime < 1.499f)
        {
            animDataIn.animTime = (Time.time - animDataIn.animStartTime) / animDataIn.animDuration;
            animDataIn.animValue = EaseValues.instance.GetAnimCurve(animDataIn.animCurveType, animDataIn.animTime);
            transform.position = Vector3.Lerp(startPos, enaPos, animDataIn.animValue);
            var value = Mathf.Lerp(0, 1, animDataIn.animValue);
            enemyBase.baseEnemyAnimator.speed = value;
            yield return null;
        }

        enemyBase.baseEnemyAnimator.SetBool("Walk", false);

        if (callBack != null) callBack.Invoke();
    }

    public IEnumerator AnimEnemyOut()
    {
        enemyBase.baseEnemyAnimator.SetTrigger("Die");

        //isAnimPlay = true;

        //while (monsterAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        //{
        //    yield return null;
        //}

        EventManager.Instance.RunEvent<EnemyTYPE>(CallBackEventType.TYPES.OnReward, enemyBase.enemyData.EnemyTYPE);

        yield return new WaitForSeconds(1f);

        //isAnimPlay = false;
        if (enemyBase.enemyData.EnemyTYPE.Equals(EnemyTYPE.Unique))
        {
            ObjectPool.instance.ReturnUniqueMonster(enemyBase.returnImageID);
        }
        else
        {
            enemyBase.ResetScale();
            ObjectPool.instance.ReturnNormalMonster(enemyBase.returnImageID);
        }

        EventManager.Instance.RunEvent<EnemyTYPE>(CallBackEventType.TYPES.OnKillEnemy, enemyBase.enemyData.EnemyTYPE);
    }

    //�Ը��� �°� �����ϼ���.
    public void PlayHitAnimation()
    {
        enemyBase.baseEnemyAnimator.SetTrigger("Hit");
    }

    public void PlayAttackAnimation()
    {
        enemyBase.baseEnemyAnimator.SetTrigger("Attack");
    }
}