using Avalonia.Threading;

namespace RanseiLink.XP;

public static class TaskExtensions
{
    /// <summary>
    /// "Avoid at all high cost" way of synchronously showing dialogs
    /// </summary>
    internal static T WaitOnDispatcherFrame<T>(this Task<T> task)
    {
        if (!task.IsCompleted)
        {
            var frame = new DispatcherFrame();
            task.ContinueWith(static (_, s) => ((DispatcherFrame)s).Continue = false, frame);
            Dispatcher.UIThread.PushFrame(frame);
        }

        return task.GetAwaiter().GetResult();
    }
}
