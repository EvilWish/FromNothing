using UnityEngine;

public class RemovingState : IBuildingState
{
    int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    ObjectPlacer objectPlacer;
    GameObject placedObjectParent;

    public RemovingState(Grid grid,
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

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if(floorData == null)
        {
            return;
        }
        else
        {
            gameObjectIndex = floorData.GetRepresentationIndex(gridPosition);

            if (gameObjectIndex == -1)
                return;


            SOItem item = objectPlacer.GetObjectAt(gameObjectIndex);
            InventoryManager.Instance.AddItem(item, 1);

            floorData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }

        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));

    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
