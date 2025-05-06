using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsData", menuName = "LevelsData", order = 51)]
public class LevelsData : ScriptableObject
{
    public List<LevelData> levels;
}