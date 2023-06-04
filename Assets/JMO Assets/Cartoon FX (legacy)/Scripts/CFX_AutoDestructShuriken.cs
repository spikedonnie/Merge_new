using System.Collections;
using UnityEngine;

// Cartoon FX  - (c) 2015 Jean Moreno

// Automatically destructs an object when it has stopped emitting particles and when they have all disappeared from the screen.
// Check is performed every 0.5 seconds to not query the particle system's state every frame.
// (only deactivates the object if the OnlyDeactivate flag is set, automatically used with CFX Spawn System)

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
    [SerializeField]
    public EPoolName ePoolName;

    // If true, deactivate the object instead of destroying it
    public bool OnlyDeactivate;

    WaitForSeconds second;

    private void Start()
    {
        second = new WaitForSeconds(0.1f);
    }

    private void OnEnable()
    {
        StartCoroutine("CheckIfAlive");
    }

    private IEnumerator CheckIfAlive()
    {
        ParticleSystem ps = this.GetComponent<ParticleSystem>();

        while (true && ps != null)
        {
            yield return second;
            
            if (!ps.IsAlive(true))
            {
                ObjectPool.instance.ReturnObject(this.gameObject, ePoolName);

                break;
            }
        }
    }
}