using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 3;
    public int damage;
    public AudioSource audio;
    public Animator anim;
    public Rigidbody2D rb;

    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Image heart4;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
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
        if (this.gameObject == null)
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

}
