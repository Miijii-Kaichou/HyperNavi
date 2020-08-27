
public interface ISystems
{
    public static void UpdateUI() { }

    public static void SubmitToManager(int score);

    /// <summary>
    /// Stop system. Score will be sumbitted
    /// </summary>
    public static void Stop();

    /// <summary>
    /// Continues System
    /// </summary>
    public static void Resume();
    /// <summary>
    /// Initialize System
    /// </summary>
    public static void Init();
}
