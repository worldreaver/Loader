using System;
using System.Collections;

namespace Worldreaver.Loading
{
    public interface ILoading
    {
        /// <summary>
        /// has tip in loading
        /// </summary>
        bool IsTip { get; }

        /// <summary>
        /// set active tip
        /// </summary>
        /// <param name="state"></param>
        void SetActiveTip(bool state);

        /// <summary>
        /// has process bar
        /// </summary>
        bool IsProcessBar { get; }

        /// <summary>
        /// tip text
        /// </summary>
        TMPro.TextMeshProUGUI TipText { get; }

        /// <summary>
        /// canvas group of process bar
        /// </summary>
        UnityEngine.CanvasGroup CanvasGroupProcessBar { get; }

        /// <summary>
        /// time fade process bar
        /// </summary>
        float TimeFadeProcessBar { get; }

        /// <summary>
        /// type load complete
        /// </summary>
        ILoadComplete LoadComplete { get; }

        /// <summary>
        /// load next scene
        /// </summary>
        void LoadNextScene();

        /// <summary>
        /// fade out canvas
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IEnumerator FadeOutCanvas(UnityEngine.CanvasGroup alpha,
            float delay = 0);

        /// <summary>
        /// fade out process bar
        /// </summary>
        void FadeOutProcessBar();

        /// <summary>
        /// idisposable tip
        /// </summary>
        IDisposable DisposableTips { get; }

        /// <summary>
        /// idisposable wait change tip
        /// </summary>
        IDisposable DisposableWaitTips { get; }

        /// <summary>
        /// idisposable change scene
        /// </summary>
        IDisposable DisposableNextScene { get; }
    }
}