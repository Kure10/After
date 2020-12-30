using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEventContent : MonoBehaviour
{
    List<GameObject> characters = new List<GameObject>();

    public void AddCharacterToContent(GameObject go)
    {
        go.transform.SetParent(this.transform);
        characters.Add(go);
    }

    public void ClearCharactersInContent()
    {
        foreach (var item in characters)
        {
            Destroy(item.gameObject);
        }

        characters.Clear();
    }
}
