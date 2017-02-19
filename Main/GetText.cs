
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
            default:
                return "Unknown reward label " + c.ToString();
        }
    }

    static public string getName(RewardType c)
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
                return "Level 3 Laser finisher enabled.";
            case RewardType.RapidFireFinisher:
                return "Level 3 RapidFire finisher enabled.";
            case RewardType.FearFinisher:
                return "Level 3 Fear afflicted enemies spread Fear to surrounding enemies.";
            case RewardType.SparklesFinisher:
                return "Level 3 Sparkles produce more sparkles when they hit their targets.";
            case RewardType.CriticalFinisher:
                return "Level 3 Critical attacks have a chance of killing their targets.";
            default:
                return "Uknown reward " + c.ToString() + " it's probably something awesome though who knows.";
        }
    }
}