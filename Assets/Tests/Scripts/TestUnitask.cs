using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestUnitask : MonoBehaviour
{
    public bool pause;

    public void Start()
    {
        TestWhenAll();
        //LoadA(HeavyTaskA, HeavyTaskB, AsyncHeavyTaskC, AsyncHeavyTaskD);
        //LoadA0(NormalHeavyTaskA, NormalHeavyTaskB, HeavyTaskC, HeavyTaskD);
        // LoadA2(UniTask.Run(NormalHeavyTaskA), UniTask.Run(NormalHeavyTaskB), UniTask.Run(HeavyTaskC), UniTask.Run(HeavyTaskD));

        //LoadA3(UniTask.Run(NormalHeavyTaskA), UniTask.Run(NormalHeavyTaskB), UniTask.Run(HeavyTaskC), UniTask.Run(HeavyTaskD));

        //LoadStyleB(NormalHeavyTaskA, NormalHeavyTaskB, HeavyTaskC, HeavyTaskD);
        //LoadStyleB0(NormalHeavyTaskA, NormalHeavyTaskB, HeavyTaskC, HeavyTaskD);
        //LoadStyleB2(UniTask.Run(NormalHeavyTaskA), UniTask.Run(NormalHeavyTaskB), UniTask.Run(HeavyTaskC), UniTask.Run(HeavyTaskD));
        //LoadStyleB3(UniTask.Run(NormalHeavyTaskA), UniTask.Run(NormalHeavyTaskB), UniTask.Run(HeavyTaskC), UniTask.Run(HeavyTaskD));


//        UniTask.Run(async () =>
//        {
//            await UniTask.WhenAll(
//                UniTask.Run(NormalHeavyTaskA),
//                UniTask.Run(NormalHeavyTaskB),
//                UniTask.Run(HeavyTaskC));
//        });

//        await UniTask.WhenAll(
//            UniTask.Run(NormalHeavyTaskA),
//            UniTask.Run(NormalHeavyTaskB),
//            UniTask.Run(HeavyTaskC));
    }


    private async UniTask HeavyTaskAsyncA1()
    {
        for (int i = 0; i < 1000000; i++)
        {
            var example = Mathf.Pow(2, 10);
        }

        Debug.Log("Done A");
    }

    private async UniTask HeavyTaskAsyncB1()
    {
        for (int i = 0; i < 8000000; i++)
        {
            var example = Mathf.Pow(2, 10);
        }

        Debug.Log("Done B");
    }


    public async void TestWhenAll()
    {
        var backgroundTasks = new[]
        {
            StartFakeLoading()
                .ToUniTask(),
            UniTask.Run(AsyncHeavyTaskC),
            UniTask.Run(HeavyTaskAsyncB1)//,
            
            // UniTask.WaitUntil(() => isDoneTaskA && isDoneTaskB)
            //     .ContinueWith(async () =>
            //     {
            //         Debug.Log("IN CONTINUTE WITH A");
            //
            //         await UniTask.Run(() =>
            //         {
            //             Debug.Log("[TaskC] Starting run wrapper...");
            //             for (int i = 0; i < 30000000; i++)
            //             {
            //                 var x = Math.Pow(2, 10);
            //             }
            //
            //             Debug.Log("[TaskC] Done run wrapper...");
            //         });
            //
            //         Debug.Log("IN CONTINUTE WITH B");
            //         pause = false;
            //     }),
        };
        await UniTask.WhenAll(backgroundTasks);

        Debug.Log("All task complete!");
    }


    public async void LoadA(
        Action a,
        Action b,
        Action c,
        Action d)
    {
        Debug.Log("[Load A] Start ========================");
        await UniTask.WhenAll(StartFakeLoading()
                .ToUniTask(),
            UniTask.Run(Worker));

        Debug.Log("[Load A] Done =========================");

        // worker
        async void Worker()
        {
            await UniTask.WhenAll(UniTask.Run(a), UniTask.Run(b), UniTask.Run(c), UniTask.Run(d));

            Debug.Log("IN CONTINUTE WITH A");
            pause = false;
        }
    }


    public async void LoadStyleB(
        Action a,
        Action b,
        Action c,
        Action d)
    {
        Debug.Log("Start method B =========================");
        await UniTask.WhenAll(UniTask.Run(() => a?.Invoke()), UniTask.Run(() => b?.Invoke()), UniTask.Run(() => c?.Invoke()), UniTask.Run(() => d?.Invoke()))
            .ContinueWith(() =>
            {
                Debug.Log("in continute with B");
                return pause = false;
            });

        Debug.Log("Complete method B =========================");
    }

    public async void LoadA0(
        params Action[] p)
    {
        Debug.Log("Start A0");

        UniTask[] p3 = new UniTask[p.Length];
        for (int i = 0; i < p3.Length; i++)
        {
            p3[i] = UniTask.Run(p[i]);
        }

        await UniTask.WhenAll(StartFakeLoading()
                .ToUniTask(),
            UniTask.WhenAll(p3)
                .ContinueWith(() =>
                {
                    Debug.Log("in continute with A0");
                    return pause = false;
                }));

        Debug.Log("Complete A0");
    }

    public async void LoadStyleB0(
        params Action[] p)
    {
        Debug.Log("Start B0");
        UniTask p2 = UniTask.Run(() =>
        {
            for (int i = 0; i < p.Length; i++)
            {
                p[i]
                    ?.Invoke();
            }
        });
        await UniTask.WhenAll(p2)
            .ContinueWith(() =>
            {
                Debug.Log("in continute with B0");
                return pause = false;
            });

        Debug.Log("Complete B0");
    }

    public async void LoadA2(
        UniTask a,
        UniTask b,
        UniTask c,
        UniTask d)
    {
        Debug.Log("Start A2");
        await UniTask.WhenAll(StartFakeLoading()
                .ToUniTask(),
            UniTask.WhenAll(a, b, c, d)
                .ContinueWith(() =>
                {
                    Debug.Log("in continute with A2");
                    return pause = false;
                }));

        Debug.Log("Complete A2");
    }

    public async void LoadStyleB2(
        UniTask a,
        UniTask b,
        UniTask c,
        UniTask d)
    {
        Debug.Log("Start B2");
        await UniTask.WhenAll(a, b, c, d)
            .ContinueWith(() =>
            {
                Debug.Log("in continute with B2");
                return pause = false;
            });

        Debug.Log("Complete B2");
    }

    public async void LoadA3(
        params UniTask[] p)
    {
        Debug.Log("Start A3");
        await UniTask.WhenAll(StartFakeLoading()
                .ToUniTask(),
            UniTask.WhenAll(p)
                .ContinueWith(() =>
                {
                    Debug.Log("in continute with A3");
                    return pause = false;
                }));

        Debug.Log("Complete A3");
    }

    public async void LoadStyleB3(
        params UniTask[] p)
    {
        Debug.Log("Start B3");
        await UniTask.WhenAll(p)
            .ContinueWith(() =>
            {
                Debug.Log("in continute with B3");
                return pause = false;
            });

        Debug.Log("Complete B3");
    }

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

    #region out

    public bool isDoneTaskA;
    public bool isDoneTaskB;
    public bool isDoneTaskC;
    public bool isDoneTaskD;

    /// <summary>
    /// heavy task a
    /// </summary>
    public void HeavyTaskA()
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
    /// </summary>
    public void HeavyTaskB()
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
    /// </summary>
    public async void AsyncHeavyTaskC()
    {
        Debug.Log("[TaskC] Starting...");
        isDoneTaskC = false;

        await UniTask.WaitUntil(() => isDoneTaskA && isDoneTaskB);
            // .ContinueWith(async () =>
            // {
            //     
            //     Debug.Log("[TaskC] Done continue with...");
            // });
        
        
        // // nếu không được bao bọc bởi Unitask.Run thì nó sẽ thực thi toàn bộ vòng lặp trong 1 frame => freeze
        // // UniTaskLoopRunnerUpdate => UnityEngine.CoreModule.dll!::UpdateFunction.Invole()  1 call
        // // nhưng khi bao bọc bởi UniTask.Run thì hàm coi như kết thúc và vì vậy thứ tự thực thi mà bạn muốn khi gọi WhenAll ở trên đã không còn đúng nữa
        // // hãy cân nhắc để tránh sẩy ra lỗi
        // await UniTask.Run(() =>
        // {
        //     Debug.Log("[TaskC] Starting run wrapper...");
        //     for (int i = 0; i < 300000000; i++)
        //     {
        //         var x = Math.Pow(2, 10);
        //     }
        //
        //     Debug.Log("[TaskC] Done run wrapper...");
        // });

        isDoneTaskC = true;
        
        Debug.Log("[TaskC] Done...");
    }

    /// <summary>
    /// heavy task d
    /// </summary>
    public async void AsyncHeavyTaskD()
    {
        Debug.Log("[TaskD] Starting...");
        isDoneTaskD = false;

        await UniTask.WhenAll(UniTask.Run(SubTaskA), UniTask.Run(SubTaskB), UniTask.Run(SubTaskC) /*, UniTask.WaitUntil(() => isDoneTaskA && isDoneTaskB)*/);

        Debug.Log("[TaskD] Done WhenAll...");
        UniTask.Run(() =>
        {
            Debug.Log("[TaskD] Starting run wrapper...");
            for (int i = 0; i < 20000000; i++)
            {
                var x = Math.Pow(2, 10);
            }

            isDoneTaskD = true;
            Debug.Log("[TaskD] Done run wrapper...");
        });

        Debug.Log("[TaskD] Done...");
    }

    /// <summary>
    /// sub task a
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