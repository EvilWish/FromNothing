using UnityEngine;

public class PlacementState : IBuildingState
{
    //private int selectedObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    ObjectPlacer objectPlacer;
    SOItem currentSelectedItem;
    GameObject placedObjectParent;

    public PlacementState(Grid grid,
                          PreviewSystem previewSystem,
                          GridData floorData,
                          ObjectPlacer objectPlacer,
                          GameObject placedObjectParent)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.objectPlacer = objectPlacer;
        this.placedObjectParent = placedObjectParent;


        if(InventoryManager.Instance.GetHandItem() != null && InventoryManager.Instance.GetHandItem().itemType == ItemType.Placeable)
        {
            this.currentSelectedItem = InventoryManager.Instance.GetHandItem();
            previewSystem.StartShowingPlacementPreview(currentSelectedItem.buildingPrefab, currentSelectedItem.buildingSize);
            this.placedObjectParent = placedObjectParent;
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (placementValidity == false)
        {
            return;
        }

        GameObject newObject = objectPlacer.PlaceObject(currentSelectedItem.buildingPrefab, grid.CellToWorld(gridPosition), placedObjectParent, out int index);

        InventoryManager.Instance.GetSelectedItem(true);
        floorData.AddObjectAt(gridPosition, currentSelectedItem.buildingSize, currentSelectedItem, index);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return floorData.CanPlaceObjectAt(gridPosition, currentSelectedItem.buildingSize);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
