using System.Collections.Generic;

public class CSVManager
{   
    public static List<Dictionary<string, object>> Spawn_Data = new List<Dictionary<string, object>>(CSVReader.Read("Spawner"));

}
