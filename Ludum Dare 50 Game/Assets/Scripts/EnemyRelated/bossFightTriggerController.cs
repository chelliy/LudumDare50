using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossFightTriggerController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject boss;
    public GameObject enemyCreator;
    public GameObject shopCollider;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("detected");
            if (Input.GetKeyDown(KeyCode.E)) {
                enemyCreator.GetComponent<EnemyGeneration>().destroy();
                boss.SetActive(true);

                Destroy(shopCollider);

                StopAllCoroutines();
                Destroy(this.gameObject);
            }
        }
    }
}
