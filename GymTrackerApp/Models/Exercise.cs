using System;
using GymTrackerApp.Utils;

namespace GymTrackerApp.Models
{
    public class Exercise : IFormattable
    {
        public delegate EffortLevel EffortCalculator(Exercise ex);
        public static EffortCalculator? CalculateEffort { get; set; }

        public EffortLevel Effort { get; private set; }
        public MuscleGroup Groups { get; private set; }
        public ExerciseMachine Machine { get; }
        public ExerciseType Type { get; }

        // paprastu pratimu parametrai
        public int? Sets { get; }
        public int? Reps { get; }
        public double? Weight { get; }

        // cardio pratimu parametrai
        public int? Minutes { get; }
        public int? Calories { get; }


        // Treniruokliai pagal tipus
        public Exercise( ExerciseMachine machine, int sets = 0, int reps = 0, double weight = 0, int minutes = 0, int calories = 0)
        {
            Machine = machine;
            Type = GetTypeFromMachine(machine);
            Groups = GetMuscleGroups(machine);

            switch (machine)
            {
                case ExerciseMachine m when m is
                    ExerciseMachine.LegPress
                    or ExerciseMachine.LegCurl
                    or ExerciseMachine.LegExtension
                    or ExerciseMachine.Abductor
                    or ExerciseMachine.Adductor
                    or ExerciseMachine.MultiHip
                    or ExerciseMachine.ChestPress
                    or ExerciseMachine.Pectoral
                    or ExerciseMachine.Row
                    or ExerciseMachine.PullDown
                    or ExerciseMachine.AssistedChinDip
                    or ExerciseMachine.BackExtension
                    or ExerciseMachine.TricepsExtension
                    or ExerciseMachine.BicepsCurl
                    or ExerciseMachine.ArmCurl
                    or ExerciseMachine.ShoulderPress
                    or ExerciseMachine.DeltsMachine
                    or ExerciseMachine.ReverseFly:

                    Sets = sets;
                    Reps = reps;
                    Weight = weight;
                    break;

                case ExerciseMachine.AbdominalCrunch:
                    Sets = sets;
                    Reps = reps;
                    Weight = null;
                    break;

                case ExerciseMachine.StairStepper:
                case ExerciseMachine.Treadmill:
                case ExerciseMachine.Bike:
                    Minutes = minutes;
                    Calories = calories;
                    break;

                default:
                    throw new Exception("Unknown machine type.");
            }

            if (CalculateEffort != null)
                Effort = CalculateEffort(this);
            else
                RecalculateEffort();
        }

        // Treniruoklai pagal raumenų grupes
        private MuscleGroup GetMuscleGroups(ExerciseMachine machine)
        {
            return machine switch
            {
                // Legs
                ExerciseMachine.LegPress or
                ExerciseMachine.LegCurl or
                ExerciseMachine.LegExtension or
                ExerciseMachine.Abductor or
                ExerciseMachine.Adductor or
                ExerciseMachine.MultiHip
                    => MuscleGroup.Legs,

                // Chest
                ExerciseMachine.ChestPress or
                ExerciseMachine.Pectoral
                    => MuscleGroup.Chest,

                // Back
                ExerciseMachine.Row or
                ExerciseMachine.PullDown or
                ExerciseMachine.AssistedChinDip or
                ExerciseMachine.BackExtension
                    => MuscleGroup.Back,

                // Arms
                ExerciseMachine.TricepsExtension or
                ExerciseMachine.BicepsCurl or
                ExerciseMachine.ArmCurl
                    => MuscleGroup.Arms,

                // Shoulders
                ExerciseMachine.ShoulderPress or
                ExerciseMachine.DeltsMachine or
                ExerciseMachine.ReverseFly
                    => MuscleGroup.Shoulders,

                // Abs
                ExerciseMachine.AbdominalCrunch
                    => MuscleGroup.Abs,

                // Cardio affects Legs + Cardio group
                ExerciseMachine.StairStepper
                    => MuscleGroup.Cardio | MuscleGroup.Legs,

                ExerciseMachine.Treadmill
                    => MuscleGroup.Cardio | MuscleGroup.Legs,

                ExerciseMachine.Bike
                    => MuscleGroup.Cardio | MuscleGroup.Legs,

                _ => MuscleGroup.None
            };
        }
        public MuscleGroup GetMuscleGroup(){return GetMuscleGroups(Machine);}
        private void RecalculateEffort()
        {
            // --- CARDIO ---
            if (Machine is ExerciseMachine.StairStepper or ExerciseMachine.Treadmill or ExerciseMachine.Bike
                && Calories is int kcal)
            {
                Effort = kcal switch
                {
                    < 100 => EffortLevel.Low,
                    >= 100 and < 200 => EffortLevel.Medium,
                    _ => EffortLevel.High
                };
                return;
            }

            if (Machine == ExerciseMachine.AbdominalCrunch && Reps is int r)
            {
                Range low = 0..15;
                Range mid = 15..30;
                Range high = 30..int.MaxValue;

                Effort = r switch
                {
                    _ when low.Contains(r) => EffortLevel.Low,
                    _ when mid.Contains(r) => EffortLevel.Medium,
                    _ when high.Contains(r) => EffortLevel.High,
                    _ => EffortLevel.Low
                };
                return;
            }

            if (Weight is double w)
            {
                Effort = w switch
                {
                    < 20 => EffortLevel.Low,
                    >= 20 and < 40 => EffortLevel.Medium,
                    _ => EffortLevel.High
                };
                return;
            }
            Effort = EffortLevel.Low;
        }

        private ExerciseType GetTypeFromMachine(ExerciseMachine machine)
        {
            return machine switch
            {
                ExerciseMachine.LegPress or ExerciseMachine.LegCurl or ExerciseMachine.LegExtension or
                ExerciseMachine.Abductor or ExerciseMachine.Adductor or ExerciseMachine.MultiHip
                    => ExerciseType.Legs,

                ExerciseMachine.ChestPress or ExerciseMachine.Pectoral
                    => ExerciseType.Chest,

                ExerciseMachine.Row or ExerciseMachine.PullDown or ExerciseMachine.AssistedChinDip or ExerciseMachine.BackExtension
                    => ExerciseType.Back,

                ExerciseMachine.TricepsExtension or ExerciseMachine.BicepsCurl or ExerciseMachine.ArmCurl
                    => ExerciseType.Arms,

                ExerciseMachine.ShoulderPress or ExerciseMachine.DeltsMachine or ExerciseMachine.ReverseFly
                    => ExerciseType.Shoulders,

                ExerciseMachine.AbdominalCrunch
                    => ExerciseType.Abs,

                ExerciseMachine.StairStepper or ExerciseMachine.Treadmill or ExerciseMachine.Bike
                    => ExerciseType.Cardio,

                _ => ExerciseType.Legs
            };
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return format switch
            {
                "short" => $"{Machine}",
                "long" => ToString(),
                _ => ToString()
            };
        }

        public override string ToString()
        {
            if (Minutes == null)
                return $"{Machine} — {Sets}x{Reps} @ {Weight}kg — Effort: {Effort}";

            return $"{Machine} — {Minutes} min, {Calories} kcal — Effort: {Effort}";
        }
    }
}
