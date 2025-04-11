
using UnityEngine;
using UnityEngine.SceneManagement;
using MultiplayerGameModes;

public class MapLoader : MonoBehaviour
{
    // make instance of the class
    public static MapLoader instance;

    // make dont destroy on scene load
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] GameMode gameMode;
    // scene to load
    [SerializeField] string sceneToLoad;


    public void LoadMap()
    {
        // load the scene
        SceneManager.LoadScene(sceneToLoad);
    }

    public GameMode GetGameMode()
    {
        return gameMode;
    }

    // set the game mode
    public void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    // set the scene to load
    public void SetSceneToLoad(string sceneToLoad)
    {
        this.sceneToLoad = sceneToLoad;
    }



}
