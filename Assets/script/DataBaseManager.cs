using System;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    private static DataBaseManager _instance;
    public static DataBaseManager Instance => _instance;
    [SerializeField] public EnemyDataBase enemyDataBase; // Référence à la base de données des ennemis

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject); // Détruire le GameManager si une autre instance existe déjà

        DontDestroyOnLoad(gameObject); // Ne pas détruire cet objet lors du chargement d'une nouvelle scène
        
    }
}
