using System.Text.Json;
using ApprenticeManagement.Models;

namespace ApprenticeManagement.Data;

/// <summary>
/// Root object that is serialised to / deserialised from <c>data.json</c>.
/// </summary>
public class AppData
{
    public List<Company>          Companies          { get; set; } = [];
    public List<VocationalTrainer> VocationalTrainers { get; set; } = [];
    public List<Apprentice>       Apprentices        { get; set; } = [];
}

/// <summary>
/// Manages loading and saving application data to a local <c>data.json</c> file.
/// All public collections are kept in memory; call <see cref="Save"/> after
/// any mutation to persist changes.
/// </summary>
public class DataStore
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented       = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly string _filePath;

    public List<Company>           Companies          { get; private set; } = [];
    public List<VocationalTrainer> VocationalTrainers { get; private set; } = [];
    public List<Apprentice>        Apprentices        { get; private set; } = [];

    public DataStore(string filePath = "data.json")
    {
        _filePath = filePath;
        Load();
    }

    /// <summary>Loads data from the JSON file. Creates an empty store if the file doesn't exist.</summary>
    public void Load()
    {
        if (!File.Exists(_filePath))
        {
            Companies          = [];
            VocationalTrainers = [];
            Apprentices        = [];
            return;
        }

        try
        {
            string json  = File.ReadAllText(_filePath);
            var    data  = JsonSerializer.Deserialize<AppData>(json, _jsonOptions);

            Companies          = data?.Companies          ?? [];
            VocationalTrainers = data?.VocationalTrainers ?? [];
            Apprentices        = data?.Apprentices        ?? [];
        }
        catch (JsonException)
        {
            // Corrupt / incompatible file – start fresh.
            Companies          = [];
            VocationalTrainers = [];
            Apprentices        = [];
        }
    }

    /// <summary>Persists the current in-memory state to the JSON file.</summary>
    public void Save()
    {
        var data = new AppData
        {
            Companies          = Companies,
            VocationalTrainers = VocationalTrainers,
            Apprentices        = Apprentices,
        };

        string json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }
}
