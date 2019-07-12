using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/* One time only quick and dirty script
 * Generate text file for map editor with from unity screen 
 * Needed for proper grid creation
 */
public class MapGenerator : MonoBehaviour
{
    public GameObject floor;
    public GameObject wall;
    public int gridSizeX;
    public int gridSizeY;

    // Start is called before the first frame update
    void Start()
    {
        var grid = new char[gridSizeY, gridSizeX];
        for(int i = 0; i < grid.GetLength(0); i++)
        {
           for(int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = '0';
            }
        }

        foreach (Transform child in floor.transform)
        {
            
            grid[Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.z)] = 'E';
        }
        foreach (Transform child in wall.transform)
        {
            grid[Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.z)] = 'W';
        }

        string path = "Assets/Editor/test.txt";

        StreamWriter writer = new StreamWriter(path, true);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                sb.Append(char.ToString(grid[i, j]) + ' ');
            }
            writer.WriteLine(sb.ToString());
            sb.Clear();
        }
        writer.Close();
    }
}
