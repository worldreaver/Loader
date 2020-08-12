using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TestCancelationToken : MonoBehaviour
{
   private CancellationTokenSource _tokenSource = new CancellationTokenSource();
   private CancellationToken _token;

   private async void Start()
   {
      _token = _tokenSource.Token;

      var task = Task.Run(() =>
         {
            _token.ThrowIfCancellationRequested();

            bool moreToDo = true;
            while (moreToDo)
            {
               Debug.Log("task run");
               // if (_token.IsCancellationRequested)
               // {
               //    _token.ThrowIfCancellationRequested();
               // }
            }
         },
         _tokenSource.Token);

      StartCoroutine(IeWait());
      try
      {
         await task;
      }
      catch (Exception e)
      {
         Debug.Log($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
      }
      finally
      {
         _tokenSource.Dispose();
      }
   }

   private IEnumerator IeWait()
   {
      yield return new WaitForSeconds(0.1f);
      _tokenSource.Cancel();
   }
}
