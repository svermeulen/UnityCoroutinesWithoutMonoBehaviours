using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TestAsyncProcessor
{
    AsyncProcessor _asyncHandler;

    [SetUp]
    public void Setup()
    {
        _asyncHandler = new AsyncProcessor();
    }

    [Test]
    public void TestRunForSeconds()
    {
        _asyncHandler.Process(RunFor1Second());

        var start = Time.realtimeSinceStartup;
        RunProcessesToEnd();

        Assert.That((Time.realtimeSinceStartup - start) >= 1);
    }

    [Test]
    public void TestMultiple()
    {
        // They should run in parallel
        _asyncHandler.Process(RunFor1Second());
        _asyncHandler.Process(RunFor1Second());
        _asyncHandler.Process(RunFor1Second());

        var start = Time.realtimeSinceStartup;
        RunProcessesToEnd();
        Assert.That((Time.realtimeSinceStartup - start) >= 1);
    }

    [Test]
    public void TestNested()
    {
        // They should run in parallel
        _asyncHandler.Process(RunNested());

        var start = Time.realtimeSinceStartup;
        RunProcessesToEnd();
        Assert.That((Time.realtimeSinceStartup - start) >= 2);
    }

    IEnumerator RunNested()
    {
        yield return RunFor1Second();
        yield return RunFor1Second();
    }

    void RunProcessesToEnd()
    {
        while (_asyncHandler.IsRunning)
        {
            _asyncHandler.Tick();
            Thread.Sleep(10);
        }
    }

    IEnumerator RunFor1Second()
    {
        var start = Time.realtimeSinceStartup;

        while ((Time.realtimeSinceStartup - start) < 1)
        {
            yield return null;
        }
    }
}
