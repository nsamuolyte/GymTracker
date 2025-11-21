using GymTrackerApp.Models;
using System.Globalization;

namespace GymTrackerApp.Services
{
    public class WorkoutService
    {
        private const string FilePath = "workouts.txt";
        private readonly List<Workout> _workouts = new();

        public WorkoutService()
        {
            LoadFromFile();
        }

        public IEnumerable<Workout> GetAllWorkouts() => _workouts;

        public void AddWorkout(Workout w)
        {
            _workouts.Add(w);
            SaveToFile();
        }

        public void SaveToFile()
        {
            using var writer = new StreamWriter(FilePath);

            foreach (var w in _workouts)
            {
                writer.WriteLine($"WORKOUT|{w.Date:yyyy-MM-dd}|{w.Title}");

                foreach (var ex in w)
                {
                    // STRENGTH
                    if (ex.Minutes == null)
                    {
                        writer.WriteLine(
                            $"EX|{ex.Machine}|{ex.Sets}|{ex.Reps}|{ex.Weight}"
                        );
                    }
                    // CARDIO
                    else
                    {
                        writer.WriteLine(
                            $"EX|{ex.Machine}|{ex.Minutes}|0|{ex.Calories}"
                        );
                    }
                }

                writer.WriteLine("END");
                writer.WriteLine();
            }
        }



        public void LoadFromFile()
        {
            if (!File.Exists(FilePath))
                return;

            var lines = File.ReadAllLines(FilePath);
            Workout? current = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // START WORKOUT
                if (line.StartsWith("WORKOUT|"))
                {
                    var parts = line.Split('|');
                    if (parts.Length < 3) continue;

                    if (!DateTime.TryParse(parts[1], out var date))
                        continue;

                    current = new Workout(date, parts[2]);
                    _workouts.Add(current);
                }

                // EXERCISE
                else if (line.StartsWith("EX|") && current != null)
                {
                    var p = line.Split('|');
                    if (p.Length < 5) continue;

                    if (!Enum.TryParse<ExerciseMachine>(p[1], out var machine))
                        continue;

                    int.TryParse(p[2], out int setsOrMin);
                    int.TryParse(p[3], out int reps);
                    double.TryParse(p[4], out double weightOrKcal);

                    Exercise ex;

                    // CARDIO (minutes > 0 and reps == 0)
                    if (machine is ExerciseMachine.Bike
                        or ExerciseMachine.StairStepper
                        or ExerciseMachine.Treadmill)
                    {
                        ex = new Exercise(
                            machine,
                            minutes: setsOrMin,
                            calories: (int)weightOrKcal
                        );
                    }
                    // ABS
                    else if (machine == ExerciseMachine.AbdominalCrunch)
                    {
                        ex = new Exercise(
                            machine,
                            sets: setsOrMin,
                            reps: reps
                        );
                    }
                    // STRENGTH
                    else
                    {
                        ex = new Exercise(machine, setsOrMin, reps, weightOrKcal);
                    }

                    current.AddExercise(ex);
                }

                // END WORKOUT
                else if (line == "END")
                {
                    current = null;
                }
            }
        }
    }
}
