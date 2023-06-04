using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi; //conf
using GooglePlayGames;  //platfom
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class CloudManager : MonoBehaviour
{

    private static CloudManager instance;

    public static CloudManager Instance
    {
        get
        {
            return instance;
        }
    }

    private const string FILE_NAME = "GrowMercenaryCloudData";

    public bool isProcessing = false;

    private string loadedData;

    private bool trySave = false;
    private bool tryLoad = false;

    public bool initProcessing = false;

    private void Awake()
    {
        instance = this;
    }


    //게임서비스 플러그인 초기화시에 EnableSavedGames()를 넣어서 저장된 게임 사용할 수 있게 합니다.

    //구글플레이 개발자 콘솔의 게임서비스에서 해당게임의 세부정보에서 저장된 게임 사용 설정

    //클라우드 초기화 
    public void Init()
    {
        //PlayGamesClientConfiguration conf = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        //PlayGamesPlatform.InitializeInstance(conf);
        //PlayGamesPlatform.DebugLogEnabled = true;
        //PlayGamesPlatform.Activate();
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
        .Builder()
        .EnableSavedGames()
        .RequestServerAuthCode(false)
        .RequestIdToken()
        .Build();
        //커스텀된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        //GPGS 시작.
        PlayGamesPlatform.Activate();
        initProcessing = true;

    }

    //데이터 확인 프로세스
    void ProcessCloudData(byte[] cloudData)
    {
        if (cloudData == null)
        {
            //서버 연결 실패. 다시 시도하십시요
            Debug.Log("서버 연결 실패. 다시 시도하십시요");
            return;
        }

        //데이타 불러오기가 완료 되었습니다. 잠시만 기다려 주십시요 

        Debug.Log("데이타 불러오기가 완료 되었습니다. 잠시만 기다려 주십시요 ");

        string progress = BytesToString(cloudData);

        loadedData = progress;

    }

    //로드 시도하기
    void LoadFromCloud(Action<string> afterLoadAction)
    {
        if (Social.localUser.authenticated && !isProcessing)
        {
            //데이타 불러오기 중입니다. 잠시만 기다려주세요
            Debug.Log("데이타 불러오기 중입니다. 잠시만 기다려주세요");
           StartCoroutine(LoadFromCloudRoutin(afterLoadAction));

        }
        else
        {
            GoogleManager.Instance.LoginGPGS();
        }
    }

    IEnumerator LoadFromCloudRoutin(Action<string> loadAction)
    {
        isProcessing = true;

        Debug.Log("Loading game progress from the cloud.");

        ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
            FILE_NAME, //name of file.
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            OnFileOpenToLoad);

        while (isProcessing)
        {
            yield return null;
        }

        loadAction.Invoke(loadedData);
    }

    //저장 시도하기
    void SaveToCloud(string dataToSave)
    {

        if (Social.localUser.authenticated)
        {
            //데이타 저장중입니다. 잠시만 기다려주세요
            Debug.Log("데이타 저장중입니다. 잠시만 기다려주세요");
            loadedData = dataToSave;
            isProcessing = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(FILE_NAME, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);

        }
        else
        {
            GoogleManager.Instance.LoginGPGS();
        }
    }

    //저장가능한지 체크
    void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {

            byte[] data = StringToBytes(loadedData);

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

            SavedGameMetadataUpdate updatedMetadata = builder.Build();

            ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metaData, updatedMetadata, data, OnGameSave);

        }
        else
        {
            //서버 연결 실패. 다시 시도하십시요
            Debug.Log("서버 연결 실패. 다시 시도하십시요");
        }
    }
    //로드 가능한지 체크
    void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metaData, OnGameLoad);

        }
        else
        {
            //서버 연결 실패. 다시 시도하십시요
            Debug.Log("서버 연결 실패. 다시 시도하십시요");
        }
    }

    //실제 로드
    void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
    {
        if (status != SavedGameRequestStatus.Success)
        {
            //서버 연결 실패. 다시 시도하십시요
            Debug.Log("서버 연결 실패. 다시 시도하십시요");
        }
        else
        {
            ProcessCloudData(bytes);
            tryLoad = false;
        }

        isProcessing = false;
    }

    //실제 저장
    void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status != SavedGameRequestStatus.Success)
        {
            //서버 연결 실패. 다시 시도하십시요
            Debug.Log("서버 연결 실패. 다시 시도하십시요");
        }
        else
        {
            //데이타 저장이 완료
            Debug.Log("데이타 저장이 완료");

        }
        trySave = false;
        isProcessing = false;
    }

    byte[] StringToBytes(string stringToConvert)
    {
        return Encoding.UTF8.GetBytes(stringToConvert);
    }

    string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }


    public void StringToGameInfo(string localData)
    {
        if (localData != string.Empty)
        {
            Debug.Log(localData);

            GameDataManager.instance.SetSaveData(JsonConvert.DeserializeObject<SaveData>(localData));
                    //영웅 레벨 생성
            GameDataManager.instance.GetSaveData.isFirstConnect = false;
            GameController.instance.mergeController.ClearHeroList();
            var herolist = GameDataManager.instance.GetSaveData.battleSaveData.heroList;

            for (int i = 0; i < herolist.Count; i++)
            {
                if (herolist[i] >= (int)MercenaryType.DemonMagic3)
                    herolist[i] = (int)MercenaryType.DemonMagic3;

                GameController.instance.mergeController.GetNewHeroByObjectPool.CreateCharacter(herolist[i]);
            }

            Debug.Log("로컬 데이터 로드 및 저장 완료");
            tryLoad = false;
        }
    }


    private string GameInfoToString()
    {
        return JsonConvert.SerializeObject(GameDataManager.instance.GetSaveData);
    }

    
    public void CloudLoad()
    {
        if (tryLoad) return;
        tryLoad = true;
        LoadFromCloud(StringToGameInfo);
    }

    public void CloudSave()
    {
        if (trySave) return;
        trySave = true;

        string str = GameInfoToString();
    
        StartCoroutine(SaveFromCloud(str));
    }

    IEnumerator SaveFromCloud(string gameinfo)
    {
        SaveToCloud(gameinfo);
        //로딩바 시작
        Debug.Log("클라우드 저장 시작");
        yield return new WaitUntil(() => !trySave);
        UIController.instance.SendPopupMessage(AlarmTYPE.SAVED_CLOUD_DATA);
        Debug.Log("클라우드 저장 끝");
        //로딩바 끝
    }




}


