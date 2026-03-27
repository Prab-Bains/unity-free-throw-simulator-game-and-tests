using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayTests
{
    private GameManager gameManager;

    [UnitySetUp]
    public IEnumerator Setup(){
        // 1. Load the actual game world
        SceneManager.LoadScene("SampleScene");

        yield return null; 

        // Find the GameManager from loaded scene
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    [UnityTest]
    public IEnumerator Verify_Initial_Object_Positions() {
        // Verify 3D object positions
        GameObject hoop = GameObject.Find("Hoop");
        Assert.IsNotNull(hoop, "Hoop object missing from hierarchy!");
        Assert.AreEqual(new Vector3(0, 0, 8), hoop.transform.position, "Hoop is not at (0,0,8)");

        GameObject floor = GameObject.Find("Floor");
        Assert.IsNotNull(floor, "Floor object missing!");
        Assert.AreEqual(new Vector3(0, 0, 0), floor.transform.position);

        GameObject net = GameObject.Find("Net");
        Assert.IsNotNull(net, "Net object missing!");
        Assert.AreEqual(new Vector3(0, 3, 8), net.transform.position);

        // Verify 2D object positions
        // Use tolerance to avoid float precision errors
        float tolerance = 0.1f;

        GameObject shootButton = GameObject.Find("Button");
        Assert.IsNotNull(shootButton, "Button missing!");
        Vector2 buttonPos = shootButton.GetComponent<RectTransform>().anchoredPosition;
        Assert.LessOrEqual(Vector2.Distance(buttonPos, new Vector2(0, -300)), tolerance, $"Button at {buttonPos} is not near (0,-300)");

        GameObject slider = GameObject.Find("ShotMeter");
        Assert.IsNotNull(slider, "ShotMeter missing!");
        Vector2 sliderPos = slider.GetComponent<RectTransform>().anchoredPosition;
        Assert.LessOrEqual(Vector2.Distance(sliderPos, new Vector2(0, -260)), tolerance, "Slider misaligned!");

        Transform bgTransform = slider.transform.Find("Background");
        Assert.IsNotNull(bgTransform, "Could not find 'Background' child inside ShotMeter");
        Image sliderBg = bgTransform.GetComponent<Image>();
        Assert.AreEqual(Color.black, sliderBg.color, "Slider background should be Black at start");

        GameObject scoreText = GameObject.Find("ScoreText");
        Assert.IsNotNull(scoreText, "ScoreText missing!");
        Vector2 scorePos = scoreText.GetComponent<RectTransform>().anchoredPosition;
        Assert.LessOrEqual(Vector2.Distance(scorePos, new Vector2(0, 250)), tolerance, "ScoreText misaligned!");

        yield return null;
    }

    [UnityTest]
    public IEnumerator Verify_Slider_Changes_Colour_Depending_On_Shot_Accuracy() {
        // --- GREEN (0.45 to 0.55) ---
        yield return TestColorAtValue(0.451f, Color.green);
        yield return TestColorAtValue(0.549f, Color.green);

        // --- YELLOW (0.35-0.45 and 0.55-0.65) ---
        yield return TestColorAtValue(0.351f, Color.yellow);
        yield return TestColorAtValue(0.449f, Color.yellow);
        yield return TestColorAtValue(0.551f, Color.yellow);
        yield return TestColorAtValue(0.649f, Color.yellow);
        
        // --- ORANGE (0.25-0.35 and 0.65-0.75) ---
        Color orange = new Color(1f, 0.64f, 0f);
        yield return TestColorAtValue(0.251f, Color.orange);
        yield return TestColorAtValue(0.349f, Color.orange);
        yield return TestColorAtValue(0.651f, Color.orange);
        yield return TestColorAtValue(0.749f, Color.orange);

        // --- RED (Below 0.25 or Above 0.75) ---
        yield return TestColorAtValue(0.24f, Color.red);
        yield return TestColorAtValue(0.76f, Color.red);
        yield return TestColorAtValue(0.0f, Color.red);
        yield return TestColorAtValue(1.0f, Color.red);
    }

    [UnityTest]
    public IEnumerator Verify_Correct_Animation_Is_Played_After_Made_Shot() {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        Slider slider = GameObject.Find("ShotMeter").GetComponent<Slider>();
        GameObject ball = GameObject.Find("Basketball"); 
        
        // Force a "Green" shot (Guaranteed Make)
        slider.value = 0.5f; 
        gm.Shoot();
        yield return new WaitForEndOfFrame();

        AnimatorStateInfo state = gm.ballAnimator.GetCurrentAnimatorStateInfo(0);
        Assert.IsTrue(state.IsName("shot_make"), "Animation 'shot_make' did not play on a Green shot!");

        // Verify ball has moved halfway through the animation
        yield return new WaitForSeconds(0.5f);
        Vector3 startingPos = new Vector3(0, 1, 0);
        Assert.AreNotEqual(startingPos, ball.transform.position, "Ball is stuck at start position halfway through animation!");

        // Verify ball position at the end of the animation
        yield return new WaitForSeconds(0.5f);
        
        Vector3 finalPos = new Vector3(0, 0.3f, 6.5f);
        float distance = Vector3.Distance(finalPos, ball.transform.position);
        float tolerance = 0.01f; // tolerance again

        Assert.LessOrEqual(distance, tolerance, $"Ball did not reach final position! Expected {finalPos} but was {ball.transform.position}. Distance: {distance}");

        // wait for annimation to reset to starting position (idle state)
        yield return new WaitForSeconds(2.1f);
        Assert.AreEqual(startingPos, ball.transform.position, "Ball is not at starting position after shot was reset!");

        yield return null;
    }

    [UnityTest]
    public IEnumerator Verify_Correct_Animation_Is_Played_After_Missed_Shot() {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        Slider slider = GameObject.Find("ShotMeter").GetComponent<Slider>();
        GameObject ball = GameObject.Find("Basketball"); 
        
        // Force a "Red" shot (Guaranteed Miss)
        slider.value = 0.1f; 
        gm.Shoot();
        yield return new WaitForEndOfFrame();

        AnimatorStateInfo state = gm.ballAnimator.GetCurrentAnimatorStateInfo(0);
        Assert.IsTrue(state.IsName("shot_miss"), "Animation 'shot_miss' did not play on a Red shot!");

        // Verify ball has moved halfway through the animation
        yield return new WaitForSeconds(0.5f);
        Vector3 startingPos = new Vector3(0, 1, 0);
        Assert.AreNotEqual(startingPos, ball.transform.position, "Ball is stuck at start position halfway through animation!");

        // Verify ball position at the end of the animation
        yield return new WaitForSeconds(0.5f);
        Vector3 finalPos = new Vector3(4, 0.3f, 6.5f);
        float distance = Vector3.Distance(finalPos, ball.transform.position);
        float tolerance = 0.01f; // tolerance again

        Assert.LessOrEqual(distance, tolerance, $"Ball did not reach final position! Expected {finalPos} but was {ball.transform.position}. Distance: {distance}");

        // wait for annimation to reset to starting position (idle state)
        yield return new WaitForSeconds(2.1f);
        Assert.AreEqual(startingPos, ball.transform.position, "Ball is not at starting position after shot was reset!");

        yield return null;
    }

    // Helper Method:
    private IEnumerator TestColorAtValue(float accuracy, Color expectedColor) {
        Slider slider = GameObject.Find("ShotMeter").GetComponent<Slider>();
        GameManager gm = GameObject.FindObjectOfType<GameManager>();

        slider.value = accuracy;
        gm.Shoot(); 
        Assert.AreEqual(expectedColor, gm.barBackground.color, $"Color mismatch at slider value: {accuracy}");
        
        // Use a small delay to let the UI update its internal state
        yield return new WaitForEndOfFrame();
        gm.ResetShot();
        yield return new WaitForEndOfFrame();


        Assert.AreEqual(Color.black, gm.barBackground.color, 
            $"Color mismatch at slider reset");

        yield return null; 
    }

}