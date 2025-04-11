using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsData", menuName = "Game/Character Stats")]
public class CharacterStatsData : ScriptableObject
{
    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseStrength = 10f;
    public float baseAgility = 10f;
    public float baseFireRate = 1f;
    public float baseDefense = 5f;
}