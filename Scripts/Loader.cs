#pragma warning disable 649
using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Worldreaver.Utility;

namespace Worldreaver.Loading
{
    public class Loader : MonoBehaviour, ILoading
    {
        #region properties

        [SerializeField] private ELoadingType loadingType = ELoadingType.Fake;
#pragma warning disable 0414
        [SerializeField] private ECompleteType completeType = ECompleteType.Instant;
#pragma warning restore 0414
        [Range(0.5f, 10), SerializeField] private float value = 3f;
        [Range(0.5f, 7), SerializeField] private float fadeInSpeed = 2f;
        [Range(0.5f, 7), SerializeField] private float fadeOutSpeed = 2f;

        #region tip

        [SerializeField] private bool isTip;
        [Range(1, 60), SerializeField] private float timePerTip = 1.5f;
        [Range(0.5f, 5), SerializeField] private float tipFadeSpeed = 2f;
        [SerializeField,] private TextMeshProUGUI tipText;

        #endregion

        #region slider bar

        [SerializeField] private bool isProcessBar;
        [SerializeField] private Slider processBar;
        [SerializeField] private bool isDisplayTextProcess;
        [SerializeField] private TextMeshProUGUI processText;
        [SerializeField] private string processTemplate = "Loading {0}%";

        #endregion

        [SerializeField] private CanvasGroup canvasGroupProcessBar;
        [SerializeField] private float timeFadeProcessBar = 0.2f;
        [SerializeReference] private ILoadComplete _loadComplete;
        [SerializeField] private GameObject rootUi;
        [SerializeField] private CanvasGroup fadeImageCanvas;

        [Tooltip("Canvas contains loading bar!")]
        public Canvas canvasLoading;

        public bool IsTip => isTip;
        public bool IsProcessBar => isProcessBar;
        public TextMeshProUGUI TipText => tipText;

        public CanvasGroup CanvasGroupProcessBar { get => canvasGroupProcessBar; private set => canvasGroupProcessBar = value; }

        public float TimeFadeProcessBar { get => timeFadeProcessBar; private set => timeFadeProcessBar = value; }

        public ILoadComplete LoadComplete
        {
            get
            {
                if (_loadComplete == null)
                {
                    InitializeLoadComplete();
                }

                return _loadComplete;
            }
            private set => _loadComplete = value;
        }

        public IDisposable DisposableTips { get; private set; }
        public IDisposable DisposableWaitTips { get; private set; }
        public IDisposable DisposableNextScene { get; private set; }
        
        public IDisposable DisposableFadeOut { get; private set; }

        private AsyncOperation _operation;
        private AsyncOperation _subOperation;
        private bool _isOperationStarted;
        private bool _finishLoad;
        private bool _isTipFadeOut = true;
        private string[] _tips;
        private float _lerpValue;
        private bool _pause;

        #endregion

        #region function

        #region event function

        private void Start()
        {
            if (tipText != null)
            {
                tipText.gameObject.SetActive(isTip);
            }

            transform.SetAsLastSibling();
            InitializeLoadComplete();
        }

        private void Update()
        {
            if (!_isOperationStarted)
                return;
            if (_operation == null) return;

            UpdateUi();
        }

        private void UpdateUi()
        {
            if (loadingType == ELoadingType.Async)
            {
                var p = _operation.progress + 0.1f; //Fix problem of 90%
                _lerpValue = Mathf.Lerp(_lerpValue, p, Time.unscaledDeltaTime * value);
            }

            if (isProcessBar && processBar != null)
            {
                processBar.value = _lerpValue;
            }

            if (isDisplayTextProcess && processText != null && !_finishLoad)
            {
                processText.text = string.Format(processTemplate, (Math.Min(_lerpValue * 100, 100)).ToString("F0"));
            }
        }

        private void OnFinish()
        {
            _finishLoad = true;
            LoadComplete.OnFinish(this);
        }

        public void InitializeTips(
            string[] tips)
        {
            _tips = tips;
        }

#if UNITY_EDITOR
        private void OnValidate() { Invoke(nameof(InitializeLoadComplete), 0.1f); }
#endif

        #endregion

        #region load

        /// <summary>
        /// setup operation load <paramref name="scene"/>
        /// </summary>
        /// <param name="scene"></param>
        private void StartAsyncOperation(
            string scene)
        {
            _operation = LoaderUtility.LoadAsync(scene);
            _operation.allowSceneActivation = false;
            _isOperationStarted = true;
        }

