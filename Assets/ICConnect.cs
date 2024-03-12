using Newtonsoft.Json;
using UnityEngine;

public class ICConnect : MonoBehaviour
{
    public static string characterClass;
    public static string characterUrl; //= "https://pbs.twimg.com/media/EN4EWu4WAAA9t14.png";

    public void InitData(string json)
    {
        JsonResoult characterData = JsonConvert.DeserializeObject<JsonResoult>(json);
        characterClass = characterData.CharacterClass;
        characterUrl = characterData.CharacterUrl;
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

