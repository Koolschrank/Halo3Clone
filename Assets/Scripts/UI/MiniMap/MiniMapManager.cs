using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    
    public Action<Vector3> OnObjectiveMoved;
    public Action<int> OnObjectiveTeamIndexChanged;
    public Action<int> OnObjectiveTimerChanged;

    // instance
    public static MiniMapManager instance;

    List<MiniMapObject> miniMapObjects = new List<MiniMapObject>();
    List<MiniMapObjectiv> miniMapObjectivDatas = new List<MiniMapObjectiv>();

    void Awake()
    {
        instance = this;
        ObjectiveIndicator.instance.OnObjectiveAdded += (objective) =>
        {
            var miniMapObjective = new MiniMapObjectiv(objective.Position, objective.TeamIndex, objective.Number);
            miniMapObjectivDatas.Add(miniMapObjective);
            objective.OnPositionChange += (position) => { miniMapObjective.ObjectivePosition = position; };
            objective.OnTeamIndexChange += (index) => { miniMapObjective.ObjectiveTeamIndex = index; };
            miniMapObjective.ObjectiveTeamIndex = objective.TeamIndex;
            objective.OnNumberChanged += (number) =>
            {
                miniMapObjective.ObjectiveNumber = number;
            };
            miniMapObjective.ObjectiveNumber = objective.Number;
        };
    }

    public MiniMapObjectiv GetObjective(int index)
    {

        return miniMapObjectivDatas[index];
    }

    public int GetObjectiveCount()
    {
        return miniMapObjectivDatas.Count;
    }




    public bool HasObjective { get { return ObjectiveIndicator.instance.GetObjective(0).IsActive; } }

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

    public Vector2 GetMiniMapObjectivePosition(int objectiveIndex,Vector3 position, float maxDistance)
    {
        var objectivePosition = miniMapObjectivDatas[objectiveIndex].ObjectivePosition;
        var positionWithoutY = new Vector3(position.x, 0, position.z);


        var objPosWithoutY = new Vector3(objectivePosition.x, 0, objectivePosition.z);
        var distance = Vector3.Distance(positionWithoutY, objPosWithoutY);
        Vector3 direction = objPosWithoutY - positionWithoutY;



        Vector3 mapPosition = direction.normalized * Mathf.Min(1.1f ,(distance / maxDistance));


        return new Vector2(mapPosition.x, mapPosition.z);
    }
}

public class MiniMapObjectiv
{
    public Action<Vector3> OnObjectiveMoved;
    public Action<int> OnObjectiveTeamIndexChanged;
    public Action<int> OnObjectiveNumberChanged;


    Vector3 objectivePosition;
    int objectiveTeamIndex;
    int objectiveNumber;

    public MiniMapObjectiv()
    {
        objectivePosition = Vector3.zero;
        objectiveTeamIndex = 0;
        objectiveNumber = 0;
    }
    public MiniMapObjectiv(Vector3 objectivePosition, int objectiveIndex, int objectiveNumber)
    {
        this.objectivePosition = objectivePosition;
        OnObjectiveMoved?.Invoke(objectivePosition);
        this.objectiveTeamIndex = objectiveIndex;
        OnObjectiveTeamIndexChanged?.Invoke(objectiveTeamIndex);
        this.objectiveNumber = objectiveNumber;
        OnObjectiveNumberChanged?.Invoke(objectiveNumber);
    }

    public Vector3 ObjectivePosition
    {
        get { return objectivePosition; }
        set
        {
            objectivePosition = value;
            OnObjectiveMoved?.Invoke(objectivePosition);
        }
    }

    public int ObjectiveTeamIndex
    {
        get { return objectiveTeamIndex; }
        set
        {
            objectiveTeamIndex = value;
            OnObjectiveTeamIndexChanged?.Invoke(objectiveTeamIndex);
        }
    }

    public int ObjectiveNumber
    {
        get { return objectiveNumber; }
        set
        {
            objectiveNumber = value;
            OnObjectiveNumberChanged?.Invoke(objectiveNumber);
        }
    }
}
public class MiniMapObjectData
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