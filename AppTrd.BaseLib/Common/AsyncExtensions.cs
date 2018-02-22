using System.Threading.Tasks;

namespace AppTrd.BaseLib.Common
{
    public static class AsyncExtensions
    {
        public static T RunSync<T>(this Task<T> task)
        {
            task.Wait();

            if (task.Exception != null)
                throw task.Exception;

            return task.Result;
        }

        public static void RunSync(this Task task)
        {
            task.Wait();

            if (task.Exception != null)
                throw task.Exception;
        }
    }
}
