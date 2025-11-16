using UnityEngine;

[System.Serializable]
public class Wave
{
    public string waveName;
    public GameObject[] enemyPrefab;
    public Transform[] spawnPoints;
    public float delayBetweenSpawns = 0.5f;
}
