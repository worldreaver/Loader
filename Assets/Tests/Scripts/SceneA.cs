using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneA : MonoBehaviour
{
    public string nameScene;

    public string nameSceneB;

    private void Start() { Load(); }

    /// <summary>
    /// load single scene
    /// </summary>
    public void Load() { TestLoading.instance.loader.Load(nameScene, LoadSceneMode.Single, null); }

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
}