using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;

public class FireBaseManager : MonoBehaviour
{
    FirebaseApp app;

    private void Start()
    {
#if UNITY_ANDROID
        InitFireBase();
#endif
    }

    void InitFireBase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;

                Debug.Log("파이어베이스 앱 초기화 완료");
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", task.Result));
            }
        });

        FirebaseAnalytics.LogEvent("GameProgress","stage", 10);
        FirebaseAnalytics.LogEvent("Tutorial","end_level", 10);


    }



}
