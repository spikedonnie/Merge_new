using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using System.Collections;
using GooglePlayGames.BasicApi;
using Firebase;
using Google.Play.AppUpdate;
using Google.Play.Common;

public class GoogleManager : MonoBehaviour
{
    public bool IsLogin { get; private set; }

    private static GoogleManager instance;

    public static GoogleManager Instance
    {
        get
        {
            return instance;
        }
    }

    AppUpdateManager appUpdateManager;


    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
        //    .Builder()
        //    .RequestServerAuthCode(false)
        //    .RequestIdToken()
        //    .Build();
        ////커스텀된 정보로 GPGS 초기화
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.DebugLogEnabled = true;
        ////GPGS 시작.
        //PlayGamesPlatform.Activate();
        IsLogin = false;
        

    }

    public IEnumerator CheckForUpdate()
    {
        appUpdateManager = new AppUpdateManager();
        //가능한 업데이트가 있는지 확인
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        //업데이트 정보를 받아올 때까지 기다림
        yield return appUpdateInfoOperation;
        //업데이트 정보를 성공적으로 받아옴
        if (appUpdateInfoOperation.IsSuccessful)
        {
            //업데이트 정보 정의
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            
            //업데이트 가능 여부 확인
            if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                //실제 업데이트
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(
                    appUpdateInfoResult,
                    appUpdateOptions);
                yield return startUpdateRequest;
            }

            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.
        }
        else
        {
            Debug.Log(appUpdateInfoOperation.Error);
            // Log appUpdateInfoOperation.Error.
        }

        yield return null;
    }

    public void LoginGPGS()
    {
        //로그인이 안되어 있으면
        if (!Social.localUser.authenticated)
        {

            Social.localUser.Authenticate((bool isSuccess) =>
            {
                if (isSuccess)
                {
                    IsLogin = true;
                    Debug.Log("Login");
                }
                else
                {
                    IsLogin = false;
                    Debug.Log("Can Not Login");

                }
            });
        }
    }
    
    // 구글 토큰 받아옴
//    public string GetTokens() // 딱히 수정할 필요 없음
//    {
//#if UNITY_ANDROID
//        if (PlayGamesPlatform.Instance.localUser.authenticated)
//        {
//            // 유저 토큰 받기 첫번째 방법
//            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
//            // 두번째 방법
//            // string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
//            return _IDtoken;
//        }
//        else
//        {
//            //MessagePopManager.instance.ShowPop("접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail", 10f);
//            Debug.Log("접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
//        }
//#endif

//        return null;
//    }

    //    public void ReportDamageLevel(long sum, System.Action<string> str)
    //    {
    //#if UNITY_ANDROID
    //        Social.ReportScore(sum, GPGSIds.leaderboard, (bool success) =>
    //        {
    //            if (success)
    //            {
    //                LocalRanking(str);
    //            }

    //        });

    //        //if (sum > LocalHighScore())
    //        //{
    //        //    Social.ReportScore(sum, leaderboard_shipDamage, (bool success) => {
    //        //        if (success)
    //        //            ShowLeaderBoard();

    //        //    });
    //        //}
    //        //else
    //        //{
    //        //    ShowLeaderBoard();

    //        //}
    //#endif


    //    }

    public void ShowLeaderBoard()
    {
#if (UNITY_ANDROID)
        if (!IsLogin)
        {
            LoginGPGS();
            return;
        }
        Social.ShowLeaderboardUI();
        //RetrieveScore();
#endif
    }
    public void ShowAchievement()
    {
#if (UNITY_ANDROID)
        if (!IsLogin)
        {
            LoginGPGS();
            return;
        }
        Social.ShowAchievementsUI();
#endif
    }
    public void SendAchivement(string name)
    {
#if (UNITY_ANDROID)
        Social.ReportProgress(name, 100f, null);
#endif
    }

    public void CheckNetwork()
    {
        //인터넷 연결이 불안정하면
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //네트워크 연결이 필요합니다.
        }
    }

    //public void LocalRanking(System.Action<string> str)
    //{

    //    ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
    //    lb.id = GPGSIds.leaderboard;
    //    lb.userScope = UserScope.Global;
    //    lb.range = new Range(1, 10);
    //    lb.timeScope = TimeScope.AllTime;

    //    string data = "";

    //    lb.LoadScores(scores =>
    //    {

    //        uint all_player = lb.maxRange;
    //        int my_rank = lb.localUserScore.rank;
    //        decimal percent = (decimal)my_rank / (decimal)all_player;

    //        data = string.Format("총 {0}명 중 나의 순위는 {1}위입니다", all_player, my_rank);
    //        str(data);
    //        Debug.Log("\nAllPlayer: " + all_player + "\nMyRank: " + my_rank + "\nPercent: " + percent + "%");
    //    });

    //}

    //    public void LBWorldScore()
    //    {
    //        if (isLogin)
    //        {
    //#if (UNITY_ANDROID)

    //            string[] myScore = new string[5];
    //            Social.LoadScores(leaderboard_shipDamage, scores =>
    //            {
    //                if (scores.Length > 0)
    //                {
    //                    for (int i = 0; i < 5; i++)
    //                    {
    //                        myScore[i] = scores[i].formattedValue;
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.Log("No scores loaded");
    //                }
    //            });
    //            //return myScore;
    //#endif
    //        }
    //        else
    //        {
    //            LoginGPGS();
    //        }
    //    }


    ////나의최고기록불러오기
    //public long LocalHighScore()
    //{
    //    string _localScore = null;

    //    Social.LoadScores(leaderboard_shipDamage, scores =>
    //    {
    //        if (scores.Length > 0)
    //        {
    //            foreach (IScore score in scores)
    //            {
    //                if (score.userID == Social.localUser.id)
    //                {
    //                    _localScore = score.formattedValue;
    //                }
    //            }
    //        }
    //        else
    //            Debug.Log("No scores loaded");
    //    });
    //    return long.Parse(_localScore);
    //}

    //public int LocalRanking()
    //{
    //    int _localRank = 0;

    //    Social.LoadScores(GPGSIds.leaderboard, scores =>
    //    {
    //        if (scores.Length > 0)
    //        {
    //            foreach (IScore score in scores)
    //            {
    //                if (score.userID == Social.localUser.id)
    //                {
    //                    _localRank = score.rank;
    //                }
    //            }
    //        }
    //        else
    //            Debug.Log("No scores loaded");
    //    });
    //    return _localRank;
    //}


    //void RetrieveScore()
    //{

    //    PlayGamesPlatform.Instance.LoadScores(
    //        GPGSIds.leaderboard,
    //       LeaderboardStart.TopScores,
    //       10,
    //       LeaderboardCollection.Public,
    //       LeaderboardTimeSpan.AllTime,

    //     (data) =>
    //        {
    //            for (int i = 0; i < data.Scores.Length; i++)
    //            {
    //                Debug.Log("ID : " + data.Scores[i].userID + "   Score : " + data.Scores[i].formattedValue + "   Rank : " + data.Scores[i].rank);

    //            }
    //        });

    //}



}