        /// <summary>
        /// setup load <paramref name="scene"/>
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        private void SetupLoad(
            string scene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
        {
            if (onSceneLoaded != null)
            {
                LoaderUtility.RegisterOnLoaded(onSceneLoaded);
            }

            SetupUi();
            Clear();
            StartAsyncOperation(scene);
            _pause = true;
        }

        /// <summary>
        /// load scene with scene name and callback <paramref name="onSceneLoaded"/>
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        public async void Load(
            string scene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
        {
            SetupLoad(scene, onSceneLoaded);
            _pause = false;
            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                    .ToUniTask());
            }

            OnCompleteWait();
        }

        /// <summary>
        /// loading scene with name and callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            params UniTask[] unitasks)
        {
            SetupLoad(scene, onSceneLoaded);
            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(() => _pause = false));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(() => _pause = false);
            }

            OnCompleteWait();
        }

        /// <summary>
        /// loading scene with name and callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed and <paramref name="uniTaskContinueWith"/> run completed
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="uniTaskContinueWith">callback run after all params <paramref name="unitasks"/> run completed</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            UniTask uniTaskContinueWith,
            params UniTask[] unitasks)
        {
            SetupLoad(scene, onSceneLoaded);
            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(async () =>
                        {
                            await uniTaskContinueWith;
                            return _pause = false;
                        }));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(async () =>
                    {
                        await uniTaskContinueWith;
                        return _pause = false;
                    });
            }

            OnCompleteWait();
        }

        #endregion

        #region load adtive

        /// <summary>
        /// done loading
        /// </summary>
        private void OnCompleteWait()
        {
            //TODO done loading
            if (!_finishLoad)
            {
                OnFinish();
            }
        }

        /// <summary>
        /// setup operation load scene name and LoadSceneMode <paramref name="mode"/>
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="mode">mode load scene</param>
        private void StartAsyncOperation(
            string scene,
            LoadSceneMode mode)
        {
            _operation = LoaderUtility.LoadAsync(scene, mode);
            _operation.allowSceneActivation = false;
            _isOperationStarted = true;
        }

        /// <summary>
        /// setup load scene name and LoadSceneMode <paramref name="mode"/>
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="mode">mode load scene</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        private void SetupLoad(
            string scene,
            LoadSceneMode mode,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
        {
            if (onSceneLoaded != null)
            {
                LoaderUtility.RegisterOnLoaded(onSceneLoaded);
            }

            SetupUi();
            Clear();
            StartAsyncOperation(scene, mode);
            _pause = true;
        }

        /// <summary>
        /// load scene with name with LoadSceneMode <paramref name="mode"/> and callback <paramref name="onSceneLoaded"/>
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="mode">mode load scene</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        public async void Load(
            string scene,
            LoadSceneMode mode,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
        {
            SetupLoad(scene, mode, onSceneLoaded);
            _pause = false;
            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                    .ToUniTask());
            }

            OnCompleteWait();
        }

        /// <summary>
        /// load scene with name with LoadSceneMode <paramref name="mode"/> and callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="mode">mode load scene</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            LoadSceneMode mode,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            params UniTask[] unitasks)
        {
            SetupLoad(scene, mode, onSceneLoaded);

            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(() => _pause = false));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(() => _pause = false);
            }

            OnCompleteWait();
        }

        /// <summary>
        /// load scene with name with LoadSceneMode <paramref name="mode"/> and callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed and <paramref name="uniTaskContinueWith"/> run completed
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="mode">mode load scene</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="uniTaskContinueWith">callback run after all params <paramref name="unitasks"/> run completed</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            LoadSceneMode mode,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            UniTask uniTaskContinueWith,
            params UniTask[] unitasks)
        {
            SetupLoad(scene, mode, onSceneLoaded);

            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(async () =>
                        {
                            await uniTaskContinueWith;
                            return _pause = false;
                        }));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(async () =>
                    {
                        await uniTaskContinueWith;
                        return _pause = false;
                    });
            }

            OnCompleteWait();
        }

        /// <summary>
        /// unload scene with name
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void UnLoad(
            string scene,
            params UniTask[] unitasks)
        {
            await LoaderUtility.UnloadAsync(scene);
            await UniTask.WhenAll(unitasks);
        }

        #endregion

        #region sub load

        /// <summary>
        /// complete loading
        /// </summary>
        private void OnCompleteWaitAllSubType()
        {
            //TODO done loading
            // ReSharper disable once InvertIf
            if (!_finishLoad)
            {
                OnFinish();
                _subOperation.allowSceneActivation = true;
            }
        }

        /// <summary>
        /// setup operation load <paramref name="scene"/>
        /// </summary>
        /// <param name="scene"></param>
        private void SubStartAsyncOperation(
            string scene)
        {
            _subOperation = LoaderUtility.LoadAsync(scene, LoadSceneMode.Additive);
            _subOperation.allowSceneActivation = false;
        }

        /// <summary>
        /// setup load scene and subscene
        /// </summary>
        /// <param name="scene">scene name load with mode single</param>
        /// <param name="subScene">sub scene name load with mode additive</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        private void SetupLoadSubType(
            string scene,
            string subScene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
        {
            if (onSceneLoaded != null)
            {
                LoaderUtility.RegisterOnLoaded(onSceneLoaded);
            }

            SetupUi();
            Clear();
            StartAsyncOperation(scene);
            SubStartAsyncOperation(subScene);
            _pause = true;
        }

        /// <summary>
        /// setup load scene and subscene
        /// </summary>
        /// <param name="scene">scene name load with LoadSceneMode <paramref name="mode"/></param>
        /// <param name="mode">mode loading of <paramref name="scene"/></param>
        /// <param name="subScene">sub scene name load with mode additive</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        private void SetupLoadSubType(
            string scene,
            LoadSceneMode mode,
            string subScene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
        {
            if (onSceneLoaded != null)
            {
                LoaderUtility.RegisterOnLoaded(onSceneLoaded);
            }

            SetupUi();
            Clear();
            StartAsyncOperation(scene, mode);
            SubStartAsyncOperation(subScene);
            _pause = true;
        }

        /// <summary>
        /// load two scene <paramref name="scene"/> and <paramref name="subScene"/> with callback <paramref name="onSceneLoaded"/>
        /// be careful <paramref name="onSceneLoaded"/> will be called twice, one of <paramref name="scene"/> loaded and one of <paramref name="subScene"/> loaded
        /// </summary>
        /// <param name="scene">scene name will load with mode single</param>
        /// <param name="subScene">sub scene name will load with mode additive</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        public async void Load(
            string scene,
            string subScene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
        {
            SetupLoadSubType(scene, subScene, onSceneLoaded);
            _pause = false;
            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                    .ToUniTask());
            }

            OnCompleteWaitAllSubType();
        }

        /// <summary>
        /// load two scene <paramref name="scene"/> and <paramref name="subScene"/> with callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed
        /// be careful <paramref name="onSceneLoaded"/> will be called twice, one of <paramref name="scene"/> loaded and one of <paramref name="subScene"/> loaded
        /// </summary>
        /// <param name="scene">scene name will load with mode single</param>
        /// <param name="subScene">sub scene name will load with mode additive</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            string subScene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            params UniTask[] unitasks)
        {
            SetupLoadSubType(scene, subScene, onSceneLoaded);

            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(() => _pause = false));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(() => _pause = false);
            }

            OnCompleteWaitAllSubType();
        }

        /// <summary>
        /// load two scene <paramref name="scene"/> and <paramref name="subScene"/> with callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed and <paramref name=" uniTaskContinueWith"/> completed
        /// be careful <paramref name="onSceneLoaded"/> will be called twice, one of <paramref name="scene"/> loaded and one of <paramref name="subScene"/> loaded
        /// </summary>
        /// <param name="scene">scene name will load with mode single</param>
        /// <param name="subScene">sub scene name will load with mode additive</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="uniTaskContinueWith">callback run after all params <paramref name="unitasks"/> run completed</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            string subScene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            UniTask uniTaskContinueWith,
            params UniTask[] unitasks)
        {
            SetupLoadSubType(scene, subScene, onSceneLoaded);

            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(async () =>
                        {
                            await uniTaskContinueWith;
                            return _pause = false;
                        }));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(async () =>
                    {
                        await uniTaskContinueWith;
                        return _pause = false;
                    });
            }

            OnCompleteWaitAllSubType();
        }

        /// <summary>
        /// load two scene <paramref name="scene"/> and <paramref name="subScene"/> with callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed
        /// be careful <paramref name="onSceneLoaded"/> will be called twice, one of <paramref name="scene"/> loaded and one of <paramref name="subScene"/> loaded
        /// </summary>
        /// <param name="scene">scene name will load with LoadSceneMode <paramref name="mode"/></param>
        /// <param name="mode">mode load of <paramref name="scene"/></param>
        /// <param name="subScene">sub scene name will load with mode additive</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            LoadSceneMode mode,
            string subScene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            params UniTask[] unitasks)
        {
            SetupLoadSubType(scene, mode, subScene, onSceneLoaded);

            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(() => _pause = false));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(() => _pause = false);
            }

            OnCompleteWaitAllSubType();
        }

        /// <summary>
        /// load two scene <paramref name="scene"/> and <paramref name="subScene"/> with callback <paramref name="onSceneLoaded"/>
        /// loading will succeed when base load complete and all params <paramref name="unitasks"/> run completed and <paramref name=" uniTaskContinueWith"/> completed
        /// be careful <paramref name="onSceneLoaded"/> will be called twice, one of <paramref name="scene"/> loaded and one of <paramref name="subScene"/> loaded
        /// </summary>
        /// <param name="scene">scene name will load with LoadSceneMode <paramref name="mode"/></param>
        /// <param name="mode">mode load of <paramref name="scene"/></param>
        /// <param name="subScene">sub scene name will load with mode additive</param>
        /// <param name="onSceneLoaded">callback call when scene loaded</param>
        /// <param name="uniTaskContinueWith">callback run after all params <paramref name="unitasks"/> run completed</param>
        /// <param name="unitasks">params unitask will invoked during time loading</param>
        public async void Load(
            string scene,
            LoadSceneMode mode,
            string subScene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            UniTask uniTaskContinueWith,
            params UniTask[] unitasks)
        {
            SetupLoadSubType(scene, mode, subScene, onSceneLoaded);

            if (loadingType == ELoadingType.Fake)
            {
                await UniTask.WhenAll(StartFakeLoading()
                        .ToUniTask(),
                    UniTask.WhenAll(unitasks)
                        .ContinueWith(async () =>
                        {
                            await uniTaskContinueWith;
                            return _pause = false;
                        }));
            }
            else
            {
                await UniTask.WhenAll(unitasks)
                    .ContinueWith(async () =>
                    {
                        await uniTaskContinueWith;
                        return _pause = false;
                    });
            }

            OnCompleteWaitAllSubType();
        }
        
        #endregion

        #region helper

        private void SetupUi()
        {
            if (fadeImageCanvas != null)
            {
                fadeImageCanvas.alpha = 1;
                DisposableFadeOut?.Dispose();
                DisposableFadeOut = Observable.FromMicroCoroutine(() => FadeOutCanvas(fadeImageCanvas))
                    .Subscribe()
                    .AddTo(this);
            }

            if (isProcessBar)
            {
                processBar.value = 0;
                canvasGroupProcessBar.alpha = 1;
            }

            LoadComplete.Setup();

            if (isDisplayTextProcess && processText != null)
            {
                processText.text = string.Format(processTemplate, (_lerpValue * 100).ToString("F0"));
            }

            if (isTip && tipText != null)
            {
                DisposableTips?.Dispose();
                DisposableTips = Observable.FromMicroCoroutine(TipsLoop)
                    .Subscribe()
                    .AddTo(this);
            }

            rootUi.SetActive(true);
        }

        public void LoadNextScene()
        {
            DisposableNextScene?.Dispose();
            DisposableNextScene = Observable.FromMicroCoroutine(LoadNextSceneIe)
                .Subscribe()
                .AddTo(this);
        }

        private IEnumerator StartFakeLoading()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) yield break;
