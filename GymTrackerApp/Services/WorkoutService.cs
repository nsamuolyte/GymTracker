using System.Collections.Generic;
using GymTrackerApp.Models;

namespace GymTrackerApp.Services
{
    public class WorkoutService
    {
        private readonly List<Workout> _workouts = new();

        public void AddWorkout(Workout workout)
        {
            _workouts.Add(workout);
        }

        public IEnumerable<Workout> GetAllWorkouts()
        {
            return _workouts;
        }
    }
}
