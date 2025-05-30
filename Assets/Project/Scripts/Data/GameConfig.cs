using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    [Header("Game Settings")]
    public float GameDuration = 30f;
    public int BasePointValue = 1;
    public Vector2 SpawnInterval = new Vector2(1f, 2f);

    [Header("Coin Settings")]
    public float CoinLifetime = 3f;
    public int MaxCoinsOnScreen = 5;

    [Header("Pool Settings")]
    public int InitialPoolSize = 10;
    public int MaxPoolSize = 20;

    [Header("Performance")]
    public int TargetFrameRate = 60;
    public bool UseVSync = true;
}