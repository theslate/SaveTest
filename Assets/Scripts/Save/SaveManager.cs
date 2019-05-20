using System.Linq;
using CompleteProject;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides save/load callbacks for menus and converts player, camera, and enemy managers to a game save state.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public Transform Player;
    public Transform EnemyManagers;
    public Transform Camera;
    public PauseManager PauseManager;

    private PlayerPrefsGameSaveSystem gameSaveSystem;

    public void Awake()
    {
        gameSaveSystem = new PlayerPrefsGameSaveSystem();
    }

    public void Save()
    {
        var saveState = CreateSaveState();

        gameSaveSystem.Save(saveState);
    }

    public void Load()
    {
        var saveState = PlayerPrefsGameSaveSystem.Load();
        CreateGameSaveLoader(saveState);
        
        // Save loads quickly, so async loading is not required
        SceneManager.LoadScene(saveState.Scene);
    }

    private GameSaveState CreateSaveState()
    {
        return new GameSaveState
        {
            Scene = SceneManager.GetActiveScene().buildIndex,
            CameraSaveState = CreateTransformSaveState(Camera),
            SpawnerSaveState = CreateSpawnerSaveState(EnemyManagers),
            PlayerSaveState = CreatePlayerSaveState(Player)
        };
    }

    private static PlayerSaveState CreatePlayerSaveState(Transform player)
    {
        var health = player.GetComponent<PlayerHealth>();
        return new PlayerSaveState
        {
            Score = ScoreManager.score,
            Health = health.currentHealth,
            Transform = CreateTransformSaveState(player.transform)
        };
    }

    private static void CreateGameSaveLoader(GameSaveState saveState)
    {
        // Create GameSaveLoader with save state for the new scene
        var loader = new GameObject("Loader");
        var saveLoader = loader.AddComponent<GameSaveLoader>();
        DontDestroyOnLoad(loader);
        saveLoader.State = saveState;
    }

    private static SpawnerSaveState[] CreateSpawnerSaveState(Transform enemyManagerGameObject)
    {
        var enemyManagers = enemyManagerGameObject.GetComponents<EnemyManager>();

        return enemyManagers.Select(GetSpawnerSaveState).ToArray();
    }

    private static SpawnerSaveState GetSpawnerSaveState(EnemyManager manager)
    {
        var livingEnemies = manager.spawnedEnemies.Where(enemy => enemy != null);
        var enemySaveStates = livingEnemies
            .Select(CreateEnemySaveState)
            .Where(enemyState => enemyState.Health > 0)
            .ToArray();
        
        return new SpawnerSaveState
        {
            Name = manager.enemy.name,
            Enemies = enemySaveStates
        };
    }

    private static EnemySaveState CreateEnemySaveState(GameObject enemy)
    {
        return new EnemySaveState
        {
            Health = enemy.GetComponent<EnemyHealth>().currentHealth,
            AttackTime = enemy.GetComponent<EnemyAttack>().timer,
            TransformSaveState = CreateTransformSaveState(enemy.transform),
        };
    }

    private static TransformSaveState CreateTransformSaveState(Transform transform)
    {
        return new TransformSaveState
        {
            Position = transform.position,
            Rotation = transform.rotation,
            Name = transform.name
        };
    }
}