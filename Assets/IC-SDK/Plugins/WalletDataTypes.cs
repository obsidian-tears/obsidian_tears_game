using System.Text;
using UnityEngine.Scripting;

namespace IC_SDK
{
    [System.Serializable]
    [Preserve]
    public class NftDelails
    {
        [Preserve] public string name;
        [Preserve] public string image;
        [Preserve] public string description;
        [Preserve] public string collection;
        [Preserve] public bool isOwner;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Name: {this.name}");
            sb.AppendLine($"Description: {this.description}");
            sb.AppendLine($"Image: {this.image}");
            sb.AppendLine($"Collection :{this.collection}");
            sb.AppendLine($"Is owner :{this.isOwner}");
            return sb.ToString();
        }
    }
}