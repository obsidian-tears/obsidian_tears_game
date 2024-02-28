using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
public class ICConnect : MonoBehaviour
{
    public string characterClass;
    public string characterUrl;

    public void InitData(string json)
    {
        JsonResoult characterData = JsonConvert.DeserializeObject<JsonResoult>(json);
        this.characterClass = characterData.CharacterClass;
        this.characterUrl = characterData.CharacterUrl;
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

