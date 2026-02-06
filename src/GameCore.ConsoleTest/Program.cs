using System;
using GameCore.Core;
using GameCore.Config;
using GameCore.Grid;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== 2D 净化塔防控制台测试 ===\n");

        // 加载配置
        var configLoader = new ConfigLoader();
        var towerConfigs = configLoader.LoadTowers("../GameCore/Resources/static/towers.json");

        // 初始化游戏
        var game = new GameLoop(12, 12);
        
        // 初始状态
        Console.WriteLine($"初始资源: 20");
        Console.WriteLine($"地图尺寸: 12×12");
        Console.WriteLine($"安全区: 中心 4×4\n");

        // 模拟游戏流程
        Console.WriteLine("开始模拟游戏流程...\n");
        
        // 前10回合：观察资源增长
        for (int i = 0; i < 10; i++)
        {
            game.Tick(1.0f);
            if (i % 5 == 0)
            {
                Console.WriteLine($"第 {i} 回合 - 资源: 待实现");
            }
        }

        // 建造第一座塔
        Console.WriteLine("\n--- 建造第一座基础净化塔 ---");
        game.PlaceTower(5, 5, towerConfigs[0]);

        // 继续游戏到50%净化度
        Console.WriteLine("\n--- 继续净化到50%触发反扑 ---");
        while (true)
        {
            game.Tick(1.0f);
            // 这里需要添加进度显示逻辑
            break; // 暂时跳出避免无限循环
        }

        Console.WriteLine("\n测试完成！");
        Console.ReadKey();
    }
}