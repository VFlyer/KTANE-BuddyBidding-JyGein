using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using System.Security.AccessControl;
using Events;
using Assets.Scripts.Records;
using Assets.Scripts.Utility;
using static MB3_MeshBakerRoot.ZSortObjects;

//HI :3 gl with modding


// this template here need to be the EXACT same thing as yo module type
public class BuddyBidding : MonoBehaviour {
     // might aswell name the script file the same thing

    // Modding Tutorial by Deaf: https://www.youtube.com/watch?v=YobuGSBl3i0

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public CreditCard creditCard;
    public BiddingPad biddingPad;

    public KMSelectable LeftButton;
    public KMSelectable RightButton;
    public List<KMSelectable> KeypadNumbers;
    public KMSelectable SubmitButton;
    public KMSelectable DeleteButton;

    public List<Animator> Exclamations;

    public List<Texture> Items;
    public List<Texture> Buddies;
    public Buddy TodaysBuddy;

    string ModuleName;
    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved = false;

    public bool IsOpen = false;
    public List<Auction> Auctions = new List<Auction>();
    public int CurrentAuction = 0;
    public List<Item> ChosenItems = new List<Item>();
    public int PlayerBalance = 0;
    public int SpentMoney = 0;
    public bool ActuallyDonating = false;
    public int LostWantedItems = 0;
    public int BoughtWantedItems = 0;
    public int CurrentPlayerInput = 0;
    public int DonataedAmount = 0;
    public List<ModuleBot> WantedItemBots = new List<ModuleBot>();
    public int WantedItemMaxBidRunningTotal = 0;
    public int MaxofBotMaxes = 0;

#pragma warning disable IDE0051
    void Awake ()
    { //Shit that happens before Start
#pragma warning restore IDE0051
        ModuleName = Module.ModuleDisplayName;
        ModuleId = ModuleIdCounter++;
        //Module.OnActivate += Activate;
        TodaysBuddy = (Buddy)Rnd.Range(0, Buddies.Count);
        biddingPad.SetDisplay(Buddies[(int)TodaysBuddy]);
        creditCard.GenerateNewCard();
        creditCard.Log += Log;
        foreach(KMSelectable child in GetComponent<KMSelectable>().Children)
        {
            child.OnInteract += delegate () { child.AddInteractionPunch(); return false; };
        }
        for(int i = 0; i <= 9; i++)
        {
            int dummy = i;
            KeypadNumbers[i].OnInteract += delegate () { HandleNumber(dummy); return false; };
        }
        LeftButton.OnInteract += delegate () { HandleLeft(); return false; };
        RightButton.OnInteract += delegate () { HandleRight(); return false; };
        SubmitButton.OnInteract += delegate () { HandleSubmit(); return false; };
        DeleteButton.OnInteract += delegate () { HandleDelete(); return false; };
        GetComponent<KMSelectable>().OnFocus += delegate () { IsSelected = true; };
        GetComponent<KMSelectable>().OnDefocus += delegate () { IsSelected = false; };
        Auctions.Add(new Auction { Item = Item.Buddy });

    }
#pragma warning disable IDE0051
    void Start ()
    { //Shit
#pragma warning restore IDE0051
        //Log($"Your Credit Card Number: {creditCard.CreditCardNumber}");
        //Log($"Your Credit Card Value: {creditCard.CVVNumber}");
        //Log($"Your Credit Card Company: {creditCard.Company}");
        Log($"Your Buddy's Color: {TodaysBuddy}");
        PlayerBalance = Step1.GenerateBalance(this, Bomb);
        MaxofBotMaxes = PlayerBalance - 30 - PlayerBalance / 100;
        Log($"Your Balance: {PlayerBalance}");
        ChosenItems = Step2.GenerateItems(this, Bomb);
        if (ChosenItems[0] == Item.Buddy)
        {
            Log("Unicorn met, donate your balance to Buddy to solve.");
            ActuallyDonating = true;
        }
        else
        {
            Log("Wanted Items:");
            int count = 0;
            foreach (Item item in ChosenItems)
            {
                count++;
                Log($"Item {count}: {item}");
                int BotNum = Rnd.Range(0, 3);
                List<ModuleBot> NewBots = new List<ModuleBot>();
                int ThisMaxBotsBid = 0;
                for (int i = 0; i < BotNum; i++)
                {
                    int maxBid = Rnd.Range(50, (MaxofBotMaxes - WantedItemMaxBidRunningTotal)/Auctions.Count());
                    ModuleBot NewBot = new ModuleBot { MaxBid = maxBid };
                    NewBots.Add(NewBot);
                    WantedItemBots.Add(NewBot);
                    ThisMaxBotsBid = ThisMaxBotsBid > maxBid ? ThisMaxBotsBid : maxBid;
                    Log($"New bot generated on Item {item}. It's max bid is {maxBid}.");
                }
                WantedItemMaxBidRunningTotal += ThisMaxBotsBid;
                Auctions.Add(new Auction { Item = item, ModuleBidders = NewBots });
            }
            AttemptToMaxoutMaxBids();
        }
        Log("Other Items:");
        while (Auctions.Count < 7)
        {
            Item thisOnesItem = (Item)Rnd.Range(0, 10);
            while (Auctions.Select((a) => a.Item).Contains(thisOnesItem)) thisOnesItem = (Item)Rnd.Range(0, 10);
            Log($"{thisOnesItem}");
            List<ModuleBot> NewBots = new List<ModuleBot>();
            int BotNum = Rnd.Range(0, 3);
            for (int i = 0; i < BotNum; i++)
            {
                int maxBid = Rnd.Range(50, 400);
                NewBots.Add(new ModuleBot { MaxBid = maxBid });
                Log($"New bot generated on Item {thisOnesItem}. It's max bid is {maxBid}.");
            }
            Auctions.Add(new Auction { Item = thisOnesItem, ModuleBidders = NewBots });
        }
        Auctions = Auctions.OrderBy(a => a.Item).ToList();
    }

