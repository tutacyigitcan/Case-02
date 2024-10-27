
using System;
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
    public string saveTime;
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
        this.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
