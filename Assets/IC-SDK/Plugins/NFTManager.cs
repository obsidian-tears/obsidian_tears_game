using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;


namespace IC_SDK
{
    public class NFTManager : MonoBehaviour
    {
        public static NFTManager Instance;
        [SerializeField] private ScrollViewManager managerScrollView;

        public List<NftDelails> collection;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There is an existing NFT Managers, destroying...");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public void RequestNFT(string json)
        {
            Debug.Log("REQUEST NFT");
            Debug.Log(json);

            if (json != null)
            {
                collection = JsonConvert.DeserializeObject<List<NftDelails>>(json);;

                if (collection == null)
                {
                    Debug.LogError("Error in the deserialization of the collection.");
                }
                else
                {
                    if (collection.Count > 0)
                    {
                        Debug.Log($"amount of NFTs {collection.Count}");

                        managerScrollView.AssignImage(this.collection);
                    }
                    else
                    {
                        Debug.Log("There is no NFTs");
                    }
                }
            }
            else
            {
                Debug.LogError("Error in the deserialization of the collection");
            }
        }

        public static void RequestNFTs()
        {
            ReacFunctions.GetNFT();
        }
    }
}