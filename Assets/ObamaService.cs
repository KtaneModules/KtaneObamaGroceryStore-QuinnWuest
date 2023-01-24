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

    public bool UsingDefaultList = false;

    private string _settingsFile;
    private ObamaSettings _settings;

    private static readonly string[] _authorNames = new string[6] { "Timwi", "Royal_Flu$h", "Speakingevil", "Deaf", "TasThiluna", "Blananas2" };

    private static readonly string[][] _backupAuthorModList = {
        new string[86] {"SIMON SIGNALS", "CONCENTRATION", "VORONOI MAZE", "NOT X-RAY", "VARIETY", "NOT POKER", "AQUARIUM", "NONBINARY PUZZLE", "WIRE ASSOCIATION", "MARITIME SEMAPHORE", "SIMON SHOUTS", "VOLTORB FLIP", "PUZZWORD", "ULTIMATE TIC TAC TOE", "DACH MAZE", "BLOXX", "KYUDOKU", "COLOR BRAILLE", "MYSTERY MODULE", "CORNERS", "THE ULTRACUBE", "ODD ONE OUT", "THE HYPERCUBE", "DECOLORED SQUARES", "DISCOLORED SQUARES", "SIMON SPEAKS", "HOGWARTS", "REGULAR CRAZY TALK", "BROKEN GUITAR CHORDS", "BINARY PUZZLE", "CURSED DOUBLE-OH", "SIMON SPINS", "KUDOSUDOKU", "MAHJONG", "101 DALMATIANS", "DIVIDED SQUARES", "LION’S SHARE", "TENNIS", "3D TUNNELS", "DRAGON ENERGY", "UNCOLORED SQUARES", "PATTERN CUBE", "MARITIME FLAGS", "BLACK HOLE", "LASERS", "SIMON SHRIEKS", "SIMON SENDS", "SIMON SINGS", "MARBLE TUMBLE", "DR. DOCTOR", "SUPERLOGIC", "HUMAN RESOURCES", "POLYHEDRAL MAZE", "MAFIA", "BRAILLE", "SYMBOL CYCLE", "S.E.T.", "PERPLEXING WIRES", "COLORED SWITCHES", "GRIDLOCK", "COLOR MORSE", "X-RAY", "YAHTZEE", "POINT OF ORDER", "ZOO", "THE CLOCK", "RUBIK'S CUBE", "ONLY CONNECT", "LIGHT CYCLE", "COORDINATES", "DOUBLE-OH", "WIRE PLACEMENT", "BATTLESHIP", "SIMON SCREAMS", "WORD SEARCH", "SOUVENIR", "ADJACENT LETTERS", "COLORED SQUARES", "BITMAPS", "HEXAMAZE", "ROCK-PAPER-SCISSORS-LIZARD-SPOCK", "BLIND ALLEY", "THE BULB", "FRIENDSHIP", "FOLLOW THE LEADER", "TIC TAC TOE" },
        new string[75] {"SIMON STAGES", "PRIME ENCRYPTION", "MEMORABLE BUTTONS", "WEIRD AL YANKOVIC", "SIMON'S ON FIRST", "THE MATRIX", "STAINED GLASS", "THE TROLL", "WESTEROS", "HOMOPHONES", "SIMON SQUAWKS", "FREE PARKING", "SIMON'S STAGES", "BROKEN GUITAR CHORDS", "THE HANGOVER", "SKINNY WIRES", "THE FESTIVE JUKEBOX", "SPINNING BUTTONS", "THE LABYRINTH", "STREET FIGHTER", "NEEDY MRS BOB", "HIEROGLYPHICS", "CHRISTMAS PRESENTS", "THE TRIANGLE", "MODULO", "RETIREMENT", "BLOCKBUSTERS", "CATCHPHRASE", "COUNTDOWN", "CRUEL COUNTDOWN", "THE CRYSTAL MAZE", "T-WORDS", "THE JACK-O'-LANTERN", "THE PLUNGER BUTTON", "ACCUMULATION", "SNOOKER", "COFFEEBUCKS", "THE SPHERE", "QUINTUPLES", "SONIC & KNUCKLES", "HORRIBLE MEMORY", "BENEDICT CUMBERBATCH", "WIRE SPAGHETTI", "REVERSE MORSE", "FLASHING LIGHTS", "BRITISH SLANG", "ALPHABET NUMBERS", "THE NUMBER CIPHER", "THE STOCK MARKET", "SIMON'S STAR", "LIGHTSPEED", "GUITAR CHORDS", "GRAFFITI NUMBERS", "THE JEWEL VAULT", "TAX RETURNS", "THE CUBE", "THE MOON", "THE SUN", "THE LONDON UNDERGROUND", "THE WIRE", "THE STOPWATCH", "RAPID BUTTONS", "EUROPEAN TRAVEL", "SKYRIM", "THE SWAN", "THE IPHONE", "LED GRID", "MORTAL KOMBAT", "MAINTENANCE", "IDENTITY PARADE", "THE JUKEBOX", "ALGEBRA", "SONIC THE HEDGEHOG", "POKER", "SYMBOLIC COORDINATES" },
        new string[66] {"CA-RPS", "TERMITE", "DISCOLOUR FLASH", "NOT EMOJI MATH", "NOT SYMBOLIC COORDINATES", "NOT THE BULB", "NOT WORD SEARCH", "NOT X01", "SIMPLETON'T", "UNCOLOUR FLASH", "MAZESWAPPER", "DOOMSDAY BUTTON", "MACRO MEMORY", "POLYGRID", "TURN FOUR", "CRUEL COLOUR FLASH", "NOT COLOUR FLASH", "NOT CONNECTION CHECK", "NOT COORDINATES", "NOT CRAZY TALK", "NOT MORSEMATICS", "NOT MURDER", "SIMON SUBDIVIDES", "MINESWAPPER", "EEB GNILLEPS", "CRUELLO", "RULLO", "LIGHTS ON", "RUNE MATCH I", "RUNE MATCH II", "RUNE MATCH III", "CRUEL MATCH 'EM", "ASCII MAZE", "RGB ARITHMETIC", "RGB LOGIC", "MATCH 'EM", "MULTITASK", "SILHOUETTES", "BAMBOOZLING TIME KEEPER", "14", "ULTRASTORES", "FAULTY RGB GRID", "FORGET ME LATER", "RGB MAZE", "BAMBOOZLING BUTTON GRID", "THE VERY ANNOYING BUTTON", "CRYPTIC CYCLE", "HILL CYCLE", "ULTIMATE CYCLE", "JUMBLE CYCLE", "PLAYFAIR CYCLE", "PIGPEN CYCLE", "AFFINE CYCLE", "CAESAR CYCLE", "DOUBLE ARROWS", "BAMBOOZLED AGAIN", "TALLORDERED KEYS", "DISORDERED KEYS", "RECORDED KEYS", "BORDERED KEYS", "MISORDERED KEYS", "REORDERED KEYS", "UNORDERED KEYS", "ORDERED KEYS", "BAMBOOZLING BUTTON", "SIMON STORES" },
        new string[58] {"UNO!", "GAME OF COLORS", "THE BOARD WALK", "COORDINATION", "SKEWERS", "WORDS", "RED HERRING", "INFINITE LOOP", "THE ASSORTED ARRANGEMENT", "MSSNGV WLS", "LETTER GRID", "THE 1, 2, 3 GAME", "INDENTATION", "ASTROLOGICAL", "STABLE TIME SIGNATURES", "NEXT IN LINE", "KAHOOT!", "NUMBER CHECKER", "TOWERS", "SIMON SMILES", "TELEPATHY", "HIGHER OR LOWER", "THE BIOSCANNER", "DOUBLE PITCH", "SILENCED SIMON", "CHAMBER NO. 5", "THE DIALS", "MAZERY", "CENSORSHIP", "RULES", "THE KANYE ENCOUNTER", "PIXEL ART", "CHALICES", "ULTRALOGIC", "THE CALCULATOR", "21", "RGB SEQUENCES", "ENGLISH ENTRIES", "AMNESIA", "JAILBREAK", "NEEDEEZ NUTS", "EMOTIGUY IDENTIFICATION", "BASIC MORSE", "DICTATION", "MENTAL MATH", "MORE CODE", "MODULE RICK", "SPOT THE DIFFERENCE", "CHICKEN NUGGETS", "STOCK IMAGES", "1D MAZE", "CRUEL GARFIELD KART", "THE SAMSUNG", "QUICK ARITHMETIC", "ECHOLOCATION", "THE HYPERLINK", "GARFIELD KART", "JACK ATTACK" },
        new string[50] {"PIRAGUA", "ANTISTRESS", "MELODY MEMORY", "BOOZLESNAP", "MIRROR", "PHONES", "CARTINESE", "WATCHING PAINT DRY", "LLAMA, LLAMA, ALPACA", "SMALL CIRCLE", "BLACK ARROWS", "FAULTY CHINESE COUNTING", "SUGAR SKULLS", "N&NS", "ARS GOETIA IDENTIFICATION", "DEVILISH EGGS", "M&MS", "TENPINS", "BIRTHDAYS", "DUMB WAITERS", "IPA", "SIMON STASHES", "M&NS", "ICONIC", "EXOPLANETS", "SNOWFLAKES", "THE SAMSUNG", "SHELL GAME", "WIDDERSHINS", "SYMBOLIC TASHA", "SEMAMORSE", "BOXING", "TOPSY TURVY", "BOOZLEGLYPH IDENTIFICATION", "HYPERNEEDY", "MARCO POLO", "RED LIGHT GREEN LIGHT", "ROTATING SQUARES", "CODENAMES", "POLYGONS", "SCAVENGER HUNT", "OBJECT SHOWS", "LOOPOVER", "N&MS", "CHINESE COUNTING", "ENCRYPTION BINGO", "INSANAGRAMS", "QWIRKLE", "TERRARIA QUIZ", "BRUSH STROKES" },
        new string[68] {"UNO!", "LOGIC CHESS", "LEDS", "FLYSWATTING", "INSA ILO", "THE ARENA", "CUSTOMER IDENTIFICATION", "METEOR", "SQUEEZE", "THE IMPOSTOR", "CONNECT FOUR", "LLAMA, LLAMA, ALPACA", "THE ASSORTED ARRANGEMENT", "SIMON SUPPORTS", "GHOST MOVEMENT", "OUTRAGEOUS", "TELEPATHY", "COLOR HEXAGONS", "IÑUPIAQ NUMERALS", "FRANKENSTEIN'S INDICATOR", "THE CONSOLE", "GOLF", "DUCK, DUCK, GOOSE", "SPANGLED STARS", "THE CALCULATOR", "DEAF ALLEY", "FACTORING", "INTEGER TREES", "ARROW TALK", "BOOZLETALK", "CRAZY TALK WITH A K", "JADEN SMITH TALK", "KAYMAZEY TALK", "KILO TALK", "QUOTE CRAZY TALK END QUOTE", "STANDARD CRAZY TALK", "ICONIC", "ROGER", "QUICK ARITHMETIC", "GUESS WHO?", "DIMENSION DISRUPTION", "ECHOLOCATION", "LINES OF CODE", "BUZZFIZZ", "THINKING WIRES", "SPELLING BEE", "CHEEP CHECKOUT", "THE HYPERLINK", "ÜBERMODULE", "GARFIELD KART", "TIMING IS EVERYTHING", "COMMON SENSE", "BONE APPLE TEA", "WEIRD AL YANKOVIC", "MATCHEMATICS", "JACK ATTACK", "FLOWER PATCH", "BLOCK STACKS", "COLO(U)R TALK", "SNAKES AND LADDERS", "INSANAGRAMS", "BOOT TOO BIG", "SEVEN DEADLY SINS", "BRUSH STROKES", "MODULE MAZE", "DOMINOES", "KNOW YOUR WAY", "USA MAZE" }
    };

    void Awake()
    {

        name = "Obama Service";

        _settingsFile = Path.Combine(Path.Combine(Application.persistentDataPath, "Modsettings"), "ObamaSettings.json");

        Debug.Log(_settingsFile);

        if (!File.Exists(_settingsFile))
        {
            _settings = new ObamaSettings();
            UsingDefaultList = true;
        }
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

            UsingDefaultList = false;

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