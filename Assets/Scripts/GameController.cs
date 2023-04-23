using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] enemies;
   
    [SerializeField]
    private float enemyVelocity = 7f; 

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

    [SerializeField]
    private bool generateEnemies = true;


    [SerializeField]
    private Text textReturnToZeroPoints;

    [SerializeField]
    private Text textDistancePoints;

    [SerializeField]
    private Text textMeanX;

    [SerializeField]
    private Text textLifePoints;

    [SerializeField]
    private Text textPoints;

    [SerializeField]
    private Text textX;

    [SerializeField]
    private Text x;


    [SerializeField]
    internal Text status;

    [SerializeField]
    internal Text output;


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

        GenerateInformation();

        if (generateEnemies && timer <= 0 && actualAmount <= patch)
        {
            RestartTimer();
            EnemiesGenerate();
        }

    }

    private void GenerateInformation()
    {
        if (instantiatedPlayer != null)
        {
            var playerController = instantiatedPlayer.GetComponent<PlayerController>();
            var aiNavigator = instantiatedPlayer.GetComponent<AINavigator>();

            textReturnToZeroPoints.text = playerController.ReturnToZeroPoints.ToString();
            textDistancePoints.text = playerController.DistancePoints.ToString();
            textMeanX.text = playerController.MeanX.ToString();
            textLifePoints.text = playerController.LifePoints.ToString();
            textPoints.text = playerController.Points.ToString();
            textX.text = instantiatedPlayer.transform.position.x.ToString();

            x.text = aiNavigator.x.ToString();

            output.text = aiNavigator.output.ToString();
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
                newPosition = new Vector2(instantiatedPlayer.transform.position.x, 5);
                if(instantiatedPlayer.transform.position.x > 9 )
                {
                    newPosition = new Vector2(9, 5);
                }

                if (instantiatedPlayer.transform.position.x < -9)
                {
                    newPosition = new Vector2(-9, 5);
                }
            }

            if(IsEmptyPosition(newPosition, enemies[enemyIndex].transform.localScale))
            {
                actualAmount++;
                var enemyInstance = Instantiate(enemies[enemyIndex]);
                var enemyController = enemyInstance.GetComponent<EnemyController>();

                if(continum)
                    enemyController.velocity = enemyVelocity;

                enemyController.ActionOnDestroy += () => { actualAmount--; };
                enemyInstance.transform.position = newPosition;
                i++;
            }
        }
    }

    internal void EndGame()
    {
        Time.timeScale = 0;
    }

    private bool IsEmptyPosition(Vector3 vector, Vector3 size)
    {
        Collider2D position = Physics2D.OverlapBox(vector, size, 0f);
        return position == null;
    }

}
