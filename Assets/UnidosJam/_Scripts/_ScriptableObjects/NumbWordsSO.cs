using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Numbers", menuName = "Unidos/Numbers/Data/Create")]
public class NumbWordsSO : ScriptableObject
{
    [SerializeField] List<NumbWord> numbers;
    public List<NumbWord> list => numbers;
}


[System.Serializable]
public class NumbWord
{
    public int number;
    public List<LanguageWord> words;
}

[System.Serializable]
public class LanguageWord
{
    public string language;
    public string word;
}