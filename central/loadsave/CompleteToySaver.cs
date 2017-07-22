

[System.Serializable]
public class CompleteToySaver{

    public string toy_name = Get.NullToyName();
	public float ammo;
	public Rune rune_saver;
    public ToyType type;
    public float construnction_time;    

    public CompleteToySaver()
    {

    }
			
	
	public CompleteToySaver(string _name, float _ammo, Rune _rune, ToyType _type, float _construction_time)
	{
        toy_name = _name;
		ammo = _ammo;
		rune_saver = _rune;
		type = _type;
        construnction_time = _construction_time;
        rune_saver.ID = -99;
        rune_saver.stat_sum.towerID = -99;
        rune_saver.special_stat_sum.towerID = -99;
    }
    
}

[System.Serializable]
public class CompleteIslandSaver
{
    public IslandType island_type = IslandType.Null;
    public CompleteToySaver toy_saver;
    public float block_timer;
    public string name;

    public CompleteIslandSaver() { }

    public CompleteIslandSaver(string _name, CompleteToySaver _toy_saver)
    {
        toy_saver = _toy_saver;
        name = _name;
    }

    public CompleteIslandSaver(string _name, float _block_timer)
    {
        block_timer = _block_timer;
        name = _name;
    }
}


[System.Serializable]
public class CompleteScoreSaver
{
	public float[] score;
	public float possible_dreams;
	
	
	
	public CompleteScoreSaver() { }
	
	public CompleteScoreSaver(float[] _score, float _possible_dreams)
	{
		score = _score;
		possible_dreams = _possible_dreams;
	}
	

}


public class RewardsSaver
{
    public Reward[] rewards;

    public RewardsSaver() { }

    public RewardsSaver(Reward[] _rewards)
    {
        rewards = _rewards;
    }


}
