using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IC_SDK
{
    public class ScrollViewManager : MonoBehaviour
    {
        [Tooltip("Button prefab for NFTs")]
        [SerializeField] private GameObject _buttonImagePrefab;
        
        [Tooltip("Button prefab for NFTs")]
        [SerializeField] private GameObject _buttonBlocked;
        
        [Tooltip("Transform of the content ScrollView")]
        [SerializeField] private Transform _contentTransform;

        public async void AssignImage(List<NftDelails> listImages)
        {
            var counter = 0;
            foreach (var imageUrl in listImages)
            {
                if (imageUrl.isOwner)
                {
                    await LoadAndCreateRawImage(imageUrl.image, _buttonImagePrefab);
                }
                else
                {
                    await LoadAndCreateRawImage(imageUrl.image, _buttonBlocked);
                }
                
                counter++;
                if (counter > 60)
                {
                    break;
                }
            }
        }
        
        private Task LoadAndCreateRawImage(string imageUrl, GameObject prefab)
        {
            var image = Instantiate(prefab, _contentTransform).GetComponent<Button>().image;

            ImageLoader.Instance.AssignImage(imageUrl, image, (texture) => { });
            return Task.CompletedTask;
        }
    }
}