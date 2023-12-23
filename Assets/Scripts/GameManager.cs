using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // coffee
    [SerializeField] private List<GameObject> CoffePrefab = new List<GameObject>();
    [SerializeField] private Color[] coffeeColors = new[] { Color.white} ;
    
    
    public bool isMoving = false;

    public float counter = 0;

    //Timer And ImageFill
    public Slider timerImage;
    public float totalTime = 10.0f; 
    private float currentTime;

    // spoon movement 
    [SerializeField] GameObject spoon;
    [SerializeField] float spoonSpeed;

    Vector2 pos;
    Rigidbody2D rb;
    Camera cam;
    
    private bool gameStarted;
    private bool gameEnded;
    private bool isWinning;
    [SerializeField] private UnityEvent OnWinning, OnLoosing;
    
    private void Awake()
    {
        rb = spoon.GetComponent<Rigidbody2D>();
        Time.timeScale = 1f;

    }
    void Start()
    {
        pos = rb.position;
        cam = Camera.main;
    }

    void Update()
    {
        
        isMoving = Input.GetMouseButton(0);

        if (gameStarted && !gameEnded)
        {
            Timer();
        }

        if (isMoving)
        {
            pos.x = cam.ScreenToWorldPoint(Input.mousePosition).x;
            pos.y = cam.ScreenToWorldPoint(Input.mousePosition).y;

        }
        
        if (Input.GetMouseButtonUp(0))
        {
            CheckResult();
            //StartAnimation(false);
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            rb.MovePosition(Vector2.Lerp(rb.position, pos, spoonSpeed * Time.fixedDeltaTime));
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }


    public void Timer()
    {
        currentTime += Time.deltaTime;
        float fillAmount = currentTime / totalTime;
        timerImage.value = fillAmount;

        if(fillAmount < 0.35f)
        {
            isWinning = false;
            ChangeCofeeColor(0);
        }
        else if (fillAmount < 0.72f)
        {
            isWinning = true;
            ChangeCofeeColor(1);
        }
        else
        {
            isWinning = false;
            ChangeCofeeColor(2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Spoon" && isMoving)
        {
            StartAnimation(true);

            gameStarted = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Spoon" && isMoving)
        {
            StartAnimation(false);
        }
    }

    public void ChangeCofeeColor(int CoffeeIndex)
    {
        if (CoffeeIndex >= coffeeColors.Length)
            return;

        for (int i = 0; i < CoffePrefab.Count; i++)
        {
            CoffePrefab[i].GetComponentInChildren<SpriteRenderer>().color = coffeeColors[CoffeeIndex];
        }
    }

     private void CheckResult()
    {
        if (isWinning)
        {
            OnWinning?.Invoke();
            Debug.Log("You Won");
        }
        else
        {
            OnLoosing?.Invoke();
            Debug.Log("You Lose");
        }

        gameEnded = true;
        Time.timeScale = 0f;
    }

    private void StartAnimation(bool animationState)
    {
        for (int i = 0; i < CoffePrefab.Count; i++)
        {
            CoffePrefab[i].GetComponent<Animator>().enabled = animationState;
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }
}   
