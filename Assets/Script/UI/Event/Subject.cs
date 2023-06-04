using System.Collections.Generic;
using UnityEngine;

public class Subject
{
    private List<Observer> questObserverList = new List<Observer>();
    private int numObservers = 0;

    public void AddObserver(Observer ob)
    {
        questObserverList.Add(ob);
    }

    public void RemoveObserver(Observer ob)
    {
        questObserverList.Remove(ob);
        
    }

    protected void Notify(Entity entity, Event e)
    {
        for (int i = 0; i < numObservers; i++)
        {
            questObserverList[i].onNotify(entity, e);
        }
    }
}