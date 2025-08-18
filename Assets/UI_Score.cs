using DamageNumbersPro;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UI_Score : MonoBehaviour
{
    [SerializeField] Transform scoreSpawn;
    [SerializeField] DamageNumberGUI textLog;
    [SerializeField] TextMeshProUGUI scoreText;

	[SerializeField] TextMeshProUGUI scoreText2;

    [SerializeField] bool DEBUG_autoSpawn = false;

	public void UpdateScore(int value)
    {
        scoreText2.text = "Score";

		scoreText.text = value.ToString();
    }

	private void Awake()
	{
        scoreText.text = "";
        scoreText2.text = "";


		if (DEBUG_autoSpawn)
        {
            StartCoroutine(AutoSpawnLoop());
        }

	}

    IEnumerator AutoSpawnLoop()
    {
        while (DEBUG_autoSpawn)
        {
            yield return new WaitForSeconds(1f);
            SpawnScoreGain(Random.Range(1, 100));
        }
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
