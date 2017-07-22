using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class StaticRune
{

    public static string getProperName(Toy _toy)
    {
        
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
                return "Duration Distortion Tower";
            default:
                return _toy.runetype + " " + _toy.toy_type + " Tower";
        }
    }


    

    public static float GetDistanceBonus(string toy_name, Vector3 position, Toy _toy) // Toy t is only for setting parent toy, meh
    {
        float total_bonus = 0f;
       // float building_count = 0f;
     //   Toy closest_building = null;
      //  float closest_building_distance = 999999f;

        List<Toy> toy_parents = Peripheral.Instance.toy_parents;

        if (Peripheral.Instance.toy_parents.Count > 1)
        {
            Debug.LogError("Have more than 1 toy parent!!!\n");
        }

        for (int i = 0; i < toy_parents.Count; i++)
        {
            //Debug.Log(buildings[i].building_id.target_toy_name + " " + toy.name + "\n");
            if (toy_parents[i].isToyATarget(toy_name))
            {


                float max_range = toy_parents[i].rune.getRange();
                float bonus = 0.5f;

                float eff_distance = Mathf.Max(0f, max_range - Vector2.Distance(toy_parents[i].gameObject.transform.position, position) + 0.5f);

//                if (closest_building_distance > eff_distance) { closest_building = toy_parents[i]; closest_building_distance = eff_distance; }

                float extra_slope = 1.2f;
                if (_toy != null) _toy.parent_toy = toy_parents[i];
                float final = extra_slope * bonus * eff_distance / max_range;

                //   Debug.Log("Getting distance bonus for " + toy_name + " from " + buildings[i].name + " eff_dist " + eff_distance + 
                //  " max range " + max_range + " final " + final + "\n");

                //if (_toy != null) Debug.Log("Distance: " + Vector3.Distance(buildings[i].gameObject.transform.position, _toy.gameObject.transform.position) + " max_range: " + max_range + " bonus " + final + "\n");
                total_bonus += final;
          //      building_count++;
            }
        }
      
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
        return 0.25f;
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
        //if (Central.Instance.state != GameState.InGame) return 0f;

        //if (toytype == ToyType.Hero) return 0f;  //meh this makes things confusing cuz the hero towers are identical to regular towers but somehow do less damage during the day??
        //if (rune_type == Rune)
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
                return (now == TimeName.Dawn || now == TimeName.Day) ? default_time_bonus : 0f;
            case RuneType.Slow:
                return (now == TimeName.Night || now == TimeName.Dusk) ? ghost_time_bonus : 0f;
            case RuneType.Time:
                return (now == TimeName.Night || now == TimeName.Dusk) ? ghost_time_bonus : 0f;
            case RuneType.Fast:
                return (now == TimeName.Night || now == TimeName.Dusk) ? ghost_time_bonus : 0f;
        }
        return 0f;
    }

    public static EffectType getPrimaryDamageType(RuneType r)
    {
        switch (r)
        {         
            case RuneType.Time:
                return EffectType.Magic;
            case RuneType.Fast:
                return EffectType.Force;
            case RuneType.Slow:
                return EffectType.Magic;
            default:
                Debug.LogError("THIS IS INCORROECT WHY ARE YOU DOING THIS JUST STOP\n");
                return EffectType.Force;
        }
    }

    public static EffectType getPrimaryDamageType(Rune r)
    {
        switch (r.runetype)
        {
            case RuneType.Airy:
                return EffectType.Magic;
            case RuneType.Sensible:
                if (r.getStat(EffectType.Laser).level > 0) return Get.GetDefenseType(EffectType.Laser);
                if (r.getStat(EffectType.Diffuse).level > 0) return Get.GetDefenseType(EffectType.Diffuse);
                return EffectType.Force;
            case RuneType.Vexing:
                if (r.getStat(EffectType.RapidFire).level > 0) return Get.GetDefenseType(EffectType.RapidFire);
                if (r.getStat(EffectType.Focus).level > 0) return Get.GetDefenseType(EffectType.Focus);
                return EffectType.Force;
            case RuneType.Time:                
            case RuneType.Fast:
            case RuneType.Slow:
                return getPrimaryDamageType(r.runetype);
            default:
                return EffectType.Force;
        }
    }

    public static string getPrimaryDescription(Rune r)
    {
        string[] vars;
        string desc = "";
        switch (r.runetype)
        {
            case RuneType.Sensible:
                desc = "";

                for (int i = 0; i < r.stat_sum.stats.Length; i++)
                {
                    if (!r.stat_sum.stats[i].active) continue;
                    

                    
                    
                    if (r.stat_sum.level > 0 && r.stat_sum.stats[i].effect_type == EffectType.Force
                        && (r.stat_sum.GetStatBit(EffectType.Laser) != null ||
                            r.stat_sum.GetStatBit(EffectType.Diffuse) != null)) continue;

                    if ((r.stat_sum.GetStatBit(EffectType.Laser) != null ||
                        r.stat_sum.GetStatBit(EffectType.Diffuse) != null) &&
                        r.stat_sum.stats[i].effect_type == EffectType.ReloadTime) continue;

                    if (r.stat_sum.stats[i].effect_type == EffectType.Sparkles ||
                        r.stat_sum.stats[i].effect_type == EffectType.Transform)
                    {
                        desc += "\n" + r.stat_sum.stats[i].effect_type;
                        continue;
                    }

                    if (!(r.stat_sum.stats[i].effect_type == EffectType.ReloadTime ||
                          r.stat_sum.stats[i].effect_type == EffectType.Range ||
                          r.stat_sum.stats[i].effect_type == EffectType.Force || 
                          r.stat_sum.stats[i].effect_type == EffectType.Laser ||
                          r.stat_sum.stats[i].effect_type == EffectType.Diffuse))
                    {
                        desc += "\n" + r.stat_sum.stats[i].effect_type + ":\n";
                    }
                    
                    desc += r.stat_sum.stats[i].getCompactDescription(LabelName.SkillStrength);
                    
                }

                return desc;

            case RuneType.Airy:
                desc = "";

                for (int i = 0; i < r.stat_sum.stats.Length; i++)
                {                    
                    if (!r.stat_sum.stats[i].active) continue;

                    if (r.stat_sum.stats[i].effect_type == EffectType.Force) continue;

                    if (r.stat_sum.stats[i].effect_type == EffectType.Calamity ||
                        r.stat_sum.stats[i].effect_type == EffectType.Swarm ||
                        r.stat_sum.stats[i].effect_type == EffectType.Foil ||
                        r.stat_sum.stats[i].effect_type == EffectType.Weaken)
                    {
                        desc += "\n" + r.stat_sum.stats[i].effect_type;
                        continue;
                    }

                    if (!(r.stat_sum.stats[i].effect_type == EffectType.ReloadTime ||
                          r.stat_sum.stats[i].effect_type == EffectType.Range ||
                          r.stat_sum.stats[i].effect_type == EffectType.Speed))
                    {
                        desc += "\n" + r.stat_sum.stats[i].effect_type + ":\n";
                    }

                   

                    desc += r.stat_sum.stats[i].getCompactDescription(LabelName.SkillStrength);
                }


                return desc;

            case RuneType.Vexing:
                desc = "";


                for (int i = 0; i < r.stat_sum.stats.Length; i++)
                {
                    if (!r.stat_sum.stats[i].active) continue;
                    if (r.stat_sum.level > 0 && r.stat_sum.stats[i].effect_type == EffectType.VexingForce
                        && (r.stat_sum.GetStatBit(EffectType.Focus) != null ||
                            r.stat_sum.GetStatBit(EffectType.RapidFire) != null)) continue;

                    if (r.stat_sum.level > 0 && r.stat_sum.stats[i].effect_type == EffectType.ReloadTime
                        && (r.stat_sum.GetStatBit(EffectType.Focus) != null ||
                            r.stat_sum.GetStatBit(EffectType.RapidFire) != null)) continue;

                    if (r.stat_sum.stats[i].effect_type == EffectType.Fear ||
                        r.stat_sum.stats[i].effect_type == EffectType.DOT )
                    {
                        desc += "\n" + r.stat_sum.stats[i].effect_type;
                        continue;
                    }


                    if (!(r.stat_sum.stats[i].effect_type == EffectType.ReloadTime ||
                          r.stat_sum.stats[i].effect_type == EffectType.Range ||
                          r.stat_sum.stats[i].effect_type == EffectType.VexingForce||
                          r.stat_sum.stats[i].effect_type == EffectType.Focus ||
                          r.stat_sum.stats[i].effect_type == EffectType.RapidFire))
                    {
                        desc += "\n" + r.stat_sum.stats[i].effect_type + ":\n";
                    }

                    desc += r.stat_sum.stats[i].getCompactDescription(LabelName.SkillStrength);
                }


                return desc;

            case RuneType.Slow:
                desc = "Do <0> every <1>";
                vars = new string[2];

                vars[0] = r.getStatBit(EffectType.Force).getDetailStats()[0].toString();
                vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();

                desc = Show.FixText(desc, vars);
                return desc;
            case RuneType.Time:
                desc = "Slow down enemies by <0> every <1>";
                vars = new string[2];

                vars[0] = r.getStatBit(EffectType.Speed).getDetailStats()[0].toString();
                vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();

                desc = Show.FixText(desc, vars);
                return desc;
            case RuneType.Fast:
                desc = "Do <0> every <1>";
                vars = new string[2];

                vars[0] = r.getStatBit(EffectType.Force).getDetailStats()[0].toString();
                vars[1] = r.getStatBit(EffectType.ReloadTime).getDetailStats()[0].toString();

                desc = Show.FixText(desc, vars);
                return desc;
        }
        return desc;
    }

    public static void assignStatBits(ref StatBit[] stats, Rune rune)
    {        
        StatReq[] rt = new StatReq[1];
        bool is_hero = (rune.toy_type == ToyType.Hero);
        bool is_active = true;
        RuneType rune_type = rune.runetype;
        int i = 0;
        switch (rune_type)
        {
            case RuneType.Sensible:
                if (is_hero)
                    stats = new StatBit[9];
                else
                    stats = new StatBit[7];

                rt[0] = new StatReq(EffectType.ReloadTime, 0);
                stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.ReloadTime, is_active, rt, 0, 0, is_hero);

                StatReq[] r = new StatReq[1];
                r[0] = new StatReq(EffectType.Range, 0);
                stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.Range, is_active, r, 0, 0, is_hero);

                StatReq[] f = new StatReq[1];
                f[0] = new StatReq(EffectType.Force, 0);
                stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.Force, is_active, f, 0, 0, is_hero);

                is_active = false;
                // LASER    +      DIFFUSE


                /// LASER BRANCH
                StatReq[] l = new StatReq[1];
                l[0] = new StatReq(EffectType.Diffuse, -1);
                stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.Laser, is_active, l, 0, 0, is_hero);

                StatReq[] l2 = new StatReq[1];
                l2[0] = new StatReq(EffectType.Laser, 1);
                stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.Sparkles, is_active, l2, 0, 0, is_hero);


                //vexing Diffuse
                StatReq[] no_laser = new StatReq[1];
                no_laser[0] = new StatReq(EffectType.Laser, -1);
                stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.Diffuse, is_active, no_laser, 0, 0, is_hero);

                StatReq[] have_diffuse = new StatReq[1];
                have_diffuse[0] = new StatReq(EffectType.Diffuse, 1);
                stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.Transform, is_active, have_diffuse, 0, 0, is_hero);

                /// HERO STUFF
                if (is_hero)
                {
                    StatReq[] aa = new StatReq[1];
                    aa[0] = new StatReq(EffectType.Force, 0);
                    stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.AirAttack, is_active, aa, StaticStat.getInitRechargeTime(EffectType.AirAttack), 0, is_hero);
                   
                    stats[i++] = new StatBit(rune, RuneType.Sensible, EffectType.Meteor, is_active, aa, StaticStat.getInitRechargeTime(EffectType.Meteor), 0, is_hero);
                }
                rune.stats = stats;
                return;
            case RuneType.Airy:

                if (is_hero)
                    stats = new StatBit[11];
                else
                    stats = new StatBit[8];

                StatReq[] art = new StatReq[1];
                art[0] = new StatReq(EffectType.ReloadTime, 0);
                stats[0] = new StatBit(rune, RuneType.Airy, EffectType.ReloadTime, is_active, art, 0, 0, is_hero);

                StatReq[] ar = new StatReq[1];
                ar[0] = new StatReq(EffectType.Range, 0);
                stats[1] = new StatBit(rune, RuneType.Airy, EffectType.Range, is_active, ar, 0, 0, is_hero);

                StatReq[] def = new StatReq[1];
                def[0] = new StatReq(EffectType.Speed, 0);
                stats[2] = new StatBit(rune, RuneType.Airy, EffectType.Speed, is_active, def, 0, 0, is_hero);

                StatReq[] af = new StatReq[1];
                af[0] = new StatReq(EffectType.Force, 0); //paired with speed, updateded when speed is upgraded
                stats[3] = new StatBit(rune, RuneType.Airy, EffectType.Force, is_active, af, 0, 0, is_hero);

                //UPGRADES
                is_active = false;

                StatReq[] m = new StatReq[1];
                m[0] = new StatReq(EffectType.Weaken, -1);
                stats[4] = new StatBit(rune, RuneType.Airy, EffectType.Calamity, is_active, m, 0, 0, is_hero);

                StatReq[] sm = new StatReq[1];
                sm[0] = new StatReq(EffectType.Calamity, 1);                
                stats[5] = new StatBit(rune, RuneType.Airy, EffectType.Swarm, is_active, sm, 0, 0, is_hero);

                StatReq[] w = new StatReq[1];
                w[0] = new StatReq(EffectType.Calamity, -1);
                stats[6] = new StatBit(rune, RuneType.Airy, EffectType.Weaken, is_active, w, 0, 0, is_hero);

                StatReq[] ww = new StatReq[1];
                ww[0] = new StatReq(EffectType.Weaken, 1);                
                stats[7] = new StatBit(rune, RuneType.Airy, EffectType.Foil, is_active, ww, 0, 0, is_hero);



                if (is_hero)
                {
                    stats[8] = new StatBit(rune, RuneType.Airy, EffectType.Frost, is_active, def, StaticStat.getInitRechargeTime(EffectType.Frost), 0, is_hero);
                    //same requirements anyway
                    stats[9] = new StatBit(rune, RuneType.Airy, EffectType.EMP, is_active, def, StaticStat.getInitRechargeTime(EffectType.EMP), 0, is_hero);
                    stats[10] = new StatBit(rune, RuneType.Airy, EffectType.Plague, is_active, def, StaticStat.getInitRechargeTime(EffectType.Plague), 0, is_hero);
                }

                rune.stats = stats;
                return;
            case RuneType.Vexing:

                if (is_hero)
                    stats = new StatBit[10];
                else
                    stats = new StatBit[8];

                StatReq[] vrt = new StatReq[1];
                vrt[0] = new StatReq(EffectType.ReloadTime, 0);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.ReloadTime, is_active, vrt, 0, 0, is_hero);

                StatReq[] vr = new StatReq[1];
                vr[0] = new StatReq(EffectType.Range, 0);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.Range, is_active, vr, 0, 0, is_hero);

                StatReq[] fr = new StatReq[1];
                fr[0] = new StatReq(EffectType.VexingForce, 0);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.VexingForce, is_active, fr, 0, 0, is_hero);

                //UPGRADES
                is_active = false;

                // RAPID FIRE    +   FOCUS

                // FOCUS
                StatReq[] no_rapidfire = new StatReq[1];
                no_rapidfire[0] = new StatReq(EffectType.RapidFire, -1);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.Focus, is_active, no_rapidfire, 0, 0, is_hero);

                StatReq[] have_focus = new StatReq[1];
                have_focus[0] = new StatReq(EffectType.Focus, 1);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.Fear, is_active, have_focus, 0, 0, is_hero);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.Critical, is_active, have_focus, 0, 0, is_hero);



                // RAPID FIRE
                StatReq[] rapid_fire = new StatReq[1];
                rapid_fire[0] = new StatReq(EffectType.Focus, -1);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.RapidFire, is_active, rapid_fire, 0, 0, is_hero);

                StatReq[] DOT = new StatReq[1];
                DOT[0] = new StatReq(EffectType.RapidFire, 1);
                stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.DOT, is_active, DOT, 0, 0, is_hero);


                //vexing Focus


                if (rune.toy_type == ToyType.Hero)
                {
                    StatReq[] t = new StatReq[1];
                    t[0] = new StatReq(EffectType.Teleport, 0);
                    stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.Teleport, is_active, t, StaticStat.getInitRechargeTime(EffectType.Teleport), 0, is_hero);

                    StatReq[] b = new StatReq[1];
                    b[0] = new StatReq(EffectType.Bees, 0);
                    stats[i++] = new StatBit(rune, RuneType.Vexing, EffectType.Bees, is_active, b, StaticStat.getInitRechargeTime(EffectType.Bees), 0, is_hero);
                }

                rune.stats = stats;
                return;
            case RuneType.Slow:
                stats = new StatBit[3];                
               
                StatReq[] slrt = new StatReq[1];
                slrt[0] = new StatReq(EffectType.ReloadTime, 0);
                stats[0] = new StatBit(rune, RuneType.Slow, EffectType.ReloadTime, is_active, slrt, 0, 0, is_hero);                

                StatReq[] slr = new StatReq[1];
                slr[0] = new StatReq(EffectType.Range, 0);
                stats[1] = new StatBit(rune, RuneType.Slow, EffectType.Range, is_active, slr, 0, 0, is_hero);                

                StatReq[] slf = new StatReq[1];
                slf[0] = new StatReq(EffectType.Force, 0);
                stats[2] = new StatBit(rune, RuneType.Slow, EffectType.Force, is_active, slf, 0, 0, is_hero);                
                rune.stats = stats;
                return;

            case RuneType.Fast:
                stats = new StatBit[3];                

                StatReq[] frt = new StatReq[1];
                frt[0] = new StatReq(EffectType.ReloadTime, 0);
                stats[0] = new StatBit(rune, RuneType.Fast, EffectType.ReloadTime, is_active, frt, 0, 0, is_hero);                

                StatReq[] far = new StatReq[1];
                far[0] = new StatReq(EffectType.Range, 0);
                stats[1] = new StatBit(rune, RuneType.Fast, EffectType.Range, is_active, far, 0, 0, is_hero);
                

                StatReq[] f2 = new StatReq[1];
                f2[0] = new StatReq(EffectType.Force, 0);
                stats[2] = new StatBit(rune, RuneType.Fast, EffectType.Force, is_active, f2, 0, 0, is_hero);                
                rune.stats = stats;
                return;

            case RuneType.Time:
                stats = new StatBit[3];                

                StatReq[] rt2 = new StatReq[1];
                rt2[0] = new StatReq(EffectType.ReloadTime, 0);
                stats[0] = new StatBit(rune, RuneType.Time, EffectType.ReloadTime, is_active, rt2, 0, 0, is_hero);                

                StatReq[] r2 = new StatReq[1];
                r2[0] = new StatReq(EffectType.Range, 0);
                stats[1] = new StatBit(rune, RuneType.Time, EffectType.Range, is_active, r2, 0, 0, is_hero);                

                StatReq[] f3 = new StatReq[1];
                f3[0] = new StatReq(EffectType.Speed, 0);
                stats[2] = new StatBit(rune, RuneType.Time, EffectType.Speed, is_active, f3, 0, 0, is_hero);
                stats[2].effect_sub_type = EffectSubType.Ultra;
                rune.stats = stats;
                return;
            case RuneType.Modulator:
                stats = new StatBit[0];
                rune.stats = stats;
                return;

            case RuneType.Castle:
                //Castle = + health, + wish % , construction efficiency                
                stats = new StatBit[2];

                is_active = false;
                StatReq[] ph = new StatReq[1];
                ph[0] = new StatReq(EffectType.Renew, 0);
                stats[0] = new StatBit(rune, RuneType.Castle, EffectType.Renew, is_active, ph, 0, 0, is_hero);               

                StatReq[] ce = new StatReq[1];
                ce[0] = new StatReq(EffectType.Architect, 0);
                stats[1] = new StatBit(rune, RuneType.Castle, EffectType.Architect, is_active, ce, 0, 0, is_hero);
                rune.stats = stats;
                return;
            case RuneType.SensibleCity:
                //Sensible city = summon sensible temporary towers, base ammo
                stats = new StatBit[3];

                StatReq[] tr = new StatReq[1];
                tr[0] = new StatReq(EffectType.TowerRange, 0);
                stats[0] = new StatBit(rune, RuneType.SensibleCity, EffectType.TowerRange, is_active, tr, 0, 0, is_hero);

                StatReq[] tf = new StatReq[1];
                tf[0] = new StatReq(EffectType.TowerForce, 0);
                stats[1] = new StatBit(rune, RuneType.SensibleCity, EffectType.TowerForce, is_active, tf, 0, 0, is_hero);

                StatReq[] scr = new StatReq[1];
                scr[0] = new StatReq(EffectType.Range, 0);
                stats[2] = new StatBit(rune, RuneType.SensibleCity, EffectType.Range, is_active, scr, 0, 0, is_hero);                
                rune.stats = stats;
                return;
            default:
                return;
        }
     
    }
}

