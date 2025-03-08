using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public Dictionary<PlayerStatType, PlayerStat> stats = new();
    public float statUpdateInterval = 0.1f; // Adjust the time interval in seconds

    private HUDManager _UIPlayerStats;

    private void OnEnable()
    {
        _UIPlayerStats = FindFirstObjectByType<HUDManager>();

        stats[PlayerStatType.HEALTH] = new PlayerStat(PlayerStatType.HEALTH, 100, 0, 0, 100);
        stats[PlayerStatType.HUNGER] = new PlayerStat(PlayerStatType.HUNGER, 0, 0.1f, 0, 100);
        stats[PlayerStatType.THIRST] = new PlayerStat(PlayerStatType.THIRST, 0, 0.2f, 0, 100);
        stats[PlayerStatType.STAMINA] = new PlayerStat(PlayerStatType.STAMINA, 100, 1, 0, 100);

        StartCoroutine(UpdateStatsOverTime());
    }

    private IEnumerator UpdateStatsOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(statUpdateInterval);

            foreach (var stat in stats.Values)
            {
                // optimize
                stat.UpdateOverTime();
                _UIPlayerStats.SetSliderValue(stat.Type, stat.GetPercentageValue());
            }

            // Reduce health if hunger or thirst reaches 100
            if (stats[PlayerStatType.HUNGER].Value >= 100 || stats[PlayerStatType.THIRST].Value >= 100)
            {
                stats[PlayerStatType.HEALTH].Modify(-5);
            }

            Debug.Log($"Hunger: {stats[PlayerStatType.HUNGER].Value}, Thirst: {stats[PlayerStatType.THIRST].Value}, Stamina: {stats[PlayerStatType.STAMINA].Value}, Health: {stats[PlayerStatType.HEALTH].Value}");
        }
    }

    public float GetStatValue(PlayerStatType type) => stats.ContainsKey(type) ? stats[type].Value : 0;
    public void SetStatValue(PlayerStatType type, float value)
    {
        float newValue = stats[type].Modify(value);
        _UIPlayerStats.SetSliderValue(type, newValue);
    }
}


public class PlayerStat
{
    public PlayerStatType Type { get; private set; }
    private float _value;
    public float Value
    {
        get => _value;
        set => _value = Mathf.Clamp(value, MinValue, MaxValue);
    }

    public float GetPercentageValue()
    {
        return Value / MaxValue;
    }

    public float ChangeRate { get; private set; } // Rate of change per update
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }

    public PlayerStat(PlayerStatType type, float initialValue, float changeRate, float minValue, float maxValue)
    {
        Type = type;
        ChangeRate = changeRate;
        MinValue = minValue;
        MaxValue = maxValue;
        Value = initialValue;
    }

    public void UpdateOverTime()
    {
        Value += ChangeRate;
    }

    public float Modify(float amount)
    {
        Value += amount;
        return GetPercentageValue();
    }
}


public enum PlayerStatType
{
    HUNGER,
    HEALTH,
    THIRST,
    STAMINA
}
