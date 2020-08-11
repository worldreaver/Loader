using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneA : MonoBehaviour
{
    public string nameScene;

    public string nameSceneB;
    UniTaskCompletionSource _completionSource = new UniTaskCompletionSource();
    
    private CancellationTokenSource _source = new CancellationTokenSource();
    private CancellationToken token;
    
    private async void Start()
    {
        token = _source.Token;
        //Load();
        try
        {
             await UniTask.Run(HeavyTaskA, cancellationToken: token).WithCancellation(token);
        }
        catch (OperationCanceledException e)
        {
            return;
        }
       
    }

    public void Cancel()
    {
        token.ThrowIfCancellationRequested();
        _source.Cancel();
    }

    /// <summary>
    /// load single scene
    /// </summary>
    public void Load()
    {
        var source = new CancellationTokenSource();
        TestLoading.instance.loader.Load(nameScene,
            LoadSceneMode.Single,
            null,
            source,
            UniTask.Run(HeavyTaskA, cancellationToken: source.Token));
    }

    /// <summary>
    /// load multi scene
    /// main scene is <param name="nameScene"/>> and sub scene is <param name="nameSceneB"/>
    /// </summary>
    public void Load2() { TestLoading.instance.loader.Load(nameScene, nameSceneB, CallBack); }

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
    private void HeavyTaskA()
    {
        Debug.Log("[TaskA] Starting...");

        for (int i = 0; i < 10000; i++)
        {
            Debug.Log(i);
            var x = System.Math.Pow(2, 10);
        }

        Debug.Log("[TaskA] Done...");
    }
}