using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PlayerData : MonoBehaviour
{
    [Header("Player Default Stats")]
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float currentHealth;
    [Space]
    [SerializeField] private byte tierStage = 1;
    [Space]
    
    [Header("Carry/Backpack")]
    [SerializeField] private float maxCarryWeight = 64.0f;
    [SerializeField] private float currentCarryWeight;
    

    #region Unity
    private void Start()
    {
        OnGameLoad();
    }

    #endregion

    #region LoadData
    void OnGameLoad()
    {
        // Fallback when no PlayerSaveData !
        currentHealth = maxHealth;
        currentCarryWeight = 0;
    }
    #endregion

    #region Getter
    public float GetCurrentCarryWeight() => currentCarryWeight; 
    public float GetMaxCarryWeight() => maxCarryWeight;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    internal void AddCurrentCarryWeight(float itemWeight)
    {
        currentCarryWeight += itemWeight;
    }
    #endregion
}