#endif
            _lerpValue = 0;
            while (_lerpValue < 1)
            {
                if (!_pause)
                {
                    _lerpValue += Time.unscaledDeltaTime / value;
                }
                else
                {
                    if (_lerpValue < 0.42f)
                    {
                        _lerpValue += Time.unscaledDeltaTime / value / 5f;
                    }
                    else if (_lerpValue < 0.8f)
                    {
                        _lerpValue += Time.unscaledDeltaTime / value / 12f;
                    }
                    else if (_lerpValue < 0.95f)
                    {
                        _lerpValue += Time.unscaledDeltaTime / value / 20f;
                    }
                }

                yield return null;
            }
        }

        private IEnumerator TipsLoop()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) yield break;
#endif
            if (tipText == null || !isTip || _tips == null || _tips.Length == 0)
                yield break;

            var alpha = tipText.color;
            if (_isTipFadeOut)
            {
                tipText.text = _tips[RandomInstance.This.Next(0, _tips.Length)];
                while (alpha.a < 1)
                {
                    alpha.a += Time.unscaledDeltaTime * tipFadeSpeed;
                    tipText.color = alpha;
                    yield return null;
                }

                DisposableWaitTips?.Dispose();
                DisposableWaitTips = Observable.FromMicroCoroutine(() => WaitNextTip(timePerTip))
                    .Subscribe()
                    .AddTo(this);
            }
            else
            {
                while (alpha.a > 0)
                {
                    alpha.a -= Time.unscaledDeltaTime * tipFadeSpeed;
                    tipText.color = alpha;
                    yield return null;
                }

                tipText.text = "";
                DisposableWaitTips?.Dispose();
                DisposableWaitTips = Observable.FromMicroCoroutine(() => WaitNextTip(0.75f))
                    .Subscribe()
                    .AddTo(this);
            }
        }

        private IEnumerator WaitNextTip(
            float t)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) yield break;
