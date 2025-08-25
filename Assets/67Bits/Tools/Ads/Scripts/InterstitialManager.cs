using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Ads
{
    public class InterstitialManager : Singleton<InterstitialManager>
    {
        //public UnityEvent OnStartInterstitial;
        [SerializeField] private AdsTimer _adsTimer;
        float lastActivated = 0;

        [SerializeField] private MonoBehaviour _interstitial;

        private IAdsInterstitial InterstitialAd => _interstitial as IAdsInterstitial;

        public int intertitalDelay = 120; // 2 minutes
        public int intertitalDelayWeekneds = 180; // 3 minutes

        [Header("No ads screen")]
        [SerializeField] private GameObject noAdsScreen;
        [SerializeField] private Button noAdsButton;
        [SerializeField] private Button yesAdsButton;
        private bool isFirstIntertitial = true;

        public UnityEvent OnFinishAdEvent;

        internal virtual void Start()
        {
            StartCoroutine(Interstitial());
            StoreManager.Instance.onCompletePurchaseNoAdsEvent.AddListener(RemoveAds);

            StartCoroutine(StoreManager.Instance.RestorePurchaseAfterInitialize());

            noAdsButton.onClick.AddListener(() =>
            {
                noAdsScreen.SetActive(false);
                StoreManager.Instance.BuyProduct(StoreManager.Instance.noAds.productId);
                isFirstIntertitial = false;
            });

            yesAdsButton.onClick.AddListener(() =>
            {
                noAdsScreen.SetActive(false);
                lastActivated = Time.timeSinceLevelLoad;
                InterstitialAd.ShowInterstitial(OnFinishAdEvent: () => OnFinishAdEvent?.Invoke());
                isFirstIntertitial = false;
            });
        }

        internal virtual void RemoveAds()
        {
            InterstitialAd.HideBanner();
            SaveData.Instance.HasNoAds = true;
            SaveSystem.SaveGame();
        }

        internal virtual IEnumerator Interstitial()
        {

            lastActivated = Time.timeSinceLevelLoad;
            while (true)
            {
                if (SaveData.Instance.HasNoAds) yield break;




                if (Time.timeSinceLevelLoad - lastActivated >= intertitalDelay)
                {
                    if (isFirstIntertitial)
                    {
                        noAdsScreen.SetActive(true);
                    }
                    else
                    {
                        _adsTimer.AciveParent();
                        //OnStartInterstitial?.Invoke();
                        //InterstitialAd.ShowInterstitial(OnFinishAdEvent: () => OnFinishAdEvent?.Invoke());
                        yield return new WaitForSeconds(_adsTimer._time);
                        if (Time.timeSinceLevelLoad - lastActivated >= intertitalDelay)
                        {
                            lastActivated = Time.timeSinceLevelLoad;
                            InterstitialAd.ShowInterstitial(OnFinishAdEvent: () => OnFinishAdEvent?.Invoke());
                        }
                    }

                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}