using UnityEngine;

public class EffectBase : MonoBehaviour
{
    //private Animator anim;
    [SerializeField] public EPoolName ePoolName;
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                ObjectPool.instance.ReturnObject(this.gameObject, ePoolName);
            }
        }
    }


    //private void Awake()
    //{
    //    anim = GetComponent<Animator>();
    //}

    //private void Update()
    //{
    //    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
    //    {
    //        End();
    //    }
    //}

    //private void End()
    //{
    //    ObjectPool.instance.ReturnObject(this.gameObject, ePoolName);
    //}
}