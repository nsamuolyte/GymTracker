using GymTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace GymTrackerApp.Services
{
    public class FileWorkoutStorage
    {
        private const string FilePath = "workouts.txt";

        // ==========================================
        //               SAVE TO FILE
        // ==========================================
        public void Save(List<Workout> workouts)
        {
            using var writer = new StreamWriter(FilePath, false);

            foreach (var w in workouts)
            {
                writer.WriteLine($"WORKOUT|{w.Date:yyyy-MM-dd}|{w.Title}");

                foreach (var ex in w)
                {
                    // Strength exercise
                    if (ex.Minutes is null && ex.Weight is not null)
                    {
                        writer.WriteLine(
                            $"EX|{ex.Machine}|{ex.Sets}|{ex.Reps}|{ex.Weight}"
                        );
                    }
                    // Abs
                    else if (ex.Minutes is null && ex.Weight is null)
                    {
                        writer.WriteLine(
                            $"EX|{ex.Machine}|{ex.Sets}|{ex.Reps}"
                        );
                    }
                    // Cardio
                    else
                    {
                        writer.WriteLine(
                            $"EX|{ex.Machine}|{ex.Minutes}|{ex.Calories}"
                        );
                    }
                }

                writer.WriteLine("END");
                writer.WriteLine();
            }
        }

        // ==========================================
        //               LOAD FROM FILE
        // ==========================================
        public List<Workout> Load()
        {
            var list = new List<Workout>();

            if (!File.Exists(FilePath))
                return list;

            var lines = File.ReadAllLines(FilePath);
            Workout? current = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('|');

                // ---- WORKOUT ----
                if (parts[0] == "WORKOUT")
                {
                    var date = DateTime.Parse(parts[1]);
                    var title = parts[2];

                    current = new Workout(date, title);
                    list.Add(current);
                }

                // ---- EXERCISE ----
                else if (parts[0] == "EX" && current != null)
                {
                    var machine = Enum.Parse<ExerciseMachine>(parts[1]);

                    // CARDIO — EX|Machine|minutes|kcal
                    if (machine is ExerciseMachine.Bike
                        or ExerciseMachine.Treadmill
                        or ExerciseMachine.StairStepper)
                    {
                        int minutes = int.Parse(parts[2]);
                        int kcal = int.Parse(parts[3]);

                        var cardio = new Exercise(
                            machine: machine,
                            minutes: minutes,
                            calories: kcal
                        );
                        current.AddExercise(cardio);
                    }
                    // ABS — EX|Machine|sets|reps
                    else if (machine == ExerciseMachine.AbdominalCrunch)
                    {
                        int sets = int.Parse(parts[2]);
                        int reps = int.Parse(parts[3]);

                        var abs = new Exercise(machine, sets, reps);
                        current.AddExercise(abs);
                    }
                    // STRENGTH — EX|Machine|sets|reps|weight
                    else
                    {
                        int sets = int.Parse(parts[2]);
                        int reps = int.Parse(parts[3]);
                        double weight = double.Parse(parts[4]);

                        var strength = new Exercise(machine, sets, reps, weight);
                        current.AddExercise(strength);
                    }
                }
                else if (parts[0] == "END")
                {
                    current = null;
                }
            }

            return list;
        }
    }
}
