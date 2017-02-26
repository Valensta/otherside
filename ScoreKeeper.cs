using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ScoreDetails
{
    public float difficulty;
    public int level;
    public float score;

    public ScoreDetails()
    {
    }

    public ScoreDetails (float _diff, int _lvl, float _score)
    {
        difficulty = _diff;
        level = _lvl;
        score = _score;
    }
}

public class ScoreKeeper : MonoBehaviour {  // THIS IS INTRA LEVEL
    float total_score; // not same as sum of score_details, cuz you could have used some up
    float possible_dreams = 1;
    float possible_wishes = 1;
    float dreams_factor = 100f;
    float wishes_factor = 25f;
    
    
    float prev_sensible_wish = 0;
    float current_difficulty;
    int current_lvl;
    float health_factor = 10f;

    List<ScoreDetails> score_details = new List<ScoreDetails>();

    private static ScoreKeeper instance;
    public static ScoreKeeper Instance { get; private set; }

    public delegate void ScoreUpdateHandler(float price);
    public static event ScoreUpdateHandler onScoreUpdate;


    void Awake()
    { Debug.Log("scorekeeper awake\n");


        if (Instance != null && Instance != this) {
            Debug.Log("scorekeeper got destroyeed\n");
            Destroy(gameObject);
        }
        Instance = this;
       // Inventory.onWishChanged += onWishChanged;
       // Peripheral.onDreamsAdded += onDreamsAdded;
    }

    public float getPossibleWishes()
    {
        return possible_wishes;
    }

    public float getPossibleDreams()
    {
        return possible_dreams;
    }

    public void setPossibleWishes(float possible_wishes)
    {
        this.possible_wishes = possible_wishes;
    }

    public void setPossibleDreams(float possible_dreams)
    {
        this.possible_dreams = possible_dreams;
    }

    public void SetLevel(int level, float difficulty)
    {
 //       Debug.Log("Setting level for ScoreKeeper " + level + " difficulty " + difficulty + "\n");
        if (checkIfAlreadyHaveScore(level, difficulty))
        {
      //      Debug.Log("Already found a score for level " + level + " difficulty " + difficulty + "\n");
            Inventory.onWishChanged -= onWishChanged;
            Peripheral.onDreamsAdded -= onDreamsAdded;
        }
        else
        {
            Inventory.onWishChanged += onWishChanged;
            Peripheral.onDreamsAdded += onDreamsAdded;
        }
        current_lvl = level;
        current_difficulty = difficulty;
    }

   


    public void useScore(float use_score)
    {
        total_score -= use_score;
        if (onScoreUpdate != null) onScoreUpdate(total_score);
    }

    public float getTotalScore()
    {

        return total_score;
    }

    public void setTotalScore(float total_score)
    {
        this.total_score = total_score;
    }

    public float getCurrentLevelScore()
    {
        foreach (ScoreDetails sd in score_details)
        {
            if (sd.level == current_lvl && sd.difficulty == current_difficulty) return sd.score;
        }
        return 0f;
    }

    public void SetScore(float i)
    {
        total_score = i;
        score_details.Add(new ScoreDetails(current_difficulty, current_lvl, i));
        if (onScoreUpdate != null) onScoreUpdate(total_score);
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
        //SetScore(0);
        score_details = new List<ScoreDetails>();
  //      Debug.Log("Resetting scorekeeper\n");
        possible_dreams = 1;
        possible_wishes = 1;
        
    }
    

    public void onDreamsAdded(float d, Vector3 pos)
    {
      //  Debug.Log("On dreams added " + d + "\n");
        
            if (d > 0) possible_dreams += d;
    }

    public void onWishChanged(Wish w, bool added, bool visible, float delta)
    {
        //      Debug.Log("On wishes added " + delta + "\n");
        if (w.type != WishType.Sensible) return;
        if (w.Strength - prev_sensible_wish > 0)
            possible_wishes += w.Strength - prev_sensible_wish;

        prev_sensible_wish = w.Strength;
    }

    bool checkIfAlreadyHaveScore(int level, float difficulty)
    {
        foreach (ScoreDetails sd in score_details)
        {
            if (sd.level == level)
            {
                if (difficulty <= difficulty) return true;
            }
        }
        return false;
    }

    public void CalcScore(float remaining_dreams, float remaining_health, float remaining_wishes){
 
        if (checkIfAlreadyHaveScore(current_lvl, current_difficulty))
        {
            Debug.Log("Calculating score for a level that already has a score, not.\n");
            return;
        }


        float dream_score = (1f - remaining_dreams / possible_dreams) * dreams_factor;
        float wish_score = (1f - remaining_wishes / possible_wishes) * wishes_factor;
        float health_score = remaining_health * health_factor;
        Debug.Log("remaining dreams " + remaining_dreams + " possible dreams " + possible_dreams + "\n");
        Debug.Log("remaining wishes " + remaining_wishes + " possible wishes " + possible_wishes + "\n");

        Debug.Log("Score: dreams - " + dream_score + " wishes " + wish_score + " health " + health_score + "\n");
        float new_score = dream_score + wish_score + health_score + 50;
        
        
        
        SetScore(total_score + new_score);
        
	}

}