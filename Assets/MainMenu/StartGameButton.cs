using UnityEngine;

public class StartGameButton : MonoBehaviour
{

    public void StartGame()
    {
        var mapLoader = MapLoader.instance;
        if (mapLoader != null)
        {
            mapLoader.LoadMap();
        }
    }
}
