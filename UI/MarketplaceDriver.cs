using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class MarketplaceDriver : MonoBehaviour
{   
    //public List_Panel selected_skill_panel;
    public List<MarketplaceObject> marketplace_objects;
    public Text default_currency_amount;
    public GameObject view;
    public MarketplaceID default_currency = MarketplaceID.ScorePoint;
    public Button sell_all_button;
    public MyText sell_all_amount_text;

    public void Init(bool enable)
    {
        view.SetActive(enable);

        if (!enable)
        {
            Central.Instance.game_saver.SaveGame(SaveWhen.BetweenLevels);
            return;
        }
        
        foreach (MarketplaceObject obj in marketplace_objects)
        {
            
            FXRate rate = Marketplace.getFXRate(obj.ID);
            if (rate == null) continue;
                
            obj.Init(this,getCurrentAmount(obj.ID), rate,getDefaultCurrency());
            
        }


        setSellAllButton();
        updateDefaultCurrency();
        
    }

    public void setSellAllButton()
    {
        int sell_all_for_this_much = 0;
        foreach (MarketplaceObject obj in marketplace_objects)
        {
            sell_all_for_this_much += Mathf.FloorToInt(obj.fxRate.sell_rate * getCurrentAmount(obj.ID));
        }



        sell_all_button.interactable = sell_all_for_this_much > 0;
        sell_all_amount_text.setText((sell_all_for_this_much > 0) ? sell_all_for_this_much.ToString() : "");

    }

    public int getDefaultCurrency()
    {
        return ScoreKeeper.Instance.getTotalScore();
    }

    public void SellAll()
    {
        foreach (MarketplaceObject obj in marketplace_objects)
        {
            obj.SellAll();
        }
    }
    
    public bool Sell(MarketplaceID ID, int amount, bool all)
    {
        if (all) amount = getCurrentAmount(ID);
        if (amount == 0) return true;
        Debug.Log("Sell " + ID.ToString() + " " + amount + "\n");
        FXRate rate = Marketplace.getFXRate(ID);
        _process(ID, -amount);        
        _process(default_currency, amount * rate.sell_rate);

        getObject(ID).UpdateLabels(getCurrentAmount(ID), getDefaultCurrency());
        setSellAllButton();
        updateDefaultCurrency();
        return true;
    }

    public void updateDefaultCurrency()
    {
        default_currency_amount.text = getDefaultCurrency().ToString();
    }

    public bool Buy(MarketplaceID ID, int amount)
    {
        Debug.Log("Buy " + ID.ToString() + " " + amount + "\n");
        FXRate rate = Marketplace.getFXRate(ID);
        _process(ID, amount);
        _process(default_currency, -amount * rate.buy_rate);

        getObject(ID).UpdateLabels(getCurrentAmount(ID), getDefaultCurrency());
        setSellAllButton();
        updateDefaultCurrency();
        return true;
    }

    private bool _process(MarketplaceID ID, float amount)
    {
        Debug.Log("Processing " + ID + " " + amount);

        Tracker.Log(PlayerEvent.MarketPlace, false,
            customAttributes: new Dictionary<string, string>(){ { "attribute_1", ID.ToString()  }},
            customMetrics: new Dictionary<string, double>() { { "metric_1", amount } });

        switch (ID)
        {
            case MarketplaceID.Sensible:
                Peripheral.Instance.my_inventory.AddWish(WishType.Sensible, Mathf.FloorToInt(amount),1);
                return true;
            case MarketplaceID.MoreDamage:
                Peripheral.Instance.my_inventory.AddWish(WishType.MoreDamage, 1,Mathf.FloorToInt(amount));
                return true;
            case MarketplaceID.MoreHealth:
                Peripheral.Instance.my_inventory.AddWish(WishType.MoreHealth, 1,Mathf.FloorToInt(amount));
                return true;
            case MarketplaceID.MoreDreams:
                Peripheral.Instance.my_inventory.AddWish(WishType.MoreDreams, 1,Mathf.FloorToInt(amount));
                return true;
            case MarketplaceID.MoreXP:
                Peripheral.Instance.my_inventory.AddWish(WishType.MoreXP, 1, Mathf.FloorToInt(amount));
                return true;
            case MarketplaceID.ScorePoint:
                int new_score = ScoreKeeper.Instance.getTotalScore() + Mathf.FloorToInt(amount);
                Debug.Log("new score " + new_score + "\n");
                ScoreKeeper.Instance.SetTotalScore(new_score);
                return true;
            case MarketplaceID.Dreams:
                Peripheral.Instance.addDreams(amount, Vector3.zero, false);
                return true;
        }
        return false;
    }
    

    public int getCurrentAmount(MarketplaceID ID)
    {
        switch (ID)
        {
            case MarketplaceID.Sensible:
                return Peripheral.Instance.my_inventory.GetWishCount(WishType.Sensible);
            case MarketplaceID.MoreDamage:
                return Peripheral.Instance.my_inventory.GetWishCount(WishType.MoreDamage);
            case MarketplaceID.MoreDreams:
                return Peripheral.Instance.my_inventory.GetWishCount(WishType.MoreDreams);
            case MarketplaceID.MoreHealth:
                return Peripheral.Instance.my_inventory.GetWishCount(WishType.MoreHealth);
            case MarketplaceID.MoreXP:
                return Peripheral.Instance.my_inventory.GetWishCount(WishType.MoreXP);
            case MarketplaceID.Dreams:
                return Mathf.FloorToInt(Peripheral.Instance.getDreams());
            case MarketplaceID.ScorePoint:
                return ScoreKeeper.Instance.getTotalScore();
            default:
                return 0;
        }
    }
    public MarketplaceObject getObject(MarketplaceID ID)
    {
        foreach (MarketplaceObject obj in marketplace_objects)
            if (obj.ID == ID) return obj;

        return null;
    }
}