namespace GameCore.Grid;

public class GridCell(int x, int y, float pollution)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public float Pollution { get; set; } = pollution;
}