namespace Worldreaver.Loading
{
    public interface ILoadComplete
    {
        /// <summary>
        /// on finish
        /// </summary>
        /// <param name="iLoad"></param>
        void OnFinish(ILoading iLoad);

        /// <summary>
        /// setup
        /// </summary>
        void Setup();
    }
}