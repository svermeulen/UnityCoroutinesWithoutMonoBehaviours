using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CoRoutineUtil
{
    // This can be used to make an untyped coroutine typed
    // (it's nice sometimes to work with untyped coroutines so you can yield other coroutines)
    public static IEnumerator<T> Wrap<T>(IEnumerator runner)
    {
        var coroutine = new CoRoutine(runner);

        while (coroutine.Pump())
        {
            yield return default(T);
        }

        if (coroutine.ReturnValue != null)
        {
            if (!typeof(T).IsAssignableFrom(coroutine.ReturnValue.GetType()))
            {
                throw new CoRoutine.AssertException(
                    string.Format("Unexpected type returned from coroutine!  Expected '{0}' and found '{1}'", typeof(T).Name, coroutine.ReturnValue.GetType().Name));
            }
        }

        yield return (T)coroutine.ReturnValue;
    }

    public static IEnumerator WaitSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }
    }

    // Block synchronously until the given coroutine completes
    public static void SyncWait(IEnumerator runner)
    {
        var coroutine = new CoRoutine(runner);

        while (coroutine.Pump())
        {
        }
    }

    // Block synchronously until the given coroutine completes
    // And give up after a timeout
    public static void SyncWaitWithTimeout(IEnumerator runner, float timeout)
    {
        var startTime = DateTime.UtcNow;
        var coroutine = new CoRoutine(runner);

        while (coroutine.Pump())
        {
            if ((DateTime.UtcNow - startTime).TotalSeconds > timeout)
            {
                throw new TimeoutException();
            }
        }
    }

    public static T SyncWaitGet<T>(IEnumerator<T> runner)
    {
        var coroutine = new CoRoutine(runner);

        while (coroutine.Pump())
        {
        }

        return (T)coroutine.ReturnValue;
    }

    public static T SyncWaitGet<T>(IEnumerator runner)
    {
        var coroutine = new CoRoutine(runner);

        while (coroutine.Pump())
        {
        }

        return (T)coroutine.ReturnValue;
    }

    // Execute all the given coroutines in parallel
    public static IEnumerator MakeParallelGroup(IEnumerable<IEnumerator> runners)
    {
        var runnerList = runners.Select(x => new CoRoutine(x)).ToList();

        while (runnerList.Any())
        {
            foreach (var runner in runnerList)
            {
                runner.Pump();
            }

            foreach (var runner in runnerList.Where(x => x.IsDone).ToList())
            {
                runnerList.Remove(runner);
            }

            yield return null;
        }
    }
}
