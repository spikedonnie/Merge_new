using System.Collections.Generic;
using UnityEngine;

public enum EQuestType
{
    Quest1,
    Quest2,
    Quest3,
    Quest4,
    Quest5,
}

public class Quest
{
    public string questName;
    public EQuestType questType;
    public int targetScore;
}

public class QuestManager : MonoBehaviour
{
    private delegate void QuestCheckFunction(Quest quest);

    private Dictionary<EQuestType, QuestCheckFunction> mCheckFunction;

    private readonly List<Quest> mQuests = new List<Quest>();

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        mCheckFunction = new Dictionary<EQuestType, QuestCheckFunction>()
        {
            {EQuestType.Quest1, CheckComplete},
            {EQuestType.Quest2, CheckComplete},
            {EQuestType.Quest3, CheckComplete},
            {EQuestType.Quest4, CheckComplete},
            {EQuestType.Quest5, CheckComplete},
        };

        mQuests.Clear();
        //mQuests.Capacity = TBL_Quest.CountEntities;
    }

    //TBL_Quest.ForEachEntity(questData =>
    //{
    //var quest = new Quest(questData);
    //mQuest.Add(Quest);
    //});

    private void CheckComplete(Quest quest)
    {
    }
}