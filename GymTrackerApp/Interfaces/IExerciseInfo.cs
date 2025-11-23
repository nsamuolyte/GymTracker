namespace GymTrackerApp.Interfaces
{
    public interface IExerciseInfo
    {
        string GetInfo();
        double GetTotalWeight(); // sets * reps * weight
    }
}
