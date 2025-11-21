using System;
using System.Collections;
using System.Collections.Generic;

namespace GymTrackerApp.Models
{
    public class Workout : IEnumerable<Exercise>, IComparable<Workout>, IEquatable<Workout>
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        private List<Exercise> _exercises = new();

        public Workout(DateTime date, string title)
        {
            Date = date;
            Title = title;
        }

        public void AddExercise(Exercise exercise)
        {
            _exercises.Add(exercise);
        }

        // IEnumerable
        public IEnumerator<Exercise> GetEnumerator() => _exercises.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // IComparable (rikiavimas pagal datą)
        public int CompareTo(Workout other)
        {
            if (other == null) return 1;
            return Date.CompareTo(other.Date);
        }

        // IEquatable (ar tai ta pati treniruotė)
        public bool Equals(Workout other)
        {
            if (other == null) return false;
            return Date.Date == other.Date.Date;
        }

        public override bool Equals(object obj) => Equals(obj as Workout);

        public override int GetHashCode() => Date.GetHashCode();

        public override string ToString() => $"{Date:yyyy-MM-dd} - {Title} ({_exercises.Count} exercises)";
    }
}
