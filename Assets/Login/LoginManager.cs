using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft;
using Newtonsoft.Json;
public class LoginManager : MonoBehaviour
{
    public string principal;
    public string alias;
    private bool successLog;



    public bool SuccessLog { get => successLog; set => successLog = value; }

    [SerializeField]

    void Start()
    {
       

    }

    public void GetAlias(string alias)
    {
        if (!string.IsNullOrEmpty(alias))
        {
            this.alias = alias;
            Debug.Log("el alias es :" + alias);
        }
        else
        {
            Debug.Log("me llego un alias null");
        }
    }
    public void GetPrincipal(string principal)
    {
        if (principal != null)
        {
            this.principal = principal;
            Debug.Log($"Mi Principal es: {principal}");
            SuccessLog = true;
            PedirNFTS();

            //logInSuccesfull.SetActive(true);
            //canvasAfterLogIn.SetActive(true);
            //foreach (var item in canvasToLogIn)
            //{
            //    item.SetActive(false);
            //}

            //_nickNameText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Error en getPrincipal");
            SuccessLog = false;
        }
    }

    public void InitData(string json)
    {
        JsonResoult characterData = JsonConvert.DeserializeObject<JsonResoult>(json);
        string characterClass = characterData.Clase;
        string characterUrl = characterData.Url;
    }



    public void PedirNFTS()
    {
        Debug.Log("Pidiendo NFTS...");
        ReactFunctions.GetNFT();
    }

    void Login()
    {
        Debug.Log("Logueandose...");
        ReactFunctions.LoginIc();
        //text = "Loading...";
    }

}

public class JsonResoult
{
    private string clase;
    private string url;

    public string Clase { get => clase; set => clase = value; }
    public string Url { get => url; set => url = value; }
}
