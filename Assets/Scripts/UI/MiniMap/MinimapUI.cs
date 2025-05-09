using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] float maxDistance = 15f;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] Transform playerTransform;
    [SerializeField] float iconPositionMultiplier = 1f;
    [SerializeField] Transform map;
    

    List<MiniMapUIObject> objectsInUI = new List<MiniMapUIObject>();


    [Header("Objective icon")]
    [SerializeField] MiniMapUIObjective objectiveIconPrefab = null;
    List<MiniMapUIObjective> objectivesInUI = new List<MiniMapUIObjective>();


    [SerializeField] Color objectiveBaseColor;

    [SerializeField]
    Color[] objectiveTeamColors;

    private void Start()
    {
        if (MiniMapManager.instance.HasObjective)
        {
            for (int i = 0; i < MiniMapManager.instance.GetObjectiveCount(); i++)
            {
                CreateObjective( i);
            }
        }
    }

    public void CreateObjective(int index)
    {
        var obj = Instantiate(objectiveIconPrefab.gameObject, map).GetComponent<MiniMapUIObjective>();
        
        objectivesInUI.Add(obj);
        var obiManager = MiniMapManager.instance.GetObjective(index);
        obiManager.OnObjectiveTeamIndexChanged += (index) => SetObjectiveIndexColor(obj, index);
        obiManager.OnObjectiveTextChanged += obj.ChangeText;
        SetObjectiveIndexColor(obj, obiManager.ObjectiveTeamIndex);
        obj.ChangeText(obiManager.ObjectiveText);


    }

    public void SetObjectiveIndexColor(MiniMapUIObjective objective, int teamIndex)
    {
        var color = objectiveBaseColor;
        if (teamIndex >= 0 && teamIndex < objectiveTeamColors.Length)
            color = objectiveTeamColors[teamIndex];
        objective.ChangeColor(color);
    }


    public void EnableMiniMap()
    {
        gameObject.SetActive(true);
    }

    public void DisableMiniMap()
    {
        gameObject.SetActive(false);
    }

    public void Update()
    {

        foreach (var objectInUI in objectsInUI)
        {
            objectInUI.SetUpdatedThisFrame(false);
        }

        if (playerTransform == null)
        {
            return;
        }

        var allObjectsInMinimap = MiniMapManager.instance.GetMiniMapObjectsInDistance(playerTransform.position, maxDistance);

        foreach (MiniMapObjectData objectData in allObjectsInMinimap)
        {
            var uiObj = CheckIfObjectIsAlreadyInList(objectData.MapObject);

            if (uiObj == null)
            {
                GameObject icon = Instantiate(objectData.MapObject.GetIcon(playerTeam.TeamIndex), map);
                


                var objUI = new MiniMapUIObject(objectData.MapObject, icon);
                objectsInUI.Add(objUI);


                Vector3 uiPosition = objectData.MiniMapPosition;
                float distance = objectData.MiniMapPosition.magnitude;
                Vector2 uiDirection = GetObjectDirection(objectData.MapObject.transform.position, playerTransform) ;

                objUI.UpdatePosition(uiDirection* distance * iconPositionMultiplier);


            }
            else
            {
                Vector3 uiPosition = objectData.MiniMapPosition * iconPositionMultiplier;
                float distance = objectData.MiniMapPosition.magnitude;
                Vector2 uiDirection = GetObjectDirection(objectData.MapObject.transform.position, playerTransform) ;

                uiObj.UpdatePosition(uiDirection* distance * iconPositionMultiplier);

            }

        }


        for (int i = 0; i < objectivesInUI.Count; i++)
        {
            var obj = objectivesInUI[i];
            Vector2 pos = MiniMapManager.instance.GetMiniMapObjectivePosition(i, playerTransform.position, maxDistance);
            Vector3 position = ObjectiveIndicator.instance.GetObjective(i).Position;
            float distance = pos.magnitude;
            Vector2 uiDirection = GetObjectDirection(position, playerTransform);

            obj.RectTransform.localPosition = (uiDirection * distance * iconPositionMultiplier);
        }



        // destroy objects that are not updated this frame
        for (int i = objectsInUI.Count - 1; i >= 0; i--)
        {
            if (!objectsInUI[i].UpdatedThisFrame)
            {
                Destroy(objectsInUI[i].Icon);
                objectsInUI.RemoveAt(i);
            }
        }


    }

    public MiniMapUIObject CheckIfObjectIsAlreadyInList(MiniMapObject mapObject)
    {
        foreach (MiniMapUIObject obj in objectsInUI)
        {
            if (obj.MapObject == mapObject)
            {
                return obj;
            }
        }
        return null;
    }


    public Vector2 GetObjectDirection(Vector3 objectPosition, Transform playerTransform)
    {
        // Get direction from enemy to player
        Vector3 directionToObject = (objectPosition - playerTransform.position).normalized;

        // Project onto XZ plane
        Vector3 flatDirection = new Vector3(directionToObject.x, 0, directionToObject.z).normalized;

        // Convert world direction to local space relative to player's forward direction
        float forwardDot = Vector3.Dot(flatDirection, playerTransform.forward); // Front/Back
        float rightDot = Vector3.Dot(flatDirection, playerTransform.right);     // Right/Left

        // Convert to UI coordinates
        Vector2 uiDirection = new Vector2(rightDot, forwardDot).normalized;

        return uiDirection;
    }


}

public class MiniMapUIObject
{
    MiniMapObject mapObject;
    RectTransform icon;
    Vector2 miniMapPosition;

    bool updatedThisFrame = false;

    public bool UpdatedThisFrame => updatedThisFrame;

    public void SetUpdatedThisFrame(bool updated)
    {
        updatedThisFrame = updated;
    }

    public MiniMapUIObject(MiniMapObject mapObject, GameObject icon)
    {
        this.mapObject = mapObject;
        this.icon = icon.GetComponent<RectTransform>();
    }

    public MiniMapObject MapObject => mapObject;
    public GameObject Icon => icon.gameObject;

    // update icon position
    public void UpdatePosition(Vector2 position)
    {
        miniMapPosition = position;
        updatedThisFrame = true;

        icon.localPosition = new Vector3(position.x, position.y, 0);
    }

    public Vector2 MiniMapPosition => miniMapPosition;


}