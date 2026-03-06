// <copyright file="OnceExecutor.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Locking;

public class OnceExecutor
{
    private readonly SemaphoreSlim initLock = new(1, 1);
    private bool executed;

    public async Task ExecuteOnce(Func<Task> action)
    {
        await initLock.WaitAsync();

        try
        {
            if (!executed)
            {
                await action();
                executed = true;
            }
        }
        finally
        {
            initLock.Release();
        }
    }
}
