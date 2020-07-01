using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    GameManager gameManager;

    [SerializeField] private int scoreIncrementAmount = 1;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        gameManager.score += scoreIncrementAmount;
    }
}
