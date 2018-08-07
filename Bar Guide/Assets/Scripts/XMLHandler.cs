using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using System;

public class XMLHandler : MonoBehaviour {

    public static List<Cocktail> cocktails;
    public static List<Cocktail> ownCocktails;

    private static string path = "DownloadedCocktails.xml";
    private static string ownPath = "OwnCocktails.xml";

    public static TextAsset cocktailXML;

    public static string url = "https://syncandshare.lrz.de/dl/fiDhfzcQorQAhGzFTNmhmQwx/Cocktails.xml";

    public static bool stayOnOwn = false;

    public static void LoadCocktailsFromXML()
    {
        cocktails = new List<Cocktail>();
        ownCocktails = new List<Cocktail>();
        LoadList(cocktails, true);
        LoadList(ownCocktails, false);
    }

    private static void LoadList(List<Cocktail> list, bool standart)
    {
        XmlDocument xmlDoc = new XmlDocument();       
        string p = Path.Combine(Application.persistentDataPath, standart ? path : ownPath);

        try
        {
            xmlDoc.Load(p);
        }
        catch (Exception)
        {
            return;
        }

        XmlNodeList cocktailList = xmlDoc.GetElementsByTagName("Cocktail");
        foreach (XmlNode cocktailInfo in cocktailList)
        {
            XmlNodeList cocktailContent = cocktailInfo.ChildNodes;
            Cocktail cocktail = new Cocktail();
            foreach (XmlNode cocktailItem in cocktailContent)
            {
                if (cocktailItem.Name == "Name")
                    cocktail.cocktailName = cocktailItem.InnerText;

                if (cocktailItem.Name == "Glas")
                    cocktail.glas = cocktailItem.InnerText;

                if (cocktailItem.Name == "Preis")
                    cocktail.preis = cocktailItem.InnerText;

                if (cocktailItem.Name == "Zutaten")
                {
                    XmlNodeList zutaten = cocktailItem.ChildNodes;
                    foreach (XmlNode zutat in zutaten)
                    {
                        cocktail.zutaten.Add(zutat.InnerText);
                    }
                }
            }
            list.Add(cocktail);
        }
    }

    public static IEnumerator DownloadList()
    {
        WWW www = new WWW(url);
        yield return www;
        string p = Path.Combine(Application.persistentDataPath, path);
        File.WriteAllBytes(p, www.bytes);
        LoadCocktailsFromXML();
    }

    public static void SaveCocktailsToXML()
    {
        string p = Path.Combine(Application.persistentDataPath, ownPath);
        XmlDocument xmlDoc = new XmlDocument();

        string[] initXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Cocktails>", "</Cocktails>" };
        File.WriteAllLines(p, initXML);

        xmlDoc.Load(p);
        XmlElement elemRoot = xmlDoc.DocumentElement;

        foreach (Cocktail c in ownCocktails)
        {
            XmlElement elemNew = xmlDoc.CreateElement("Cocktail");
            XmlElement name = xmlDoc.CreateElement("Name");
            name.InnerText = c.cocktailName;
            XmlElement zutaten = xmlDoc.CreateElement("Zutaten");
            foreach (string s in c.zutaten)
            {
                XmlElement zutat = xmlDoc.CreateElement("Zutat");
                zutat.InnerText = s;
                zutaten.AppendChild(zutat);
            }
            XmlElement glas = xmlDoc.CreateElement("Glas");
            glas.InnerText = c.glas;
            XmlElement preis = xmlDoc.CreateElement("Preis");
            preis.InnerText = c.preis;

            elemNew.AppendChild(name);
            elemNew.AppendChild(zutaten);
            elemNew.AppendChild(glas);
            elemNew.AppendChild(preis);
            elemRoot.AppendChild(elemNew);
        }
        xmlDoc.Save(p);
    }
}