using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Cocktail : MonoBehaviour{
    public string cocktailName;
    public List<string> zutaten = new List<string>();
    public string glas;
    public string preis;

    public void print()
    {
        print(cocktailName);
        foreach (string s in zutaten)
            print(s);
        print(glas);
        print(preis);
    }
}