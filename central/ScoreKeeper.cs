using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.UI;
[System.Serializable]
public class ScoreDetails
{
    public Difficulty difficulty;
    public int level;
    public int score;
    

    public ScoreDetails()
    {
    }

    public ScoreDetails (Difficulty _diff, int _lvl, int _score)
    {
        difficulty = _diff;
        level = _lvl;
        score = _score;
    }
}

public class ScoreKeeper : MonoBehaviour {  // THIS IS INTRA LEVEL
    int total_score; // not same as sum of score_details, cuz you could have used some up
    float possible_dreams = 1;
    List<WishDial> possible_wishes = new List<WishDial>();
    float dreams_factor = 100f;
    //float wishes_factor = 25f;
    public Text score_details_text;
    public Text new_level_score_text;
    public Text total_level_score_text;
    
    bool keeping_score = false;
    float prev_sensible_wish = 0;
    Difficulty current_difficulty;
    int current_lvl;
    float health_factor = 16f;
    private float dream_factor = 0.5f;
    float level_time = 0f;
    public GameObject won_visual;         //if ya was keeping score
    public GameObject won_replay_visual;  //if wasn't because already played this combination
    

    List<ScoreDetails> score_details = new List<ScoreDetails>();

    private static ScoreKeeper instance;
    public static ScoreKeeper Instance { get; private set; }

    public delegate void ScoreUpdateHandler(int price);
    public static event ScoreUpdateHandler onScoreUpdate;


    void Awake()
    {// Debug.Log("scorekeeper awake\n");


        if (Instance != null && Instance != this) {
            Debug.Log("scorekeeper got destroyeed\n");
            Destroy(gameObject);
        }
        Instance = this;
       // Inventory.onWishChanged += onWishChanged;
       // Peripheral.onDreamsAdded += onDreamsAdded;
    }

    public WishDial getPossibleWish(WishType type)
    {
        foreach (WishDial d in possible_wishes) if (d.type == type) return d;
        return null;
    }

    public List<WishDial> getPossibleWishes()
    {
        return CloneUtil.copyList<WishDial>(possible_wishes);
    }

    public float getPossibleDreams()
    {
        return possible_dreams;
    }

    public void setPossibleWishes(List<WishDial> possible_wishes)
    {
        
        this.possible_wishes = CloneUtil.copyList<WishDial>(possible_wishes);
        
    }

    public void setPossibleDreams(float possible_dreams)
    {
        this.possible_dreams = possible_dreams;
    }

    public void OnDisable()
    {
        Inventory.onWishChanged -= onWishChanged;
        Peripheral.onDreamsAdded -= onDreamsAdded;
    }

    public void OnEnable()
    {
        Inventory.onWishChanged += onWishChanged;
        Peripheral.onDreamsAdded += onDreamsAdded;
    }
    public void SetLevel(int level, Difficulty difficulty)
    {
        Debug.Log("Setting level for ScoreKeeper " + level + " difficulty " + difficulty + "\n");
        //keeping_score = !checkIfAlreadyHaveScore(level, difficulty);
        keeping_score = true;
      
        current_lvl = level;
        current_difficulty = difficulty;        
            //possible_wishes = new List<WishDial>();
            //possible_dreams = 1f;
        
    }

   


    public void useScore(int use_score)
    {
        total_score -= use_score;
        Debug.Log("SCORE\n");
        if (onScoreUpdate != null) onScoreUpdate(total_score);
    }



    public int getTotalScore()
    {

        return total_score;
    }


    public int getLevelScore(int level, Difficulty difficulty)
    {
        foreach (ScoreDetails sd in score_details)
        {
            if (sd.level == level && sd.difficulty == difficulty) return sd.score;
        }
        return 0;
    }


    public int getCurrentLevelScore()
    {
        return getLevelScore(current_lvl, current_difficulty);
        
    }

    public float getLevelTime()
    {
        return level_time;
    }

    public void setLevelTime(float t)
    {
        level_time = t;             
    }

    public void SetTotalScore(int i)
    {
        total_score = i;
//        Debug.Log("SCORE\n");
        if (onScoreUpdate != null) onScoreUpdate(total_score);
    }

    public int SetCurrentScore(int i)
    {
        total_score += i;
        bool good = false;
        foreach (ScoreDetails sd in score_details)
        {
            if (sd.level == current_lvl && sd.difficulty == current_difficulty) {
                sd.score += i;
                good = true;
            }
        }
    
        if (!good){
            score_details.Add(new ScoreDetails(current_difficulty, current_lvl, i));
        }
        Debug.Log("SCORE\n");
        if (onScoreUpdate != null) onScoreUpdate(total_score);
        return total_score;
    }

    public List<ScoreDetails> getScoreDetails()
    {
        return score_details;
    }

    public void setScoreDetails(List<ScoreDetails> score_details)
    {
        this.score_details = score_details;
    }

