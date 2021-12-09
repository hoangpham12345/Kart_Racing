using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameOptions", order = 1)]
public class GameOptions : ScriptableObject
{
    public int dataSample = 0123;
    public int map = 0;
    public int kart = 0;
}
