namespace GymTrackerApp.Models
{
    public sealed class StrengthExercise : ExerciseBase
    {
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }

        public override string ToString(string? format, IFormatProvider? provider)
        {
            return format switch
            {
                "short" => $"{Name}: {Sets}x{Reps}",
                "long"  => $"{Name}: {Sets} sets × {Reps} reps @ {Weight}kg",
                _       => $"{Name}: {Sets}x{Reps} @ {Weight}kg"
            };
        }
    }
}
