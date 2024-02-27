using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using Newtonsoft.Json;

public class NftManager : MonoBehaviour
{
    public static NftManager Instance;


    public List<NftDetails> colleccion { get; private set; }

    [SerializeField]
    public Dictionary<string, bool> colleccionIDS = new Dictionary<string, bool>()
    {
        ["Hallow's End"] = false,
        ["The Genesis Collection"] = false,
        ["Purple's Legion"] = false,
        ["Dark Irina"] = false,
        ["Dark Dfinity"] = false
    };

    //[SerializeField] public ScrollViewManager managerScrollView;
    //[SerializeField] public AvatarManager avatarManager;
    //[SerializeField] public bool TieneNFTS = false;
    //[SerializeField] CameraViewManager skinManager;


    public List<Tuple<string, string>>
        walletList = new List<Tuple<string, string>>(); //primer string: ic o xerial - segundo: wallet

    private void Awake()
    {
        Instance = this;
        LoadCollection();
    }

    public void RequestNFT(string json)
    {
        Debug.Log(json);
        if (json != null)
        {
            StartCoroutine(DeserializeCoroutine(json));
        }
        else
        {
            Debug.LogError("Error en el REQUESTNFT");
        }
    }

    IEnumerator DeserializeCoroutine(string json)
    {
        List<NftDetails> nftList = JsonConvert.DeserializeObject<List<NftDetails>>(json);

        yield return new WaitForSeconds(1);

        if (nftList == null)
        {
            Debug.LogError("Error en la deserealizacion de la collecion");
        }
        else
        {
            // foreach (var nft in nftList)
            // {
            //     Debug.Log(nft.ToString());
            // }

            colleccion = nftList;

            //managerScrollView.AsignarImagenes(this.colleccion);
            //CollecionesDelUsuario();
            //skinManager.SetSkins();
            //avatarManager.SetAvatars();
            //TieneNFTS = true;
        }
    }

    //public void UpdateCollection()
    //{
    //    managerScrollView.AsignarImagenes(this.colleccion);
    //    CollecionesDelUsuario();
    //    skinManager.SetSkins();
    //    avatarManager.SetAvatars();
    //    TieneNFTS = true;
    //}

    private void CollecionesDelUsuario()
    {
        Debug.Log("Collciones");
        LoadCollection();

        colleccionIDS.TryAdd("The Genesis Collection", false);
        colleccionIDS.TryAdd("Hallow's End", false);
        colleccionIDS.TryAdd("Purple's Legion", false);
        colleccionIDS.TryAdd("Dark Irina", false);

        foreach (NftDetails nft in this.colleccion)
        {
            if (nft.collection == "Hallow's End")
            {
                if (colleccionIDS.ContainsKey("Hallow's End"))
                    colleccionIDS["Hallow's End"] = true;
                else
                    colleccionIDS.TryAdd("Hallow's End", true);
            }

            if (nft.collection == "The Genesis Collection")
            {
                if (colleccionIDS.ContainsKey("The Genesis Collection"))
                    colleccionIDS["The Genesis Collection"] = true;
                else
                    colleccionIDS.TryAdd("The Genesis Collection", true);
            }

            if (nft.collection == "Purple's Legion")
            {
                if (colleccionIDS.ContainsKey("Purple's Legion"))
                    colleccionIDS["Purple's Legion"] = true;
                else
                    colleccionIDS.TryAdd("Purple's Legion", true);
            }

            if (nft.collection == "Dark Irina")
            {
                if (colleccionIDS.ContainsKey("Dark Irina"))
                    colleccionIDS["Dark Irina"] = true;
                else
                    colleccionIDS.TryAdd("Dark Irina", true);
            }
        }
    }

    private void LoadCollection()
    {
        colleccionIDS.TryAdd("Hallow's End", false);
        colleccionIDS.TryAdd("The Genesis Collection", false);
        colleccionIDS.TryAdd("Purple's Legion", false);
        colleccionIDS.TryAdd("Dark Irina", false);
        colleccionIDS.TryAdd("Dark Dfinity", false);
    }
}
