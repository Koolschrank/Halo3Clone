using UnityEngine;

public class AI_Disable : MonoBehaviour
{
    [SerializeField] CharacterHealth characterHealth;
    [SerializeField] PlayerAim playerAim;



    void Start()
    {
        characterHealth.OnDeath += DisableAi;
    }

    public void DisableAi()
    {
        gameObject.SetActive(false);

        playerAim.UpdateAimInput(Vector2.zero);
    }
}
