using System.Collections.Generic;
using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] PlayerInteraction playerInteraction;
    [SerializeField] Grid grid;

    [SerializeField] GameObject gridVisualization;

    private GridData floorData;


    [SerializeField]
    private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] public GameObject placedObjectParent;

    private bool isBuildModeActive = false;
    private bool isRemoving = false;


    IBuildingState buildingState;

    private void Start()
    {
        gridVisualization.SetActive(false);
        StopPlacement();
        floorData = new GridData();
    }

    public void StartPlacement()
    {
        StopPlacement();

        if (InventoryManager.Instance.GetHandItem() != null && InventoryManager.Instance.GetHandItem().itemType == ItemType.Placeable)
        {
            buildingState = new PlacementState(grid, previewSystem, floorData, objectPlacer, placedObjectParent);
            gridVisualization.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlaceStructure();
            StopPlacement();
        }
    }

    public void StartRemoving()
    {
        if (isRemoving)
        {
            if (InventoryManager.Instance.GetHandItem() == null)
            {
                gridVisualization.SetActive(true);
                buildingState = new RemovingState(grid, previewSystem, floorData, objectPlacer, placedObjectParent);
            }
            else
            {
                StopPlacement();
                gridVisualization.SetActive(false);
                isRemoving = false;
            }
        }
    }

    private void PlaceStructure()
    {
        Vector3 mousePosition = playerInteraction.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (buildingState != null)
            buildingState.OnAction(gridPosition);

    }



    private void StopPlacement()
    {
        if (buildingState == null)
            return;

        gridVisualization.SetActive(false);
        isRemoving = false;
        isBuildModeActive = false; 
        lastDetectedPosition = Vector3Int.zero;
        buildingState.EndState();
        buildingState = null;
    }

    public bool IsBuilding()
    {
        return isBuildModeActive;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            isRemoving = !isRemoving;
            isBuildModeActive = false;
            StopPlacement();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuildModeActive = !isBuildModeActive;
            isRemoving = false;
            StopPlacement();
        }

        
        if (isBuildModeActive && !isRemoving && InventoryManager.Instance.GetHandItem() != null && InventoryManager.Instance.GetHandItem().itemType == ItemType.Placeable)
        {
            if (buildingState is not PlacementState)
            {
                buildingState = new PlacementState(grid, previewSystem, floorData, objectPlacer, placedObjectParent);
                gridVisualization.SetActive(true);
            }
        }
        else if (isRemoving)
        {
            if (buildingState is not RemovingState)
            {
                buildingState = new RemovingState(grid, previewSystem, floorData, objectPlacer, placedObjectParent);
                gridVisualization.SetActive(true);
            }
        }
        else
        {
            if (buildingState != null)
                StopPlacement();
        }

        if (buildingState == null)
            return;

        Vector3 mousePosition = playerInteraction.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            buildingState.OnAction(gridPosition);
        }
    }
}
