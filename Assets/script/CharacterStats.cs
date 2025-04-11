using System.Collections.Generic;

public class CharacterStats
{
    private Dictionary<StatType, float> stats;

    public CharacterStats(CharacterStatsData data)
    {
        stats = new Dictionary<StatType, float>
        {
            { StatType.Health, data.baseHealth },
            { StatType.Strength, data.baseStrength },
            { StatType.Agility, data.baseAgility },
        };
    }

    public float GetStat(StatType type) => stats.ContainsKey(type) ? stats[type] : 0f;
    public float GetMaxHealth() => stats[StatType.Health];
}