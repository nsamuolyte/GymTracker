namespace GymTrackerApp.Utils
{
    public static class RangeExtensions
    {
        public static bool Contains(this Range range, int value)
        {
            int start = range.Start.Value;
            int end   = range.End.Value;
            return value >= start && value < end;
        }

        public static bool Contains(this Range range, double value)
        {
            double start = range.Start.Value;
            double end   = range.End.Value;
            return value >= start && value < end;
        }
    }
}
