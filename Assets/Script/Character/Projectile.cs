using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MercenaryType mType;
    private Vector3 offset;
    private Vector3 target;

    public bool IsEndBattle { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector3 t, Vector3 current, MercenaryType type)
    {
        mType = type;
        IsEndBattle = false;
        target = t;
        target.x = UnityEngine.Random.Range(target.x - 1f, target.x + 1f);
        transform.position = current;
        spriteRenderer.sprite = Utils.GetProjectileSprite(GameDataManager.instance.SheetJsonLoader.GetPlayerData(type).ArrowType).uiSprite;
        StartCoroutine(MoveTarget());
    }

    private IEnumerator MoveTarget()
    {
        while (true)
        {
            if (IsEndBattle)
            {
                Destroy(gameObject);

                yield break;
            }

            offset = target - transform.position;

            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5f);

            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (offset.sqrMagnitude < 0.1f)
            {
                EventManager.Instance.RunEvent(CallBackEventType.TYPES.OnHitEnemy, mType, transform.position);

                Destroy(gameObject);
                yield break;
            }


            yield return null;
        }
    }
}