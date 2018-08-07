using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AddCocktail : MonoBehaviour {

    public Button quit, save;
    public InputField[] inputs;
    public Color outlineColor;
    public Vector2 outlineDistance;

	// Use this for initialization
	void Start () {
        save.onClick.AddListener(delegate { OnSave(); });
        quit.onClick.AddListener(delegate { OnQuit(); });
        ChangeOutlineRecursively(transform);
    }

    private void ChangeOutlineRecursively(Transform parent)
    {
        parent.tag = "Tested";
        if (parent.GetComponent<Outline>() ?? null)
        {
            parent.GetComponent<Outline>().effectColor = outlineColor;
            parent.GetComponent<Outline>().effectDistance = outlineDistance;
        }

        if (parent.childCount <= 0)
            return;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.tag != "Tested")
                ChangeOutlineRecursively(child);
        }
    }

    private void OnSave()
    {
        Cocktail cocktail = new Cocktail();
        cocktail.cocktailName = inputs[0].text;
        string[] zutaten = inputs[1].text.Split('\n');
        foreach (string s in zutaten)
        {
            if(s != "")
                cocktail.zutaten.Add(s);
        }
        cocktail.glas = inputs[2].text;
        cocktail.preis = inputs[3].text + " €";
        XMLHandler.ownCocktails.Add(cocktail);
        XMLHandler.SaveCocktailsToXML();
        OnQuit();
    }

    private void OnQuit()
    {
        XMLHandler.stayOnOwn = true;
        SceneManager.LoadScene("Testing");
    }
}