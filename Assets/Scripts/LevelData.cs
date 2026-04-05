using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficultyMode {
    AdditionOnly,
    SubtractionOnly,
    Both
}

[CreateAssetMenu]
public class LevelData : ScriptableObject {
    public int startValue;
    public int targetValue;
    public int[] availableBlocks;
    public DifficultyMode mode = new DifficultyMode();
}
