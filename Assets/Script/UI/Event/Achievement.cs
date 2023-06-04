using UnityEngine;

public class Achievement : Observer
{
    public override void onNotify(Entity entity, Event e)
    {
        switch (e)
        {
            default:
                break;
        }
    }

    private void Unlock(AchieveNode achive)
    {
    }
}