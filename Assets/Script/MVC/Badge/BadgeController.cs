using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BadgeController : MonoBehaviour
{
    List<BadgeModel> badgeModelList;
    public BadgeView badgeView;
    
    public IEnumerator Init()
    {
        badgeModelList = new List<BadgeModel>();

        for (BADGE_TYPE i = 0; i < BADGE_TYPE.MAX; i++)
        {
            var badge = GameDataManager.instance.SheetJsonLoader.GetBadgeData(i);

            badge.badgeIconSprite = Utils.GetBadgeSprite(badge.badgeName);
            badgeModelList.Add(badge);
        }  
        
        badgeView.InitView(badgeModelList);

        yield return null;
    }






}
