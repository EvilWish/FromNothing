using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedGameObjects = new();

    public GameObject PlaceObject(GameObject placedPrefab, Vector3 position, GameObject objectParent, out int index)
    {
        GameObject newObject = Instantiate(placedPrefab);
        newObject.transform.parent = objectParent.transform;
        newObject.transform.position = position;
        placedGameObjects.Add(newObject);
        index = placedGameObjects.Count - 1;
        return newObject;
    }

    public void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null) return;

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }

    public SOItem GetObjectAt(int gameObjectIndex)
    {
        return placedGameObjects[gameObjectIndex].GetComponent<ItemData>().baseItem;
    }
}
