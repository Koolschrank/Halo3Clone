using DamageNumbersPro;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UI_Score : MonoBehaviour
{
    [SerializeField] Transform scoreSpawn;
    [SerializeField] DamageNumberGUI textLog;
    [SerializeField] TextMeshProUGUI scoreText;

    public void UpdateScore(int value)
    {
        scoreText.text = value.ToString();
    }

    public void SpawnScoreGain(int value)
    {
        if (gameObject.activeSelf)
        {
            // You can customize this with different positions, values, and styles
            var textObject = textLog.Spawn(Vector3.zero, value);
            textObject.gameObject.transform.SetParent(scoreSpawn.transform);
            textObject.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            // set rotation to zero
            textObject.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            // set scale to 1
            textObject.gameObject.transform.localScale = new Vector3(1, 1, 1);
            textObject.gameObject.layer = gameObject.layer;
        }
    }
}
