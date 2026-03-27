using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;


public class ScoreTests {
    GameObject gameObj;
    GameManager gameManager;

    [SetUp]
    public void Setup() {
        gameObj = new GameObject();
        gameManager = gameObj.AddComponent<GameManager>();
    }

    [Test]
    public void Score_Starts_At_0_0() {
        Assert.AreEqual(0, gameManager.GetMakes());
        Assert.AreEqual(0, gameManager.GetAttempts());
    }

    [Test]
    public void Made_Shot_Increments_Both_Counters() {
        gameManager.UpdateScore();
        
        Assert.AreEqual(1, gameManager.GetMakes());
        Assert.AreEqual(1, gameManager.GetAttempts());
    }
    
    [Test]
    public void Missed_Shot_Only_Increments_Attempts() {
        gameManager.UpdateAttempt();
        
        Assert.AreEqual(0, gameManager.GetMakes());
        Assert.AreEqual(1, gameManager.GetAttempts());
    }
    
    [Test]
    public void Score_Format_Displays_Correctly() {
        GameObject textObj = new GameObject();
        Text mockText = textObj.AddComponent<Text>();
        gameManager.scoreText = mockText;

        gameManager.UpdateScore();
        gameManager.UpdateAttempt(); 
        gameManager.UpdateAttempt(); 
        gameManager.UpdateScoreText();

        Assert.AreEqual("1/3", gameManager.GetScoreText().text);
    }
}
