using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemies;

    private float timer;
    
    [SerializeField]
    private float enemiesGeneratorCooldown = 5f;

    [SerializeField]
    private int level = 1;

    [SerializeField]
    private int experience = 0;

    [SerializeField]
    private int reqXp = 5;

    [SerializeField]
    private float dificulty = 4;

    [SerializeField]
    private int patch = 4;

    [SerializeField]
    private int actualAmount = 0;

    [SerializeField]
    private GameObject player;
    internal GameObject instantiatedPlayer;

    [SerializeField]
    private bool followPlayer = false;

    [SerializeField]
    private bool continum = false;

    void Start()
    {
        RestartTimer();
        InstantiatePlayer();
        
    }

    private void InstantiatePlayer()
    {
        instantiatedPlayer = Instantiate(player);
    }


    private void RestartTimer()
    {
        timer = enemiesGeneratorCooldown;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        ResetSceneIfPlayerIsDead();

        if (timer <= 0 && actualAmount <= patch)
        {
            RestartTimer();
            EnemiesGenerate();
        }
    }

    private void ResetSceneIfPlayerIsDead()
    {
        if (instantiatedPlayer == null)
        {
            SummonNewPlayer();
            ResetEnemies();
            ResetLevel();
            RestartTimer();
        }
    }

    private void SummonNewPlayer()
    {
        InstantiatePlayer();
    }

    private void ResetEnemies()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(var enemy in enemies)
        {
            Destroy(enemy, 0);
        }
    }

    private void ResetLevel()
    {
        level = 1;
    }

    public void UpXp(int points)
    {
        experience += points;

        if(experience > reqXp)
        {
            experience = 0;
            level++;
        }
    }

    void EnemiesGenerate()
    {
        int chance = Random.Range(0, level);
        int enemyIndex = 0;

        if (chance > 2)
            enemyIndex = 1;

        if((actualAmount <= patch || continum))
        {
            PatchOfEnemiesGenerate(level * dificulty, enemyIndex);
        }
    }

    private void PatchOfEnemiesGenerate(float patchSize, int enemyIndex)
    {
        var i = 0;
        var tentativas = 0;
        var totalOfEneimes = patchSize;

        if (continum)
            totalOfEneimes = 1;

        while (i < totalOfEneimes && (actualAmount <= patch || continum))
        {
            if(tentativas >= 200)
            {
                tentativas = 0;
                break;
            }

            tentativas++;

            Vector2 newPosition = new Vector2(Random.Range(-8, 8), Random.Range(5, 15));

            if(followPlayer)
            {
                newPosition = new Vector2(instantiatedPlayer.transform.position.x, Random.Range(5, 15));
            }

            if(IsEmptyPosition(newPosition, enemies[enemyIndex].transform.localScale))
            {
                actualAmount++;
                var enemyInstance = Instantiate(enemies[enemyIndex]);
                var enemyController = enemyInstance.GetComponent<EnemyController>();

                if(continum)
                    enemyController.velocity = 7f;

                enemyController.ActionOnDestroy += () => { actualAmount--; };
                enemyInstance.transform.position = newPosition;
                i++;
            }
        }
    }

    private bool IsEmptyPosition(Vector3 vector, Vector3 size)
    {
        Collider2D position = Physics2D.OverlapBox(vector, size, 0f);
        return position == null;
    }

}
