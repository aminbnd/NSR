
namespace SilverTau.NSR.Samples
{
    public interface ISCProcess<T>
    {
        void Change(T option);
        void ResetProcess();
    }
}