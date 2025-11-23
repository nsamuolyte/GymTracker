namespace GymTrackerApp.Models
{
    public abstract class ExerciseBase : IFormattable
    {
        public string Name { get; set; }
        public ExerciseType Type { get; set; }
        public abstract string ToString(string? format, IFormatProvider? formatProvider);
        public override string ToString() => ToString(null, null);
    }
}