    private bool IsSelected = false;
#pragma warning disable IDE0051
    void Update ()
    { //Shit that happens at any point after initialization
#pragma warning restore IDE0051
        if (IsSelected)
        {
            for(int i = 0; i <= 9; i++) {
                if(Input.GetKeyDown(KeyCode.Keypad0 + i)) {
                  HandleNumber(i);
                }
            }
            if (Input.GetKeyDown(KeyCode.KeypadEnter)) HandleSubmit();
            if (Input.GetKeyDown(KeyCode.Backspace)) HandleDelete();
            if (Input.GetKeyDown(KeyCode.RightArrow)) HandleRight();
            if (Input.GetKeyDown(KeyCode.LeftArrow)) HandleLeft();
        }
        if (IsOpen && !ModuleSolved)
        {
            foreach (Auction auction in Auctions)
            {
                auction.Update(this);
            }
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        BidInfo selectedBidInfo = Auctions[CurrentAuction].GetBidInfo();
        if (CurrentPlayerInput > 0) selectedBidInfo.currentBid = CurrentPlayerInput;
        biddingPad.UpdateDisplay(selectedBidInfo);
    }

    void AttemptToMaxoutMaxBids()
    {
        List<Auction> ValidAuctions = Auctions.Where((a) => a.ModuleBidders.Count > 0 && ChosenItems.Contains(a.Item)).ToList();
        if (ValidAuctions.Count() >= 3)
        {
            Auction SelectedAuction = ValidAuctions[Rnd.Range(0, 3)];
            ModuleBot SelectedBot = SelectedAuction.ModuleBidders.OrderByDescending(b => b.MaxBid).ToList()[0];
            Log($"All wanted item auctions have a bot, changing the max bid of the bot on the auction for item {SelectedAuction.Item} with max bid {SelectedBot.MaxBid} to {MaxofBotMaxes - WantedItemMaxBidRunningTotal + SelectedBot.MaxBid}.");
            SelectedBot.MaxBid += MaxofBotMaxes - WantedItemMaxBidRunningTotal;
            WantedItemMaxBidRunningTotal = MaxofBotMaxes;
        }
    }

    void HandleNumber(int digit)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);
        KeypadNumbers[digit].GetComponent<Animator>().Play("Keypad Press");
        if (!IsOpen) return;
        if (CurrentPlayerInput.ToString().Count() >= 3) return;
        CurrentPlayerInput = int.Parse(CurrentPlayerInput.ToString() + digit.ToString());
    }

    void HandleSubmit()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);
        SubmitButton.GetComponent<Animator>().Play("Keypad Press");
        if (!IsOpen) {
            Log($"Opening auctions!");
            IsOpen = true;
            foreach(Auction auction in Auctions)
            {
                auction.Start(this);
            }
            foreach (Animator exclamation in Exclamations)
            {
                exclamation.Play("Blinking Exclamation");
            }
            return;
        }
        Auction SelectedAuction = Auctions[CurrentAuction];
        if (SelectedAuction.Item == Item.Buddy)
        {
            Log($"Attempting to donate {CurrentPlayerInput} to Buddy.");
            if (ActuallyDonating)
            {
                if (CurrentPlayerInput + DonataedAmount > PlayerBalance) Strike("You donated too much money! Strike Issued!");
                else
                {
                    Log($"Donation received.");
                    DonataedAmount += CurrentPlayerInput;
                    if (DonataedAmount >= PlayerBalance)
                    {
                        Solve("Buddy thanks you for your donation! Module Solved!");
                    }
                }
            }
            else Log($"Unicorn not active, input removed.");
            CurrentPlayerInput = 0;
            return;
        }
        Log($"Bidding {CurrentPlayerInput} coins on {SelectedAuction.Item}.");
        if (SelectedAuction.CurrentBid < CurrentPlayerInput)
        {
            if (!ChosenItems.Contains(SelectedAuction.Item) && CurrentPlayerInput > 50)
            {
                int diff = SelectedAuction.PlayerBid >= 50 ? CurrentPlayerInput - SelectedAuction.PlayerBid : CurrentPlayerInput - 50;
                if (WantedItemBots.Count() > 0)
                {
                    Log($"Your bluff will make the bot on a wanted item with max bid {WantedItemBots.OrderByDescending(b => b.MaxBid).ToList()[0].MaxBid} have {diff} less to bid.");
                    WantedItemBots.OrderByDescending(b => b.MaxBid).ToList()[0].MaxBid -= diff;
                    WantedItemMaxBidRunningTotal -= diff;
                    MaxofBotMaxes -= diff;
                }
            }
            SelectedAuction.PlayerBid = CurrentPlayerInput;
            if (SelectedAuction.ModuleBidders.Count() <= 0)
            {
                int maxBid = Rnd.Range(50, ChosenItems.Contains(SelectedAuction.Item) ? MaxofBotMaxes - WantedItemMaxBidRunningTotal : 400);
                ModuleBot NewBot = new ModuleBot { MaxBid = maxBid };
                SelectedAuction.ModuleBidders.Add(NewBot);
                Log($"New bot generated on Item {SelectedAuction.Item}. It's max bid is {maxBid}.");
                if (ChosenItems.Contains(SelectedAuction.Item))
                {
                    WantedItemBots.Add(NewBot);
                    WantedItemMaxBidRunningTotal += maxBid;
                    AttemptToMaxoutMaxBids();
                }
            }
            SelectedAuction.OnBidPlaced();
        }
        else Log($"Bid is lower than the current highest bid, input removed.");
        CurrentPlayerInput = 0;
        return;
    }

    void HandleDelete()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);
        DeleteButton.GetComponent<Animator>().Play("Keypad Press");
        if (!IsOpen) return;
        if (CurrentPlayerInput.ToString().Count() <= 0) return;
        if (CurrentPlayerInput.ToString().Count() == 1) CurrentPlayerInput = 0;
        else CurrentPlayerInput = int.Parse(CurrentPlayerInput.ToString().Substring(0, CurrentPlayerInput.ToString().Count()-1));
    }

    void HandleLeft()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);
        LeftButton.GetComponent<Animator>().Play("Arrow Press");
        if (!IsOpen)
        {
            return;
        };
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.PageTurn, gameObject.transform);
        CurrentAuction = Util.mod(CurrentAuction - 1, Auctions.Count());
        CurrentPlayerInput = 0;
        UpdateDisplay();
    }

    void HandleRight()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);
        RightButton.GetComponent<Animator>().Play("Arrow Press");
        if (!IsOpen)
        {
            return;
        };
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.PageTurn, gameObject.transform);
        CurrentAuction = Util.mod(CurrentAuction + 1, Auctions.Count());
        CurrentPlayerInput = 0;
        UpdateDisplay();
    }

    public void SellItem(Auction auction)
    {
        bool IsChosenItem = ChosenItems.Contains(auction.Item);
        CostState Buyer = auction.GetCostState();
        Log($"Item {auction.Item} sold to {(Buyer == CostState.NoBid ? "no one" : Buyer.ToString())} for {auction.CurrentBid}!");
        if (Buyer != CostState.Player && IsChosenItem)
        {
            Strike($"You wanted the {auction.Item} but it was sold to someone else! Strike!!");
            LostWantedItems++;
            if (LostWantedItems >= 3)
            {
                Log("You lost all of your wanted items!! Goodbye!!");
                Detonate();
                return;
            }
        }
        if (Buyer == CostState.Player && !IsChosenItem)
        {
            Strike($"You didn't want the {auction.Item} and yet you bought it! Strike!!");
            SpentMoney += auction.CurrentBid;
            if (WantedItemBots.Count() > 0)
            {
                WantedItemBots.OrderByDescending(b => b.MaxBid).ToList()[0].MaxBid -= 50;
                WantedItemMaxBidRunningTotal -= 50;
                MaxofBotMaxes -= 50;
            }
        }
        if (Buyer == CostState.Player && IsChosenItem)
        {
            BoughtWantedItems++;
            Log($"{auction.Item} sold to the defuser!");
            SpentMoney += auction.CurrentBid;
        }
        if (BoughtWantedItems + LostWantedItems >= 3)
        {
            Solve("All wanted items have been sold! Module solved.");
        }
    }

    void Solve (string message)
    {
        if (ModuleSolved) return;
        biddingPad.Timer.text = "";
        Module.HandlePass();
        Log(message);
        ModuleSolved = true;
    }

    void Strike (string message)
    {
        if (ModuleSolved) return;
        Module.HandleStrike();
        Log(message);
    }

    void Detonate ()
    {
        if (TwitchPlaysActive || ZenModeActive)
        {
            if (TwitchPlaysActive)
                Solve("Detonation prevented due to Twitch Plays being enabled!");
            else if (ZenModeActive)
                Solve("Detonation prevented due to Zen Mode being used!");
            return;
        }
        var bomb = GetComponentInParent<Bomb>();
        if (bomb && !bomb.HasDetonated)
        {
            float elapsed = bomb.GetTimer().TimeElapsed;

            var strikes = RecordManager.Instance.GetCurrentRecord().Strikes;

            strikes[strikes.Length - 1] = new StrikeSource
            {
                ComponentType = Assets.Scripts.Missions.ComponentTypeEnum.Mod,
                InteractionType = InteractionTypeEnum.PushButton,
                Time = elapsed,
                ComponentName = "Buddy Bidding Bidding you farewell you non-Buddy",
            };

            RecordManager.Instance.SetResult(GameResultEnum.ExplodedDueToStrikes, elapsed, SceneManager.Instance.GameplayState.GetElapsedRealTime());

            bomb.Detonate();
        }
    }
    public void Log (string message) 
    {
        Debug.Log($"[{ModuleName} #{ModuleId}] {message}");
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} #/d/e/</>/c to press a number button/delete button/enter button/left button/right button/flip the credit card. This can be chained like !{0} 123e. Use !{0} Bid X ### to bid on item number X with ### coins (Buddy is item number 1). Use !{0} Cycle to cycle through the items up for auction.";
    private bool TwitchPlaysActive;
    private bool ZenModeActive;
