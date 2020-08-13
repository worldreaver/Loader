using UnityEngine;
using Worldreaver.Loading;

public class LoaderHolder : MonoBehaviour
{
    public static LoaderHolder instance;
    public Loader loader;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
}