using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing2 : MonoBehaviour
{
    public Character Anita;
    public Character guy;
    DialogueSystem dialogue;


    // Start is called before the first frame update
    void Start()
    {
        dialogue = DialogueSystem.instance;
        Anita = CharacterManager.instance.GetCharacter ("Anita", enableCreatedCharacterOnStart: false);
        guy = CharacterManager.instance.GetCharacter ("guy", enableCreatedCharacterOnStart: false);
        DialogueSystem.instance.openPan();
    }

    public string[] speech;
    int i =0;

    public string[] speech2;

    public string [] bothtext = new string[]
    {
        "Hi, my name is Atlas. I am the one on the right!:Atlas",
        "I was originally on Nexomon and now I am being used as some prototype.",
        "And I am some other Guy!:Guy",
        "I don't know my name because I forgot but I am also being used as some prototype.",
        "I am the one on the right.",
        "And I am the one on the left!: Atleas", 
        "This isn't fully finished, but I hope you get where I am coming from :D",
        "My laptop can't record things very well, so I'm using my phone. So sorry about that!",
        "Hope this is enough!!!!!:Guy"
    };


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length)
                Anita.Say (speech[i]);
            else if (i >= speech.Length && (i-speech.Length) < speech2.Length)
                guy.Say(speech2[i-speech.Length]);

            else if((i-speech.Length) >= speech2.Length && (i-(speech.Length+speech2.Length)) < bothtext.Length)
                Say(bothtext[i-(speech.Length+speech2.Length)]);
            else
                DialogueSystem.instance.Close();
            print((i-speech.Length)+1 < speech2.Length);
            i++;
        }
    }

    void Say(string bothtext)
    {
        string[] parts = bothtext.Split(':');
        string Speech = parts[0];
        string speaker = (parts.Length >=2) ? parts[1] : "";

        dialogue.Say(Speech, speaker); 
    }
}
