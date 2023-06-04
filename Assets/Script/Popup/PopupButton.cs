using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PopupButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonString = null;
    [SerializeField] private TextMeshProUGUI closeCountText = null;

    private List<Action> callbackEvent = null;

    private GameObject target = null;

    Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void Init(string _text, List<Action> _callback, GameObject _target)
    {
        // �ʱ�ȭ - �Ű������� ���� �̸��� �ݹ��Լ��� Ŭ���� �ݹ��Լ��� ȣ�����ִ� �˾���ư
        this.buttonString.text = _text;
        this.callbackEvent = _callback;
        this.target = _target;
        btn.onClick.AddListener(OnButton);

    }

    void OnEnable()
    {
        StartCoroutine("AutoClose");
    }

    IEnumerator AutoClose()
    {
        float time = 3f;

        while(time > 0)
        {
            time -= Time.deltaTime;
            closeCountText.text = string.Format("{0:F0}s",time);
            yield return null;
        }

       OnButton();
    }

    public void OnButton()
    {
        StopCoroutine("AutoClose");

        foreach (var action in this.callbackEvent)
        {
            action();
        }


        Destroy(target);
    }
}
public class PopupButtonInfo
{
    public string text = null;
    public List<Action> callback = null;

    public PopupButtonInfo(string _text, List<Action> _callback)
    {
        this.text = _text;
        if (_callback != null)
        {
            this.callback = _callback;
        }
    }
}