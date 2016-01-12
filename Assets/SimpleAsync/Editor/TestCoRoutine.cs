using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TestCoRoutine
{
    [Test]
    public void TestObjectTrace()
    {
        try
        {
            var runner = new CoRoutine(Runner());

            while (runner.Pump())
            {
            }
        }
        catch (Exception e)
        {
            // Should print out a readable object trace
            Debug.LogException(e);
        }
    }

    IEnumerator Runner()
    {
        yield return null;
        yield return null;
        yield return null;

        yield return Run2();

        yield return null;
        yield return null;
        yield return null;
    }

    IEnumerator Run2()
    {
        yield return null;
        yield return null;

        yield return new NestedTest().Runner();

        yield return null;
        yield return null;
    }

    public class NestedTest
    {
        public IEnumerator Runner()
        {
            yield return null;
            yield return null;

            TestCoRoutine test = null;
            test.TestObjectTrace();

            yield return null;
        }
    }
}
