using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveIndicatorUI : MonoBehaviour
{
    [SerializeField] Camera playerCam;
    [SerializeField] Transform playerTransform;
    [SerializeField] RectTransform uiMarker;
    [SerializeField] Image uiImage;
    [SerializeField] float distanceFromMiddleOfScreen = 100f;
    [SerializeField] RectTransform uiCanvas;
    [SerializeField] Vector3 offset;
    [SerializeField] float lerpChangeTime = 1f;

    [SerializeField] float maxAngleForDirectView = 60f;

    [SerializeField] Color baseColor;

    [SerializeField]
    Color[] teamColors;

    Vector3 targetPosition;
    float hideDistance = 0;


    
    float lerpCurrent = 0f;

    private void Start()
    {

        
        ObjectiveIndicator.instance.OnPositionChange += UpdatePosition;
        ObjectiveIndicator.instance.OnActivated += Show;
        ObjectiveIndicator.instance.OnDeactivated += Hide;
        ObjectiveIndicator.instance.OnHideDistanceChange += SetHideDistance;
        ObjectiveIndicator.instance.OnTeamIndexChange += SetColor;

        SetHideDistance(ObjectiveIndicator.instance.GetHideDistance());

        if (ObjectiveIndicator.instance.IsActive)
        {
            Show();
        }
        else
        {
            Hide();
        }

    }

    public void SetHideDistance(float distance)
    {
        hideDistance = distance;
    }

    private void UpdatePosition(Vector3 position)
    {
        targetPosition = position;


        
    }

    public Vector2 GetIndicatorDirection(Vector3 targetPosition, Transform playerTransform)
    {
        // Get direction from enemy to player
        Vector3 directionToEnemy = (targetPosition - playerTransform.position).normalized;

        // Project onto XZ plane
        Vector3 flatDirection = new Vector3(directionToEnemy.x, 0, directionToEnemy.z).normalized;

        // Convert world direction to local space relative to player's forward direction
        float forwardDot = Vector3.Dot(flatDirection, playerTransform.forward); // Front/Back
        float rightDot = Vector3.Dot(flatDirection, playerTransform.right);     // Right/Left

        // Convert to UI coordinates
        Vector2 uiDirection = new Vector2(rightDot, forwardDot).normalized;

        return uiDirection;
    }

    private void Update()
    {

        if (hideDistance > 0)
        {
            float distance = Vector3.Distance(playerTransform.position, targetPosition);
            if (distance < hideDistance)
            {
                Hide();
                return;
            }
            else
            {
                Show();
            }
        }

        Vector3 screenPos = GetSceenPosition();
        Vector3 directionPosition = GetDirectionPosition();

        

        if (IsPositionOnScreen(targetPosition, playerCam))
        {
            lerpCurrent = Math.Min(lerpCurrent + Time.deltaTime / lerpChangeTime, 1);
        }
        else
        {
            lerpCurrent = Math.Max(lerpCurrent - Time.deltaTime / lerpChangeTime, 0);
        }

        uiMarker.localPosition = Vector3.Lerp(directionPosition, screenPos, lerpCurrent);











    }

    private void Show()
    {
        uiMarker.gameObject.SetActive(true);
    }

    private void Hide()
    {
        uiMarker.gameObject.SetActive(false);
    }

    public void SetColor(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex >= teamColors.Length)
        {
            uiImage.color = baseColor;
        }
        else
        {
            uiImage.color = teamColors[teamIndex];
        }
    }


    bool IsPositionOnScreen(Vector3 worldPos, Camera cam)
    {
        Vector3 viewportPoint = cam.WorldToViewportPoint(worldPos);

        // Check if position is within screen bounds
        bool isOnScreen = viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                          viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                          viewportPoint.z > 0; // Ensure it's in front of the camera

        return isOnScreen;
    }

    public Vector3 GetSceenPosition()
    {
        Vector3 screenPos = playerCam.WorldToScreenPoint(targetPosition + offset);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas, screenPos, playerCam, out localPoint);

        return localPoint;
    }

    public Vector3 GetDirectionPosition()
    {
        Vector3 direction = targetPosition - playerTransform.position;
        direction.y = 0;
        direction.Normalize();
        Vector2 indicatorDirection = GetIndicatorDirection(targetPosition, playerTransform);

        Vector3 indicatorPosition = new Vector3(indicatorDirection.x, indicatorDirection.y, 0) * distanceFromMiddleOfScreen;

        return indicatorPosition;
    }

}
