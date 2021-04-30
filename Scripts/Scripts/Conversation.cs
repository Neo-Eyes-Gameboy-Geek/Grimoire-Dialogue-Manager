using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The Line data type used to fill a conversation
[System.Serializable]
public class Line
{
    //a line will contain an index into the portrait array to display on the UI
    public int PortraitIndex;
    //a line to display on screen
    public string Dialogue;

    //A non-arg constructor to provide robust code
    public Line()
    {
        this.PortraitIndex = 0;
        this.Dialogue = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
    }
    //Argumented full state constructor 
    public Line(int PortraitIndex, string Dialogue)
    {
        this.PortraitIndex = PortraitIndex;
        this.Dialogue = Dialogue;
    }
}

//This is a conversation, simply used to make conversation construction easier since a conversation object can be made
[CreateAssetMenu(fileName = "New Conversation", menuName = "Neo Eyes Dialogue System/Conversation")]
public class Conversation : ScriptableObject
{
    //Obviously we need an array of Lines to use for the dialogue queue
    public Line[] lines;
    //and an array of sprites that go with the lines to act as a portrait palatte
    public Sprite[] sprites; 
}
