using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    // instance
    public static MiniMapManager instance;

    List<MiniMapObject> miniMapObjects = new List<MiniMapObject>();

    void Awake()
    {
        instance = this;
    }

    // add object to list
    public void AddMinimapObject(MiniMapObject obj)
    {
        miniMapObjects.Add(obj);
    }

    // remove object from list
    public void RemoveMinimapObject(MiniMapObject obj)
    {
        miniMapObjects.Remove(obj);
    }

    public List<MiniMapObjectData> GetMiniMapObjectsInDistance(Vector3 position, float maxDistance)
    {
        List<MiniMapObjectData> objects = new List<MiniMapObjectData>();
        var positionWithoutY = new Vector3(position.x, 0, position.z);
        foreach (MiniMapObject obj in miniMapObjects)
        {
            var objPosWithoutY = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
            var distance = Vector3.Distance(positionWithoutY, objPosWithoutY);
            if (distance <= maxDistance)
            {
                Vector3 direction = objPosWithoutY - positionWithoutY;
                // rotate direction with player forward
                //direction = Quaternion.Euler(0, -forward.y, 0) * direction;



                Vector3 mapPosition  = direction.normalized * distance / maxDistance;
                objects.Add(new MiniMapObjectData(obj, new Vector2(mapPosition.x, mapPosition.z)));

            }
        }
        return objects;
    }

}


public struct MiniMapObjectData
{
    MiniMapObject mapObject;
    Vector2 miniMapPosition;

    public MiniMapObjectData(MiniMapObject mapObject, Vector2 miniMapPosition)
    {
        this.mapObject = mapObject;
        this.miniMapPosition = miniMapPosition;
    }

    public MiniMapObject MapObject
    {
        get { return mapObject; }
    }

    public Vector2 MiniMapPosition
    {
        get { return miniMapPosition; }
    }

}