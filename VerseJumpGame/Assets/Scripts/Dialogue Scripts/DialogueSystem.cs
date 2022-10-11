using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour
{

    public static DialogueSystem instance;
    public ELEMENTS elements;
    
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// Say somethig and show it on the speeker
    public void Say(string speech, string speaker ="")
    {
        StopSpeaking();
        speaking = StartCoroutine(Speaking(speech, speaker));

    }

    public void StopSpeaking() 
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
       speaking = null;
    }

    public bool isSpeaking {get{return speaking != null;}}
    public bool isWaitingForUserInput = false;

    Coroutine speaking = null;
    IEnumerator Speaking(string targetSpeech, string speaker ="")
    {
        speechPanel.SetActive(true);
        speechText.text = "";
        speakerNameText.text = DetermineSpeaker(speaker);
        isWaitingForUserInput = false;

        while(speechText.text != targetSpeech)
        {
            speechText.text += targetSpeech[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }
        //finished text
        isWaitingForUserInput = true;
        while (isWaitingForUserInput)
            yield return new WaitForEndOfFrame();
            
        StopSpeaking();
    }

    string DetermineSpeaker(string s) {
        string retVal = speakerNameText.text;
        if (s != speakerNameText.text && s != "")
            retVal = (s.ToLower().Contains("narrator")) ? "": s;
        
        return retVal;
    }

	public void openPan()
    {
        speechPanel.SetActive(true);
    }

	public void Close()
    {
        StopSpeaking();
        speechPanel.SetActive(false);
    }

    [System.Serializable]
    public class ELEMENTS
    {
        // The main panel containg all dialogue related elements on the UI
        public GameObject speechPanel;
        public Text speakerNameText;
        public Text speechText;

    }
    public GameObject speechPanel {get{return elements.speechPanel;}}
    public Text speakerNameText {get{return elements.speakerNameText;}}
    public Text speechText {get{return elements.speechText;}}


}
