using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class ObjectiveIndicator : MonoBehaviour
{
    public Action<Objective> OnObjectiveAdded;

    List<Objective> objectives = new List<Objective>();
    [SerializeField] Vector3 offset = new Vector3(0, 2, 0);

    

    // singelton instance
    public static ObjectiveIndicator instance;

    
    public int ObjectiveCount { get { return objectives.Count; } }

    public Objective GetObjective(int index)
    {
        if (index >= objectives.Count)
        {
            for (int i = objectives.Count; i <= index; i++)
            {
                var obj = new Objective();
                objectives.Add(obj);
                OnObjectiveAdded?.Invoke(obj);
            }
        }
        return objectives[index];
    }

    public void Awake()
    {
        instance = this;
    }
}

public class Objective
{

    public Action OnActivated;
    public Action OnDeactivated;
    public Action<Vector3> OnPositionChange;
    public Action<float> OnHideDistanceChange;
    public Action<int> OnTeamIndexChange;
    public Action<int> OnNumberChanged;

    public Objective()
    {
        
    }


    bool isActive = false;
    float hideDistance = 0;
    int teamIndex = -1;
    int number = 0;
    Vector3 position = Vector3.zero;

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

    public int Number { get { return number; } }

    public bool IsActive { get { return isActive; } }

    public void SetTeamIndex(int index)
    {
        teamIndex = index;
        OnTeamIndexChange?.Invoke(index);
    }

    public void SetNumber(int number)
    {
        this.number = number;
        OnNumberChanged?.Invoke(number);
    }

    public void SetHideDistance(float distance)
    {
        hideDistance = distance;
        OnHideDistanceChange?.Invoke(distance);
    }

    public float HideDistance { get { return hideDistance; } }





}
