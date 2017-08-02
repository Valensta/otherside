using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;


public class FXRate
{
    public MarketplaceID type;
    public float buy_rate;
    public float sell_rate;

    public FXRate(MarketplaceID type, float buy_rate, float sell_rate)
    {
        this.type = type;
        this.buy_rate = buy_rate;
        this.sell_rate = sell_rate;
    }

    public FXRate(MarketplaceID type, float buy_rate)
    {
        this.type = type;
        this.buy_rate = buy_rate;
        this.sell_rate = Mathf.FloorToInt(buy_rate * Marketplace.defaultSellPenalty);
    }
}

public static class Marketplace
{

    public static Dictionary<MarketplaceID, FXRate> rates;
    public static float defaultSellPenalty = 1f;

    public static FXRate getFXRate(MarketplaceID ID)
    {
        if (rates == null) initSettings();
        FXRate rate = null;
        rates.TryGetValue(ID, out rate);
        if (rate == null) Debug.LogError("Tryin to get an exchange rate for an undefined type " + ID + "\n");
        return rate;
    }



    //XP        DREAM        SENSIBLE       remove_cap      LULL     INTERVAL
    public static void initSettings()
    {
        rates = new Dictionary<MarketplaceID, FXRate>();

        rates.Add(MarketplaceID.Sensible, new FXRate(MarketplaceID.Sensible, 2));
        rates.Add(MarketplaceID.MoreHealth, new FXRate(MarketplaceID.MoreHealth, 5));
        rates.Add(MarketplaceID.MoreXP, new FXRate(MarketplaceID.MoreXP, 2));
        rates.Add(MarketplaceID.MoreDamage, new FXRate(MarketplaceID.MoreDamage, 3));
        rates.Add(MarketplaceID.MoreDreams, new FXRate(MarketplaceID.MoreDreams, 3));

    }
}