#pragma warning restore 414

    // Twitch Plays (TP) documentation: https://github.com/samfundev/KtaneTwitchPlays/wiki/External-Mod-Module-Support

#pragma warning disable IDE0051
    IEnumerator ProcessTwitchCommand (string Command)
    {
#pragma warning restore IDE0051
        Command = Command.ToLower();
        Match m = Regex.Match(Command, @"^\s*([\d|e|d|<|>|c]+)\s*$");
        if (m.Success)
        {
            yield return "strike";
            yield return "solve";
            string capture = m.Groups[1].Value;
            foreach(char c in capture)
            {
                int i;
                if(int.TryParse(c.ToString(), out i))
                {
                    KeypadNumbers[i].OnInteract();
                }
                if(c == 'e')
                {
                    SubmitButton.OnInteract();
                }
                if (c == 'd')
                {
                    DeleteButton.OnInteract();
                }
                if (c == '<')
                {
                    LeftButton.OnInteract();
                }
                if (c == '>')
                {
                    RightButton.OnInteract();
                }
                if (c == 'c')
                {
                    creditCard.GetComponent<KMSelectable>().OnInteract();
                }
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }
        m = Regex.Match(Command, @"^\s*bid ([1-7]) (\d{1,3})\s*$");
        if (m.Success && IsOpen)
        {
            yield return "strike";
            yield return "solve";
            Log(m.Groups[2].Value);
            int auction = int.Parse(m.Groups[1].Value) - 1;
            string bid = m.Groups[2].Value;
            CurrentAuction = auction;
            CurrentPlayerInput = 0;
            UpdateDisplay();
            foreach(char c in bid)
            {
                KeypadNumbers[int.Parse(c.ToString())].OnInteract();
            }
            SubmitButton.OnInteract();
            yield break;
        }
        m = Regex.Match(Command, @"^\s*cycle\s*$");
        if (m.Success && IsOpen)
        {
            yield return "strike";
            yield return "solve";
            for (int i = 0; i < Auctions.Count(); i++)
            {
                RightButton.OnInteract();
                yield return new WaitForSeconds(1);
            }
            yield break;
        }
    }

#pragma warning disable IDE0051
    IEnumerator TwitchHandleForcedSolve ()
    {
#pragma warning restore IDE0051
        ModuleSolved = true;
        StopAllCoroutines();
        yield return true;
    }
}
