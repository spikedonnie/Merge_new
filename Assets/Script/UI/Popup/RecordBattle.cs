using UnityEngine;

public class RecordBattle : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;


    void OnEnable()
    {
        particle.Stop();
        particle.Play();
    }


}