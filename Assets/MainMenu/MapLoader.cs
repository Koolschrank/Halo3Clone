
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] bool isSwat;

    [SerializeField] bool dualWieldPlus;

    [SerializeField] bool noMinimap;

    [SerializeField] bool randomWeaponSpawn;

    [SerializeField] float damageMultiplier = 1;
    [SerializeField] float moveSpeedMultiplier = 1;


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

    public bool IsSwat()
    {
        return isSwat;
    }

    public void SetIsSwat(bool isSwat)
    {
        this.isSwat = isSwat;
    }

    public bool IsDualWieldPlus()
    {
        return dualWieldPlus;
    }

    public void SetDualWieldPlus(bool dualWieldPlus)
    {
        this.dualWieldPlus = dualWieldPlus;
    }

    public bool HasNoMiniMap()
    {
        return noMinimap;
    }

    public void SetNoMinimap(bool noMinimap)
    {
        this.noMinimap = noMinimap;
    }

    public bool IsRandomWeaponSpawn()
    {
        return randomWeaponSpawn;
    }

    public void SetRandomWeaponSpawn(bool randomWeaponSpawn)
    {
        this.randomWeaponSpawn = randomWeaponSpawn;
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public void SetDamageMultiplier(float damageMultiplier)
    {
        this.damageMultiplier = damageMultiplier;
    }

    public float GetMoveSpeedMultiplier()
    {
        return moveSpeedMultiplier;
    }

    public void SetMoveSpeedMultiplier(float moveSpeedMultiplier)
    {
        this.moveSpeedMultiplier = moveSpeedMultiplier;
    }



}
