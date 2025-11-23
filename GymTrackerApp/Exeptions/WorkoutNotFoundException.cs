namespace GymTrackerApp.Exceptions
{
    public class WorkoutNotFoundException : Exception
    {
        public WorkoutNotFoundException(string message) : base(message) {}
    }
}
