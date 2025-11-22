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
            using var writer = new StreamWriter(FilePath, false);

            foreach (var w in _workouts)
            {
                writer.WriteLine($"WORKOUT|{w.Date:yyyy-MM-dd}|{w.Title}");

                foreach (var ex in w)
                {
                    // ---- CARDIO ----
                    if (ex.Minutes is not null)
                    {
                        writer.WriteLine($"EX|{ex.Machine}|C|{ex.Minutes}|{ex.Calories}");
                    }
                    // ---- ABS (no weight) ----
                    else if (ex.Weight is null)
                    {
                        writer.WriteLine($"EX|{ex.Machine}|A|{ex.Sets}|{ex.Reps}");
                    }
                    // ---- STRENGTH ----
                    else
                    {
                        writer.WriteLine($"EX|{ex.Machine}|S|{ex.Sets}|{ex.Reps}|{ex.Weight}");
                    }
                }

                writer.WriteLine("END");
            }
        }

        public void LoadFromFile()
        {
            _workouts.Clear();

            if (!File.Exists(FilePath))
                return;

            var lines = File.ReadAllLines(FilePath);
            Workout? current = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var p = line.Split('|');

                // --------------------------
                // WORKOUT
                // --------------------------
                if (p[0] == "WORKOUT")
                {
                    var date = DateTime.Parse(p[1]);
                    var title = p[2];
                    current = new Workout(date, title);
                    _workouts.Add(current);
                }

                // --------------------------
                // EXERCISE
                // --------------------------
                else if (p[0] == "EX" && current != null)
                {
                    var machine = Enum.Parse<ExerciseMachine>(p[1]);
                    var type = p[2]; // S / A / C

                    Exercise ex = type switch
                    {
                        // Strength
                        "S" => new Exercise(
                                    machine,
                                    int.Parse(p[3]),
                                    int.Parse(p[4]),
                                    double.Parse(p[5])),

                        // Abs
                        "A" => new Exercise(
                                    machine,
                                    int.Parse(p[3]),
                                    int.Parse(p[4])),

                        // Cardio
                        "C" => new Exercise(
                                    machine,
                                    minutes: int.Parse(p[3]),
                                    calories: int.Parse(p[4])),

                        _ => throw new Exception("Unknown EX type")
                    };

                    current.AddExercise(ex);
                }

                // --------------------------
                // END
                // --------------------------
                else if (p[0] == "END")
                {
                    current = null;
                }
            }
        }
    }
}
