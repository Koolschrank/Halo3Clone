using UnityEngine;
using System;

public class ObjectiveIndicator : MonoBehaviour
{
    public Action OnActivated;
    public Action OnDeactivated;

    public Action<Vector3> OnPositionChange;
    public Action<float> OnHideDistanceChange;
    public Action<int> OnTeamIndexChange;


    bool isActive = true;
    float hideDistance = 0;
    int teamIndex = -1;
    [SerializeField] Vector3 offset = new Vector3(0, 2, 0);

    

    // singelton instance
    public static ObjectiveIndicator instance;

    public void SetHideDistance(float distance)
    {
        hideDistance = distance;
        OnHideDistanceChange?.Invoke(distance);
    }

    public float GetHideDistance()
    {
        return hideDistance;
    }

    public void Awake()
    {
        instance = this;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position + offset;
        OnPositionChange?.Invoke(position);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);

        if (isActive)
        {
            OnActivated?.Invoke();
        }
        else
        {
            OnDeactivated?.Invoke();
        }
    }

    // get is active
    public bool IsActive()
    {
        return isActive;
    }

    public void SetTeamIndex(int index)
    {
        teamIndex = index;
        OnTeamIndexChange?.Invoke(index);
    }








}
