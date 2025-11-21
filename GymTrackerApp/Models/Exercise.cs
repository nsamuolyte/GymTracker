using System;
using GymTrackerApp.Models;

namespace GymTrackerApp.Models
{
    public class Exercise : IFormattable
    {
        public ExerciseMachine Machine { get; set; }
        public ExerciseType Type { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }

        public Exercise(ExerciseMachine machine, ExerciseType type, int sets, int reps, double weight)
        {
            Machine = machine;
            Type = type;
            Sets = sets;
            Reps = reps;
            Weight = weight;
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return format switch
            {
                "short" => $"{Machine}: {Sets}x{Reps}",
                "long"  => $"{Machine} ({Type}) - {Sets} sets × {Reps} reps @ {Weight}kg",
                _       => ToString()
            };
        }

        public override string ToString()
            => $"{Machine} - {Sets}x{Reps} @ {Weight}kg";
    }
}
