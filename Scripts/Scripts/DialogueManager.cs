using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //First we need a textbox and a Image to use for the portraits 
    public TextMeshProUGUI textBox;
    public Image portrait;

    //A canvas group to slide the alpha on, normally this would be attached to the canvas below
    public CanvasGroup canvasGroup;
    //And a self reference to the canvas this is being ran on so it can be disabled by code when the dialogue ends
    public Canvas canvas;
    //How quickly to fade the canvas in and out
    public float fadeSpeed;

    //The conversation currently being worked on, in case things need to be reset
    public Conversation activeConversation;
    //We also need an array of images to load into memory to prevent duplication
    private Sprite[] portraits; //portraits stored here to avoid memory overhead by holding multiple references
    //and a queue of lines to cycle through
    private Queue<Line> lines;

    //and pause durations for the coroutine for playing text
    public float pauseDuration;
    public float punctuationPauseDuration;
    //a pause duration for when the line starts, just to make sure everything is showing before the dialogue starts typing
    public float initialStartPause;

    //This is a coroutine for printing out the text, kept private since its just referenced to keep all the coroutines being halted
    private Coroutine textPrinter = null;

    //This one is for fading the canvas in and out
    private Coroutine canvasFader = null;
    //And a boolean to indicate while a fade is running
    private bool isFading = false;

    // Start is called before the first frame update
    void Start()
    {
        //Just in case something has been set in the inspector make sure its loaded in and ready to use straight away
        LoadConversation(activeConversation);
    }

    public void LoadConversation(Conversation newConversation)
    {
        //This simply involves taking the new conversation given and swapping out the active variables
        //with the conversations 
        //Assuming we havent been handed a null conversation
        if(newConversation != null)
        {
            activeConversation = newConversation;
            portraits = newConversation.sprites;
            lines = new Queue<Line>(newConversation.lines);
        }

    }

    public void StartConversation()
    {
        //if the fader coroutine is currently running stop it 
        if(canvasFader != null)
        {
            StopCoroutine(canvasFader);
        }
        //enable the UI if it happens to be disabled
        if (!canvas.gameObject.activeInHierarchy)
        {
            canvasFader = StartCoroutine(showDialogueCanvas(fadeSpeed));
        }
        //and assuming there is an active conversation, get it rolling
        if(activeConversation != null)
        {
            AdvanceConversation();
        }
    }

    public void AdvanceConversation()
    {
        //if there is printing currently going on, stop it to avoid jumbled messes
        if(textPrinter != null)
        {
            StopCoroutine(textPrinter);
        }

        if (lines.Count != 0)
        {
            Line currentLine = lines.Dequeue();
            //now pull the portrait index from the line and if the index exists in the portraits array set the portrait to the image
            int index = currentLine.PortraitIndex;
            if (index < portraits.Length)
            {
                portrait.sprite = portraits[index];
            }
            //And pull the text from the line and type it to the screen
            string text = currentLine.Dialogue;
            //now store a reference to this coroutine 
            textPrinter = StartCoroutine(printDialogue(text));
        }
        else
        {
            //if its empty we can disable the canvas since the conversation is ended
            if (canvasFader != null)
            {
                StopCoroutine(canvasFader);
            }
            canvasFader = StartCoroutine(hideDialogueCanvas(fadeSpeed));
        }
    }

    //Coroutine that actually prints the text to the screen typewriter style
    public IEnumerator printDialogue(string dialogue)
    {
        //empty the textbox before starting
        textBox.text = "";

        //Wait for any fade ins to finish
        while (isFading)
        {
            yield return null;
        }

        //Wait a couple of moments just in case the dialogue box only just opened up
        yield return new WaitForSeconds(initialStartPause);
        //now we go through letter by letter and add it to the dialogue text, then wait the allotted time
        foreach (char c in dialogue.ToCharArray())
        {
            textBox.text += c;
            //now check if C is a punctuation mark pause for punctuation time else wait normal time
            if (char.IsPunctuation(c))
            {
                yield return new WaitForSeconds(punctuationPauseDuration);
            }
            else
            {
                yield return new WaitForSeconds(pauseDuration);
            }
        }
    } 

    //Two Methods designed to fade the dialogue box in and out 
    private IEnumerator showDialogueCanvas(float fadeSpeed)
    {
        //Now we are fading so set the bool as such
        isFading = true;
        //Make sure the canvas is turned on before trying this
        canvas.gameObject.SetActive(true);
        
        //continue while the alpha isnt fully opaque keep going
        while(canvasGroup.alpha < 1)
        {
            float newAlpha = Mathf.Clamp(canvasGroup.alpha + (fadeSpeed * Time.deltaTime), 0, 1); //add the amount we are fading by to the current alpha and make sure it doesnt leave a valid range
            canvasGroup.alpha = newAlpha;
            yield return null;
        }
        //now we are done we can set isFading back to false since we are done
        isFading = false;
    }

    private IEnumerator hideDialogueCanvas(float fadeSpeed)
    {
        //Now we are fading so set the bool as such
        isFading = true;
        //continue until the alpha is gone
        while (canvasGroup.alpha > 0)
        {
            float newAlpha = Mathf.Clamp(canvasGroup.alpha - (fadeSpeed * Time.deltaTime), 0, 1); //add the amount we are fading by to the current alpha and make sure it doesnt leave a valid range
            canvasGroup.alpha = newAlpha;
            yield return null;
        }
        //once thats done we can disable the canvas
        canvas.gameObject.SetActive(false);
        //now we are done we can set isFading back to false since we are done
        isFading = false;
    }
}