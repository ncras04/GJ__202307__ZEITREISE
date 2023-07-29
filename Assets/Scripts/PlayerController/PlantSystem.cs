using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this class to the non gunner player. Lets him plant trees for the other side.
/// </summary>
public class PlantSystem : MonoBehaviour
{
    [SerializeField] GameObject plantPrefab;
    [SerializeField] GameObject plantEffect;
    [SerializeField] float yPlantPositionTop;
    [SerializeField] float yPlantPositionBottom;
    [SerializeField] private GlobalInventory _globalInventory;

    PlayerController playerController;
    PlayerManager playerManager;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        if(playerController != null )
        {
            playerController.InteractionHandler += PlantSaporling;
        }

        playerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

    private void PlantSaporling()
    {
        // If global inventory has this and the location is valid.
        if(playerManager != null)
        {
            float yOffset = !playerManager.IsSwapped ? yPlantPositionTop : yPlantPositionBottom;

            // Eventually a boxcast, but dunno what to check for, so just plant it.
            Instantiate(plantPrefab, new Vector3(transform.position.x,yOffset,transform.position.z), Quaternion.identity);

            // Instantiate the particle effect.
            Instantiate(plantEffect, transform.position, Quaternion.identity);

        }
    }

}
