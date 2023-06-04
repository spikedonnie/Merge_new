using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Image followFill;

    float maxHealth;
    float currentHealth;
    float afterHealth;

    public float speed;

    /// <summary>
    /// 체력바 초기화, 타겟위치, 타입을 파라미터로 받는다.
    /// </summary>
    public void SettingHpBar(Transform trans, float maxHp)
    {
        maxHealth = maxHp;
        //targetTransform = trans;
        fill.fillAmount = 1;
        //hpBarPosition = targetTransform.position;
        //hpBarPosition.y = hpBarPosition.y - 0.3f;
        transform.position = Camera.main.WorldToScreenPoint(trans.position);
    }

    public void UpdateHpBar(Vector3 trans)
    {
        transform.position = Camera.main.WorldToScreenPoint(trans);
    }

    public void UpdateUIHealth(float reduce, float current)
    {
        currentHealth = current;//현재 Hp값
        afterHealth = current - reduce;//감소된후 Hp값
        fill.fillAmount = afterHealth / maxHealth;

        followFill.fillAmount = currentHealth / maxHealth;

        followFill.DOFillAmount(afterHealth / maxHealth, 0.5f);

    }


}