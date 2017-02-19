

[System.Serializable]
public class ToySaver{

    public string toy_name = Get.NullToyName();
	public float ammo;
	public Rune rune;
    public ToyType type;
    
    public ToySaver()
    {

    }
			
	
	public ToySaver(string _name, float _ammo, Rune _rune, ToyType _type)
	{
        toy_name = _name;
		ammo = _ammo;
		rune = _rune;
		type = _type;
	}
    
}

[System.Serializable]
public class IslandSaver
{
    public IslandType island_type = IslandType.Null;
    public ToySaver toy_saver;
    public float block_timer;
    public string name;

    public IslandSaver() { }

    public IslandSaver(string _name, ToySaver _toy_saver)
    {
        toy_saver = _toy_saver;
        name = _name;
    }

    public IslandSaver(string _name, float _block_timer)
    {
        block_timer = _block_timer;
        name = _name;
    }
}


[System.Serializable]
public class ScoreSaver
{
	public float[] score;
	public float possible_dreams;
	
	
	
	public ScoreSaver() { }
	
	public ScoreSaver(float[] _score, float _possible_dreams)
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
