using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum NeedsKind
{
    social, working, eating, relation, finanse
}

public class NeedsData
{
    public float value;
    public float time;
    public bool home = true;
}

public class Face : MonoBehaviour
{

    public static readonly Dictionary<NeedsKind, NeedsData> Needs = new Dictionary<NeedsKind, NeedsData>();

    static readonly Dictionary<NeedsKind, string[]> messages = new Dictionary<NeedsKind, string[]>()
    {
        { NeedsKind.social , new string[] {
            "Read twitter messages" ,
            "Check whatsap",
            "looked at the instagram pictures",
            "see tic tok videos",
        } },
        { NeedsKind.working , new string[] {
            "check emails" ,
            "listen voice mails",
            "write some emails",
            "image self as SEO",
        } },
        { NeedsKind.eating , new string[] {
            "think about apple" ,
            "I thought of caesar salad",
            "I wonder how pumpkin soup tastes",
            "I have to buy avocados tomorrow",
        } },
        { NeedsKind.relation , new string[] {
            "do i need a prince on a white horse?" ,
            "I imagined a white horse",
            "maybe I'm bisexual",
            "how many children would I like",
        } },
        { NeedsKind.finanse , new string[] {
            "thinking about my bank account" ,
            "how many many I have",
            "maybe I need a salary supplement ",
            "how to rob a bank",
        } },


    };

    public static Vector2 CameraBound;

    static Face instance;

    [SerializeField]
    private Animator anim;


    [SerializeField]
    UIHint hint;

    [SerializeField]
    LayerMask walkable;

    [SerializeField]
    GameObject startScreen;

    [SerializeField]
    GameObject pauseScreen;

    [SerializeField]
    GameObject loseScreen;

    [SerializeField]
    GameObject losePanel;

    [SerializeField]
    GameObject victoryScreen;

    [SerializeField]
    GameObject victoryPanel;

    [SerializeField]
    AudioSource audioBack;

    [SerializeField]
    AudioSource audioVictory;

    [SerializeField]
    AudioSource audioLose;

    [SerializeField]
    AudioSource audioIdea;

    public static bool gameEnded => 
        instance.loseScreen.activeInHierarchy || 
        instance.victoryScreen.activeInHierarchy ||
        instance.startScreen.activeInHierarchy ||
        instance.pauseScreen.activeInHierarchy;

    float animTop = 0;
    float _animTop = 0;

    float animSmile = 0;
    float _animSmile = 0;

    float animLid = 0;
    float _animLid = 0;

    float loseTime = 0;
    float victoryTime = 0;

    private void Awake()
    {
        instance = this;
        foreach (NeedsKind need in Enum.GetValues(typeof(NeedsKind)))
            Needs.Add(need, new NeedsData() { value = 0, time = 0, home = true});
    }

    void Start()
    {
        CameraBound = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, - Camera.main.transform.position.z));
    }

    private void Update()
    {
        if (gameEnded) 
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseScreen.SetActive(true);
            return;
        }

        RandomizeNeeds();

        _animSmile = Mathf.MoveTowards(_animSmile, animSmile, Time.deltaTime);
        anim.SetFloat("Smile", _animSmile);

        _animTop = Mathf.MoveTowards(_animTop, animTop, Time.deltaTime);
        anim.SetFloat("Top", _animTop);

        _animLid = Mathf.MoveTowards(_animLid, animLid, Time.deltaTime);
        anim.SetFloat("Lid", _animLid);
    }

    void RandomizeNeeds()
    {
        Array arrayKind = Enum.GetValues(typeof(NeedsKind));

        foreach (NeedsKind need in arrayKind)
        {
            if (!Needs[need].home)
                continue;

            if (Needs[need].time > 0)
            {
                Needs[need].time -= Time.deltaTime;
                continue;
            }

            Needs[need].time = Random.Range(30f, 40f); ;
            Needs[need].value = Random.Range(0, 1f);
            if (Needs[need].value < 0.4f)
                Needs[need].value = 0;

            if (Needs[need].value > 0)
            {
                Needs[need].home = false;
                audioIdea.Play();
            }

        }

        animTop = 0;
        foreach (NeedsKind need in arrayKind)
        {
            if (!Needs[need].home)
            {
                animTop = 1;
                break;
            }
        }
        //Debug.LogFormat("Anim top: {0}",animTop);

        if (animTop == 0)
        {
            animLid = 1;
            victoryTime += Time.deltaTime;
            if (victoryTime > 7)
            {
                Victory();
            }
        } else
        {
            animLid = 0;
            victoryTime = 0;
        }

        float sum = 0;
        foreach (NeedsKind need in arrayKind)
        {
            sum += Needs[need].value;
        }
        
        animSmile = sum / arrayKind.Length;
        if (animSmile > 0.8f)
        {
            loseTime += Time.deltaTime;
            if (loseTime > 10)
            {
                Lose();
            }
        }
        else
            loseTime = 0;
    }

    public static Vector2 RandomPoint(NeedsKind kind)
    {
        if (Needs[kind].value <= 0)
            return Vector2.zero;

        Vector2 result = Vector2.zero;
        int cnt = 0;
        float cx = 2 + (CameraBound.x - 2f) * Needs[kind].value;
        float cy = CameraBound.y - 0.3f;
        do
        {
            result.x = Random.Range(-cx , cx);
            result.y = Random.Range(-cy, cy);
            cnt++;

        } while (Physics2D.OverlapPoint(result, instance.walkable) == null && cnt < 50);

        return result;
    }

    int click = 10;
    public static void Click (NeedsKind kind)
    {
        instance.click--;
        if (instance.click > 0)
            return;

        string[] sa = messages[kind];
        instance.hint.Caption = sa[Random.Range(0, sa.Length)];
        instance.click = Random.Range(20,35);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame() 
    {
        Needs.Clear();
        SceneManager.LoadScene("SampleScene");
    }

    public void StartGame()
    {
        startScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    void Victory()
    {
        audioBack.Stop();
        audioVictory.Play();

        victoryScreen.SetActive(true);
        victoryPanel.SetActive(true);
    }

    void Lose()
    {
        audioBack.Stop();
        audioLose.Play();

        loseScreen.SetActive(true);
        losePanel.SetActive(true);
    }


    private void OnDrawGizmos()
    {
        //CameraBound = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, -Camera.main.transform.position.z));
        //Gizmos.DrawCube(Vector3.zero, new Vector3(CameraBound.x * 2f, CameraBound.y * 2f, 1));
    }

    
}
