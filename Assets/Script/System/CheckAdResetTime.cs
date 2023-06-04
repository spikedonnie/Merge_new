using System;
using UnityEngine;


public class CheckAdResetTime
{

    public void CheckOverDayTime(Action<string> callBack)
    {
        DateTime now = DateTime.Now;
        DateTime scheduledTime = DateTime.Parse(GameDataManager.instance.GetSaveData.adResetTime);//???? ?©£?

        if (now > scheduledTime)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0);
            scheduledTime = scheduledTime.Date + ts;
            scheduledTime = scheduledTime.AddDays(1);
            
            if (callBack != null)
            {
                callBack.Invoke(scheduledTime.ToString("yyyy/MM/dd HH:mm"));
            }
        }

    }

    //???? ?©£??? ???? ?©£??? ???????? ??
    public void CheckOverEventNewUserDayTime(Action<string> callBack)
    {
        DateTime now = DateTime.Now;//???? ?©£?
        DateTime scheduledTime = DateTime.Parse(GameDataManager.instance.GetSaveData.eventNewUserTime);//???? ?©£?

        //???? ?©£??? ???? ???¨ö©£??? ??????
        if (now > scheduledTime)
        {
            // ??????????? ?????? 0?? 0?? 0??? ???????
            TimeSpan ts = new TimeSpan(0, 0, 0);
            scheduledTime = scheduledTime.Date + ts;
            scheduledTime = scheduledTime.AddDays(1);

            //?????? ??????? ?????? ???? ???
            if (callBack != null)
            {
                callBack.Invoke(scheduledTime.ToString("yyyy/MM/dd HH:mm"));
            }
        }

    }

    public bool IsOverEventResetTime()
    {
        DateTime now = DateTime.Now;//???? ?©£?
        DateTime scheduledTime = DateTime.Parse(GameDataManager.instance.GetSaveData.eventNewUserTime);//???? ?©£?

        if(now > scheduledTime)
        {
            return true;
        }

        return false;
    }


    //public bool CheckAdTime()
    //{
    //    string oldTime = GameDataManager.instance.GetSaveData().adResetTime;

    //    DateTime sysTime = DateTime.Now;

    //    DateTime oldDateTime = DateTime.Parse(oldTime);

    //    TimeSpan ts = sysTime - oldDateTime;

    //    float diffmin = (float)ts.Hours;

    //    if (diffmin >= 24)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        //PopUpCanvasUI.ShowOneButtonPopupp(string.Format("{0:F0}?©£? {1:F0}?? ?¨¨? ?????? ?????? ????", hour, min), null);
    //        return false;
    //    }
    //}

    //public int AdChargeHour()
    //{

    //    string oldTime = GameDataManager.instance.GetSaveData().adResetTime;

    //    DateTime sysTime = DateTime.Now;

    //    DateTime oldDateTime = DateTime.Parse(oldTime);

    //    TimeSpan ts = sysTime - oldDateTime;

    //    float diffHour = (float)ts.TotalHours;

    //    return 23 - (int)diffHour;
    //}

    //public int AdChargeMinite()
    //{

    //    string oldTime = GameDataManager.instance.GetSaveData().adResetTime;

    //    DateTime sysTime = DateTime.Now;

    //    DateTime oldDateTime = DateTime.Parse(oldTime);

    //    TimeSpan ts = sysTime - oldDateTime;

    //    float diffmin = (float)ts.TotalMinutes;

    //    int min = (int)(diffmin % 60);

    //    return 59 - min;
    //}







}
