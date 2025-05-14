using UnityEngine;
using DamageNumbersPro;
using System.Collections;

public class GameLogUI : MonoBehaviour
{
    [SerializeField] DamageNumberGUI textLog;

    void Start()
    {
        StartCoroutine(LogLoop());

    }

    IEnumerator LogLoop()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(1f);
            if (textLog != null)
            {
                // You can customize this with different positions, values, and styles
                textLog.Spawn(Vector3.zero, "Test Log Message", this.transform);
            }

        }
    }
}
