using UnityEngine;

public class RandomOffset : MonoBehaviour
{
    public float maxOffset = 0.1f;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        transform.localPosition += new Vector3(
            Random.Range(-maxOffset, maxOffset),
            Random.Range(-maxOffset, maxOffset),
            Random.Range(-maxOffset, maxOffset)
        );

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
