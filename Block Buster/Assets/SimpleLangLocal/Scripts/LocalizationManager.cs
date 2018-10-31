using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance = null;

    public string[] tags;

    public TextAsset languageFile;

    private string lang;

    private string Lang
    {
        get
        {
            return lang;
        }

        set
        {
            PlayerPrefs.SetString("SLanguageL", value);
            lang = value;
        }
    }

    public string GetLang()
    {
        return lang;
    }

    public void SetLang(string lan)
    {
        PlayerPrefs.SetString("SLanguageL", lan);
        lang = PlayerPrefs.GetString("SLanguageL");

    }

    private Dictionary<string, Dictionary<string, string>> languages;

    private XmlDocument xmlDoc = new XmlDocument();
    private XmlReader reader;



    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this.GetComponent<LocalizationManager>();

        if (!PlayerPrefs.HasKey("SLanguageL"))
        {
            Lang = tags[0];
        }
        else
        {
            Lang = PlayerPrefs.GetString("SLanguageL");
        }

        languages = new Dictionary<string, Dictionary<string, string>>();
        reader = XmlReader.Create(new StringReader(languageFile.text));
        xmlDoc.Load(reader);

        for (int i = 0; i < tags.Length; i++)
        {
            languages.Add(tags[i], new Dictionary<string, string>());
            XmlNodeList langs = xmlDoc["Data"].GetElementsByTagName(tags[i]);
            for (int j = 0; j < langs.Count; j++)
            {
                languages[tags[i]].Add(langs[j].Attributes["Key"].Value, langs[j].Attributes["Word"].Value);
            }
        }
    }

    

    public string GetWord(string lan, string key)
    {
        return languages[lan][key];
    }

    public string GetWord(string key)
    {
        return languages[lang][key];
    }
}
