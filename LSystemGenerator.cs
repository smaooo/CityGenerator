using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Structures;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentence;
    [Range(0,10)]
    public int iterationLimit = 1;

    public bool randomIgnoreRuleModifier = true;
    [Range(0, 1)]
    public float changeToIgnoreRule = 0.3f;
    private void Start()
    {
        //Debug.Log(GenerateSentence());
    }
    public Tuple<string,List<int>> GenerateSentence(List<GrowthDirection> direction, string word = null)
    {
        if (word == null)
        {
            word = rootSentence;
        }
        string finalWord = "";
        for (int i = 0; i < direction.Count; i++)
        {
            finalWord += "+[" + rules[0].GetResult() + "]|";
            //if (i < direction.Count -1)
            //else
            //finalWord += "-[" + word + "]";
        }
        finalWord = GrowRecursive(finalWord);
        string tmp = finalWord;
        List<int> indices = new List<int>();
        for (int i = 0; i < direction.Count; i++)
        {
            int fi = tmp.IndexOf("[");
            int li = tmp.IndexOf("]");
            string sub = tmp.Substring(fi, li - fi);
            int index = 0;
            foreach (var c in sub)
            {
                if (c == 'F')
                {
                    index++;
                }
            }
            indices.Add(index);
            tmp.Remove(0, li);
        }
        print(finalWord);
        return new Tuple<string, List<int>>(GrowRecursive(finalWord), indices);
    }

    private string GrowRecursive(string word, int iterationIndex = 0)
    {
        if (iterationIndex >= iterationLimit) 
        {
            return word;
        }
        StringBuilder newWord = new StringBuilder();

        foreach (var c in word) 
        {
            newWord.Append(c);
            ProcessRulesRecursively(newWord, c, iterationIndex);
        }
        return newWord.ToString();
    }

    private void ProcessRulesRecursively(StringBuilder newWord, char c, int iterationIndex)
    {
        foreach (var rule in rules) 
        {
            if (rule.letter == c.ToString()) 
            {
                if (randomIgnoreRuleModifier && iterationIndex > 1) 
                {
                    if (UnityEngine.Random.value < changeToIgnoreRule) 
                    {
                        return;
                    }
                }
                newWord.Append(GrowRecursive(rule.GetResult(),iterationIndex +1));
            }
        }
    }
}
