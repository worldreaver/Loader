using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScene : MonoBehaviour
{
    public string nameScene;

    public string nameSceneA;
    public string nameSubSceneA;
    public void Load()
    {
        TestLoading.instance.loader.Load(nameScene, LoadSceneMode.Single, null);
    }

    public void Load2()
    {
        TestLoading.instance.loader.Load(nameSceneA, nameSubSceneA, CallBack);
    }

    private void CallBack(Scene arg0,
        LoadSceneMode arg1)
    {
        Debug.Log("OnSceneLoaded call");
    }
}
