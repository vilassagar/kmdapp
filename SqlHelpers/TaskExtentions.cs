using System.Threading.Tasks;

namespace SqlHelpers
{
    internal static class TaskExtentions
    {
        internal static T TaskResult<T>(this Task<T> task)
        {
            return task.GetAwaiter().GetResult();
        }
    }
}
