using UnityEngine;
using System;

[CreateAssetMenu(menuName = "GameModes/KingOfTheHill")]
public class GameMode_KingOfTheHill : GameMode
{
    
    

    [SerializeField] float timeToScore = 0;

    [SerializeField] bool moveHill = false;
    [SerializeField] float hillMoveTime = 0;


    public float TimeToScore { get { return timeToScore; } }
    public bool MoveHill { get { return moveHill; } }
    public float HillMoveTime { get { return hillMoveTime; } }







}
