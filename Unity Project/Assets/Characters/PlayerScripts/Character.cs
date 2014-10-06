using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Character : MonoBehaviour
{
    public List<TasteCollection.Taste> myTastes;
    //public CharacterTimers Timer;
    public int characterNum;
    public LetterController letterControl;
    public GameObject letterGenerator;
    private VariableControl variables;
    //Dictionary of taste ID's to names
    public static Dictionary<int, TasteCollection.Taste> tasteDictionary;
    public List<string> wordsFedToMe;
	public string thingsILike = ""; //What should be displayed as the creatures tastes when you click it
	private static Dictionary<int, string> humanReadableTasteDictionary; //for looking up the human-readable version of my tastes

    public int Likes(string word)
    {
        if (characterNum != 0) { //If we're not the trash character... 
						if (letterControl.checkForWord (word) == false) {
								Debug.Log ("Not a word and this isn't the trash character");
								return 0;
						}
				} else {
						//If we ARE the trash character, don't let people throw away single letters
						if (word.Length == 1) {
				Debug.Log ("You can't throw away fewer than two letters. The GDD says so.");
							return 0;
						}
				}
        //If we get here, either we're the trash character, or it was a proper word
        //Debug.Log("About to score the word");
        float tempScore = scoreWord(word);
        return (int)tempScore;
    }

    float scoreWord(string word)
    {
        float wordScore = 0;
        foreach (char letter in word)
        {
            wordScore += LetterController.letterScores[letter];
        }
        Debug.Log("Score for the letters is " + wordScore);
        foreach (TasteCollection.Taste t in myTastes)
        {
            wordScore *= t(word);
        }
        Debug.Log("Score after tastes is " + wordScore);
        return wordScore;
    }

    public void AddTaste(TasteCollection.Taste taste)
    {
        myTastes.Add(taste);
    }

    public void AddTaste(List<TasteCollection.Taste> tastes)
    {
        foreach (TasteCollection.Taste t in tastes)
            if (!myTastes.Contains(t))
                myTastes.Add(t);
    }

    public void RemoveTaste(TasteCollection.Taste taste)
    {
        if (myTastes.Contains(taste))
            myTastes.Remove(taste);
    }
    // Use this for initialization
    void Start()
    {
        //get the same variables everyone else is using
        variables = GameObject.Find("GameController").GetComponent<VariableControl>();
        
        if (tasteDictionary == null) //We only need (or can have, since it's static) one copy of this game-wide, so if it's been done already, don't do it again
        {
            tasteDictionary = new Dictionary<int, TasteCollection.Taste>();
            //Create the dictionary of taste ID's to functions
            tasteDictionary.Add(0, TasteCollection.threeLetters);
            tasteDictionary.Add(1, TasteCollection.fiveOrLonger);
            tasteDictionary.Add(2, TasteCollection.unCommonLetters);
            tasteDictionary.Add(3, TasteCollection.endsWithVowel);
            tasteDictionary.Add(4, TasteCollection.twoOrMoreVowels);
            tasteDictionary.Add(5, TasteCollection.twoOrMoreSame);
            tasteDictionary.Add(6, TasteCollection.startsWithVowel);
            tasteDictionary.Add(7, TasteCollection.startsAndEndsWithSame);
            tasteDictionary.Add(8, TasteCollection.fourLetters);
            tasteDictionary.Add(9, TasteCollection.noPreference);
            tasteDictionary.Add(10, TasteCollection.trashCollection);
        }
		if (humanReadableTasteDictionary == null) { //We only need (or can have, since it's static) one copy of this game-wide, so if it's been done already, don't do it again
			humanReadableTasteDictionary = new Dictionary<int, string> ();
			//Create the dictionary of taste ID's to Human-readable text
			humanReadableTasteDictionary.Add (0, "three letters");
			humanReadableTasteDictionary.Add (1, "five letters or more");
			humanReadableTasteDictionary.Add (2, "uncommon letters(F,H,V,W,Y,K,J,X,Q,Z)");
			humanReadableTasteDictionary.Add (3, "ends in a vowel");
			humanReadableTasteDictionary.Add (4, "more than one vowel");
			humanReadableTasteDictionary.Add (5, "two or more of the same letter");
			humanReadableTasteDictionary.Add (6, "starts with a vowel");
			humanReadableTasteDictionary.Add (7, "starts and ends with same");
			humanReadableTasteDictionary.Add (8, "four letters");
			humanReadableTasteDictionary.Add (9, "anything");
			humanReadableTasteDictionary.Add (10, "trash - things that aren't words");
				}
		myTastes = new List<TasteCollection.Taste>();
		//We have to do this all the time now, in order to display the tastes correctly.
		//{
			//initialize the tastes for the characters
            //First make a generic list of the character taste arrays so that I can
            //easily access my tastes with my character number
            List<int[]> characterTastes = new List<int[]>();
            characterTastes.Add(new int[] { 10 }); //The trash character, ID 0, has one taste, taste 10
            characterTastes.Add(variables.TastesForCharacter1);
            characterTastes.Add(variables.TastesForCharacter2);
            characterTastes.Add(variables.TastesForCharacter3);
            characterTastes.Add(variables.TastesForCharacter4);
            characterTastes.Add(variables.TastesForCharacter5);
            //now we add an arbitrary number of tastes
			//also we set up the text to be displayed for the character's tastes
            foreach (int t in characterTastes[characterNum]) //step through this character's tastes and store the ID of the taste in t
            {
                this.AddTaste(tasteDictionary[t]); //look it up in the dictionary and add Delegate function to our list of Taste delegate functions
				thingsILike = thingsILike + humanReadableTasteDictionary[t]; //Then add it to our text to display
				//if the size of myTastes isn't the same as the size of the array of this character's tastes, then we
				//haven't gotten all of them yet and therefore need an "and" in our human-readable string
				if (myTastes.Count != characterTastes[characterNum].Length) { //If this isn't the last taste we've got
					thingsILike = thingsILike + " and ";
				}
            }
			//Let's see if all that text-making worked or not
			Debug.Log("My character number is " + characterNum + " and I like " + thingsILike);
		if (Application.loadedLevelName == "WordMaking"){
            letterGenerator = GameObject.FindGameObjectWithTag("letterController");
            letterControl = letterGenerator.GetComponent<LetterController>();
        }
    }
    // Update is called once per frame
    void Update()
    {
	}

    void OnMouseDown()
    {
        if (Application.loadedLevelName == "WordMaking")
        {
            //First grab the word - we're gonna need it!
            string word = letterControl.sendWord();
            //score the word - do we have a score?
            int wordScore = Likes(word);
            Debug.Log(word);
            //If it was valid, we'll get a score above 0, so update our score and get that word out of here!
            if (wordScore > 0)
            {
                //if(1 > 0){
                //Keep track of words fed to me!
                wordsFedToMe.Add(word);
                //update the score!
                variables.score += wordScore;
                Debug.Log("The total score is" + variables.score);
                letterControl.ResetStove();
            }
        }
    }
}