#endif
            _isTipFadeOut = !_isTipFadeOut;

            float current = 0;
            while (current < t)
            {
                current += Time.unscaledDeltaTime;
                yield return null;
            }

            DisposableTips?.Dispose();
            DisposableTips = Observable.FromMicroCoroutine(TipsLoop)
                .Subscribe()
                .AddTo(this);
        }

        private IEnumerator LoadNextSceneIe()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) yield break;
#endif
            _operation.allowSceneActivation = true;
            fadeImageCanvas.alpha = 1;
            if (isProcessBar)
            {
                canvasGroupProcessBar.alpha = 0;
            }

            while (fadeImageCanvas.alpha > 0)
            {
                fadeImageCanvas.alpha -= Time.unscaledDeltaTime * fadeInSpeed;
                yield return null;
            }

            Dispose();
        }

        public IEnumerator FadeOutCanvas(
            CanvasGroup alpha,
            float delay = 0)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) yield break;
#endif
            float current = 0;
            while (current < delay)
            {
                current += Time.unscaledDeltaTime;
                yield return null;
            }

            while (alpha.alpha > 0)
            {
                alpha.alpha -= Time.unscaledDeltaTime * fadeOutSpeed;
                yield return null;
            }
        }

        private IEnumerator FadeInCanvas(
            CanvasGroup alpha,
            float delay = 0)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) yield break;
