using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class DataLoader : Node
{
    public static DataLoader Instance { get; private set; }

    public List<string> _Countries = new() { "FRANCE", "ENGLAND", "ARGENTINA", "BRAZIL", "GERMANY", "ITALY", "SPAIN", "USA" };

    private List<CountryData> _countriesDataList = new();

    public override void _Ready()
    {
        Instance = this;
        var jsonFile = FileAccess.Open("res://Assets/json/squads.json", FileAccess.ModeFlags.Read);
        if (jsonFile == null)
        {
            GD.Print("找不到文件！");
        }
        var jsonText = jsonFile.GetAsText();
        // 反序列化为对象列表
        _countriesDataList = JsonConvert.DeserializeObject<List<CountryData>>(jsonText);
        // 遍历输出每个国家的球员
        // foreach (var country in countries)
        // {
        //     GD.Print($"Country: {country.Country}");
        //     foreach (var player in country.Players)
        //     {
        //         GD.Print($"  Name: {player.FullName}, Role: {player.Role}, Speed: {player.Speed}, Power: {player.Power}");
        //     }
        // }
        jsonFile.Close();
    }

    public List<PlayerResource> GetSquad(string country)
    {
        foreach (var item in _countriesDataList)
        {
            if (item.Country == country && item.Players.Count == 6)
            {
                return item.Players;
            }
        }
        return null;
    }
    

    
}
