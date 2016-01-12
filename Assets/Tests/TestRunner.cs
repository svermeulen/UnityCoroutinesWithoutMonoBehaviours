using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRunner : MonoBehaviour
{
    AsyncProcessor processor;

    public void Start()
    {
        processor = new AsyncProcessor();
        processor.Process(Coroutine1());
        processor.Process(GetExample1());
        processor.Process(GetExample2());
    }

    IEnumerator Coroutine1()
    {
        Debug.Log("Started Coroutine1 at " + Time.realtimeSinceStartup);

        // Note that you can yield other coroutines with AsyncProcessor
        yield return WaitABit();

        Debug.Log("Completed Coroutine1 at " + Time.realtimeSinceStartup);
    }

    IEnumerator WaitABit()
    {
        var start = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - start < 2)
        {
            yield return null;
        }
    }

    IEnumerator GetExample1()
    {
        Debug.Log("Looking up string 1...");
        var stringValue = SlowLookup();
        yield return stringValue;
        Debug.Log("Received string 1: " + stringValue.Current);
    }

    IEnumerator<string> SlowLookup()
    {
        var start = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - start < 2)
        {
            yield return null;
        }

        yield return "return value string";
    }

    IEnumerator GetExample2()
    {
        Debug.Log("Looking up string 2...");
        var stringValue = SlowLookup2();
        yield return stringValue;
        Debug.Log("Received string 2: " + stringValue.Current);
    }

    IEnumerator<string> SlowLookup2()
    {
        // Here we make SlowLookup untyped so we can return nested ienumerators
        return CoRoutineUtil.Wrap<string>(SlowLookup2Impl());
    }

    IEnumerator SlowLookup2Impl()
    {
        yield return WaitABit();
        yield return WaitABit();
        yield return WaitABit();
        yield return "returned string value";
    }

    public void Update()
    {
        processor.Tick();
    }
}
