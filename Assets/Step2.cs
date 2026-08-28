using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public static class Step2 {
    static readonly List<List<Item>> Item1Table = new List<List<Item>> {//i couldn't be arsed to tab out the rest
        new List<Item> { Item.Chair,        Item.MegaMushroom,  Item.Shoe, Item.Smartphone, Item.Table, Item.PileofTrash, Item.SpinyShell, Item.Smartphone, Item.Bookshelf, Item.SuperStar, Item.Shoe, Item.Smartphone, Item.PileofTrash, Item.Shoe, Item.Table, Item.Table, Item.PileofTrash, Item.Smartphone },
        new List<Item> { Item.Smartphone,   Item.PrinterInk,    Item.PileofTrash, Item.Smartphone, Item.Bookshelf, Item.PileofTrash, Item.Smartphone, Item.MegaMushroom, Item.PrinterInk, Item.SpinyShell, Item.Table, Item.SpinyShell, Item.Chair, Item.SuperStar, Item.Table, Item.Smartphone, Item.Shoe, Item.Bookshelf },
        new List<Item> { Item.PrinterInk,   Item.SuperStar,     Item.PrinterInk, Item.Chair, Item.PileofTrash, Item.Bookshelf, Item.SpinyShell, Item.Shoe, Item.SuperStar, Item.Bookshelf, Item.PileofTrash, Item.Chair, Item.Chair, Item.MegaMushroom, Item.Chair, Item.Bookshelf, Item.SuperStar, Item.MegaMushroom },
        new List<Item> { Item.MegaMushroom, Item.SpinyShell,    Item.Table, Item.Table, Item.SuperStar, Item.SpinyShell, Item.PileofTrash, Item.SpinyShell, Item.Chair, Item.Smartphone, Item.PrinterInk, Item.SuperStar, Item.SpinyShell, Item.PrinterInk, Item.SpinyShell, Item.MegaMushroom, Item.Table, Item.PrinterInk },
        new List<Item> { Item.Smartphone,   Item.Shoe,          Item.Shoe, Item.Table, Item.Smartphone, Item.Table, Item.Chair, Item.Chair, Item.Bookshelf, Item.PrinterInk, Item.MegaMushroom, Item.Bookshelf, Item.PrinterInk, Item.SuperStar, Item.Shoe, Item.SuperStar, Item.Table, Item.Table },
        new List<Item> { Item.MegaMushroom, Item.Chair,         Item.Shoe, Item.PrinterInk, Item.MegaMushroom, Item.SuperStar, Item.Bookshelf, Item.Table, Item.Table, Item.SuperStar, Item.Chair, Item.Table, Item.PrinterInk, Item.PileofTrash, Item.SuperStar, Item.Chair, Item.SpinyShell, Item.Table },
        new List<Item> { Item.Chair,        Item.SpinyShell,    Item.MegaMushroom, Item.PrinterInk, Item.Smartphone, Item.Shoe, Item.Chair, Item.PileofTrash, Item.Chair, Item.Shoe, Item.SpinyShell, Item.SpinyShell, Item.Table, Item.Shoe, Item.MegaMushroom, Item.Bookshelf, Item.PrinterInk, Item.Table },
        new List<Item> { Item.Bookshelf,    Item.Smartphone,    Item.SpinyShell, Item.SuperStar, Item.Shoe, Item.Smartphone, Item.Chair, Item.PrinterInk, Item.SuperStar, Item.Bookshelf, Item.Smartphone, Item.Bookshelf, Item.PileofTrash, Item.Smartphone, Item.MegaMushroom, Item.Chair, Item.MegaMushroom, Item.PileofTrash },
        new List<Item> { Item.Bookshelf,    Item.PrinterInk,    Item.PrinterInk, Item.Shoe, Item.PileofTrash, Item.Shoe, Item.Smartphone, Item.MegaMushroom, Item.SpinyShell, Item.Bookshelf, Item.SuperStar, Item.PileofTrash, Item.PileofTrash, Item.MegaMushroom, Item.SuperStar, Item.SuperStar, Item.Shoe, Item.Bookshelf },
        new List<Item> { Item.MegaMushroom, Item.PrinterInk,    Item.Chair, Item.Chair, Item.MegaMushroom, Item.PileofTrash, Item.SpinyShell, Item.Smartphone, Item.Bookshelf, Item.Shoe, Item.Bookshelf, Item.SuperStar, Item.Table, Item.Shoe, Item.Smartphone, Item.Chair, Item.PileofTrash, Item.Shoe },
        new List<Item> { Item.SuperStar,    Item.SuperStar,     Item.Chair, Item.SuperStar, Item.Shoe, Item.Smartphone, Item.PileofTrash, Item.Chair, Item.SpinyShell, Item.Chair, Item.PrinterInk, Item.Chair, Item.Bookshelf, Item.Smartphone, Item.Smartphone, Item.Shoe, Item.SpinyShell, Item.Shoe },
        new List<Item> { Item.Shoe,         Item.Chair,         Item.Bookshelf, Item.Shoe, Item.Table, Item.SuperStar, Item.Shoe, Item.Chair, Item.PileofTrash, Item.Shoe, Item.MegaMushroom, Item.MegaMushroom, Item.MegaMushroom, Item.Table, Item.SuperStar, Item.PrinterInk, Item.Chair, Item.Chair },
        new List<Item> { Item.Bookshelf,    Item.Shoe,          Item.PileofTrash, Item.Table, Item.MegaMushroom, Item.PrinterInk, Item.MegaMushroom, Item.SuperStar, Item.Table, Item.SuperStar, Item.Bookshelf, Item.Chair, Item.SpinyShell, Item.SpinyShell, Item.Table, Item.Chair, Item.PileofTrash, Item.PileofTrash },
        new List<Item> { Item.Bookshelf,    Item.MegaMushroom,  Item.PrinterInk, Item.PileofTrash, Item.MegaMushroom, Item.MegaMushroom, Item.SpinyShell, Item.SuperStar, Item.Chair, Item.Smartphone, Item.Chair, Item.PileofTrash, Item.Chair, Item.SuperStar, Item.PileofTrash, Item.Smartphone, Item.Bookshelf, Item.SpinyShell },
        new List<Item> { Item.Shoe,         Item.Bookshelf,     Item.PrinterInk, Item.Smartphone, Item.SuperStar, Item.Chair, Item.SuperStar, Item.Shoe, Item.PileofTrash, Item.Table, Item.Chair, Item.PileofTrash, Item.MegaMushroom, Item.Smartphone, Item.SpinyShell, Item.Shoe, Item.Bookshelf, Item.Smartphone },
        new List<Item> { Item.Smartphone,   Item.SpinyShell,    Item.Table, Item.MegaMushroom, Item.SpinyShell, Item.SpinyShell, Item.Chair, Item.SuperStar, Item.Smartphone, Item.PileofTrash, Item.PrinterInk, Item.SuperStar, Item.SpinyShell, Item.PileofTrash, Item.PileofTrash, Item.SuperStar, Item.Table, Item.SpinyShell },
        new List<Item> { Item.Table,        Item.Chair,         Item.Bookshelf, Item.SuperStar, Item.Smartphone, Item.SpinyShell, Item.SuperStar, Item.SuperStar, Item.Smartphone, Item.PrinterInk, Item.Chair, Item.Chair, Item.MegaMushroom, Item.Shoe, Item.PrinterInk, Item.SpinyShell, Item.Bookshelf, Item.SpinyShell },
        new List<Item> { Item.MegaMushroom, Item.PileofTrash,   Item.Table, Item.SpinyShell, Item.SuperStar, Item.Bookshelf, Item.PrinterInk, Item.PileofTrash, Item.PrinterInk, Item.PrinterInk, Item.SuperStar, Item.PileofTrash, Item.Shoe, Item.Chair, Item.Bookshelf, Item.Table, Item.Shoe, Item.Table },
    };
    static readonly List<int> Item2Table = new List<int> { 3, 7, 13, 17, 19 };
    static readonly Dictionary<Company, List<Item>> Item3Table = new Dictionary<Company, List<Item>> {
        { Company.KoopaCash,        new List<Item> {   Item.SpinyShell,  Item.SuperStar,   Item.Bookshelf,     Item.MegaMushroom,  Item.Table, Item.SuperStar   } },   
        { Company.Royalcard,        new List<Item> {   Item.Smartphone,  Item.Chair,       Item.Smartphone,    Item.Smartphone,    Item.Table, Item.Shoe        } },
        { Company.PurpleBanking,    new List<Item> {   Item.Bookshelf,   Item.Table,       Item.SpinyShell,    Item.PrinterInk,    Item.Shoe,  Item.PileofTrash } }
    };
    static Item Item1(BuddyBidding Module, KMBombInfo Bomb) {
        return Item1Table[Util.mod(Util.SumPosition(Module.creditCard.CreditCardNumberStr, true) - 36, 18)][Util.mod(Util.SumPosition(Module.creditCard.CreditCardNumberStr, false) - 36, 18)];
    }
    static Item Item2(BuddyBidding Module, KMBombInfo Bomb) {
        return (Item)((Util.DigitalRoot(Module.PlayerBalance) * Item2Table[(int)Module.TodaysBuddy]) % 10);
    }
    static Item Item3(BuddyBidding Module, KMBombInfo Bomb) {
        List<Item> outputList = Item3Table[Module.creditCard.Company];
        if(Bomb.GetIndicators().Count() >= 4) return outputList[0];
        if(int.Parse(Module.creditCard.CreditCardNumberStr[9].ToString()) == Bomb.GetBatteryCount()) return outputList[1];
        if(Util.SumPosition(Module.creditCard.CreditCardNumberStr, true) < 20) return outputList[2];
        if(Util.SumPosition(Module.creditCard.CreditCardNumberStr, false) > 40) return outputList[3];
        if(Bomb.IsPortPresent(KModkit.Port.Parallel) || Bomb.IsPortPresent(KModkit.Port.Serial)) return outputList[4];
        return outputList[5];
    }
    public static List<Item> GenerateItems(BuddyBidding Module, KMBombInfo Bomb) {
        if (Bomb.IsIndicatorPresent(Indicator.BOB) && Bomb.IsIndicatorOn(Indicator.BOB) && Bomb.GetBatteryCount() == 3 && Util.ShareALetter((string)Bomb.GetSerialNumberLetters(), "BUDDY")) return new List<Item> { Item.Buddy };
        List<Item> ChosenItems = new List<Item> { Item1(Module, Bomb) };
        Item item2 = Item2(Module, Bomb);
        while (ChosenItems.Contains(item2)) item2 = (Item)((int)(item2 + 1) % 10);
        ChosenItems.Add(item2);
        Item item3 = Item3(Module, Bomb);
        while (ChosenItems.Contains(item3)) item3 = (Item)((int)(item3 + 1) % 10);
        ChosenItems.Add(item3);
        return ChosenItems;
    }
}