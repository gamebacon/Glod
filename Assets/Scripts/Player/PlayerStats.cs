using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public Dictionary<PlayerStatType, PlayerStat> stats = new();
    public float statUpdateInterval = 5f; // Adjust the time interval in seconds

    private void Awake()
    {
        stats[PlayerStatType.HEALTH] = new PlayerStat(PlayerStatType.HEALTH, 100, 0, 0, 100);
        stats[PlayerStatType.HUNGER] = new PlayerStat(PlayerStatType.HUNGER, 0, 5, 0, 100);
        stats[PlayerStatType.THIRST] = new PlayerStat(PlayerStatType.THIRST, 0, 7, 0, 100);
        stats[PlayerStatType.STAMINA] = new PlayerStat(PlayerStatType.STAMINA, 100, 0, 0, 100);

        StartCoroutine(UpdateStatsOverTime());
    }

    private IEnumerator UpdateStatsOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(statUpdateInterval);

            foreach (var stat in stats.Values)
            {
                stat.UpdateOverTime();
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

    public void Modify(float amount)
    {
        Value += amount;
    }
}


public enum PlayerStatType
{
    HUNGER,
    HEALTH,
    THIRST,
    STAMINA
}
