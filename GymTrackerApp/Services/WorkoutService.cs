using GymTrackerApp.Models;
using System.Globalization;

namespace GymTrackerApp.Services
{
    public class WorkoutService
    {
        public delegate bool ExerciseFilter(Exercise ex);

        public ExerciseFilter? Filter { get; set; }

        private const string FilePath = "workouts.txt";
        private readonly List<Workout> _workouts = new();

        public WorkoutService(){ LoadFromFile(); }


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
                    
                    if (ex.Minutes is not null) // Cardio
                        writer.WriteLine($"EX|{ex.Machine}|C|{ex.Minutes}|{ex.Calories}");
                    
                    else if (ex.Weight is null) // ABS
                        writer.WriteLine($"EX|{ex.Machine}|A|{ex.Sets}|{ex.Reps}");
                    
                    else // Kiti pratimai (Strength)
                        writer.WriteLine($"EX|{ex.Machine}|S|{ex.Sets}|{ex.Reps}|{ex.Weight}");
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

                if (p[0] == "WORKOUT")
                {
                    var date = DateTime.Parse(p[1]);
                    var title = p[2];
                    current = new Workout(date, title);
                    _workouts.Add(current);
                }

                else if (p[0] == "EX" && current != null)
                {
                    var machine = Enum.Parse<ExerciseMachine>(p[1]);
                    var type = p[2];

                    Exercise ex = type switch
                    {
                        "S" => new Exercise(
                                    machine,
                                    int.Parse(p[3]),
                                    int.Parse(p[4]),
                                    double.Parse(p[5])),

                        "A" => new Exercise(
                                    machine,
                                    int.Parse(p[3]),
                                    int.Parse(p[4])),

                        "C" => new Exercise(
                                    machine,
                                    minutes: int.Parse(p[3]),
                                    calories: int.Parse(p[4])),

                        _ => throw new Exception("Unknown EX type")
                    };

                    current.AddExercise(ex);
                }
                else if (p[0] == "END")
                {
                    current = null;
                }
            }
        }
    }
}
