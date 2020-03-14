# Readme

## What

- Provide Loading for Unity

## Requirements
[![Unity 2019.3+](https://img.shields.io/badge/unity-2019.3+-brightgreen.svg?style=flat&logo=unity&cacheSeconds=2592000)](https://unity3d.com/get-unity/download/archive)
[![.Net 2.0 Scripting Runtime](https://img.shields.io/badge/.NET-2.0-blueviolet.svg?style=flat&cacheSeconds=2592000)](https://docs.unity3d.com/2019.1/Documentation/Manual/ScriptingRuntimeUpgrade.html)

## Installation

## Usage


## Ex

```csharp
    private bool check;

    public async void Test()
    {
        await UniTask.WhenAll(StartFakeLoading().ToUniTask(), UniTask.WhenAll(UniTask.Run(HeavyTaskA), UniTask.Run(HeavyTaskB)).ContinueWith(() =>
        {
            _pause = false;
            Debug.Log("Done two task");
        }));
        Debug.Log("Complete");
    }


    public void HeavyTaskA()
    {
        for (int i = 0; i < 10000000; i++)
        {
            var ex = Math.Pow(2, 10);
        }

        Debug.Log("Done Task A");
    }

    public void HeavyTaskB()
    {
        for (int i = 0; i < 6; i++)
        {
            var ex = Math.Pow(2, 10);
        }

        Debug.Log("Done Task B");
    }

    private bool _pause;

    private IEnumerator StartFakeLoading()
    {
        var _lerpValue = 0f;
        _pause = true;
        var _deltaTime = 0.02f;
        var fakeLoadingTime = 10f;

        while (_lerpValue < 1)
        {
            if (!_pause)
            {
                _lerpValue += _deltaTime / fakeLoadingTime;
            }
            else
            {
                if (_lerpValue < 0.42f)
                {
                    _lerpValue += _deltaTime / fakeLoadingTime / 5f;
                }
                else if (_lerpValue < 0.8f)
                {
                    _lerpValue += _deltaTime / fakeLoadingTime / 12f;
                }
                else if (_lerpValue < 0.99f)
                {
                    _lerpValue += _deltaTime / fakeLoadingTime / 20f;
                }
            }

            yield return null;
        }

        Debug.Log("Fake done");
    }
```

