using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InstancerBehaviour : MonoBehaviour
{
    [SerializeField] Texture2D texture;

    private void Start()
    {
        GenerateHeighMap();
    }

   
    private void GenerateHeighMap()
    {
        texture.Reinitialize(300,300);
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices; // Pobieramy wierzcho�ki
        Color[] colors = mesh.colors; // Pobieramy kolory wierzcho�k�w (je�li istniej�)
        float bigestY = 0;
        float smalestY = 0;
        for (int i =0;i < vertices.Count();i++) 
        {
            
            if (vertices[i].y > bigestY)
            { 
                bigestY = vertices[i].y;
            }else if (vertices[i].y < smalestY)
            {
                smalestY = vertices[i].y;
            }
        }
        Debug.Log("bigest "+ bigestY + "  smallest "+ smalestY);
    }
}