using System.Text.Json;

public class TowerConfig {
    public int id { get; set; }
    public string name { get; set; }
    public float cost { get; set; }
    public float cleanPower { get; set; }
    public int range { get; set; }
    public float maxDurability { get; set; }
}

public class ConfigLoader {
    public List<TowerConfig> LoadTowers(string jsonPath) {
        string json = File.ReadAllText(jsonPath);
        var data = JsonSerializer.Deserialize<Dictionary<string, List<TowerConfig>>>(json);
        return data["towers"];
    }
}