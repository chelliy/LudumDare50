using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 3;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
            health--;
            Debug.Log(health);
            if (health == 0)
            {
                OnDeath();
            }
        }
    }

    void OnDeath()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}
