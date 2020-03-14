using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScene : MonoBehaviour
{
    public string nameScene;
    public void Load()
    {
        TestLoading.instance.loader.Load(nameScene, LoadSceneMode.Single, null);
    }
}
