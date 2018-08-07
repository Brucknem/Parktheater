using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GUIBehaviour_XML : MonoBehaviour
{
    public Transform cocktailViewer;
    public Text title;
    public Dropdown auswahl;
    public Button shownList, sort, download, add, remove;
    public Sprite sorted, unsorted;
    public Material gold;
    public Color outlineColor;
    public Vector2 outlineDistance;
    public Font font;

    private int activeElement = 0;
    private Text shownListButtonText, sortedButtonText;

    private readonly string STANDART_COCKTAILS = "Standart Cocktails";
    private readonly string OWN_COCKTAILS = "Meine Cocktails";
    private readonly string UNSORTED = "Uns.";
    private readonly string SORTED = "A - Z";

    private bool isSorted = false;

    private List<Cocktail> list;

    
    // Use this for initialization
    void Start()
    {
        add.onClick.AddListener(delegate { OnAdd(); });
        remove.onClick.AddListener(delegate { OnRemove(); });
        sort.onClick.AddListener(delegate { OnSort(); });
        download.onClick.AddListener(delegate { OnDownload(); });
        auswahl.onValueChanged.AddListener(delegate { ChangeShownCocktail(auswahl.value, true); });
        shownList.onClick.AddListener(delegate { OnChangeShownList(); });
        XMLHandler.LoadCocktailsFromXML();
        list = XMLHandler.cocktails;
        shownListButtonText = shownList.GetComponentInChildren<Text>();
        if (XMLHandler.stayOnOwn)
            OnChangeShownList();
        ChangeShownList();
        ChangeOutlineRecursively(transform);
    }
   
    private void ChangeOutlineRecursively(Transform parent)
    {
        parent.tag = "Tested";
        if(parent.GetComponent<Outline>() ?? null)
        {
            parent.GetComponent<Outline>().effectColor = outlineColor;
            parent.GetComponent<Outline>().effectDistance = outlineDistance;
        }

        if (parent.childCount <= 0)
            return;
        
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if(child.tag != "Tested")
                ChangeOutlineRecursively(child);
        }
    }

    public void ChangeShownCocktail(int number, bool jump)
    {
        if (jump)
            activeElement = number;

        else
        {
            activeElement = (activeElement + (int)Mathf.Sign(number)) % list.Count;
            if (activeElement < 0)
                activeElement = list.Count - 1;
        }
        auswahl.value = activeElement;
        ShowCocktail();
    }

    public void ShowCocktail()
    {
        ClearViewer();
        if (list.Count == 0)
        {
            ClearViewer();
            if (shownListButtonText.text == STANDART_COCKTAILS)
            {
                title.text = "Wilkommen";
                AddText("Bitte lade zunächst");
                AddText("die aktuelle Cocktailliste");
                AddText("herunter");
            }
            else
            {
                title.text = "Eigene Cocktails";
                AddText("Füge deinen");
                AddText("ersten eigenen");
                AddText("Cocktail hinzu");
            }
            return;
        }

        title.text = list[activeElement].cocktailName;

        foreach(string s in list[activeElement].zutaten)
            AddText(s);
        AddText(list[activeElement].glas);
        AddText(list[activeElement].preis);
    }

    public void AddText(string text)
    {
        GameObject newGO = new GameObject("info");
        newGO.AddComponent<LayoutElement>();
        newGO.transform.SetParent(cocktailViewer);
        Text myText = newGO.AddComponent<Text>();
        myText.font = font;
        myText.fontSize = 24;
        myText.alignment = TextAnchor.MiddleCenter;
        myText.transform.localScale = new Vector3(1, 1, 1);
        myText.rectTransform.sizeDelta = new Vector2(355, 30);
        myText.material = gold;
        myText.gameObject.AddComponent<PositionAsUV1>();
        
        Outline o = myText.gameObject.AddComponent<Outline>();
        o.effectColor = outlineColor;
        o.effectDistance = outlineDistance;
        
        myText.text = text;
    }

    public void ClearViewer()
    {
        List<GameObject> toRemove = new List<GameObject>();
        title.text = "";
        for(int i = 2; i < cocktailViewer.transform.childCount; i++)
            toRemove.Add(cocktailViewer.transform.GetChild(i).gameObject);

        foreach (var t in toRemove)
            Destroy(t);
    }

    public void OnAdd()
    {
        SceneManager.LoadScene("Add");
    }

    public void OnRemove()
    {
        if (XMLHandler.ownCocktails.Count == 0)
            return;
        XMLHandler.ownCocktails.RemoveAt(activeElement);
        XMLHandler.SaveCocktailsToXML();
        ChangeShownList();
    }

    public void OnSort()
    {
        if(isSorted)
        {
            isSorted = false;
        }
        else
        {
            isSorted = true;
        }
        ChangeShownList();
    }

    private void SetSortedIcon()
    {
        if(isSorted)
            sort.GetComponent<Image>().sprite = sorted;
        else
            sort.GetComponent<Image>().sprite = unsorted;

    }

    public void OnDownload()
    {
        StartCoroutine(Download());
    }

    private IEnumerator Download()
    {
        yield return StartCoroutine(XMLHandler.DownloadList());
        ChangeShownList();
    }

    private void InitDropDown()
    {
        auswahl.ClearOptions();
        List<string> options = new List<string>();
        if (list.Count != 0)
            foreach (var o in list)
                options.Add(o.cocktailName);
        else
            options.Add("Keine vorhanden");

        auswahl.AddOptions(options);
    }

    private void OnChangeShownList()
    {
        if (shownListButtonText.text == STANDART_COCKTAILS)
            shownListButtonText.text = OWN_COCKTAILS;
        else
            shownListButtonText.text = STANDART_COCKTAILS;

        add.gameObject.SetActive(!add.gameObject.activeSelf);
        remove.gameObject.SetActive(!remove.gameObject.activeSelf);
        download.gameObject.SetActive(!download.gameObject.activeSelf);
        ChangeShownList();
    }

    private void ChangeShownList()
    {
        if(shownListButtonText.text == STANDART_COCKTAILS)
            list = XMLHandler.cocktails;
        else
            list = XMLHandler.ownCocktails;

        if (isSorted)
        {
            list = list.OrderBy(o => o.cocktailName).ToList();
        }


        SetSortedIcon();

        InitDropDown();
        ChangeShownCocktail(0, true);
    }
}