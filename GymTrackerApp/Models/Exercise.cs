using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using System;

namespace GymTrackerApp.Models
{
    public class Exercise : IFormattable, IEquatable<Exercise>, IComparable<Exercise>
    {
        public string Name { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }

        // IEquatable
        public bool Equals(Exercise other)
        {
            if (other is null) return false;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
            => Equals(obj as Exercise);

        public override int GetHashCode()
            => Name.GetHashCode();

        // IComparable
        public int CompareTo(Exercise other)
        {
            if (other is null) return 1;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        // IFormattable
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return format switch
            {
                "short" => $"{Name}: {Sets}x{Reps}",
                "long"  => $"{Name}: {Sets} sets × {Reps} reps @ {Weight}kg",
                _       => ToString()
            };
        }

        public override string ToString()
            => $"{Name}: {Sets}x{Reps} @ {Weight}kg";
    }
}


