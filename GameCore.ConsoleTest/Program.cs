using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using GameCore.Core;
using GameCore.Config;
using GameCore.Grid;
using GameCore.Tower;

class Program
{
    static void Main(string[] args)
    {
        // 配置Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("Logs/game_log_.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Console.WriteLine("=== 2D 净化塔防控制台测试 ===\n");
            Log.Information("=== 2D 净化塔防控制台测试 ===");

            // 加载配置
            var configLoader = new ConfigLoader();
            var towerConfigs = ConfigLoader.LoadTowers("../GameCore/Resources/static/towers.json");
            Log.Information("加载塔配置: {Count} 种塔类型", towerConfigs.Count);

            // 初始化游戏
            var game = new GameLoop(12, 12, 10f);  // 12x12地图，初始资源10
            Log.Information("游戏初始化完成 - 地图尺寸: 12x12, 初始资源: 20");

            // 初始状态
            Console.WriteLine($"初始资源: {game.CurrentResources}");
            Console.WriteLine($"地图尺寸: 12×12");
            Console.WriteLine($"安全区: 中心 4×4\n");
            Log.Information("初始状态 - 资源: {Resources}, 净化进度: {Progress:P1}", game.CurrentResources, game.PurificationProgress);

            // 模拟游戏流程
            Console.WriteLine("开始模拟游戏流程...\n");
            
            int tickCount = 0;
            int mapDisplayInterval = 30;  // 每30个tick显示一次地图
            int autoBuildInterval = 15;   // 每15个tick尝试自动建造
            float fixedDeltaTime = 0.1f;  // 固定时间步长：0.1秒
            
            // 记录开始时间
            var startTime = DateTime.Now;
            var random = new Random();

            while (true)
            {
                game.Tick(fixedDeltaTime);
                tickCount++;

                if (game.IsBacklashActive)
                {
                    Log.Information("第 {Time:F1} 秒 - 反扑触发", game.CurrentTime);
                    Console.WriteLine($"第 {game.CurrentTime:F1} 秒 - 反扑触发");
                }

                // 每x tick打印地图
                if (tickCount % mapDisplayInterval == 0)
                {
                    PrintGameMap(game);
                    Log.Information("第 {Time:F1} 秒 - 资源: {Resources:F1}, 净化度: {Progress:P1}", 
                        game.CurrentTime, game.CurrentResources, game.PurificationProgress);
                    Console.WriteLine($"第 {game.CurrentTime:F1} 秒 - 资源: {game.CurrentResources:F1}, 净化度: {game.PurificationProgress:P1}");
                }

                // 自动建造塔
                if (tickCount % autoBuildInterval == 0)
                {
                    AutoBuildTowers(game, random, towerConfigs);
                }

                if (game.IsGameOver)
                {
                    break;
                }
            }

            // 计算实际用时
            var endTime = DateTime.Now;
            var actualDuration = (endTime - startTime).TotalSeconds;

            Console.WriteLine("\n测试完成！");
            Log.Information("游戏结束 - 状态: {Result}, 游戏时间: {GameTime:F1}秒, 实际用时: {ActualTime:F1}秒", 
                game.GameResult, game.CurrentTime, actualDuration);
            Console.WriteLine($"🎉 胜利！ 实际用时 {actualDuration:F0} 秒");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"程序执行出错: {ex.Message}");
            Log.Fatal(ex, "程序异常");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static void PrintGameMap(GameLoop game)
    {
        string mapInfo = $"=== 游戏地图状态 ===\n时间: {game.CurrentTime}秒 | 资源: {game.CurrentResources:F1} | 进度: {game.PurificationProgress * 100:F1}%\n游戏状态: {(game.IsGameOver ? "结束" : "进行中")}";
        if (game.IsGameOver)
        {
            mapInfo += $"\n结果: {game.GameResult}";
        }
        Console.WriteLine(mapInfo);
        Log.Information(mapInfo); // 添加日志记录

        // 获取网格和塔数据
        var grid = game.GetGrid();
        var towers = game.GetTowers();

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // 创建塔位置映射
        var towerPositions = new Dictionary<(int, int), string>();
        foreach (var tower in towers)
        {
            if (tower.config.Id == 1)
            {
                towerPositions[(tower.gridX, tower.gridY)] = "#";
            }
            else
            {

                towerPositions[(tower.gridX, tower.gridY)] = "@";
            }
        }

        // 打印地图
        string mapString = "";
        string header = "   ";
        for (int x = 0; x < width; x++)
        {
            header += $"{x:D2} "; // 列标题
        }
        mapString += header + "\n";

        for (int y = 0; y < height; y++)
        {
            string row = $"{y:D2} "; // 行标题
            for (int x = 0; x < width; x++)
            {
                if (towerPositions.ContainsKey((x, y)))
                {
                    row += " " + towerPositions[(x, y)] + " "; // 塔的位置
                }
                else
                {
                    int pollution = (int)grid[x, y].Pollution;
                    if (pollution == 0)
                    {
                        row += " . "; // 无污染格子
                    }
                    else
                    {
                        // 确保污染值在00-99范围内显示
                        pollution = Math.Clamp(pollution, 0, 99);
                        row += $"{pollution:D2} ";
                    }
                }
            }
            mapString += row + "\n";
        }

        Console.Write(mapString);
        Log.Information("地图状态:\n{MapString}==================\n", mapString); // 记录地图到日志
    }

    static void AutoBuildTowers(GameLoop game, Random random, List<TowerConfig> towerConfigs)
    {
        // 随机选择一个塔配置（优先高级塔）
        TowerConfig selectedConfig = towerConfigs[0]; // 默认选择基础塔

        // 如果有多个配置，有一定概率选择高级塔
        if (towerConfigs.Count > 1 && random.NextDouble() > 0.7) // 30%概率选择高级塔
        {
            selectedConfig = towerConfigs[1];
        }

        // 随机选择一个可建造的位置（污染=0的位置）
        int mapSize = 12;
        int attempts = 0;
        const int maxAttempts = 50; // 最大尝试次数

        // 预先收集所有可建造位置
        List<(int x, int y)> validPositions = [];
        var grid = game.GetGrid();
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (grid[x, y].Pollution == 0)
                {
                    validPositions.Add((x, y));
                }
            }
        }

        while (attempts < maxAttempts && validPositions.Count > 0)
        {
            int index = random.Next(0, validPositions.Count);
            var (x, y) = validPositions[index];

            // 直接尝试建造，GameLoop会处理位置验证
            if (game.PlaceTower(x, y, selectedConfig, out string errorMessage))
            {
                Log.Information("在 ({X},{Y}) 建造{TowerName}", x, y, selectedConfig.Name);
                Console.WriteLine($"🤖 自动建造 {selectedConfig.Name} at ({x},{y})");
                return;
            }
            else
            {
                Log.Warning("在 ({X},{Y}) 建造{TowerName}失败: {ErrorMessage}", x, y, selectedConfig.Name, errorMessage);
                Console.WriteLine($"🤖 自动建造 {selectedConfig.Name} at ({x},{y}) 失败: {errorMessage}");
                // 移除失败的位置，避免重复尝试
                validPositions.RemoveAt(index);
            }

            attempts++;
        }

        Log.Warning("自动建造尝试失败 - 无合适位置");
        Console.WriteLine("🤖 自动建造尝试失败 - 无合适位置");
    }
}