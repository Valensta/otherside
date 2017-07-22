
static public class GetText
{
    static public string getLabel(RewardType c)
    {
        switch (c)
        {
            case RewardType.Modulator:
                return "Use <1> wishes. So far: <2>.";
            case RewardType.Determined:
                return "Play for <1> minutes. So far: <2>.";
            case RewardType.HeroMobility:
                return "Sell <1> toys. So far: <2>.";
            case RewardType.RapidFireFinisher:
                return "Use the RapidFire upgrade <1> times. So far: <2>.";
            case RewardType.LaserFinisher:
                return "Use the Laser upgrade <1> times. So far: <2>.";
            case RewardType.CriticalFinisher:
                return "Use the Critical upgrade <1> times. So far: <2>.";
            case RewardType.SparklesFinisher:
                return "Use the Sparkles upgrade <1> times. So far: <2>.";            
            case RewardType.FearFinisher:
                return "Use the Fear upgrade <1> times. So far: <2>.";
            case RewardType.TransformFinisher:
                return "Use the Transform upgrade <1> times. So far: <2>.";
            default:
                return "Unknown reward label " + c.ToString();
        }
    }

    static public string getName(RewardType c)  //this text and descriptions are also manually set in reward intro popups because fuck you
    {
        switch (c) //these are also set in Tutorial/Rewards/reward_***_intro
        {
            case RewardType.Modulator:
                return "Optimistic Child";
            case RewardType.Determined:
                return "Determined";
            case RewardType.HeroMobility:
                return "Indecisive";
            case RewardType.LaserFinisher:
                return "Confetti Pyromaniac";
            case RewardType.RapidFireFinisher:
                return "Interruptor";
            case RewardType.FearFinisher:
                return "Fearmongerer";
            case RewardType.SparklesFinisher:
                return "Perpetual Party Crasher";
            case RewardType.CriticalFinisher:
                return "Reality Distortion Master";
            case RewardType.TransformFinisher:
                return "Improbability Field Manipulator";
            default:
                return "Unknown reward name " + c.ToString();
        }
    }

    static public string getReward(RewardType c)
    {
        switch (c)
        {
            case RewardType.Modulator:
                return "Imagination wishes horses something something. Gain the modulator. Use it to change island color.";
            case RewardType.Determined:
                return "Have some extra (whatever). You earned it.";
            case RewardType.HeroMobility:
                return "Heroes can now be relocated to any other island on the map.";
            case RewardType.LaserFinisher:
                return "Level " + StaticStat.getFinisherLvl() + " Laser finisher enabled.";
            case RewardType.RapidFireFinisher:
                return "Level " + StaticStat.getFinisherLvl() + " RapidFire finisher enabled.";
            case RewardType.FearFinisher:
                return "Level " + StaticStat.getFinisherLvl() + " Fear afflicted enemies spread fear to surrounding enemies.";
            case RewardType.SparklesFinisher:
                return "Level " + StaticStat.getFinisherLvl() + " Sparkles produce more sparkles when they hit their targets.";
            case RewardType.CriticalFinisher:
                return "Level " + StaticStat.getFinisherLvl() + " Critical attacks have a chance of killing their targets.";
            case RewardType.TransformFinisher:
                return "Level " + StaticStat.getFinisherLvl() + " Transform may turn the enemy into a giant whale.";
            default:
                return "Uknown reward " + c.ToString() + " it's probably something awesome though who knows.";
        }
    }
}