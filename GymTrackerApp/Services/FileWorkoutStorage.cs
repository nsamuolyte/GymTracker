using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GymTrackerApp.Models;

namespace GymTrackerApp.Services
{
    public static class FileWorkoutStorage
    {
        private static readonly string Path = "workouts.txt";

        // save all workouts
        public static void Save(List<Workout> workouts)
        {
            using var writer = new StreamWriter(Path, false);

            foreach (var w in workouts)
            {
                writer.WriteLine($"#WORKOUT|{w.Date:yyyy-MM-dd}|{w.Title}");

                foreach (var ex in w)
                {
                    writer.WriteLine($"EX|{ex.Machine}|{ex.Sets}|{ex.Reps}|{ex.Weight}");
                }
            }
        }

        // load all workouts
        public static List<Workout> Load()
        {
            var list = new List<Workout>();

            if (!File.Exists(Path))
                return list;

            Workout? current = null;

            foreach (var line in File.ReadLines(Path))
            {
                var parts = line.Split('|');

                if (line.StartsWith("#WORKOUT"))
                {
                    current = new Workout(DateTime.Parse(parts[1]), parts[2]);
                    list.Add(current);
                }
                else if (line.StartsWith("EX") && current != null)
                {
                    var machine = Enum.Parse<ExerciseMachine>(parts[1]);
                    int sets = int.Parse(parts[2]);
                    int reps = int.Parse(parts[3]);
                    double weight = double.Parse(parts[4]);

                    current.AddExercise(new Exercise(machine, sets, reps, weight));
                }
            }
            return list;
        }
    }
}
