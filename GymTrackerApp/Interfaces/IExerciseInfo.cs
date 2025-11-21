namespace GymTrackerApp.Interfaces
{
    public interface IExerciseInfo
    {
        string GetInfo();      // aprašymas
        double GetTotalWeight(); // sets * reps * weight
    }
}
