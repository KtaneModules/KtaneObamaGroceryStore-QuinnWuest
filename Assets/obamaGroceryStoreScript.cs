using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using KModkit;
using System.Collections;

public class obamaGroceryStoreScript : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMGameInfo Game;

    public KMSelectable[] obamaBodyButtons;
    public KMSelectable[] weaponButtons, sidekickButtons, foodButtons;
    public KMSelectable displayButton;
    public GameObject mainObject, weaponObject, sidekickObject, foodObject;
    public SpriteRenderer obamaRenderer;
    public MeshRenderer[] weaponBorders, sidekickBorders, foodBorders;
    public TextMesh displayText;
    public Color darkColor;

    static int moduleIdCounter = 1;
    int moduleId;
    bool solved;
    
    string lastSolved = "OBAMA GROCERY STORE";
    List<string> currentSolves;

    private readonly List<KMSelectable[]> TPButtons = new List<KMSelectable[]>();
    int currentlyShown; // 0 = main screen, 1 = weapon, 2 = sidekick, 3 = food
    static readonly string[] btnNames = { "Obomba", "Obamace", "Obamachete", "Stilettobama", "Torpedobama", "Ammobama", "Joe Biden", "Donald Trump", "Michelle Obama", "Rob-ama", "Oba-mary", "O-bob-a", "Obamango", "Obamacaroni", "Obeana", "Kabobama", "Avocadobama" };
    static readonly string[] stageNames = { "weapon", "sidekick", "food" };
    readonly int[] selectedBtns = { 99, 99, 99 };
    readonly int[] correctBtns = { 0, 0, 0 };

    static readonly string[][] commonWords = {
        new string[3] { "MAZE", "NUMBER", "BEAN" },
        new string[3] { "SIMON", "WIRE", "IDENTIFICATION" },
        new string[4] { "COLOR", "COLOUR", "CRUEL", "TIME" },
        new string[4] { "CIPHER", "FORGET", "BOOZLE", "BAMBOOZLING" },
        new string[3] { "KEY", "MORSE", "TALK" },
        new string[4] { "BUTTON", "AND", "&", "WORD" },
    };
    static readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    static readonly string[][] authorMods = {
        new string[86] {"SIMON SIGNALS", "CONCENTRATION", "VORONOI MAZE", "NOT X-RAY", "VARIETY", "NOT POKER", "AQUARIUM", "NONBINARY PUZZLE", "WIRE ASSOCIATION", "MARITIME SEMAPHORE", "SIMON SHOUTS", "VOLTORB FLIP", "PUZZWORD", "ULTIMATE TIC TAC TOE", "DACH MAZE", "BLOXX", "KYUDOKU", "COLOR BRAILLE", "MYSTERY MODULE", "CORNERS", "THE ULTRACUBE", "ODD ONE OUT", "THE HYPERCUBE", "DECOLORED SQUARES", "DISCOLORED SQUARES", "SIMON SPEAKS", "HOGWARTS", "REGULAR CRAZY TALK", "BROKEN GUITAR CHORDS", "BINARY PUZZLE", "CURSED DOUBLE-OH", "SIMON SPINS", "KUDOSUDOKU", "MAHJONG", "101 DALMATIANS", "DIVIDED SQUARES", "LION’S SHARE", "TENNIS", "3D TUNNELS", "DRAGON ENERGY", "UNCOLORED SQUARES", "PATTERN CUBE", "MARITIME FLAGS", "BLACK HOLE", "LASERS", "SIMON SHRIEKS", "SIMON SENDS", "SIMON SINGS", "MARBLE TUMBLE", "DR. DOCTOR", "SUPERLOGIC", "HUMAN RESOURCES", "POLYHEDRAL MAZE", "MAFIA", "BRAILLE", "SYMBOL CYCLE", "S.E.T.", "PERPLEXING WIRES", "COLORED SWITCHES", "GRIDLOCK", "COLOR MORSE", "X-RAY", "YAHTZEE", "POINT OF ORDER", "ZOO", "THE CLOCK", "RUBIK'S CUBE", "ONLY CONNECT", "LIGHT CYCLE", "COORDINATES", "DOUBLE-OH", "WIRE PLACEMENT", "BATTLESHIP", "SIMON SCREAMS", "WORD SEARCH", "SOUVENIR", "ADJACENT LETTERS", "COLORED SQUARES", "BITMAPS", "HEXAMAZE", "ROCK-PAPER-SCISSORS-LIZARD-SPOCK", "BLIND ALLEY", "THE BULB", "FRIENDSHIP", "FOLLOW THE LEADER", "TIC TAC TOE" },
        new string[75] {"SIMON STAGES", "PRIME ENCRYPTION", "MEMORABLE BUTTONS", "WEIRD AL YANKOVIC", "SIMON'S ON FIRST", "THE MATRIX", "STAINED GLASS", "THE TROLL", "WESTEROS", "HOMOPHONES", "SIMON SQUAWKS", "FREE PARKING", "SIMON'S STAGES", "BROKEN GUITAR CHORDS", "THE HANGOVER", "SKINNY WIRES", "THE FESTIVE JUKEBOX", "SPINNING BUTTONS", "THE LABYRINTH", "STREET FIGHTER", "NEEDY MRS BOB", "HIEROGLYPHICS", "CHRISTMAS PRESENTS", "THE TRIANGLE", "MODULO", "RETIREMENT", "BLOCKBUSTERS", "CATCHPHRASE", "COUNTDOWN", "CRUEL COUNTDOWN", "THE CRYSTAL MAZE", "T-WORDS", "THE JACK-O'-LANTERN", "THE PLUNGER BUTTON", "ACCUMULATION", "SNOOKER", "COFFEEBUCKS", "THE SPHERE", "QUINTUPLES", "SONIC & KNUCKLES", "HORRIBLE MEMORY", "BENEDICT CUMBERBATCH", "WIRE SPAGHETTI", "REVERSE MORSE", "FLASHING LIGHTS", "BRITISH SLANG", "ALPHABET NUMBERS", "THE NUMBER CIPHER", "THE STOCK MARKET", "SIMON'S STAR", "LIGHTSPEED", "GUITAR CHORDS", "GRAFFITI NUMBERS", "THE JEWEL VAULT", "TAX RETURNS", "THE CUBE", "THE MOON", "THE SUN", "THE LONDON UNDERGROUND", "THE WIRE", "THE STOPWATCH", "RAPID BUTTONS", "EUROPEAN TRAVEL", "SKYRIM", "THE SWAN", "THE IPHONE", "LED GRID", "MORTAL KOMBAT", "MAINTENANCE", "IDENTITY PARADE", "THE JUKEBOX", "ALGEBRA", "SONIC THE HEDGEHOG", "POKER", "SYMBOLIC COORDINATES" },
        new string[66] {"CA-RPS", "TERMITE", "DISCOLOUR FLASH", "NOT EMOJI MATH", "NOT SYMBOLIC COORDINATES", "NOT THE BULB", "NOT WORD SEARCH", "NOT X01", "SIMPLETON'T", "UNCOLOUR FLASH", "MAZESWAPPER", "DOOMSDAY BUTTON", "MACRO MEMORY", "POLYGRID", "TURN FOUR", "CRUEL COLOUR FLASH", "NOT COLOUR FLASH", "NOT CONNECTION CHECK", "NOT COORDINATES", "NOT CRAZY TALK", "NOT MORSEMATICS", "NOT MURDER", "SIMON SUBDIVIDES", "MINESWAPPER", "EEB GNILLEPS", "CRUELLO", "RULLO", "LIGHTS ON", "RUNE MATCH I", "RUNE MATCH II", "RUNE MATCH III", "CRUEL MATCH 'EM", "ASCII MAZE", "RGB ARITHMETIC", "RGB LOGIC", "MATCH 'EM", "MULTITASK", "SILHOUETTES", "BAMBOOZLING TIME KEEPER", "14", "ULTRASTORES", "FAULTY RGB GRID", "FORGET ME LATER", "RGB MAZE", "BAMBOOZLING BUTTON GRID", "THE VERY ANNOYING BUTTON", "CRYPTIC CYCLE", "HILL CYCLE", "ULTIMATE CYCLE", "JUMBLE CYCLE", "PLAYFAIR CYCLE", "PIGPEN CYCLE", "AFFINE CYCLE", "CAESAR CYCLE", "DOUBLE ARROWS", "BAMBOOZLED AGAIN", "TALLORDERED KEYS", "DISORDERED KEYS", "RECORDED KEYS", "BORDERED KEYS", "MISORDERED KEYS", "REORDERED KEYS", "UNORDERED KEYS", "ORDERED KEYS", "BAMBOOZLING BUTTON", "SIMON STORES" },
        new string[58] {"UNO!", "GAME OF COLORS", "THE BOARD WALK", "COORDINATION", "SKEWERS", "WORDS", "RED HERRING", "INFINITE LOOP", "THE ASSORTED ARRANGEMENT", "MSSNGV WLS", "LETTER GRID", "THE 1, 2, 3 GAME", "INDENTATION", "ASTROLOGICAL", "STABLE TIME SIGNATURES", "NEXT IN LINE", "KAHOOT!", "NUMBER CHECKER", "TOWERS", "SIMON SMILES", "TELEPATHY", "HIGHER OR LOWER", "THE BIOSCANNER", "DOUBLE PITCH", "SILENCED SIMON", "CHAMBER NO. 5", "THE DIALS", "MAZERY", "CENSORSHIP", "RULES", "THE KANYE ENCOUNTER", "PIXEL ART", "CHALICES", "ULTRALOGIC", "THE CALCULATOR", "21", "DEAF ALLEY", "RGB SEQUENCES", "ENGLISH ENTRIES", "AMNESIA", "JAILBREAK", "NEEDEEZ NUTS", "EMOTIGUY IDENTIFICATION", "BASIC MORSE", "DICTATION", "MENTAL MATH", "MORE CODE", "MODULE RICK", "SPOT THE DIFFERENCE", "CHICKEN NUGGETS", "STOCK IMAGES", "1D MAZE", "CRUEL GARFIELD KART", "THE SAMSUNG", "QUICK ARITHMETIC", "ECHOLOCATION", "THE HYPERLINK", "GARFIELD KART", "JACK ATTACK" },
        new string[50] {"PIRAGUA", "ANTISTRESS", "MELODY MEMORY", "BOOZLESNAP", "MIRROR", "PHONES", "CARTINESE", "WATCHING PAINT DRY", "LLAMA, LLAMA, ALPACA", "SMALL CIRCLE", "BLACK ARROWS", "FAULTY CHINESE COUNTING", "SUGAR SKULLS", "N&NS", "ARS GOETIA IDENTIFICATION", "DEVILISH EGGS", "M&MS", "TENPINS", "BIRTHDAYS", "DUMB WAITERS", "IPA", "SIMON STASHES", "M&NS", "ICONIC", "EXOPLANETS", "SNOWFLAKES", "THE SAMSUNG", "SHELL GAME", "WIDDERSHINS", "SYMBOLIC TASHA", "SEMAMORSE", "BOXING", "TOPSY TURVY", "BOOZLEGLYPH IDENTIFICATION", "HYPERNEEDY", "MARCO POLO", "RED LIGHT GREEN LIGHT", "ROTATING SQUARES", "CODENAMES", "POLYGONS", "SCAVENGER HUNT", "OBJECT SHOWS", "LOOPOVER", "N&MS", "CHINESE COUNTING", "ENCRYPTION BINGO", "INSANAGRAMS", "QWIRKLE", "TERRARIA QUIZ", "BRUSH STROKES" },
        new string[68] {"UNO!", "LOGIC CHESS", "LEDS", "FLYSWATTING", "INSA ILO", "THE ARENA", "CUSTOMER IDENTIFICATION", "METEOR", "SQUEEZE", "THE IMPOSTOR", "CONNECT FOUR", "LLAMA, LLAMA, ALPACA", "THE ASSORTED ARRANGEMENT", "SIMON SUPPORTS", "GHOST MOVEMENT", "OUTRAGEOUS", "TELEPATHY", "COLOR HEXAGONS", "IÑUPIAQ NUMERALS", "FRANKENSTEIN'S INDICATOR", "THE CONSOLE", "GOLF", "DUCK, DUCK, GOOSE", "SPANGLED STARS", "THE CALCULATOR", "DEAF ALLEY", "FACTORING", "INTEGER TREES", "ARROW TALK", "BOOZLETALK", "CRAZY TALK WITH A K", "JADEN SMITH TALK", "KAYMAZEY TALK", "KILO TALK", "QUOTE CRAZY TALK END QUOTE", "STANDARD CRAZY TALK", "ICONIC", "ROGER", "QUICK ARITHMETIC", "GUESS WHO?", "DIMENSION DISRUPTION", "ECHOLOCATION", "LINES OF CODE", "BUZZFIZZ", "THINKING WIRES", "SPELLING BEE", "CHEEP CHECKOUT", "THE HYPERLINK", "ÜBERMODULE", "GARFIELD KART", "TIMING IS EVERYTHING", "COMMON SENSE", "BONE APPLE TEA", "WEIRD AL YANKOVIC", "MATCHEMATICS", "JACK ATTACK", "FLOWER PATCH", "BLOCK STACKS", "COLO(U)R TALK", "SNAKES AND LADDERS", "INSANAGRAMS", "BOOT TOO BIG", "SEVEN DEADLY SINS", "BRUSH STROKES", "MODULE MAZE", "DOMINOES", "KNOW YOUR WAY", "USA MAZE" }
    };
    static readonly string[] commonLetters = { "ACJRW", "EGLXY", "FIPTZ", "DMOSV", "BHKNQU" };

    static readonly float[] punchLengths = { .4f, .251f, .157f };
    bool animationPlaying = false;
    bool lightsOn = false;

    private void Awake()
    {
        moduleId = moduleIdCounter++;
        displayButton.OnInteract += delegate () { if (!animationPlaying) { PressDisplay(); } displayButton.AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Module.transform); return false; };
        for (int i = 0; i < 6; i++)
        {
            int j = i;
            weaponButtons[i].OnInteract += delegate { PressWeapon(j); weaponButtons[j].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); return false; };
            weaponButtons[i].OnHighlight += delegate { HighlightOption(j, true); };
            weaponButtons[i].OnHighlightEnded += delegate { HighlightOption(j, false); };

            sidekickButtons[i].OnInteract += delegate { PressSidekick(j); sidekickButtons[j].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); return false; };
            sidekickButtons[i].OnHighlight += delegate { HighlightOption(6+j, true); };
            sidekickButtons[i].OnHighlightEnded += delegate { HighlightOption(6+j, false); };

            if (i != 5)
            {
                foodButtons[i].OnInteract += delegate { PressFood(j); foodButtons[j].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); return false; };
                foodButtons[i].OnHighlight += delegate { HighlightOption(12+j, true); };
                foodButtons[i].OnHighlightEnded += delegate { HighlightOption(12+j, false); };
            }
        }
        for (int i = 0; i < 3; i++)
        {
            int j = i;
            obamaBodyButtons[i].OnInteract += delegate { if (!animationPlaying) { SelectPart(j); } obamaBodyButtons[j].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); return false; };
        }

        currentSolves = Bomb.GetSolvedModuleNames();
        mainObject.SetActive(true);

        Game.OnLightsChange += ToggleObama;
        for (var i = 0; i < 3; i++)
            TPButtons.Add(i == 0 ? weaponButtons : (i == 1 ? sidekickButtons : foodButtons));
    }
    
    private void Update()
    {
        if (solved) { return; }
        if (currentSolves.Count != Bomb.GetSolvedModuleNames().Count)
        {
            lastSolved = getLatestSolve(Bomb.GetSolvedModuleNames(), currentSolves).ToUpperInvariant();
            // DebugMsg("Last solved mod is now " + lastSolved + "...");
        }
    }

    private void Start()
    {
        Audio.PlaySoundAtTransform("letmebeclear", Module.transform);
        DebugMsg("Do not fear, for Obama is here!");
        displayText.text = "Submit";

        weaponObject.SetActive(false);
        sidekickObject.SetActive(false);
        foodObject.SetActive(false);
    }

    string getLatestSolve(List<string> s, List<string> s2)
    {
        string name = "";
        for (int i = 0; i < s2.Count; i++)
        {
            s.Remove(s2.ElementAt(i));
        }
        for (int i = 0; i < s.Count; i++)
        {
            currentSolves.Add(s.ElementAt(i));
        }
        name = s.ElementAt(0);
        return name;
    }

    void PressDisplay()
    {
        if (currentlyShown == 0)
        {
            DebugMsg("The last solved module was " + lastSolved + ".");
            // Weapon check
            bool[] rows = { false, false, false, false, false, false };
            for (int i = 0; i < 6; i++)
                foreach (var word in commonWords[i])
                    if (lastSolved.Contains(word)) { rows[i] = true; break; }

            if (!rows.Any(x => x)) // no rows in common
            {
                int sum = 0;
                char[] lastSolvedLetters = lastSolved.Where(x => alphabet.Contains(x)).ToArray();
                for (int i = 0; i < lastSolvedLetters.Length / 5; i++)
                    sum += Array.IndexOf(alphabet, lastSolvedLetters[i * 5 + Bomb.GetBatteryCount() % 5]) + 1;
                correctBtns[0] = sum % 6;
            }
            else if (rows.Count(x => x) == 1) // one row in common
                correctBtns[0] = Array.IndexOf(rows, true);
            else
                for (int i = 0; i < 6; i++)
                    if (rows[i]) { correctBtns[0] = i; break; }

            // Sidekick check
            rows = new[] { false, false, false, false, false, false };
            for (int i = 0; i < 6; i++)
                foreach (var mod in authorMods[i])
                    if (lastSolved == mod) { rows[i] = true; break; }

            if (!rows.Any(x => x)) // no rows in common
            {
                int sum = 0;
                char[] lastSolvedLetters = lastSolved.Where(x => alphabet.Contains(x)).ToArray();
                List<char> allLetters = Bomb.GetSerialNumberLetters().ToList();
                foreach (var indicator in Bomb.GetIndicators())
                {
                    allLetters.Add(indicator.ElementAt(0));
                    allLetters.Add(indicator.ElementAt(1));
                    allLetters.Add(indicator.ElementAt(2));
                }

                foreach (var letter in lastSolvedLetters)
                    if (allLetters.Contains(letter))
                        sum++;
                correctBtns[1] = sum % 6;
            }
            else if (rows.Count(x => x) == 1) // one row in common
                correctBtns[1] = Array.IndexOf(rows, true);
            else
                for (int i = 5; i >= 0; i--)
                    if (rows[i]) { correctBtns[1] = i; break; }

            // Food check
            
            rows = new[] { false, false, false, false, false };
            for (int i = 0; i < 5; i++)
                if (!commonLetters[i].Any(x => lastSolved.Contains(x)))
                    rows[i] = true;
            
            if (!rows.Any(x => x)) // no rows in common
            {
                int sum = 0;
                int vowelCount = 0;
                char[] vowels = "AEIOU".ToCharArray();
                int firstHalfCount = 0;
                char[] firstHalfabet = "ABCDEFGHIJKLM".ToCharArray();

                char[] lastSolvedLetters = lastSolved.Where(x => alphabet.Contains(x)).ToArray();
                foreach (var letter in lastSolvedLetters)
                {
                    if (vowels.Contains(letter))
                        vowelCount++;
                    if (firstHalfabet.Contains(letter))
                        firstHalfCount++;
                }

                if (Bomb.GetPortCount() == 0)
                    sum += lastSolvedLetters.Length;
                if (Bomb.IsPortPresent(Port.Serial))
                    sum += lastSolvedLetters.Length - vowelCount;
                if (Bomb.IsPortPresent(Port.Parallel))
                    sum += vowelCount;
                if (Bomb.IsPortPresent(Port.RJ45) || Bomb.IsPortPresent(Port.PS2))
                    sum += firstHalfCount;
                if (Bomb.IsPortPresent(Port.DVI) || Bomb.IsPortPresent(Port.StereoRCA))
                    sum += lastSolvedLetters.Length - firstHalfCount;
                
                correctBtns[2] = sum % 5;
            }
            else if (rows.Count(x => x) == 1) // one row in common
                correctBtns[2] = Array.IndexOf(rows, true);
            else
                for (int i = 0; i < 5; i++)
                    if (commonLetters[i].Contains(Bomb.GetSerialNumberLetters().ElementAt(1))) { correctBtns[2] = i; break; }

            // Check if submission is right

            DebugMsg("The intended solution was " + btnNames[correctBtns[0]] + ", " + btnNames[6 + correctBtns[1]] + ", and " + btnNames[12 + correctBtns[2]] + ".");
            StartCoroutine(Animation(correctBtns[0] == selectedBtns[0] && correctBtns[1] == selectedBtns[1] && correctBtns[2] == selectedBtns[2]));
        }
        else
        {
            weaponObject.SetActive(false);
            sidekickObject.SetActive(false);
            foodObject.SetActive(false);
            mainObject.SetActive(true);
            if (lightsOn)
                obamaRenderer.color = Color.white;
            currentlyShown = 0;
            displayText.text = "Submit";
        }
    }

    void PressWeapon(int btn)
    {
        if (selectedBtns[0] != 99)
            weaponBorders[selectedBtns[0]].material.color = Color.black;
        selectedBtns[0] = btn;
        weaponBorders[btn].material.color = Color.green;
    }

    void PressSidekick(int btn)
    {
        if (selectedBtns[1] != 99)
            sidekickBorders[selectedBtns[1]].material.color = Color.black;
        selectedBtns[1] = btn;
        sidekickBorders[btn].material.color = Color.green;
    }

    void PressFood(int btn)
    {
        if (selectedBtns[2] != 99)
            foodBorders[selectedBtns[2]].material.color = Color.black;
        selectedBtns[2] = btn;
        foodBorders[btn].material.color = Color.green;
    }

    void SelectPart(int btn)
    {
        currentlyShown = btn + 1;
        obamaRenderer.color = darkColor;
        switch (btn)
        {
            case 0:
                weaponObject.SetActive(true);
                sidekickObject.SetActive(false);
                foodObject.SetActive(false);
                mainObject.SetActive(false);
                break;
            case 1:
                weaponObject.SetActive(false);
                sidekickObject.SetActive(true);
                foodObject.SetActive(false);
                mainObject.SetActive(false);
                break;
            case 2:
                weaponObject.SetActive(false);
                sidekickObject.SetActive(false);
                foodObject.SetActive(true);
                mainObject.SetActive(false);
                break;
        }
        displayText.text = "Go Back";
    }

    void HighlightOption(int btn, bool highlighting)
    {
        displayText.text = highlighting ? btnNames[btn] : "Go Back";
    }

    void ToggleObama(bool state)
    {
        lightsOn = state;
        obamaRenderer.color = state ? Color.white : darkColor;
    }

    IEnumerator Animation(bool correct)
    {
        mainObject.SetActive(false);
        animationPlaying = true;
        Audio.PlaySoundAtTransform("BARACKOBAMA", Module.transform);
        displayText.text = "FIGHT!";

        yield return new WaitForSeconds(3.5f);
        for (int i = 0; i < Random.Range(10, 20); i++)
        {
            int placeholder = Random.Range(0, 3);
            Audio.PlaySoundAtTransform("punch" + (placeholder+1), Module.transform);
            obamaBodyButtons[Random.Range(0, 3)].AddInteractionPunch((3 - placeholder) * 3 + Random.Range(0, 3));
            obamaRenderer.flipX = i % 2 == 0;
            yield return new WaitForSeconds(punchLengths[placeholder]);
        }

        obamaRenderer.flipX = false;

        if (correct)
        {
            Audio.PlaySoundAtTransform("applause", Module.transform);
            if (Random.Range(0, 20) == 0)
                Audio.PlaySoundAtTransform("bigchungus", Module.transform);
            else
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Module.transform);
            Module.HandlePass();
            solved = true;
            DebugMsg("Module solved!");
            obamaRenderer.color = Color.green;
            yield return new WaitForSeconds(1f);
            while (!lightsOn)
            {
                obamaRenderer.color = darkColor;
                yield return new WaitForSeconds(.1f);
            }
            obamaRenderer.color = Color.white;
            displayText.text = "GOD BLESS AMERICA";
        }
        else
        {
            Audio.PlaySoundAtTransform("awww", Module.transform);
            Module.HandleStrike();
            displayText.text = lastSolved;

            DebugMsg("Strike!");

            for (int i = 0; i < 3; i++)
            {
                if (correctBtns[i] != selectedBtns[i])
                {
                    if (selectedBtns[i] == 99)
                        DebugMsg("You didn't select a " + stageNames[i] + "!");
                    else
                        DebugMsg("You selected the " + btnNames[6 * i + selectedBtns[i]] + " when you were supposed to select the " + btnNames[6 * i + correctBtns[i]] + "!");
                }
            }

            obamaRenderer.color = Color.red;
            yield return new WaitForSeconds(1f);
            while (!lightsOn)
            {
                obamaRenderer.color = darkColor;
                yield return new WaitForSeconds(.1f);
            }
            obamaRenderer.color = Color.white;
            animationPlaying = false;
        }
        mainObject.SetActive(true);
    }

    void DebugMsg(string msg)
    {
        Debug.LogFormat("[Obama Grocery Store #{0}] {1}", moduleId, msg);
    }

    private const string TwitchHelpMessage =
        "Browse items using '!{0} browse <weapons, sidekicks, foods>'. Select an item using '!{0} select <item>'. Submit your answer using '!{0} submit'.";

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();

        if (command == "submit")
        {
            if (currentlyShown != 0)
            {
                displayButton.OnInteract();
                yield return new WaitForSeconds(.1f);
            }

            yield return null;
            displayButton.OnInteract();
            yield break;
        }
        if (command == "back")
        {
            if (currentlyShown == 0)
                yield break;
            yield return null;
            displayButton.OnInteract();
            yield break;
        }

        var parsedCommand = command.Split(' ');
        switch (parsedCommand[0])
        {
            case "browse":
                if (parsedCommand.Length != 2 || !new[] {"weapon", "sidekick", "food", "weapons", "sidekicks", "foods"}.Contains(parsedCommand[1]))
                    yield break;
                yield return null;
                if (currentlyShown != 0)
                {
                    displayButton.OnInteract();
                    yield return new WaitForSeconds(.1f);
                }
                obamaBodyButtons[new[] {"weapon", "sidekick", "food", "weapons", "sidekicks", "foods"}.ToList().IndexOf(parsedCommand[1]) % 3].OnInteract();
                break;
            case "select":
                if (currentlyShown == 0)
                    yield break;
                var selected = parsedCommand.Skip(1).Join();
                if (!btnNames.Select(x => x.ToLowerInvariant()).Skip(6 * (currentlyShown - 1)).Take(6).Contains(selected))
                    yield break;
                yield return null;
                TPButtons[currentlyShown - 1][btnNames.Select(x => x.ToLowerInvariant()).Skip(6 * (currentlyShown - 1)).Take(6).ToList().IndexOf(selected)]
                    .OnInteract();
                break;
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        if (currentlyShown != 0)
            displayButton.OnInteract();
        StartCoroutine(Animation(true));
        while (!solved)
            yield return true;
    }
}
