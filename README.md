# Readme

## What

- Provide Loading for Unity

## Requirements
[![Unity 2019.3+](https://img.shields.io/badge/unity-2019.3+-brightgreen.svg?style=flat&logo=unity&cacheSeconds=2592000)](https://unity3d.com/get-unity/download/archive)
[![.Net 2.0 Scripting Runtime](https://img.shields.io/badge/.NET-2.0-blueviolet.svg?style=flat&cacheSeconds=2592000)](https://docs.unity3d.com/2019.1/Documentation/Manual/ScriptingRuntimeUpgrade.html)

## Installation

### Install via git URL

After Unity 2019.3.4f1, Unity 2020.1a21, that support path query parameter of git package. You can add `https://github.com/worldreaver/Loader.git?path=Assets/Root` to Package Manager

If you want to set a target version, Loader is using *.*.* release tag so you can specify a version like #1.0.0. For example https://github.com/worldreaver/Loader.git?path=Assets/Root#1.0.0

### Dependencies

- "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
- "com.neuecc.unirx": "https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts",
- "com.worldreaver.utility": "https://github.com/worldreaver/Utility.git?path=Assets/Root",

## Usage

- use method Load in Loader

-
```csharp
/// <summary>
/// load scene with scene name and callback <paramref name="onSceneLoaded"/>
/// </summary>
/// <param name="scene">scene name</param>
/// <param name="onSceneLoaded">callback call when scene loaded</param>
public async void Load(
            string scene,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded)
```

-
```csharp
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
```

-
```csharp
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
```



### scene will load with mode single

- 
```csharp
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
```

-
```csharp
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
```

-
```csharp
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
            UniTask uniTaskContinueWith,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            params UniTask[] unitasks)
```



### subScene will load with additive

-
```csharp
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
```

-
```csharp
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
```

-
```csharp
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
            UniTask uniTaskContinueWith,
            UnityAction<Scene, LoadSceneMode> onSceneLoaded,
            params UniTask[] unitasks)
```

-
```csharp
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
```

-
```csharp
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
```


### unload scene with params `UniTask`
-
```csharp
/// <summary>
/// unload scene with name
/// </summary>
/// <param name="scene">scene name</param>
/// <param name="unitasks">params unitask will invoked during time loading</param>
public async void UnLoad(
            string scene,
            params UniTask[] unitasks)
```


### Cancel Loading

- call method CancelLoading when you want cancel loading
```csharp
/// <summary>
/// cancel token source
/// </summary>
public void CancelLoading()
```

### Exception

- call method CheckThrowToken where you want to stop when you have canceled from token source
```csharp
/// <summary>
/// cancel
/// </summary>
public void CheckThrowToken()
```


## Ex

- sample load scene
```csharp
.Load(nameScene, LoadSceneMode.Single, null);
```

```csharp

[SerializeField] private Loader loader;
public string nameScene;

...

/// <summary>
/// heavy task a
/// normal function
/// </summary>
private void HeavyTaskA()
{
    Debug.Log("[TaskA] Starting...");
    isDoneTaskA = false;
    for (int i = 0; i < 50000000; i++)
    {
        var x = Math.Pow(2, 10);
    }

    isDoneTaskA = true;
    Debug.Log("[TaskA] Done...");
}



...

loader.Load(nameScene,
            LoadSceneMode.Single,
            null,
            UniTask.Run(() => HeavyTaskA(loader.CheckThrowToken), cancellationToken: loader.Token));

```

