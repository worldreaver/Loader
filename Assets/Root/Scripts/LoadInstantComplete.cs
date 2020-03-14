namespace Worldreaver.Loading
{
    public class LoadInstantComplete : ILoadComplete
    {
        public void OnFinish(ILoading iLoad)
        {
            iLoad.SetActiveTip(false);
            iLoad.DisposableTips?.Dispose();
            iLoad.DisposableWaitTips?.Dispose();
            iLoad.FadeOutProcessBar();
            iLoad.LoadNextScene();
        }

        public void Setup()
        {
        }
    }
}