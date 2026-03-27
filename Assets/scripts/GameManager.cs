using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [Header("UI References")]
    public Slider powerBar;
    public Image barBackground;
    public Text scoreText;

    [Header("Settings")]
    public float barSpeed = 1.0f;
    public Animator ballAnimator; 
    
    private ShotLogic logic = new ShotLogic();
    private int score = 0;
    private int totalShots = 0;
    private bool movingUp = true;
    private bool isShooting = false; 

    void Update() {
        // The bar moves ONLY when we aren't in the middle of a shot
        if (!isShooting) {
            MoveBar();
        }
    }

    void MoveBar() {
        if (movingUp) {
            powerBar.value += Time.deltaTime * barSpeed;
        }
        else {
            powerBar.value -= Time.deltaTime * barSpeed;
        }

        if (powerBar.value >= 1f) movingUp = false;
        if (powerBar.value <= 0f) movingUp = true;
    }

    // This is the function your Button's "On Click()" should point to
    public void Shoot() {
      // ensure user can't spam shoot
      if (isShooting) {
        return; 
      }
      isShooting = true;
      float timing = powerBar.value; 
      string zone = logic.GetZone(timing);
      
      // Change bar color to the zone hit
      if (barBackground != null) {
          barBackground.color = zone switch {
              "Green"  => Color.green,
              "Yellow" => Color.yellow,
              "Orange" => Color.orange, 
              _ => Color.red
          };
      }

      bool made = logic.IsShotMade(zone, Random.Range(0f, 1f));

      if (made) {
          UpdateScore();
          if (ballAnimator != null) ballAnimator.Play("shot_make");
      }
      else {
          UpdateAttempt();
          if (ballAnimator != null) ballAnimator.Play("shot_miss");
      }

      UpdateScoreText();
      
      // Wait 2 seconds, then call reset game
      Invoke("ResetShot", 2f);
    }

    public void ResetShot() {
        isShooting = false; // Resume the bar movement
        powerBar.value = 0;
        
        if (barBackground != null) {
          barBackground.color = Color.black;
        }

        if (ballAnimator != null) {
          ballAnimator.Play("Idle");
        }
    }

    // getters and setters
    public int GetMakes() {
      return score;
    }

    public int GetAttempts() {
      return totalShots;
    }

    public void UpdateScore() {
      score++;
      totalShots++;
    }

    public void UpdateAttempt() {
      totalShots++;
    }

    public Text GetScoreText() {
      return scoreText;
    } 

    public void UpdateScoreText() {
      scoreText.text = GetMakes() + "/" + GetAttempts();
    }
}