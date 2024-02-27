using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

public class NftDetails 
{
    [Preserve] public string name;
    [Preserve] public string image;
    [Preserve] public string description;
    [Preserve] public string collection;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"nombre: {this.name}");
        sb.AppendLine($"descripcion: {this.description}");
        sb.AppendLine($"imagen: {this.image}");
        sb.AppendLine($"collection :{this.collection}");
        return sb.ToString();
    }
}
