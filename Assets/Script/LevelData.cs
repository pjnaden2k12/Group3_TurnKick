using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public PivotData[] pivots;      // Pivot thường
    public PivotData[] pivotXs;     // ✅ Pivot đặc biệt đổi trạng thái theo lượt

    public BarData bar;             // Thanh người chơi xoay
    public BarData targetBar;       // Thanh mẫu (đích)
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