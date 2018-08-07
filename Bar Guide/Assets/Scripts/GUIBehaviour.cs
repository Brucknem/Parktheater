using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GUIBehaviour: MonoBehaviour {

    public GameObject[] cocktails;
    public Dropdown auswahl;
    public Button exit;
    public int activeElement = 0;

	// Use this for initialization
	void Start () {
        InitDropDown();
        exit.onClick.AddListener(delegate { OnQuit(); });
        auswahl.onValueChanged.AddListener(delegate { ShowCocktail(auswahl.value, true); });
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowCocktail(int number, bool jump)
    {
        cocktails[activeElement].SetActive(false);
        if (jump)
        {
            activeElement = number;
        }
        else
        {
            activeElement = (activeElement + (int) Mathf.Sign(number)) % cocktails.Length;
            if (activeElement < 0)
                activeElement = cocktails.Length - 1;
        }
        cocktails[activeElement].SetActive(true);
        auswahl.value = activeElement;
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    private void InitDropDown()
    {
        auswahl.ClearOptions();
        List<string> options = new List<string>();
        foreach(var o in cocktails)
        {
            options.Add(o.name);
        }
        auswahl.AddOptions(options);
    } 
}
