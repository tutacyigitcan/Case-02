
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //public int currentLives;
    public string sceneName;
    public int coins;
    public float[] position;
    public int storyProgress;
    public List<string> inventory;
    public int health;
    public PlayerData(string sceneName, Vector3 playerPosition, int health, List<string> inventory, int storyProgress)
    {
        this.sceneName = sceneName;
        position = new float[3];
        this.position[0] = playerPosition.x;
        this.position[1] = playerPosition.y;
        this.position[2] = playerPosition.z;
        
        this.health = health;
        this.inventory = new List<string>(inventory);
        this.storyProgress = storyProgress;
    }

    #region MyRegion

    /*
  public static PlayerData Instance { get; private set; }
  public string PlayerName { get; set; } = "Player";
  public Vector3 PlayerPosition { get; set; } = Vector3.zero;
  public int CurrentLives { get; set; } = 5;
  public int Coins { get; set; } = 0;
  public int Experience { get; set; } = 0;

  private PlayerData() {}

  private void Awake()
  {
      if (Instance == null)
      {
          Instance = this;
          DontDestroyOnLoad(gameObject);
      }
      else
      {
          Destroy(gameObject);
      }
  }*/

    #endregion
}
