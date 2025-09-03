using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class ObjectiveIndicator : MonoBehaviour
{
    public Action<Objective> OnObjectiveAdded;

    public List<Objective> objectives = new List<Objective>();

    

    // singelton instance
    public static ObjectiveIndicator instance;

    
    public int ObjectiveCount { get { return objectives.Count; } }

    public List<Objective> GetAllObjectives()
    {
        return objectives;
    }

    public Objective GetObjective(int index)
    {
        if (index >= objectives.Count)
        {
            for (int i = objectives.Count; i <= index; i++)
            {
                Debug.LogWarning($"Objective with index {index} does not exist, creating a new one.");
				var obj = new Objective();
                objectives.Add(obj);
                OnObjectiveAdded?.Invoke(obj);
                obj.SetHideDistance(10000);
            }
        }
        return objectives[index];
    }

    public void Awake()
    {
        instance = this;
    }
}

[Serializable]
public class Objective
{

    public Action OnActivated;
    public Action OnDeactivated;
    public Action<Vector3> OnPositionChange;
    public Action<float> OnHideDistanceChange;
    public Action<int> OnTeamIndexChange;
    public Action<String> OnTextChanged;

    

    public Objective()
    {
        
    }


    bool isActive = false;
    float hideDistance = 0;
    int teamIndex = -1;
    String objectiveText = "";
    [SerializeField] Vector3 position = Vector3.zero;

    public void SetActive(bool active)
    {
        isActive = active;
        if (active)
        {
            OnActivated?.Invoke();
        }
        else
        {
            OnDeactivated?.Invoke();
        }
    }

    public void SetPosition(Vector3 position)
    {
        this.position = position;
        OnPositionChange?.Invoke(position);
    }

    public Vector3 Position { get { return position; } }

    public int TeamIndex { get { return teamIndex; } }

    public String Text { get { return objectiveText; } }

    public bool IsActive { get { return isActive; } }

    public void SetTeamIndex(int index)
    {
        teamIndex = index;
        OnTeamIndexChange?.Invoke(index);
    }

    public void SetText(string objectiveText)
    {
        this.objectiveText = objectiveText;
        OnTextChanged?.Invoke(objectiveText);
    }

    public void SetHideDistance(float distance)
    {
        hideDistance = distance;
        OnHideDistanceChange?.Invoke(distance);
    }

    public float HideDistance { get { return hideDistance; } }





}
