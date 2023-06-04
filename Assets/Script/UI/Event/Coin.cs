using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector3 startPosition;
    public SpriteRenderer coinRenderer;
    private void Awake()
    {
        startPosition = transform.position;
    }

    public void Shot()
    {
        transform.DOMoveX(Random.Range(startPosition.x - 1.5f, startPosition.x + 1.5f), 0.1f).OnComplete(() => coinRenderer.DOFade(0, 1f).OnComplete(DisableCoin));
    }

    private void DisableCoin()
    {
        transform.position = startPosition;
        coinRenderer.color = Color.white; 
        ObjectPool.instance.ReturnObject(this.gameObject, EPoolName.Coin);
    }


}