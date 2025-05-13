using UnityEngine;

public class NameList : MonoBehaviour
{
    [SerializeField] string[] names_basic;
    [SerializeField] string[] names_advanced;

    // singelton
    public static NameList instance;

    // awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetRandomNameBasic()
    {
        int randomIndex = Random.Range(0, names_basic.Length);
        return names_basic[randomIndex];
    }

    public string GetRandomNameAdvanced()
    {
        int randomIndex = Random.Range(0, names_advanced.Length);
        return names_advanced[randomIndex];
    }
}
