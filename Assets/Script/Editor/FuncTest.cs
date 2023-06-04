using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework.Constraints;


[CustomEditor(typeof(Test))]
public class FuncTest : Editor
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var style = new GUIStyle(GUI.skin.button);
        style.fontSize = 20;

        Test test = (Test)target;

        if (GUILayout.Button("Elite Count Max", style))
        {
            GameController.instance.BattleManager.SetStepMax(10);
        }

        if (GUILayout.Button("Set Upgrade Level", style))
        {
            GameController.instance.trainingManager.SetCustomLevel(test.type, test.upgradeLevel);
        }

        if (GUILayout.Button("Stage Jump", style))
        {
            GameController.instance.BattleManager.SetStage(test.jumpStage);

            GameController.instance.BattleManager.SetStepMax(10);
        }

        if (GUILayout.Button("Save Data", style))
        {
            GameDataManager.instance.SaveData();
        }

        if (GUILayout.Button("Enemy Force Kill", style))
        {
            GameController.instance.BattleManager.enemyBase.Die();
        }
        if (GUILayout.Button("Auto Game", style))
        {
            GameController.instance.AutoPlayGame();
        }
        if (GUILayout.Button("Hero Level Up", style))
        {
            GameController.instance.mergeController.UpHeroLv();
        }


    }
}