using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public Action<Vector3> OnObjectiveMoved;
    public Action<int> OnObjectiveTeamIndexChanged;

    // instance
    public static MiniMapManager instance;

    List<MiniMapObject> miniMapObjects = new List<MiniMapObject>();
   

    Vector3 objectivePosition = Vector3.zero;
    int objectiveIndex = 0;

    void Awake()
    {
        instance = this;
        

    }

    private void Start()
    {
        ObjectiveIndicator.instance.OnPositionChange += SetObjectivePosition;
        SetObjectivePosition(ObjectiveIndicator.instance.Position);
        ObjectiveIndicator.instance.OnTeamIndexChange += SetObjectiveIndex;
        SetObjectiveIndex(ObjectiveIndicator.instance.TeamIndex);
    }


    public bool HasObjective { get { return ObjectiveIndicator.instance.IsActive; } }

    public void SetObjectivePosition(Vector3 position)
    {
        objectivePosition = position;
        OnObjectiveMoved?.Invoke(objectivePosition);

    }

    public void SetObjectiveIndex(int index)
    {
        objectiveIndex = index;
        OnObjectiveTeamIndexChanged?.Invoke(objectiveIndex);
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


    // todo: this intire thing needs a rework

    public Vector2 GetMiniMapObjectivePosition(Vector3 position, float maxDistance)
    {
        var positionWithoutY = new Vector3(position.x, 0, position.z);


        var objPosWithoutY = new Vector3(objectivePosition.x, 0, objectivePosition.z);
        var distance = Vector3.Distance(positionWithoutY, objPosWithoutY);
        Vector3 direction = objPosWithoutY - positionWithoutY;
        // rotate direction with player forward
        //direction = Quaternion.Euler(0, -forward.y, 0) * direction;

        Debug.Log(distance);
        Debug.Log(direction);

        Vector3 mapPosition = direction.normalized * Mathf.Min(1.1f ,(distance / maxDistance));


        return new Vector2(mapPosition.x, mapPosition.z);



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