/*******************************************************
 * Copyright (C) 2020 worldreaver
 * __________________
 * All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * @author yenmoc phongsoyenmoc.diep@gmail.com
 *******************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ButtonScene : MonoBehaviour
{
    public string nameScene;
    public void Load()
    {
        TestLoading.instance.loader.Load(nameScene, LoadSceneMode.Single, null);
    }
}
