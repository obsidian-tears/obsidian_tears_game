using Newtonsoft.Json;
using UnityEngine;

public class ICConnect : MonoBehaviour
{
    public static string characterClass;
    public static string characterUrl = "https://bhuj6-gaaaa-aaaan-qc7ba-cai.raw.icp0.io/?index=2&battle=true"; // 

    public void InitData(string json)
    {
        JsonResoult characterData = JsonConvert.DeserializeObject<JsonResoult>(json);
        characterClass = characterData.CharacterClass;
        //characterUrl = characterData.CharacterUrl; // turn off this to test the url and turn on the url of above
    }
      
}
public class JsonResoult
{
    private int characterIndex;
    private string characterClass;
    private string characterUrl;

    public int CharacterIndex { get => characterIndex; set => characterIndex = value; }
    public string CharacterClass { get => characterClass; set => characterClass = value; }
    public string CharacterUrl { get => characterUrl; set => characterUrl = value; }
}

