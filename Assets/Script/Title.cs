using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using I2.Loc;

public class Title : MonoBehaviour
{
    private AsyncOperation async; // 로딩
    public TextMeshProUGUI progressText;
    private float loadProgress;
    public Image fillImage;
    public Image hidePanel;
    private bool isLoad = false;
    private bool isGoogleLogin = false;
    public GameObject googleText;

    public TextMeshProUGUI startText;
    public Color endColor;
    public Image titleImage;
    public Sprite kr_Title;
    public Sprite en_Title;
    public string lang;

    // Start is called before the first frame update
    private void Start()
    {
        Screen.SetResolution(1080, 1920, true);
        //Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, true);
        StartCoroutine("Set");
        //SetResolution(); // 초기에 게임 해상도 고정
        googleText.SetActive(false);
        hidePanel.enabled = false;
        isGoogleLogin = true;

    }

    private IEnumerator Set()
    {
        if (Application.systemLanguage.Equals(SystemLanguage.Korean))
        {
            LocalizationManager.CurrentLanguage = "Kr";
            titleImage.sprite = kr_Title;
        }
        else
        {
            LocalizationManager.CurrentLanguage = "En";
            titleImage.sprite = en_Title;
        }

        // LocalizationManager.CurrentLanguage = "En";
        // titleImage.sprite = en_Title;
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(GameDataManager.instance.SetGameData());


        StartCoroutine(SceneLoad());
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(Login());
#endif
    }

    private IEnumerator Login()
    {
        hidePanel.enabled = true;
        googleText.SetActive(true);
        isGoogleLogin = false;

        yield return StartCoroutine(GoogleManager.Instance.CheckForUpdate());

        CloudManager.Instance.Init();
        yield return new WaitUntil(() => CloudManager.Instance.initProcessing);
        Debug.Log("Success CloudManager");

        GoogleManager.Instance.LoginGPGS();
        yield return new WaitUntil(() => GoogleManager.Instance.IsLogin);
        Debug.Log("Success GoogleManager");

        isGoogleLogin = true;
        hidePanel.enabled = false;
        googleText.SetActive(false);
        startText.DOColor(endColor, 1f).SetLoops(-1, LoopType.Yoyo);


        yield return null;
    }

    public void LoadMainScene()
    {
        if (isLoad && isGoogleLogin)
        {
            async.allowSceneActivation = true;
        }
    }

    // 로딩
    private IEnumerator SceneLoad()
    {
        async = SceneManager.LoadSceneAsync("Main");

        progressText.text = string.Format("Loading... %");

        async.allowSceneActivation = false;

        //allowSceneActivation가 false 면 isDone이 true로 바뀌지 않는다.
        while (!async.isDone)
        {
            if (loadProgress >= 0.8f)
            {
                isLoad = true;
            }

            loadProgress = async.progress / 0.9f;

            progressText.text = string.Format("Loading... {0:0} %", loadProgress * 100f);

            fillImage.fillAmount = (float)(loadProgress / 0.9f);

            yield return null;
        }
    }


    // /* 해상도 설정하는 함수 */
    // void SetResolution()
    // {
    //     int setWidth = 1920; // 사용자 설정 너비
    //     int setHeight = 1080; // 사용자 설정 높이

    //     int deviceWidth = Screen.width; // 기기 너비 저장
    //     int deviceHeight = Screen.height; // 기기 높이 저장

    //     Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

    //     if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
    //     {
    //         float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
    //         Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
    //     }
    //     else // 게임의 해상도 비가 더 큰 경우
    //     {
    //         float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
    //         Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
    //     }
    // }

}