#endif
            float current = 0;
            while (current < delay)
            {
                current += Time.unscaledDeltaTime;
                yield return null;
            }

            while (alpha.alpha < 1)
            {
                alpha.alpha += Time.unscaledDeltaTime * fadeInSpeed;
                yield return null;
            }
        }

        public void FadeOutProcessBar()
        {
            if (isProcessBar)
            {
                DisposableFadeOut?.Dispose();
                DisposableFadeOut = Observable.FromMicroCoroutine(() => FadeOutCanvas(canvasGroupProcessBar, timeFadeProcessBar))
                    .Subscribe()
                    .AddTo(this);
            }
        }

        public void SetActiveTip(
            bool state)
        {
            if (isTip) tipText.gameObject.SetActive(state);
        }

        private void Dispose()
        {
            rootUi.SetActive(false);
            DisposableTips?.Dispose();
            DisposableWaitTips?.Dispose();
            DisposableNextScene?.Dispose();
            DisposableFadeOut?.Dispose();
        }

        private void Clear() { _finishLoad = false; }

        private void InitializeLoadComplete()
        {
            if (_loadComplete == null)
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (completeType)
                {
                    case ECompleteType.Instant:
                        _loadComplete = new LoadInstantComplete();
                        break;
                    case ECompleteType.AnyKey:
                        _loadComplete = new LoadAnyKeyComplete();
                        break;
                }
            }
            else
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (completeType)
                {
                    case ECompleteType.Instant when _loadComplete.GetType() != typeof(LoadInstantComplete):
                        _loadComplete = new LoadInstantComplete();
                        break;
                    case ECompleteType.AnyKey when _loadComplete.GetType() != typeof(LoadAnyKeyComplete):
                        _loadComplete = new LoadAnyKeyComplete();
                        break;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDisable()
        {
            DisposableTips?.Dispose();
            DisposableWaitTips?.Dispose();
            DisposableNextScene?.Dispose();
            DisposableFadeOut?.Dispose();
        }
#endif

        #endregion

        #endregion
    }
}