using System;
using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class Admob : MonoBehaviour
{
    private int rewardAmount;

    private Action<bool> callBack;

    private bool isLoading = false;
    private RewardedAd rewardBasedVideo;
    private bool curVideoCompleteReward = false;

    //InterstitialAd interstitialAd;//전면광고
    //private BannerView banner;//배너광고

    private static Admob instance;

    public static Admob Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
//#if !UNITY_EDITOR

        #region 전면광고

        //interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        //interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        //interstitialAd.OnAdOpening += HandleOnAdOpened;
        //interstitialAd.OnAdClosed += HandleOnAdClosed;
        //interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        #endregion 전면광고

        #region 배너 광고

        //RequestBanner();
        //banner.Hide();

        #endregion 배너 광고
        //this.rewardBasedVideo = new RewardedAd("ca-app-pub-3940256099942544/5224354917");//테스트 광고 ID
        //this.rewardBasedVideo = new RewardedAd("ca-app-pub-5121994233839369/2664945728");//실제 광고
        // Create an empty ad request.
        //AdRequest request = new AdRequest.Builder().Build();


        LoadRewardAdvertise();
        // Load the rewarded ad with the request.
        //this.rewardBasedVideo.LoadAd(request);

        // Called when an ad request has successfully loaded.
        //this.rewardBasedVideo.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardBasedVideo.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardBasedVideo.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardBasedVideo.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        //this.rewardBasedVideo.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        //this.rewardBasedVideo.OnAdClosed += HandleRewardedAdClosed;

        //Test Device
        List<string> deviceIds = new List<string>();
        deviceIds.Add("73109040666EEABC91E01A2E8CA04A19");
        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);


        //#endif
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            //실제 보상 처리
            if (curVideoCompleteReward)
            {
                callBack?.Invoke(curVideoCompleteReward);
                callBack = null;
                curVideoCompleteReward = false;
            }
        }
    }
    //배너광고 로드
    //private void RequestBanner()
    //{
    //    string AdUnitID = "ca-app-pub-5121994233839369/6130327025";

    //    banner = new BannerView(AdUnitID, AdSize.Banner, AdPosition.TopRight);

    //    AdRequest request = new AdRequest.Builder().Build();

    //    banner.LoadAd(request);
    //}

    //유니티애즈
    //private void HandleShowResult(ShowResult result)
    //{
    //    switch (result)
    //    {
    //        case ShowResult.Finished:
    //            Debug.Log("The ad was successfully shown.");
    //            StartCoroutine("AdmobReward");
    //            break;

    //        case ShowResult.Skipped:
    //            Debug.Log("The ad was skipped before reaching the end.");
    //            break;

    //        case ShowResult.Failed:
    //            Debug.LogError("The ad failed to be shown.");
    //            break;
    //    }
    //}

    ////전면 광고 로드
    //void LoadInterAdvertise()
    //{
    //    //string adUnitId = "ca-app-pub-5121994233839369/9861757126";
    //    //테스트 전면광고 ID
    //    string adUnitId = "ca-app-pub-3940256099942544/1033173712";

    //    interstitialAd = new InterstitialAd(adUnitId);
    //    AdRequest interRequest = new AdRequest.Builder().Build();
    //    interstitialAd.LoadAd(interRequest);

    //}

    //public void AdmobInterAdShow()
    //{
    //    if (interstitialAd.IsLoaded())
    //        interstitialAd.Show();
    //}

    //    public void ShowBanner()
    //    {
    //        banner.Show();
    //    }

    private void LoadRewardAdvertise()
    {
        //보상형 동영상 테스트 ID
        //this.rewardBasedVideo = new RewardedAd("ca-app-pub-3940256099942544/5224354917");
        this.rewardBasedVideo = new RewardedAd("ca-app-pub-5121994233839369/2664945728");//실제 광고

        this.rewardBasedVideo.OnAdLoaded += HandleRewardedAdLoaded;
        this.rewardBasedVideo.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardBasedVideo.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardBasedVideo.LoadAd(request);

    }

    public void AdmobShow(Action<bool> action = null)
    {
        if (action != null)
            callBack = action;

        if (GameDataManager.instance.GetSaveData.productPackage.buyAdPassPackage)
        {

            callBack?.Invoke(true);
            callBack = null;
            //Debug.Log("광고패스 발동!");
            return;
        }

        if (isLoading) return;
        isLoading = true;

        


        //Admob 광고가 로드 되어있다면
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
        }
        else
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_LOAD_AD);
        }

        isLoading = false;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.ToString());
        LoadRewardAdvertise();
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.AdError);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        //애드몹은 광고 시청 후 ADS와 다르게 리퀘스트를 다시 빌드하여 로드해야한다.
        LoadRewardAdvertise();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + args.Amount);
        //광고 보상 받을 수 있게 flag 처리 
        curVideoCompleteReward = true;
    }





 

}