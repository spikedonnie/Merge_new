using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBuilder
{
    private Transform target = null;

    // Build�޼ҵ� ȣ���Ҷ� �˾�â�� �ٸ��ֱ� ���� �������
    private string title = null;

    private PopupButtonInfo buttonInfo = null;

    public List<RewardInfoData> rewards;

    // �����ڿ��� �θ�Ÿ�� �Ű������� �����´�.
    public PopupBuilder(Transform _target)
    {
        this.target = _target;
        rewards = new List<RewardInfoData>();
    }

    public void Build()
    {
        // ���������� ��������� ������ �˾�â����
        // MonoBehaviour�� ���ŷ� ���� Instantiate�� ���Ұ�,�����ջ����� ���� GameObject�� static�޼ҵ�� ȣ��
        GameObject popupObject = GameObject.Instantiate(Resources.Load("Popup/" + "CustomPopup", typeof(GameObject))) as GameObject;
        popupObject.transform.SetParent(this.target, false);
        CustomPopup customPopup = popupObject.GetComponent<CustomPopup>();

        // �˾�����
        customPopup.SetTitle(this.title);
        customPopup.SetButtons(this.buttonInfo);
        customPopup.SetRewardInfo(this.rewards);
        customPopup.Init();
    }

    public void SetTitle(string _title)
    {
        // Ÿ��Ʋ���� �ʱ�ȭ
        this.title = _title;
    }

    public void SetButton(string _text, List<Action> _callback = null)
    {
        // ��ư���� �ʱ�ȭ
        buttonInfo = new PopupButtonInfo(_text, _callback);
    }

    public void SetRewardInfo(RewardTYPE type,float amount )
    {
        rewards.Add(new RewardInfoData(type, amount));
    }

  

}
