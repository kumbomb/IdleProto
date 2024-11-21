using System.Collections.Generic;

public class CSVManager
{   
    public static List<Dictionary<string, object>> EXP = new List<Dictionary<string, object>>(CSVReader.Read("EXP"));

}
