using System.Collections.Generic;

[System.Serializable]
public class ToySaver{

    public string toy_name = Get.NullToyName();    
	public float ammo;
	public RuneSaver rune_saver;
    public ToyType type;
    public float construnction_time;
    public tower_stats tower_stats;
    public ToySaver()
    {

    }
			
	
	public ToySaver(string _name, float _ammo, RuneSaver _rune, ToyType _type, float _construction_time, tower_stats _tower_stats)
	{
        toy_name = _name;
		ammo = _ammo;
		rune_saver = _rune;
		type = _type;
        construnction_time = _construction_time;
        tower_stats = _tower_stats;        
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



