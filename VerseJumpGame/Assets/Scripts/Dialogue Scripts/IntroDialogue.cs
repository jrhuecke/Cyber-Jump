using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    public Character Anita;
    public Character RuPaw;
    DialogueSystem dialogue;


    // Start is called before the first frame update
    void Start()
    {
        dialogue = DialogueSystem.instance;
        Anita = CharacterManager.instance.GetCharacter ("Anita", enableCreatedCharacterOnStart: false);
        RuPaw = CharacterManager.instance.GetCharacter ("Rupaw", enableCreatedCharacterOnStart: false);
        DialogueSystem.instance.openPan();
    }

    public string[] speech = new string[]{
        "Ugh, where am I? What happened?"
    };
    int i =0;

    public string[] speech2 = new string[]{
        "Welcome, Anita Gunn.", 
        "I am Rupaw and I will be your fur-end in this Cyberspace."

    };

    public string [] bothtext = new string[]
    {
        "Cyberspace what are you talking about? How do you know my name?:Anita",
        "Also, are you a cat?",
        "All will become fur-miliar in due time.:RuPaw",
        "I assume you still remember how to use your paw-some weapon. Am I correct?",
        "Yea. I think so.:Anita",
        "Good. Good. We are not starting from scratch but I would still like to test your abilities for myself.:Rupaw", 
        "What?:Anita",
        "Purr-pare yourself Anita Gunn for I must see if you have what it takes to survive in this world!:RuPaw",
        "Wait a minute!:Anita"
    };


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length)
                Anita.Say (speech[i]);
            else if (i >= speech.Length && (i-speech.Length) < speech2.Length)
                RuPaw.Say(speech2[i-speech.Length]);

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
