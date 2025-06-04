using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public PivotData[] pivots;
    public BarData bar;
    public BarData targetBar;
}

[System.Serializable]
public class PivotData
{
    public float x, y;
}

[System.Serializable]
public class BarData
{
    public int pivotIndex;
    public float rotation;
}