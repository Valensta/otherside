using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Simulator    : MonoBehaviour {

    public List<Island_Button> islands;
	
	


	void Start(){
        //Airy();
        //Laser();
    
        RapidFire();
        //DOT();
        //Diffuse();
        //Focus();
        //Critical();
        //Sparkles();
        //Level2_Sensible();
        //  Level2_Vexing();
        //Level2_Sensible_RapidFire();
        // Level2_Sensible_Laser();
    }


    void Laser()
    {

        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[0].gameObject);
        zero.name = "default";        
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[2].gameObject);
        two.name = "laser";
        islands[2].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[4].gameObject);
        four.name = "laser2";
        //islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[4].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[4].my_toy);


        GameObject six = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[6].gameObject);
        six.name = "laser3";
        //islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[6].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[6].my_toy);


    }

    

    void Airy()
    {

        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("airy_tower", islands[0].gameObject);
        zero.name = "airy";
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[2].gameObject);
        two.name = "sensible";
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);
    }


        void RapidFire()
    {

        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[0].gameObject);
        zero.name = "default";
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[2].gameObject);
        two.name = "rapidfire";
        islands[2].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[4].gameObject);
        four.name = "rapidfire2";
        //islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[4].my_toy);


        GameObject six = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[6].gameObject);
        six.name = "rapidfire3";
        //islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[6].my_toy);


    }

    void DOT()
    {

        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[0].gameObject);
        zero.name = "default";
        islands[0].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[2].gameObject);
        two.name = "rapidfire";
        islands[2].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[2].my_toy.rune.Upgrade(EffectType.DOT, false, true);
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[4].gameObject);
        four.name = "rapidfire2";
        //islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.DOT, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.DOT, false, true);
        islands[4].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[4].my_toy);


        GameObject six = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[6].gameObject);
        six.name = "rapidfire3";
        //islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.DOT, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.DOT, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.DOT, false, true);
        islands[6].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[6].my_toy);


    }


    void Diffuse()
    {
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[0].gameObject);
        zero.name = "default";
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[2].gameObject);
        two.name = "diffuse";
        islands[2].my_toy.rune.Upgrade(EffectType.Diffuse, false, true);
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);
        
        GameObject four = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[4].gameObject);
        four.name = "diffuse2";
        //islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Diffuse, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Diffuse, false, true);
        islands[4].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[4].my_toy);


        GameObject six = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[6].gameObject);
        six.name = "diffuse3";
        //islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Diffuse, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Diffuse, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Diffuse, false, true);
        islands[6].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[6].my_toy);
        
    }



    void Focus()
    {
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[0].gameObject);
        zero.name = "default";
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[2].gameObject);
        two.name = "Focus";
        islands[2].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);
        
        GameObject four = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[4].gameObject);
        four.name = "Focus2";
        //islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[4].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[4].my_toy);


        GameObject six = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[6].gameObject);
        six.name = "Focus3";
        //islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[6].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[6].my_toy);
        
    }

    void Critical()
    {
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[0].gameObject);
        zero.name = "default";
        islands[0].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[2].gameObject);
        two.name = "Critical";
        islands[2].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[2].my_toy.rune.Upgrade(EffectType.Critical, false, true);
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[4].gameObject);
        four.name = "Critical2";
        islands[4].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Critical, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Critical, false, true);
        islands[4].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[4].my_toy);


        GameObject six = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", islands[6].gameObject);
        six.name = "Critical3";
        islands[6].my_toy.rune.Upgrade(EffectType.Focus, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Critical, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Critical, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Critical, false, true);
        islands[6].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[6].my_toy);

    }


    void Sparkles()
    {
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[0].gameObject);
        zero.name = "default";
        islands[0].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[0].my_toy);

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[2].gameObject);
        two.name = "sparkles";
        islands[2].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[2].my_toy.rune.Upgrade(EffectType.Sparkles, false, true);
        islands[2].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[2].my_toy);

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[4].gameObject);
        four.name = "sparkles2";
        //islands[4].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Sparkles, false, true);
        islands[4].my_toy.rune.Upgrade(EffectType.Sparkles, false, true);
        islands[4].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[4].my_toy);


        GameObject six = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", islands[6].gameObject);
        six.name = "sparkles2";
        //islands[6].my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Laser, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Sparkles, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Sparkles, false, true);
        islands[6].my_toy.rune.Upgrade(EffectType.Sparkles, false, true);
        islands[6].my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(islands[6].my_toy);

    }

    void Level2_Sensible()
    {
        Island_Button i = islands[2];
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        zero.name = "default0";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[3];

        GameObject one = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        one.name = "one";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[1];

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        two.name = "two";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[4];

        GameObject three = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        three.name = "three";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[5];

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        four.name = "four";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[6];

        GameObject five = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        five.name = "five";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[12];

        GameObject six = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        six.name = "six";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[8];

        GameObject seven = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        seven.name = "seven";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

    }

    void Level2_Vexing()
    {
        Island_Button i = islands[2];
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", i.gameObject);
        zero.name = "default0";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[3];

        GameObject one = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", i.gameObject);
        one.name = "one";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[1];

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", i.gameObject);
        two.name = "two";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[4];

        GameObject three = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", i.gameObject);
        three.name = "three";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[5];

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", i.gameObject);
        four.name = "four";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[6];

        GameObject five = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", i.gameObject);
        five.name = "five";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);


        i = islands[12];

        GameObject six = Peripheral.Instance.PlaceToyOnIsland("vexing_tower", i.gameObject);
        six.name = "six";
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

    }


    void Level2_Sensible_RapidFire()
    {
        Island_Button i = islands[2];
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        zero.name = "default0";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[3];

        GameObject one = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        one.name = "one";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[1];

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        two.name = "two";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[4];

        GameObject three = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        three.name = "three";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[5];

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        four.name = "four";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[6];

        GameObject five = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        five.name = "five";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[12];

        GameObject six = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        six.name = "six";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[8];

        GameObject seven = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        seven.name = "seven";
        i.my_toy.rune.Upgrade(EffectType.RapidFire, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

    }

    void Level2_Sensible_Laser()
    {
        Island_Button i = islands[2];
        GameObject zero = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        zero.name = "default0";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[3];

        GameObject one = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        one.name = "one";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[1];

        GameObject two = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        two.name = "two";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[4];

        GameObject three = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        three.name = "three";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[5];

        GameObject four = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        four.name = "four";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[6];

        GameObject five = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        five.name = "five";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[12];

        GameObject six = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        six.name = "six";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

        i = islands[8];

        GameObject seven = Peripheral.Instance.PlaceToyOnIsland("sensible_tower", i.gameObject);
        seven.name = "seven";
        i.my_toy.rune.Upgrade(EffectType.Laser, false, true);
        i.my_toy.my_tower_stats = GameStatCollector.Instance.addTowerStats(i.my_toy);

    }
}
