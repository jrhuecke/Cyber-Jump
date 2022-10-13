using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleDialogue : MonoBehaviour
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
        "Ok… are we done now?",
        "Can you please explain to me what is going on?"
    };
    int i =0;

    public string[] speech2 = new string[]{
        "We are not finished yet Anita.", 

    };

    public string [] bothtext = new string[]
    {
        "What do you mean we’re not finished yet? I beat you.:Anita",
        "What more is there to do?",
        "Here in Cyberspace, viruses attack in many different forms and ways than what you're familiar with. It’s un-fur-tunate but it’s the life we live.:RuPaw",
        "Viruses? So Viruses would be attacking me in here.:Anita",
        "Yes. As I said before, more will be revealed in time so keep the questions to a meow-nimum.:RuPaw",
        "Ok. So what are we going to do?:Rupaw", 
        "The device on your stomach will allow us to travel to where we need to go. Whenever you are ready, purr-ess the button and we will begin.:RuPaw",
        "Okay. Let’s get this done with.:Anita"
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
