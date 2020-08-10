using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestUnitask : MonoBehaviour
{
    public bool pause;

    public void Start()
    {
        //TestWhenAll();

        LoadA(UniTask.Run(HeavyTaskA), UniTask.Run(HeavyTaskB), AsyncHeavyTaskC(), AsyncHeavyTaskD());

        //LoadAWithParam(UniTask.Run(HeavyTaskA), UniTask.Run(HeavyTaskB), AsyncHeavyTaskC(), AsyncHeavyTaskD());
    }

    /// <summary>
    /// test when all
    /// </summary>
    private async void TestWhenAll()
    {
        var backgroundTasks = new[]
        {
            StartFakeLoading()
                .ToUniTask(),
            AsyncHeavyTaskD(), UniTask.Run(HeavyTaskA), UniTask.WaitUntil(() => isDoneTaskA && isDoneTaskB)
                .ContinueWith(async () =>
                {
                    Debug.Log("IN CONTINUTE WITH X Start");

                    await UniTask.Run(() =>
                    {
                        Debug.Log("[TaskX] Starting run wrapper...");
                        for (int i = 0; i < 3000000; i++)
                        {
                            var x = Math.Pow(2, 10);
                        }

                        Debug.Log("[TaskX] Done run wrapper...");
                    });

                    Debug.Log("IN CONTINUTE WITH X Done");
                    pause = false;
                }),
        };
        await UniTask.WhenAll(backgroundTasks);

        Debug.Log("All task complete!");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniTaskA"></param>
    /// <param name="uniTaskB"></param>
    /// <param name="uniTaskC"></param>
    /// <param name="uniTaskD"></param>
    private async void LoadA(
        UniTask uniTaskA,
        UniTask uniTaskB,
        UniTask uniTaskC,
        UniTask uniTaskD)
    {
        Debug.Log("[Load A] Start ========================");
        await UniTask.WhenAll(StartFakeLoading()
                .ToUniTask(),
            UniTask.WhenAll(uniTaskA, uniTaskB, uniTaskC, uniTaskD)
                .ContinueWith(() =>
                {
                    Debug.Log("IN CONTINUTE WITH A");
                    pause = false;
                }));

        Debug.Log("[Load A] Done =========================");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniTaskParam"></param>
    private async void LoadAWithParam(
        params UniTask[] uniTaskParam)
    {
        Debug.Log("[Load A With Param] Start ========================");

        await UniTask.WhenAll(StartFakeLoading()
                .ToUniTask(),
            UniTask.WhenAll(uniTaskParam)
                .ContinueWith(() =>
                {
                    Debug.Log("IN CONTINUTE WITH A WITH PARAM");
                    return pause = false;
                }));

        Debug.Log("[Load A With Param] Done ========================");
    }

    #region out

    /// <summary>
    /// enumeator fake loading
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartFakeLoading()
    {
        Debug.Log("=============> Start fake loading");
        var lerpValue = 0f;
        const float def = 0.002f;
        while (lerpValue < 1)
        {
            if (!pause)
            {
                lerpValue += def;
            }
            else
            {
                if (lerpValue < 0.42f)
                {
                    lerpValue += def / 5f;
                }
                else if (lerpValue < 0.8f)
                {
                    lerpValue += def / 12f;
                }
                else if (lerpValue < 0.95f)
                {
                    lerpValue += def / 20f;
                }
            }

            yield return null;
        }

        Debug.Log("=============> End fake loading");
    }

    public bool isDoneTaskA;
    public bool isDoneTaskB;
    public bool isDoneTaskC;
    public bool isDoneTaskD;

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

    /// <summary>
    /// heavy task b
    /// normal function
    /// </summary>
    private void HeavyTaskB()
    {
        Debug.Log("[TaskB] Starting...");
        isDoneTaskB = false;
        for (int i = 0; i < 4000000; i++)
        {
            var x = Math.Pow(2, 10);
        }

        isDoneTaskB = true;
        Debug.Log("[TaskB] Done...");
    }

    /// <summary>
    /// heavy task c
    /// for methods that use Unitask's api like: UniTask.WaitUntil (), UniTask.Run (), etc ...
    /// then it should not be wrapper by UniTask.Run () anymore
    /// instead, the return type must be UniTask
    ///
    /// does not declare AsyncHeavyTaskC method with return type as void if inside it uses api of unitask already
    ///
    /// The AsyncHeavyTaskC method now doesn't need to be wrapped by UniTask.Run () anymore
    /// </summary>
    private async UniTask AsyncHeavyTaskC()
    {
        Debug.Log("[TaskC] Starting...");
        isDoneTaskC = false;

        await UniTask.WaitUntil(() => isDoneTaskA && isDoneTaskB)
            .ContinueWith(() => { Debug.Log("[TaskC] Done continue with..."); });

        // if not be wrapper by Unitask.Run then it will execute the entire loop in 1 frame => freeze
        // UniTaskLoopRunnerUpdate => UnityEngine.CoreModule.dll!::UpdateFunction.Invole()  1 call
        // nhưng khi bao bọc bởi UniTask.Run thì hàm coi như kết thúc và vì vậy thứ tự thực thi mà bạn muốn khi gọi WhenAll ở trên đã không còn đúng nữa
        // (but when be wrapper by UniTask.Run, the function will end and so the order of execution you want when calling WhenAll above is no longer true.)
        // hãy cân nhắc để tránh sẩy ra lỗi (please consider to avoid errors)
        await UniTask.Run(() =>
        {
            Debug.Log("[TaskC] Starting run wrapper...");
            for (int i = 0; i < 30000000; i++)
            {
                var x = Math.Pow(2, 10);
            }

            Debug.Log("[TaskC] Done run wrapper...");
        });

        isDoneTaskC = true;

        Debug.Log("[TaskC] Done...");
    }

    /// <summary>
    /// heavy task d
    /// </summary>
    private async UniTask AsyncHeavyTaskD()
    {
        Debug.Log("[TaskD] Starting...");
        isDoneTaskD = false;

        await UniTask.WhenAll(UniTask.Run(SubTaskA), UniTask.Run(SubTaskB), UniTask.Run(SubTaskC), UniTask.WaitUntil(() => isDoneTaskA && isDoneTaskB));

        Debug.Log("[TaskD] Done WhenAll...");
        await UniTask.Run(() =>
        {
            Debug.Log("[TaskD] Starting run wrapper...");
            for (int i = 0; i < 20000000; i++)
            {
                var x = Math.Pow(2, 10);
            }

            Debug.Log("[TaskD] Done run wrapper...");
        });

        isDoneTaskD = true;
        Debug.Log("[TaskD] Done...");
    }

    /// <summary>
    /// sub task a
    /// normal function
    /// </summary>
    private void SubTaskA()
    {
        Debug.Log("[SubTaskA] Starting...");
        for (int i = 0; i < 100000; i++)
        {
            var x = Math.Pow(2, 10);
        }

        Debug.Log("[SubTaskA] Done...");
    }

    /// <summary>
    /// sub task b
    /// normal function
    /// </summary>
    private void SubTaskB()
    {
        Debug.Log("[SubTaskB] Starting...");
        for (int i = 0; i < 200000; i++)
        {
            var x = Math.Pow(2, 10);
        }

        Debug.Log("[SubTaskB] Done...");
    }

    /// <summary>
    /// sub task c
    /// normal function
    /// </summary>
    private void SubTaskC()
    {
        Debug.Log("[SubTaskC] Starting...");
        for (int i = 0; i < 300000; i++)
        {
            var x = Math.Pow(2, 10);
        }

        Debug.Log("[SubTaskC] Done...");
    }

    #endregion
}