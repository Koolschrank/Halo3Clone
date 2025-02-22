using UnityEngine;

public class GameSpeedSetter : MonoBehaviour
{
    [SerializeField] float gameSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = gameSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
