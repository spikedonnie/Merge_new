using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RewardNewUser : MonoBehaviour
{

    public EventSlot[] eventSlots;
    public void Init(EventUI eventUI)
    {
        //몇일차까지 진행했는지 가져온다.
        var count = GameDataManager.instance.GetSaveData.eventCount;

        for (int i = 0; i < eventSlots.Length; i++)
        {
            //일차별로 데이터를 가져온다.
            EventNewUser newUser = GameDataManager.instance.SheetJsonLoader.GetEventNewUserData(i + 1);
            

            if(i < count)
            {
                eventSlots[i].Init(newUser, true,eventUI);
            }
            else
            {
                eventSlots[i].Init(newUser, false,eventUI);

            }

        }
        if(count < eventSlots.Length)
        eventSlots[count].Interaction(true);

    }

}

