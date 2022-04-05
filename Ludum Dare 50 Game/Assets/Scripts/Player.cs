using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 3;
    public int damage = 1;
    public AudioSource audio;
    public Animator anim;
    public Rigidbody2D rb;

    public float timeRemaining = 100;

    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Image heart4;
    public Text goldNum;
    public Text timer;

    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject buff;

    public GameObject weapon;

    public int gold = 0;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        goldNum.text = gold.ToString();
        if(health == 0){
            heart1.enabled = false;
            heart2.enabled = false;
            heart3.enabled = false;
            heart4.enabled = false;
        }
        if(health == 1){
            heart1.enabled = true;
            heart2.enabled = false;
            heart3.enabled = false;
            heart4.enabled = false;
        }
        if(health == 2){
            heart1.enabled = true;
            heart2.enabled = true;
            heart3.enabled = false;
            heart4.enabled = false;
        }
        if(health == 3){
            heart1.enabled = true;
            heart2.enabled = true;
            heart3.enabled = true;
            heart4.enabled = false;
        }
        if(health == 4){
            heart1.enabled = true;
            heart2.enabled = true;
            heart3.enabled = true;
            heart4.enabled = true;
        }
        timeRemaining -= Time.deltaTime;
        timer.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(timeRemaining / 60), Mathf.FloorToInt(timeRemaining % 60));
        if (this.gameObject == null || timeRemaining <= 0)
        {
            Debug.Log("Game over");
            StopAllCoroutines();
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit(0);
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            if (this.gameObject.GetComponent<PlayerMovement>().bounceTime > 0)
            {
                health--;
                Debug.Log(health);
                if (health == 0)
                {
                    heart1.enabled = false;
                    heart2.enabled = false;
                    heart3.enabled = false;
                    heart4.enabled = false;
                    OnDeath();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //boss bullet
        if (collision.gameObject.CompareTag("bossBullet"))
        {
            health--;
            Destroy(collision.gameObject);
            if (health == 0)
            {
                heart1.enabled = false;
                heart2.enabled = false;
                heart3.enabled = false;
                heart4.enabled = false;
                OnDeath();
            }
        }
    }


    void deathAnim()
    {
        audio.PlayOneShot(audio.clip);
        anim.Play("Base Layer.Death");
    }
 
    private void OnDeath()
    {
        deathAnim();
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    public void goldIncrement() {
        gold++;
    }

    public void timeIncrement(int num) {
        if(gold >= num){
            timeRemaining+=20;
            gold-=num;
            item2.SetActive(false);
        }
    }

    public void healthIncrement(int num){
        if(gold >= num){
            health++;
            gold-=num;
            item3.SetActive(false);
        }
    }

    public void damgeIncrement(int num){
        if(gold >= num){
            gold -= num;
            item1.SetActive(false);
            buff.SetActive(true);
            weapon.GetComponent<Gun>().damage = weapon.GetComponent<Gun>().damage + 1;
        }

    }


}