    public void Reset()
    {  
        score_details = new List<ScoreDetails>();
        ResetMidLevelScore();        
    }
    
    public void ResetMidLevelScore()
    {
       // Debug.Log("Resetting midlevel score\n");
        possible_dreams = 1;
        possible_wishes = new List<WishDial>();
        level_time = 0;
    }

    public void onDreamsAdded(float d, Vector3 pos)
    {
      //  Debug.Log("On dreams added " + d + "\n");
        
            if (d > 0) possible_dreams += d;
    }

    public void onWishChanged(Wish w, bool added, bool visible, float delta)
    {
        
        //if (w.type != WishType.Sensible) return;
        WishDial possible = getPossibleWish(w.type);
        if (possible == null)
        {
            WishDial add_me = new WishDial();
            add_me.type = w.type;
            ListUtil.Add<WishDial>(ref this.possible_wishes, add_me);
            possible = getPossibleWish(w.type);
        }

        if (w.type == WishType.Sensible)
        {
       //     Debug.Log("On wishes added " + w.type + " ADD " + added + "\n");
            int add = (int)w.Strength; // I have no idea why this is so

            //adjustment = previous wish count
            if (add - (int)possible.adjustment > 0)
                possible.count += add - (int)possible.adjustment;

            possible.adjustment = add;
        }else
        {
            int add = w.count;
    //        Debug.Log("On wishes added " + w.type + " ADD " + add + "\n");
            if (added) possible.count += add;
        }
    }

    public bool checkIfAlreadyHaveScore()
    {
        foreach (ScoreDetails sd in score_details)
        {
            if (sd.level == current_lvl)
            {
                return true;
            }
        }
        return false;
    }

    
    
   public bool checkIfAlreadyHaveScore(int level, Difficulty difficulty)
    {
        foreach (ScoreDetails sd in score_details)
        {
            if (sd.level == level)
            {
                if (sd.difficulty == difficulty) return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (keeping_score)
        {
            if (Time.timeScale == 0)
            {
                level_time += Time.unscaledDeltaTime * 0.1f;
            }
            else
            {
                level_time += Time.unscaledDeltaTime;
            }

        }
    }

    public void CalcScore(float remaining_dreams, float remaining_health)
    {
        /*
               if (!keeping_score)
               {
                   Debug.Log("Calculating score for a level that already has a score, not.\n");
                   won_replay_visual.SetActive(true);
                   won_visual.SetActive(false);
                   return;
               }
               */
        float penalty_multiplier = (checkIfAlreadyHaveScore(current_lvl, current_difficulty)) ? 0.5f : 1f;

        penalty_multiplier = (current_lvl > 0) ? penalty_multiplier : penalty_multiplier / 3f;

        Debug.Log("Calculating score with penalty " + penalty_multiplier + "\n");
        won_replay_visual.SetActive(false);
        won_visual.SetActive(true);



        float expected_level_time = 30f; //in seconds, minimum for preparation or whatever

        foreach (wave w in Moon.Instance.Waves)
        {
            float extra = 0f;
            foreach (InitWavelet wlet in w.wavelets)
            {
                extra += wlet.end_wait;
            }
            expected_level_time += w.total_run_time + extra;
        }
        float time_factor = 0.25f;

        //so tiny because we decided to device score by 5
        float difficulty_mult = (current_difficulty == Difficulty.Normal)
            ? 0.16f
            : (current_difficulty == Difficulty.Hard)
                ? 0.4f
                : 0.6f;

        float final_mult = difficulty_mult * penalty_multiplier;
        int dream_score = Mathf.FloorToInt(remaining_dreams * final_mult * dream_factor);
        int health_score = Mathf.FloorToInt(remaining_health * health_factor * final_mult);

        int time_score = Mathf.Max(0,
            Mathf.FloorToInt((expected_level_time - level_time) * time_factor * final_mult));
        
        time_score = (time_score > 10) ? 10 : time_score;

        int new_score = Mathf.FloorToInt(dream_score + time_score + health_score);

        
        string details = (Central.Instance.level_list.test_mode) ? 
                              $"Score: {Get.Round(final_mult,3)}/{current_difficulty}\ndreams {dream_score}: {remaining_dreams} * {dream_factor}" 
                            + $"\nhealth {health_score}: {remaining_health} * {health_factor}"
                            + $"\ntime {time_score}: ({expected_level_time} - {level_time}) * {time_factor}" :
                                     $"Health: {health_score}\nEfficiency: {dream_score}\nTime Score: {time_score}";


        score_details_text.text = details;
        new_level_score_text.text = new_score.ToString();

        

        Tracker.Log(PlayerEvent.FinishedLevel,true,
            customAttributes: new Dictionary<string, string>() { { "attribute_1", details } },
            customMetrics: new Dictionary<string, double>() { { "metric_1", new_score }, { "metric_2", total_score } });
        

        
        total_level_score_text.text = SetCurrentScore(new_score).ToString();
    }

}