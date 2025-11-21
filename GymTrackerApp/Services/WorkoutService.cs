using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GymTrackerApp.Models;

namespace GymTrackerApp.Services
{
    public class WorkoutService
    {
        private readonly List<Workout> _workouts = new();

        private const string FilePath = "workouts.txt";

        public WorkoutService()
        {
            LoadFromFile();
        }

        public void AddWorkout(Workout workout)
        {
            _workouts.Add(workout);
            SaveToFile();
        }

        public IEnumerable<Workout> GetAllWorkouts()
        {
            return _workouts;
        }

        // -------------------------------
        //    SAVE TO FILE
        // -------------------------------
        private void SaveToFile()
        {
            using var writer = new StreamWriter(FilePath);

            foreach (var w in _workouts)
            {
                writer.WriteLine($"WORKOUT|{w.Date:yyyy-MM-dd}|{w.Title}");

                foreach (var ex in w)
                {
                    writer.WriteLine(
                        $"EX|{ex.Machine}|{ex.Sets}|{ex.Reps}|{ex.Weight}");
                }

                writer.WriteLine("END");
            }
        }

        // -------------------------------
        //    LOAD FROM FILE
        // -------------------------------
        private void LoadFromFile()
        {
            if (!File.Exists(FilePath))
                return;

            var lines = File.ReadAllLines(FilePath);

            Workout? current = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('|');

                switch (parts[0])
                {
                    case "WORKOUT":
                        DateTime date = DateTime.Parse(parts[1]);
                        string title = parts[2];
                        current = new Workout(date, title);
                        _workouts.Add(current);
                        break;

                    case "EX":
                        if (current == null) break;

                        var machine = Enum.Parse<ExerciseMachine>(parts[1]);
                        int sets = int.Parse(parts[2]);
                        int reps = int.Parse(parts[3]);
                        double weight = double.Parse(parts[4]);

                        current.AddExercise(new Exercise(machine, sets, reps, weight));
                        break;

                    case "END":
                        current = null;
                        break;
                }
            }
        }
    }
}
