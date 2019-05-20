using System.Linq;
using CompleteProject;
using UnityEngine;

/// <summary>
/// Updates the player and camera objects and creates enemies from a game save state, then destroys itself on
/// completion.
/// </summary>
public class GameSaveLoader : MonoBehaviour
{
    public GameSaveState State { get; set; }

    public SaveManager SaveManager;

    public void Update()
    {
        // We need to update the state after the rest of the scene has been loaded and activated
        if (State != null)
        {
            Load(State);
            
            // After loading, this object is no longer required
            Destroy(gameObject);
        }
    }

    public void Load(GameSaveState state)
    {
        // Find the save manager of the new scene
        // so we can use its user, camera, and enemy managers
        SaveManager = FindObjectOfType<SaveManager>();
        LoadPlayer(state.PlayerSaveState);
        LoadCamera(state.CameraSaveState);
        LoadEnemies(state.SpawnerSaveState);
        SaveManager.PauseManager.Unpause();
    }

    private void LoadEnemies(SpawnerSaveState[] spawnerSaveStates)
    {
        var enemyManagers = SaveManager.EnemyManagers.GetComponents<EnemyManager>();
        var enemyManagersByName = enemyManagers.ToDictionary(manager => manager.enemy.name);
        foreach (var spawnerSaveState in spawnerSaveStates)
        {
            var manager = enemyManagersByName[spawnerSaveState.Name];
            LoadEnemies(spawnerSaveState, manager);
            manager.StartSpawning(spawnerSaveState.NextSpawnTime);
        }
    }

    private static void LoadEnemies(SpawnerSaveState spawnerSaveState, EnemyManager enemyManager)
    {
        foreach (var enemySaveState in spawnerSaveState.Enemies)
        {
            var enemy = Instantiate(
                enemyManager.enemy,
                enemySaveState.TransformSaveState.Position,
                enemySaveState.TransformSaveState.Rotation);
            var enemyHealth = enemy.GetComponent<EnemyHealth>();
            enemyHealth.currentHealth = enemySaveState.Health;

            var enemyAttack = enemy.GetComponent<EnemyAttack>();
            enemyAttack.timer = enemySaveState.AttackTime;
            enemyManager.spawnedEnemies.Add(enemy);
        }
    }

    private void LoadPlayer(PlayerSaveState playerSaveState)
    {
        var player = SaveManager.Player;
        
        var health = player.GetComponent<PlayerHealth>();
        health.SetHealth(playerSaveState.Health);
       
        ScoreManager.score = playerSaveState.Score;
        
        ApplySavedTransform(player.transform, playerSaveState.Transform);
    }

    private void LoadCamera(TransformSaveState cameraSaveState)
    {
        var mainCamera = SaveManager.Camera;
        ApplySavedTransform(mainCamera.transform, cameraSaveState);
    }

    private void ApplySavedTransform(Transform objectTransform, TransformSaveState transformSaveState)
    {
        objectTransform.position = transformSaveState.Position;
        objectTransform.rotation = transformSaveState.Rotation;
    }
}