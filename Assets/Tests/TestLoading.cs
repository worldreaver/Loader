/*******************************************************
 * Copyright (C) 2020 worldreaver
 * __________________
 * All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * @author yenmoc phongsoyenmoc.diep@gmail.com
 *******************************************************/

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worldreaver.Loading;

public class TestLoading : MonoBehaviour
{
    public Loader loader;
    public static TestLoading instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
    }
}
