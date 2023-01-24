using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ObamaService : MonoBehaviour
{
    public bool SettingsLoaded = false;

    private string _settingsFile;
    private ObamaSettings _settings;

    private static readonly string[] _authorNames = new string[6] { "Timwi", "Royal_Flu$h", "Speakingevil", "Deaf", "TasThiluna", "Blananas2" };


    void Start()
    {
        name = "Obama Service";

        _settingsFile = Path.Combine(Path.Combine(Application.persistentDataPath, "Modsettings"), "ObamaSettings.json");

        if (!File.Exists(_settingsFile))
            _settings = new ObamaSettings();
        else
        {
            try
            {
                _settings = JsonConvert.DeserializeObject<ObamaSettings>(File.ReadAllText(_settingsFile), new StringEnumConverter());
                if (_settings == null)
                    throw new Exception("Settings could not be read. Creating new Settings...");
                SettingsLoaded = true;
                Debug.LogFormat(@"[Obama Service] Settings successfully loaded");
            }
            catch (Exception e)
            {
                Debug.LogFormat(@"[Obama Service] Error loading settings file:");
                Debug.LogException(e);
                _settings = new ObamaSettings();
            }
        }

        Debug.LogFormat(@"[Obama Service] Service is active");
        if (_settings.AutomaticUpdate || _settings.Version != 1)
            StartCoroutine(GetData());
        else
            Debug.LogFormat(@"[Obama Service] Automatic Update is disabled!");
    }

    IEnumerator GetData()
    {
        using (var http = UnityWebRequest.Get(_settings.SiteUrl))
        {
            // Request and wait for the desired page.
            yield return http.SendWebRequest();

            if (http.isNetworkError)
            {
                Debug.LogFormat(@"[Obama Service] Website {0} responded with error: {1}", _settings.SiteUrl, http.error);
                yield break;
            }

            if (http.responseCode != 200)
            {
                Debug.LogFormat(@"[Obama Service] Website {0} responded with code: {1}", _settings.SiteUrl, http.responseCode);
                yield break;
            }

            var allModules = JObject.Parse(http.downloadHandler.text)["KtaneModules"] as JArray;
            if (allModules == null)
            {
                Debug.LogFormat(@"[Obama Service] Website {0} did not respond with a JSON array at “KtaneModules” key.", _settings.SiteUrl, http.responseCode);
                yield break;
            }

            var allRelevantMods = Enumerable.Range(0, _authorNames.Length).Select(_ => new List<string>()).ToArray();

            foreach (JObject module in allModules)
            {
                var name = module["Name"] as JValue;
                if (name == null || !(name.Value is string))
                    continue;
                var contrib = module["Author"] as JValue;
                if (contrib == null || !(contrib.Value is string))
                    continue;
                foreach (var c in ((string)contrib.Value).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var pos = Array.IndexOf(_authorNames, c);
                    if (pos != -1)
                        allRelevantMods[pos].Add((string)name.Value);
                }
            }

            Debug.LogFormat(@"[Obama Service] List successfully loaded:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, allRelevantMods.Select((modules, ix) => string.Format("[Obama Service] {0} => {1}", _authorNames[ix], modules.Join(", "))).ToArray()));
            _settings.AuthorMods = allRelevantMods.Select(i => i.ToArray()).ToArray();
            _settings.Version = 1;
            SettingsLoaded = true;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(_settingsFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(_settingsFile));
                File.WriteAllText(_settingsFile, JsonConvert.SerializeObject(_settings, Formatting.Indented, new StringEnumConverter()));
            }
            catch (Exception e)
            {
                Debug.LogFormat("[Obama Service] Failed to save settings file:");
                Debug.LogException(e);
            }
        }
    }

    public IEnumerable<int> ModAuthorValue(string modName)
    {
        for (int i = 0; i < _settings.AuthorMods.Length; i++)
            if (_settings.AuthorMods[i].Any(x => x.EqualsIgnoreCase(modName)))
                yield return i;
    }
}