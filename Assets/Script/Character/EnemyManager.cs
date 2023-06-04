using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public void SetDataToEnemyType(EnemyBase eBase)
    {
        Enemy eData = GameDataManager.instance.SheetJsonLoader.GetEnemyData(eBase.enemyData.EnemyTYPE, eBase.stageIndex);
        DeepCopyEnemyData(eBase, eData);
    }

    //Sheet Data�� �����ÿ��� null���� ���õȴ�.
    private void DeepCopyEnemyData(EnemyBase type, Enemy eData)
    {
        type.enemyData.ID = eData.ID;
        type.enemyData.HP = eData.HP;
        type.enemyData.GOLD = eData.GOLD;
        type.enemyData.MONSTER_IMAGE_ID = eData.MONSTER_IMAGE_ID;
        type.enemyData.EnemyTYPE = eData.EnemyTYPE;
        type.enemyData.NAME = eData.NAME;
        type.enemyData.DIAMOND = eData.DIAMOND;
        type.enemyData.STONE = eData.STONE;
    }
}