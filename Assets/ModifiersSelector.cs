using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifiersSelector : MonoBehaviour
{
    [SerializeField] Toggle swatToggle;
    [SerializeField] Toggle dualWieldPlusToggle;
    [SerializeField] Toggle minimapToggle;
    [SerializeField] Toggle randomWeaponSpawnToggle;
    [SerializeField] Toggle aiEnemiesTeam1;
    [SerializeField] Toggle aiEnemiesTeam2;

    [SerializeField] Slider moveSpeedSlider;
    [SerializeField] TextMeshProUGUI moveSpeedValue;
    [SerializeField] Slider damageMultiplierSlider;
    [SerializeField] TextMeshProUGUI damageMultiplierValue;
    [SerializeField] Slider aiAmountSlider;
    [SerializeField] TextMeshProUGUI aiAmountText;



    [SerializeField] Enemy_Wave[] enemyWaves;


    private void Start()
    {
        bool isSwat = MapLoader.instance.IsSwat();
        bool dualWieldPlus = MapLoader.instance.IsDualWieldPlus();
        bool minimap = MapLoader.instance.HasNoMiniMap();
        bool randomWeaponSpawn = MapLoader.instance.IsRandomWeaponSpawn();
        bool aiEnemies1Value = MapLoader.instance.IsAIEnemiesTeam1();
        bool aiEnemies2Value = MapLoader.instance.IsAIEnemiesTeam2();
        float moveSpeedMultiplier = MapLoader.instance.GetMoveSpeedMultiplier();
        float damageMultiplier = MapLoader.instance.GetDamageMultiplier();
        float aiAmount = MapLoader.instance.AIAmountMultiplier;

        int moveSpeedValueInt = Mathf.RoundToInt(moveSpeedMultiplier * 10);
        int damageMultiplierValueInt = Mathf.RoundToInt(damageMultiplier * 10);

        int aiAmountValueInt = Mathf.RoundToInt(aiAmount * 10);


        swatToggle.isOn = isSwat;
        dualWieldPlusToggle.isOn = dualWieldPlus;
        minimapToggle.isOn = minimap;
        randomWeaponSpawnToggle.isOn = randomWeaponSpawn;
        moveSpeedSlider.value = moveSpeedValueInt;
        damageMultiplierSlider.value = damageMultiplierValueInt;
        aiAmountSlider.value = aiAmountValueInt;
        aiEnemiesTeam1.isOn = aiEnemies1Value;
        aiEnemiesTeam2.isOn = aiEnemies2Value;


        moveSpeedValue.text = (moveSpeedMultiplier).ToString("F1");
        damageMultiplierValue.text = (damageMultiplier).ToString("F1");
        aiAmountText.text = (aiAmount).ToString("F1");




        swatToggle.onValueChanged.AddListener(SetSwat);
        dualWieldPlusToggle.onValueChanged.AddListener(SetDualWieldPlus);
        minimapToggle.onValueChanged.AddListener(SetNoMiniMap);
        randomWeaponSpawnToggle.onValueChanged.AddListener(SetRandomWeaponSpawn);
        aiEnemiesTeam1.onValueChanged.AddListener(MapLoader.instance.SetAIEnemiesTeam1);
        aiEnemiesTeam2.onValueChanged.AddListener(MapLoader.instance.SetAIEnemiesTeam2);

        moveSpeedSlider.onValueChanged.AddListener(SetMoveSpeedMultiplier);
        damageMultiplierSlider.onValueChanged.AddListener(SetDamageMultiplier);
        aiAmountSlider.onValueChanged.AddListener(SetAIAmount);


    }

    // on disable remove listeners
    private void OnDisable()
    {
        swatToggle.onValueChanged.RemoveListener(SetSwat);
        dualWieldPlusToggle.onValueChanged.RemoveListener(SetDualWieldPlus);
        minimapToggle.onValueChanged.RemoveListener(SetNoMiniMap);
        randomWeaponSpawnToggle.onValueChanged.RemoveListener(SetRandomWeaponSpawn);
        aiEnemiesTeam1.onValueChanged.RemoveListener(MapLoader.instance.SetAIEnemiesTeam1);
        aiEnemiesTeam2.onValueChanged.RemoveListener(MapLoader.instance.SetAIEnemiesTeam2);

        moveSpeedSlider.onValueChanged.RemoveListener(SetMoveSpeedMultiplier);

        damageMultiplierSlider.onValueChanged.RemoveListener(SetDamageMultiplier);
    }

    private void SetSwat(bool value)
    {
        MapLoader.instance.SetIsSwat(value);
    }

    private void SetDualWieldPlus(bool value)
    {
        MapLoader.instance.SetDualWieldPlus(value);
    }

    private void SetNoMiniMap(bool value)
    {
        MapLoader.instance.SetNoMinimap(value);
    }

    private void SetRandomWeaponSpawn(bool value)
    {
        MapLoader.instance.SetRandomWeaponSpawn(value);
    }

    private void SetAIEnemies(bool value)
    {
        MapLoader.instance.SetAIEnemiesTeam1(value);
    }

    private void SetMoveSpeedMultiplier(float value)
    {
        MapLoader.instance.SetMoveSpeedMultiplier(value / 10);
        moveSpeedValue.text = (value / 10).ToString("F1");
    }

    private void SetDamageMultiplier(float value)
    {
        MapLoader.instance.SetDamageMultiplier(value / 10);
        damageMultiplierValue.text = (value / 10).ToString("F1");
    }

    private void SetAIAmount(float value)
    {
        MapLoader.instance.SetAIAmount(value / 10);
        aiAmountText.text = (value / 10).ToString("F1");

    }

    public void SetEnemies(int index)
    {
        MapLoader.instance.SetEnemies(enemyWaves[index]);
    }
}
