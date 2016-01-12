using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public class AsyncProcessor
{
    readonly List<CoroutineInfo> _newWorkers = new List<CoroutineInfo>();
    readonly LinkedList<CoroutineInfo> _workers = new LinkedList<CoroutineInfo>();

    public bool IsRunning
    {
        get
        {
            return _workers.Any() || _newWorkers.Any();
        }
    }

    public void Tick()
    {
        AddNewWorkers();

        if (!_workers.Any())
        {
            return;
        }

        AdvanceFrameAll();
        AddNewWorkers();
    }

    public IEnumerator Process(IEnumerator process)
    {
        return ProcessInternal(process);
    }

    void AdvanceFrameAll()
    {
        var currentNode = _workers.First;

        while (currentNode != null)
        {
            var next = currentNode.Next;
            var worker = currentNode.Value;

            try
            {
                worker.CoRoutine.Pump();
                worker.IsFinished = worker.CoRoutine.IsDone;
            }
            catch (Exception e)
            {
                worker.IsFinished = true;
                Debug.LogException(e);
            }

            if (worker.IsFinished)
            {
                _workers.Remove(currentNode);
            }

            currentNode = next;
        }
    }

    IEnumerator ProcessInternal(IEnumerator process)
    {
        var data = new CoroutineInfo()
        {
            CoRoutine = new CoRoutine(process),
        };

        _newWorkers.Add(data);

        return WaitUntilFinished(data);
    }

    IEnumerator WaitUntilFinished(CoroutineInfo workerData)
    {
        while (!workerData.IsFinished)
        {
            yield return null;
        }
    }

    void AddNewWorkers()
    {
        foreach (var worker in _newWorkers)
        {
            _workers.AddLast(worker);
        }
        _newWorkers.Clear();
    }

    class CoroutineInfo
    {
        public CoRoutine CoRoutine;
        public bool IsFinished;
    }
}
