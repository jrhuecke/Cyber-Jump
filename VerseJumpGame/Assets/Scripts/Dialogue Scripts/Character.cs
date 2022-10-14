using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character
{

public string characterName;
/// Root is the container for all images relate to the character in the scene. The root object
[HideInInspector] public RectTransform root;

public bool isMultiLayerCharacter{get{ return renderers.renderer == null;}}

public bool enabled {get{ return root.gameObject.activeInHierarchy;} set{ root.gameObject.SetActive(value);}}

DialogueSystem dialogue;

public void Say(string speech)
{
    if (!enabled)
        enabled = true;
    dialogue.Say(speech, characterName);
}
// create a new character
public Character (string _name, bool enableOnStart = true)
{
    CharacterManager cm = CharacterManager.instance;

    // locate the character prefab
    GameObject prefab = Resources.Load("DialoguePrefabs/"+_name+"") as GameObject;

    // spawn an instance of the prefab directly on the character panel
    GameObject ob = GameObject.Instantiate (prefab, cm.characterPanel);

    root = ob.GetComponent<RectTransform> ();
    characterName = _name;

//get the renderer(s)
    renderers.renderer = ob.GetComponentInChildren<RawImage> ();
    
    dialogue = DialogueSystem.instance;
    enabled = enableOnStart;


}

class Renderers
{
    // used as the only image for single layer character
    public RawImage renderer;

    //sprites use images
    //the body renderer for the multilayer character
    public Image bodyRenderer;

    //the expression renderer for the multilayer character
    public Image expressionRenderer;
}

Renderers renderers = new Renderers();
}