public static class GameEvents
{
    public delegate void OnEnemyKilled(IEnemy enemy);
    public static OnEnemyKilled EnemyKilled;

    public delegate void OnIn_Game_Leave();
    public static OnIn_Game_Leave In_Game_Leave;

    public delegate void OnIn_Game_Enter();
    public static OnIn_Game_Enter In_Game_Enter;

    public delegate void OnCombat_Started();
    public static OnCombat_Started Combat_Started;
}

