using UnityEngine;

public class SinWaves : MonoBehaviour
{
    [SerializeField] float maxYOffset = 1f; // Maximum vertical offset
    [SerializeField] float frequency = 1f; // Frequency of the sine wave

    // Update is called once per frame
    void Update()
    {
        // Calculate the new vertical position using a sine wave function
        float yOffset = Mathf.Sin(Time.time * frequency) * maxYOffset;

        // go up and down in a sine wave pattern

        transform.localPosition = new Vector3(transform.localPosition.x, yOffset, transform.localPosition.z);




    }
}
