using UnityEngine;

/// <summary>
/// Gère l'accès à la base de données des ennemis en tant que singleton persistant.
/// </summary>
public class DataBaseManager : MonoBehaviour
{
    private static DataBaseManager _instance;
    private EnemyDataBase _enemyDataBase;

    // Propriété publique pour accéder à l'instance (lecture seule)
    public static DataBaseManager Instance => _instance;

    // Propriété publique pour accéder à la base de données (lecture seule)
    public EnemyDataBase EnemyDataBase => _enemyDataBase;

    [Tooltip("Référence à la base de données des ennemis")]
    [SerializeField] private EnemyDataBase enemyDataBaseInspector;

    private void Awake()
    {
        // Gestion du singleton
        if (_instance == null)
        {
            _instance = this;
            _enemyDataBase = enemyDataBaseInspector; // Initialisation
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Méthode optionnelle pour initialiser/recharger la base de données
    public void InitializeDataBase(EnemyDataBase newDataBase)
    {
        _enemyDataBase = newDataBase;
    }
}