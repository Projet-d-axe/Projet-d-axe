using UnityEngine;

public class EnemyCrontroler : MonoBehaviour
{
    public EnemyData data;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data = DataBaseManager.Instance.enemyDataBase.datas[0]; // Récupérer les données de l'ennemi à partir de la base de données   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
