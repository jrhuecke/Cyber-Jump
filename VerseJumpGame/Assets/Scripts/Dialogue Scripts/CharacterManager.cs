using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// Responsible for adding and maintaining characters in a scene
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;


// All characters must be attached to the character panel
    public RectTransform characterPanel;
    
// A list of all characters in our scene

    public List<Character> characters = new List<Character>();

// Easy lookup for our characters
    public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
    }
    // Try to get a character by the name provided from the character list
    public Character GetCharacter(string characterName, bool createCharacterIfItDoesNotExist = true, bool enableCreatedCharacterOnStart = true)
    {
        // search our dictionary to find the character quickly if its already in our scene
        int index = -1;
        if (characterDictionary.TryGetValue (characterName, out index))
        {
            return characters [index];
        }
        else if (createCharacterIfItDoesNotExist)
        {
            return CreateCharacter (characterName, enableCreatedCharacterOnStart);
        }
        return null;
    }
    public Character CreateCharacter (string characterName, bool enabledOnStart = true)
    {
        Character newCharacter = new Character (characterName, enabledOnStart);
        characterDictionary.Add (characterName, characters.Count);
        characters.Add (newCharacter);

        return newCharacter;
    }
}