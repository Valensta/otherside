using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class StaticRune
{

    public static string getProperName(Toy _toy)
    {
        string name = "";
        switch (_toy.runetype)
        {
            case RuneType.Sensible:
                return (_toy.toy_type == ToyType.Hero) ? "Sensible Hero Blah" : "Sensible Tower";
            case RuneType.Airy:
                return (_toy.toy_type == ToyType.Hero) ? "Airy Hero Blah" : "Airy Tower";
            case RuneType.Vexing:
                return (_toy.toy_type == ToyType.Hero) ? "Vexing Hero Blah" : "Vexing Tower";
            case RuneType.Castle:
                return "Castle";
            case RuneType.Fast:
                return "Fast Night Tower";
            case RuneType.Modulator:
                return "Island Modulator";
            case RuneType.SensibleCity:
                return "Night Research Tower";
            case RuneType.Slow:
                return "Slow Night Tower";
            case RuneType.Special:
                return "Special??!";
            case RuneType.Time:
                return "Time Distortion Tower";
            default:
                return _toy.runetype + " " + _toy.toy_type + " Tower";
        }
    }


    

    public static float GetDistanceBonus(string toy_name, Vector3 position, Toy _toy) // Toy t is only for setting parent toy, meh
    {
        float total_bonus = 0f;
        float building_count = 0f;
        Toy closest_building = null;
        float closest_building_distance = 999999f;

        List<Building> buildings = Peripheral.Instance.buildings;


        for (int i = 0; i < buildings.Count; i++)
        {
            //Debug.Log(buildings[i].building_id.target_toy_name + " " + toy.name + "\n");
            if (buildings[i].isToyATarget(toy_name))
            {


                float max_range = buildings[i].rune.getRange();
                float bonus = 0.5f;

                float eff_distance = Mathf.Max(0f, max_range - Vector2.Distance(buildings[i].gameObject.transform.position, position) + 0.5f);

                if (closest_building_distance > eff_distance) { closest_building = buildings[i]; closest_building_distance = eff_distance; }

                float extra_slope = 1.2f;

                float final = extra_slope * bonus * eff_distance / max_range;

                //   Debug.Log("Getting distance bonus for " + toy_name + " from " + buildings[i].name + " eff_dist " + eff_distance + 
                //  " max range " + max_range + " final " + final + "\n");

                //if (_toy != null) Debug.Log("Distance: " + Vector3.Distance(buildings[i].gameObject.transform.position, _toy.gameObject.transform.position) + " max_range: " + max_range + " bonus " + final + "\n");
                total_bonus += final;
                building_count++;
            }
        }
        if (building_count > 1)
        {
            float factor = (2f * building_count - 1) / building_count;
            total_bonus = total_bonus / factor;
        }

        if (_toy != null && closest_building != null)
        {
            _toy.parent_toy = closest_building;
        }
        //    Debug.Log("Distance bonus is " + total_bonus + "\n");
        return total_bonus;

    }



    public static float time_bonus_aff(float stat, EffectType type, RuneType runetype, ToyType toy_type, float distance_bonus)
    {
        return time_bonus_aff(stat, type, GetTimeBonus(runetype, toy_type), distance_bonus);

    }

    public static float time_bonus_aff(float f, EffectType type, float current_time_bonus, float distance_bonus)
    {
        //     Debug.Log("Distance bonus " + distance_bonus + "\n");
        if (type == EffectType.ReloadTime) return f / ((1 + current_time_bonus) * (1f + distance_bonus));
        if (type == EffectType.Range) current_time_bonus = 0f;
        return f * (1 + current_time_bonus) * (1f + distance_bonus);
    }


    public static float getAiryDamageFrequency()
    {
        return 0.1f;
    }

    public static float getLavaDamageFrequency()
    {
        return 0.1f;
    }

    public static float getAiryDurationFactor()
    {
        return 0.5f;
    }


    public static float GetTimeBonus(RuneType rune_type, ToyType toytype)
    {
        if (Central.Instance.state != GameState.InGame) return 0f;

        if (toytype == ToyType.Hero) return 0f;
        TimeName now = Sun.Instance.GetCurrentTime();

        float default_time_bonus = 0.33f;
        float ghost_time_bonus = 0.33f;
        switch (rune_type)
        {
            case RuneType.Sensible:
                return (now == TimeName.Dawn || now == TimeName.Day) ? default_time_bonus : 0f;
            case RuneType.Airy:
                return (now == TimeName.Dawn || now == TimeName.Day) ? default_time_bonus : 0f;
            case RuneType.Vexing:
                return (now == TimeName.Dawn || now == TimeName.Day) ? ghost_time_bonus : 0f;
            case RuneType.Slow:
                return (now == TimeName.Night || now == TimeName.Dusk) ? ghost_time_bonus : 0f;
            case RuneType.Time:
                return (now == TimeName.Night || now == TimeName.Dusk) ? ghost_time_bonus : 0f;
            case RuneType.Fast:
                return (now == TimeName.Night || now == TimeName.Dusk) ? ghost_time_bonus : 0f;
        }
        return 0f;
    }

    public static string getPrimaryDescription(Rune r)
    {
        string[] vars;
        string desc = "";
        switch (r.runetype)
        {
            case RuneType.Sensible:


                if (r.level == 0)
                {
                    desc = "Do <1> every <2>\n";
                    vars = new string[2];
                    vars[0] = r.getStatBit(EffectType.Force).getDetailStats()[0].toString();
                    vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();
                    desc = Show.FixText(desc, vars);
                }
                else
                {
                    for (int i = 0; i < r.stat_sum.stats.Length; i++)
                    {
                        if (r.stat_sum.stats[i].Level == 0) continue;
                        desc += r.stat_sum.stats[i].getCompactDescription(LabelName.SkillStrength) + " ";
                    }
                }

                return desc;

            case RuneType.Airy:
                desc = "Slow down enemies by <1> and do <2> per second every <3>\n";
                vars = new string[3];
                vars[0] = r.getStatBit(EffectType.Speed).getDetailStats()[0].toString();

                MyFloat reload_time = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0];
                vars[2] = reload_time.toString();

                MyFloat[] f = r.getStatBit(EffectType.Force).getDetailStats();
                f[0].num *= reload_time.num * StaticRune.getAiryDurationFactor() / StaticRune.getAiryDamageFrequency();
                vars[1] = f[0].toString();

                desc = Show.FixText(desc, vars);

                if (r.level > 0)
                {
                    for (int i = 0; i < r.stat_sum.stats.Length; i++)
                    {
                        if (r.stat_sum.stats[i].Level == 0) continue;
                        desc += r.stat_sum.stats[i].getCompactDescription(LabelName.SkillStrength) + " ";
                    }
                }

                return desc;

            case RuneType.Vexing:

                desc = "Do <1> every <2>\n";
                vars = new string[2];

                vars[0] = r.getStatBit(EffectType.VexingForce).getDetailStats()[0].toString();
                vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();

                desc = Show.FixText(desc, vars);

                if (r.level > 0)
                {
                    for (int i = 0; i < r.stat_sum.stats.Length; i++)
                    {
                        if (r.stat_sum.stats[i].Level == 0) continue;
                        desc += r.stat_sum.stats[i].getCompactDescription(LabelName.SkillStrength) + " ";
                    }
                }

                return desc;

            case RuneType.Slow:
                desc = "Do <1> every <2>";
                vars = new string[2];

                vars[0] = r.getStatBit(EffectType.Force).getDetailStats()[0].toString();
                vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();

                desc = Show.FixText(desc, vars);
                return desc;
            case RuneType.Time:
                desc = "Slow down enemies by <1> every <2>";
                vars = new string[2];

                vars[0] = r.getStatBit(EffectType.Speed).getDetailStats()[0].toString();
                vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();

                desc = Show.FixText(desc, vars);
                return desc;
            case RuneType.Fast:
                desc = "Do <1> every <2>";
                vars = new string[2];

                vars[0] = r.getStatBit(EffectType.Force).getDetailStats()[0].toString();
                vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();

                desc = Show.FixText(desc, vars);
                return desc;
        }
        return desc;
    }


}

