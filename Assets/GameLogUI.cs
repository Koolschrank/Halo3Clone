using UnityEngine;
using DamageNumbersPro;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

public class GameLogUI : MonoBehaviour
{
    [SerializeField] DamageNumberGUI textLog;

    void Start()
    {
        //StartCoroutine(LogLoop());

    }

    public void Print(string value)
    {
        if (!gameObject.activeSelf) return;
        StartCoroutine(LogLoop(value));
    }

    IEnumerator LogLoop(string value)
    {


        yield return new WaitForSeconds(0.01f);

        if (gameObject.activeSelf)
        {
            // You can customize this with different positions, values, and styles
            var textObject = textLog.Spawn(Vector3.zero, value);
            textObject.gameObject.transform.SetParent(this.transform);
            textObject.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            // set rotation to zero
            textObject.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            // set scale to 1
            textObject.gameObject.transform.localScale = new Vector3(1, 1, 1);
            textObject.gameObject.layer = gameObject.layer;
        }

        
    }
}
