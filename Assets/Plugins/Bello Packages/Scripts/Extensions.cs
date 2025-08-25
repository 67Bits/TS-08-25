using UnityEngine;

public static class Extensions
{
    public static async Awaitable WhenAll(this Awaitable[] toWait)
    {
        var completedCount = 0;
        for (int i = 0; i < toWait.Length; i++)
        {
            toWait[i].GetAwaiter().OnCompleted(() => completedCount++);
        }
        while (completedCount < toWait.Length)
            await Awaitable.EndOfFrameAsync();
    }
}
