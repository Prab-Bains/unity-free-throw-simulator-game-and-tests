using UnityEngine;

public class ShotLogic {
    public string GetZone(float timing) {
      // GREEN (0.45 to 0.55)
      // YELLOW (0.35-0.44 and 0.56-0.65)
      // ORANGE (0.25-0.34 and 0.66-0.75)
      // RED (0.0-0.24 or 0.76-1.0)
      if (timing >= 0.45f && timing <= 0.55f)
          return "Green";
      else if ((timing >= 0.35f && timing < 0.45f) || (timing > 0.55f && timing <= 0.65f))
          return "Yellow";
      else if ((timing >= 0.25f && timing < 0.35f) || (timing > 0.65f && timing <= 0.75f))
          return "Orange";
      else
          return "Red";
    }

    public bool IsShotMade(string zone, float randomValue) {
      switch (zone)
      {
          case "Green": return true; // 100% chance a shot is made in green zone
          case "Yellow": return randomValue < 0.75f; // 75% chance a shot is made in yellow zone
          case "Orange": return randomValue < 0.5f; // 50% chance a shot is made in orange zone
          case "Red": return false; // 0% chance a shot is made in red zone
          default: return false;
      }
    }
}
