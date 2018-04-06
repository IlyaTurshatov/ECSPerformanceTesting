using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Unity.PerformanceTesting;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

public class NewTestScript : ECSTestsFixture
{

    struct TestEntity : IComponentData
    {
        double dValue;
        float fValue;
    }

    string[] markers = { "GC.Alloc" };

    [PerformanceTest]
    public void CreateEntityTest()
    {
        using (Measure.Scope(markers))
        {
            for (int i = 0; i < 500000; i++)
            {
                m_Manager.CreateEntity(typeof(TestEntity));
            }
        }
    }

    string[] markers1 = { "Instantiate", "Instantiate.Copy", "Instantiate.Produce", "Instantiate.Awake" };

    [PerformanceTest]
    public void Instantiate_CreateCubes()
    {
        using (Measure.Scope(markers1))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            for (int i = 0; i < 5000; i++)
            {
                MonoBehaviour.Instantiate(cube);
            }
        }
    }

    NativeQueue<int> m_queue;
    // Use this for initialization
    void Start()
    {
        m_queue = new NativeQueue<int>(Allocator.Persistent);
    }

    void OnDisable()
    {
        m_queue.Dispose();
    }

    struct NativeQueueEnqueue : IJobParallelFor
    {
        public NativeQueue<int>.Concurrent m_queue;
        public void Execute(int index)
        {
            m_queue.Enqueue(index);
        }
    }

    [PerformanceTest]
    public void JobsPerformanceQueue()
    {
        m_queue = new NativeQueue<int>(Allocator.Persistent);
        using (Measure.Scope())
        {
            for (int i = 0; i < 1024 * 1024; ++i)
                m_queue.Enqueue(i);
        }
        m_queue.Dispose();
    }

    [PerformanceTest]
    public void JobsPerformanceConcurrent()
    {
        m_queue = new NativeQueue<int>(Allocator.Persistent);
        using (Measure.Scope())
        {
            NativeQueue<int>.Concurrent cq = m_queue;
            for (int i = 0; i < 1024 * 1024; ++i)
                cq.Enqueue(i);
        }
        m_queue.Dispose();
    }

    [PerformanceTest]
    public void JobsPerformanceQueueSchedule()
    {
        m_queue = new NativeQueue<int>(Allocator.Persistent);
        using (Measure.Scope())
        {
            var qjob = new NativeQueueEnqueue();
            qjob.m_queue = m_queue;
            qjob.Schedule(1024 * 1024, 16).Complete();
        }
        m_queue.Dispose();
    }
}
