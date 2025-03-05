using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    
    [SerializeField] string[] mapsToSelectFrom;

    [SerializeField] GameObject mapButtonPrefab;
    [SerializeField] Transform mapButtonParent;
    [SerializeField] float buttonSpacing = 10;

    private void Start()
    {
        for (int i = 0; i < mapsToSelectFrom.Length; i++)
        {
            var map = mapsToSelectFrom[i];
            var button = Instantiate(mapButtonPrefab, mapButtonParent).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = map;
            button.onClick.AddListener(() => SelectMap(map));
            button.transform.localPosition += new Vector3(0, buttonSpacing * i, 0);
        }
        
    }

    private void SelectMap(string map)
    {
        var mapLoader = MapLoader.instance;
        if (mapLoader != null)
        {
            mapLoader.SetSceneToLoad(map);
        }
    }


}
