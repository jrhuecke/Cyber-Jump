using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDialogue : MonoBehaviour
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

    public string[] speech = new string[]{
        "Purr-fect. I believe you are ready."
    };
    int i =0;

    public string[] speech2 = new string[]{
        "Finally! What now?", 

    };

    public string [] bothtext = new string[]
    {
        "Now we see you will face the real deal.:RuPaw",
        "I will accompany you as you journey through this cyberspace. Saving this place from the invading malware sent to destroy it.",
        "You were trained to be this place’s protector, that’s all I know about you specifically. There is a paw-sibility you might remember more but...:RuPaw",
        "No need to explain further. There’s no point in wondering, it won’t help the situation we’re currently in.:Anita",
        "Anyways. I guess it’s time to move…",
        "Fur-ward.", 
        "Nya-hahahahaha now you’re getting it!:RuPaw"
    };


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length)
                guy.Say (speech[i]);
            else if (i >= speech.Length && (i-speech.Length) < speech2.Length)
                Anita.Say(speech2[i-speech.Length]);

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
