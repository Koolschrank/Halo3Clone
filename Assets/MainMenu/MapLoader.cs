
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
    [SerializeField] float aiAmountMultiplier = 1;

    [SerializeField] bool aiEnemiesTeam1;
    [SerializeField] bool aiEnemiesTeam2;

    [SerializeField] Enemy_Wave aiEnemyWave;
    public bool brStart = false;

    public void SetBRStart(bool value)
    {
        brStart = value;
	}

	public bool HasAIEnemies()
    {
        return aiEnemiesTeam1 || aiEnemiesTeam2;
    }

    public bool IsAIEnemiesTeam1()
    {
        return aiEnemiesTeam1;
    }

    public bool IsAIEnemiesTeam2()
    {
        return aiEnemiesTeam2;
    }

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

    public void SetAIEnemiesTeam1(bool aiEnemies)
    {
        this.aiEnemiesTeam1 = aiEnemies;
    }

    public void SetAIEnemiesTeam2(bool aiEnemies)
    {
        this.aiEnemiesTeam2 = aiEnemies;
    }

    public void SetAIAmount(float value)
    {
        aiAmountMultiplier = value;
    }

    public float AIAmountMultiplier { get { return aiAmountMultiplier; } }

    public Enemy_Wave Enemies => aiEnemyWave;

    public void SetEnemies(Enemy_Wave enemy)
    {
        aiEnemyWave = enemy;
    }


}
