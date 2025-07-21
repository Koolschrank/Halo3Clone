using UnityEngine;

public class RandomSpawnHopper : MonoBehaviour
{
    [SerializeField] GameObject[] spawnObjects;
    [SerializeField] Transform[] placmentPoints;

    [SerializeField] float framesToWait = 60f;

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % framesToWait == 0)
        {
            foreach (GameObject spawnObject in spawnObjects)
            {
                int randomIndex = Random.Range(0, placmentPoints.Length);
                Transform randomPoint = placmentPoints[randomIndex];
                spawnObject.transform.position = randomPoint.position;

            }
        }
    }
}
