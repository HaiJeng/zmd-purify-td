using GameCore.Grid;

namespace Grid.GridFactory;

public static class GridFactory
{
    public static GridCell[,] CreateDefaultGrid(int width, int height)
    {
        var grid = new GridCell[width, height];
        int centerX = width / 2;
        int centerY = height / 2;
        int safeRadius = 2; // 4x4安全区

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float pollution = 0;
                int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);
                
                if (distance > safeRadius)
                {
                    // 按距离设置污染值
                    if (distance <= 2) pollution = Random.Shared.Next(40, 61);
                    else if (distance <= 4) pollution = Random.Shared.Next(60, 81);
                    else pollution = Random.Shared.Next(80, 101);
                }
                
                grid[x, y] = new GridCell(x, y, pollution);
            }
        }
        return grid;
    }
}