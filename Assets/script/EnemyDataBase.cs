using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyDataBase", menuName = "DataBase/EnemyDataBase", order = 1)]
public class EnemyDataBase : ScriptableObject
{
    public List<EnemyData> datas = new();
}
