using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worldreaver.Loading;

public class SceneA : MonoBehaviour
{
    public string nameScene;
    public string nameSceneB;

    /// <summary>
    /// load single scene
    /// </summary>
    public void Load() { LoaderHolder.instance.loader.Load(nameScene, LoadSceneMode.Single, null, UniTask.Run(() => HeavyTaskA(LoaderHolder.instance.loader.CheckThrowToken), cancellationToken: LoaderHolder.instance.loader.Token)); }

    /// <summary>
    /// load multi scene
    /// main scene is <param name="nameScene"/>> and sub scene is <param name="nameSceneB"/>
    /// </summary>
    public void Load2() { LoaderHolder.instance.loader.Load(nameScene, nameSceneB, CallBack); }

    /// <summary>
    /// callback load success main scene
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void CallBack(
        Scene arg0,
        LoadSceneMode arg1)
    {
        Debug.Log("OnSceneLoaded call");
    }

    /// <summary>
    /// heavy task a
    /// normal function
    /// </summary>
    private void HeavyTaskA(
        Action actionCheckThrow)
    {
        Debug.Log("[TaskA] Starting...");

        for (int i = 0; i < 20000; i++)
        {
            Debug.Log(i);
            var x = System.Math.Pow(2, 10);
            actionCheckThrow?.Invoke();
        }

        Debug.Log("[TaskA] Done...");
    }

    public void CancelLoading() { LoaderHolder.instance.loader.CancelLoading(); }
}