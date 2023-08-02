using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;
using Random = UnityEngine.Random;

public class obamaGroceryStoreScript : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    public KMSelectable[] obamaBodyButtons;
    public KMSelectable[] weaponButtons, sidekickButtons, foodButtons;
    public KMSelectable displayButton;
    public GameObject mainObject, weaponObject, sidekickObject, foodObject, warningObject;
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

    static readonly string[] commonLetters = { "ACJRW", "EGLXY", "FIPTZ", "DMOSV", "BHKNQU" };

    static readonly float[] punchLengths = { .4f, .251f, .157f };
    bool animationPlaying = false;

    ObamaService obamaService;

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
            sidekickButtons[i].OnHighlight += delegate { HighlightOption(6 + j, true); };
            sidekickButtons[i].OnHighlightEnded += delegate { HighlightOption(6 + j, false); };

            if (i != 5)
            {
                foodButtons[i].OnInteract += delegate { PressFood(j); foodButtons[j].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); return false; };
                foodButtons[i].OnHighlight += delegate { HighlightOption(12 + j, true); };
                foodButtons[i].OnHighlightEnded += delegate { HighlightOption(12 + j, false); };
            }
        }
        for (int i = 0; i < 3; i++)
        {
            int j = i;
            obamaBodyButtons[i].OnInteract += delegate { if (!animationPlaying) { SelectPart(j); } obamaBodyButtons[j].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); return false; };
        }

        currentSolves = Bomb.GetSolvedModuleNames();
        mainObject.SetActive(true);
        for (var i = 0; i < 3; i++)
            TPButtons.Add(i == 0 ? weaponButtons : (i == 1 ? sidekickButtons : foodButtons));
    }

    private void Update()
    {
        if (solved) { return; }
        var solvedModules = Bomb.GetSolvedModuleNames();
        if (currentSolves.Count != solvedModules.Count)
            lastSolved = getLatestSolve(solvedModules, currentSolves).ToUpperInvariant();
    }

    private void Start()
    {
        Audio.PlaySoundAtTransform("letmebeclear", Module.transform);
        DebugMsg("Do not fear, for Obama is here!");
        displayText.text = "Submit";

        weaponObject.SetActive(false);
        sidekickObject.SetActive(false);
        foodObject.SetActive(false);

        ToggleObama(true);
        obamaService = FindObjectOfType<ObamaService>();
        if (obamaService == null)
        {
            Debug.LogFormat(@"[Obama Grocery Store #{0}] Catastrophic problem: Obama Service is not present.", moduleId);
            Module.HandlePass();
            solved = true;
            return;
        }
        if (obamaService.UsingDefaultList)
        {
            warningObject.SetActive(true);
            Debug.LogFormat(@"[Obama Grocery Store #{0}] Unable to download list of modules. Using default list.", moduleId);
        }
        else
        {
            warningObject.SetActive(false);
            Debug.LogFormat(@"<Obama Grocery Store #{0}> Successfully downloaded list of modules.", moduleId);
        }
    }

    string getLatestSolve(List<string> solvedModules, List<string> currentSolves)
    {
        for (int i = 0; i < currentSolves.Count; i++)
        {
            solvedModules.Remove(currentSolves.ElementAt(i));
        }
        for (int i = 0; i < solvedModules.Count; i++)
        {
            this.currentSolves.Add(solvedModules.ElementAt(i));
        }
        return solvedModules.ElementAt(0);
    }

    public IEnumerable<int> GetModAuthorValue(string modName)
    {
        var authorMods = obamaService.GetAuthorMods();
        for (int i = 0; i < authorMods.Length; i++)
            if (authorMods[i].Any(x => x.EqualsIgnoreCase(modName)))
                yield return i;
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

            var authors = GetModAuthorValue(lastSolved);
            DebugMsg(string.Format("Authors are: [{0}]", authors.Select(i => ObamaService.AuthorNames[i]).Join(", ")));
            foreach (var author in authors)
                rows[author] = true;

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
            Audio.PlaySoundAtTransform("punch" + (placeholder + 1), Module.transform);
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
            ToggleObama(true);
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
                if (parsedCommand.Length != 2 || !new[] { "weapon", "sidekick", "food", "weapons", "sidekicks", "foods" }.Contains(parsedCommand[1]))
                    yield break;
                yield return null;
                if (currentlyShown != 0)
                {
                    displayButton.OnInteract();
                    yield return new WaitForSeconds(.1f);
                }
                obamaBodyButtons[new[] { "weapon", "sidekick", "food", "weapons", "sidekicks", "foods" }.ToList().IndexOf(parsedCommand[1]) % 3].OnInteract();
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
