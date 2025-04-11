[System.Serializable]
public struct StatModifierData
{
    public string id;
    public StatType statType;
    public float value;
    public float duration;
}

public enum StatType { Health, Strength, Agility, FireRate }