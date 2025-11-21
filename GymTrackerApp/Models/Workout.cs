using System;
using System.Collections;
using System.Collections.Generic;

namespace GymTrackerApp.Models
{
    public class Workout : IEnumerable<Exercise>, IComparable<Workout>, IEquatable<Workout>
    {
        public DateTime Date { get; }
        public string Title { get; }
        private readonly List<Exercise> _exercises = new();

        public Workout(DateTime date, string title)
        {
            Date = date;
            Title = title;
        }

        public void AddExercise(Exercise ex) => _exercises.Add(ex);

        public IEnumerator<Exercise> GetEnumerator() => _exercises.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int CompareTo(Workout? other)
            => other == null ? 1 : Date.CompareTo(other.Date);

        public bool Equals(Workout? other)
            => other != null && Date.Date == other.Date.Date;

        public override bool Equals(object obj) => Equals(obj as Workout);
        public override int GetHashCode() => Date.GetHashCode();

        public override string ToString()
            => $"{Date:yyyy-MM-dd} – {Title} ({_exercises.Count} exercises)";
    }
}
