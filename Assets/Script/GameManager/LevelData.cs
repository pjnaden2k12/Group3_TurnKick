using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public PivotData[] pivots;
    public BellData[] bells;

    public ClockwiseData clockwiseShort;
    public ClockwiseData clockwiseLong;

    public ClockwiseData winTargetShort;
    public ClockwiseData winTargetLong;
    
}
[System.Serializable]
public class BellData
{
    public float x, y;
}

[System.Serializable]
public class PivotData
{
    public float x, y;
    public bool isPivotX;
}

[System.Serializable]
public class ClockwiseData
{
    public int pivotIndex;  // Chỉ số của pivot mà bar/quay liên kết
    public float rotation;   // Góc quay của bar/quay
}
