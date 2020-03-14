using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Worldreaver.Loading
{
    public class LoadAnyKeyComplete : ILoadComplete
    {
        public float timeFade = 0.25f;
        public GameObject objectAnyKey;
        public Button detect;

        public void OnFinish(ILoading iLoad)
        {
            Observable.Timer(TimeSpan.FromSeconds(timeFade)).Subscribe(_ => objectAnyKey.SetActive(true)).AddTo(objectAnyKey);
            iLoad.SetActiveTip(false);
            iLoad.DisposableTips?.Dispose();
            iLoad.DisposableWaitTips?.Dispose();
            iLoad.FadeOutProcessBar();
            detect.onClick.AddListener(() => LoadNextScene(iLoad));
            detect.interactable = true;
        }

        public void Setup()
        {
            if (objectAnyKey != null)
            {
                objectAnyKey.SetActive(false);
            }
        }

        private void LoadNextScene(ILoading iLoad)
        {
            Setup(); //disable object anykey
            iLoad.LoadNextScene();
            detect.onClick.RemoveAllListeners();
            detect.interactable = false;
        }
    }
}