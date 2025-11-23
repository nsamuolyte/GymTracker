using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GymTrackerApp.Models
{
    public class Workout : IEnumerable<Exercise>, IComparable<Workout>, IEquatable<Workout>
    {
        public DateTime Date { get; }
        public string Title { get; }

        private List<Exercise> _exercises = new();

        // Vienintelis teisingas sąrašas
        public List<string> GroupTrainings { get; } = new();

        public Workout(DateTime date, string title)
        {
            Date = date;
            Title = title ?? "Untitled";
        }

        // Copy konstruktorius (naudojamas Undo/Redo)
        public Workout(Workout other)
        {
            Date = other.Date;
            Title = other.Title;

            _exercises = other._exercises
                .Select(e => new Exercise(e))
                .ToList();

            GroupTrainings = new List<string>(other.GroupTrainings);
        }

        // ================= GROUP TRAININGS =================

        public void AddGroupTrainings(params string[] groups)
        {
            foreach (var g in groups)
            {
                if (!string.IsNullOrWhiteSpace(g))
                    GroupTrainings.Add(g.Trim());
            }
        }

        // ================= EXERCISES =================

        public void AddExercise(Exercise ex)
        {
            if (ex != null)
                _exercises.Add(ex);
        }

        public bool RemoveExercise(Exercise ex)
        {
            return _exercises.Remove(ex);
        }

        public void AddExercises(params Exercise[] exercises)
        {
            foreach (var ex in exercises)
                if (ex != null)
                    _exercises.Add(ex);
        }

        // ================= OPERATORS =================

        public static Workout operator +(Workout w, Exercise ex)
        {
            w.AddExercise(ex);
            return w;
        }

        public static Workout operator -(Workout w, Exercise ex)
        {
            w.RemoveExercise(ex);
            return w;
        }

        // ================= UTILITY =================

        public void Deconstruct(out DateTime date, out string title, out int count)
        {
            date = Date;
            title = Title;
            count = _exercises.Count;
        }

        public IEnumerator<Exercise> GetEnumerator() => _exercises.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int CompareTo(Workout? other)
            => other == null ? 1 : Date.CompareTo(other.Date);

        public bool Equals(Workout? other)
            => other != null && Date.Date == other.Date.Date;

        public override bool Equals(object? obj)
            => Equals(obj as Workout);

        public override int GetHashCode()
            => Date.GetHashCode();

        public override string ToString()
        {
            string groups = GroupTrainings.Count == 0
                ? ""
                : $" | Groups: {string.Join(", ", GroupTrainings)}";

            return $"{Date:yyyy-MM-dd} – {Title} ({_exercises.Count} exercises){groups}";
        }
    }
}
