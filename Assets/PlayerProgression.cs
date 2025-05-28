using System;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [SerializeField] public bool canGainEXP = false; // if false, player will not gain exp

    float xpGainRate = 1f; // rate at which exp is gained, can be modified later

    // singelton 
    public static PlayerProgression instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        var maploader = MapLoader.instance;
        if (maploader != null)
        {
            xpGainRate /= maploader.AIAmountMultiplier;
        }
    }


    public Action OnLevelUp;


    [SerializeField] int[] expRequirments;
    int currentLevel = 0;
    int exp = 0;

    public void GainEXP(int value)
    {

        value = Mathf.RoundToInt(value * xpGainRate); // apply xp gain rate
        if (!canGainEXP) return;
        exp += value;
        if (exp >= expRequirments[currentLevel])
        {
            LevelUp();
        }
    }


    void LevelUp()
    {
        exp -= expRequirments[currentLevel];
        currentLevel++;
        if (currentLevel >= expRequirments.Length)
        {
            currentLevel = expRequirments.Length - 1;
        }

        OnLevelUp?.Invoke();
        Debug.Log("Level Up! Current Level: " + currentLevel);
    }




}
