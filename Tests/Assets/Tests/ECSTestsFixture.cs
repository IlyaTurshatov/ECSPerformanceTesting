using NUnit.Framework;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

public class ECSTestsFixture
{
    protected World m_PreviousWorld;
    protected World World;
    protected EntityManager m_Manager;

    protected int StressTestEntityCount = 1000;

    [SetUp]
    public virtual void Setup()
    {
        m_PreviousWorld = World.Active;
        World = World.Active = new World("Test World");
        m_Manager = World.GetOrCreateManager<EntityManager>();
    }

    [TearDown]
    public virtual void TearDown()
    {
        if (m_Manager != null)
        {
            World.Dispose();
            World = null;

            World.Active = m_PreviousWorld;
            m_PreviousWorld = null;
            m_Manager = null;
        }
    }
}
