using NUnit.Framework;
using UnityEngine;

public class ShotProbabilityTests
{
    private ShotLogic logic;

    [SetUp]
    public void Setup()
    {
        logic = new ShotLogic();
    }

    [Test]
    public void GetZone_Returns_Correct_Zones()
    {
        // test the upper and lower boundry of each zone
        Assert.AreEqual("Green", logic.GetZone(0.45f));
        Assert.AreEqual("Green", logic.GetZone(0.55f));

        Assert.AreEqual("Yellow", logic.GetZone(0.35f));
        Assert.AreEqual("Yellow", logic.GetZone(0.44f));
        Assert.AreEqual("Yellow", logic.GetZone(0.56f));
        Assert.AreEqual("Yellow", logic.GetZone(0.65f));
        
        Assert.AreEqual("Orange", logic.GetZone(0.25f));
        Assert.AreEqual("Orange", logic.GetZone(0.34f));
        Assert.AreEqual("Orange", logic.GetZone(0.66f));
        Assert.AreEqual("Orange", logic.GetZone(0.75f));

        
        Assert.AreEqual("Red", logic.GetZone(0.24f));
        Assert.AreEqual("Red", logic.GetZone(0.76f));
        Assert.AreEqual("Red", logic.GetZone(0.0f));
        Assert.AreEqual("Red", logic.GetZone(1.0f));
    }

    [Test]
    public void IsShotMade_Green_Always_Makes()
    {
        Assert.IsTrue(logic.IsShotMade("Green", 0.99f));
        Assert.IsTrue(logic.IsShotMade("Green", 0.01f));
    }

    [Test]
    public void IsShotMade_Yellow_Probability_Check()
    {
        // Yellow threshold is < 0.75
        Assert.IsTrue(logic.IsShotMade("Yellow", 0.74f), "Yellow should make at 0.74");
        Assert.IsFalse(logic.IsShotMade("Yellow", 0.76f), "Yellow should miss at 0.76");
    }

    [Test]
    public void IsShotMade_Orange_Probability_Check()
    {
        // Orange threshold is < 0.50
        Assert.IsTrue(logic.IsShotMade("Orange", 0.49f), "Orange should make at 0.49");
        Assert.IsFalse(logic.IsShotMade("Orange", 0.51f), "Orange should miss at 0.51");
    }

    [Test]
    public void IsShotMade_Red_Always_Misses()
    {
        // Red should return false 100% of the time
        Assert.IsFalse(logic.IsShotMade("Red", 0.01f));
        Assert.IsFalse(logic.IsShotMade("Red", 0.99f));
    }
}