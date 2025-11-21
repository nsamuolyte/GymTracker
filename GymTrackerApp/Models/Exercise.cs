using System;

namespace GymTrackerApp.Models
{
    public class Exercise : IFormattable
    {
        public EffortLevel Effort { get; private set; }
        public ExerciseMachine Machine { get; }
        public ExerciseType Type { get; }

        // Strength fields
        public int? Sets { get; }
        public int? Reps { get; }
        public double? Weight { get; }

        // Cardio fields
        public int? Minutes { get; }
        public int? Calories { get; }

        // ------------------ CONSTRUCTOR ------------------
        public Exercise(
            ExerciseMachine machine,
            int sets = 0,
            int reps = 0,
            double weight = 0,
            int minutes = 0,
            int calories = 0)
        {
            Machine = machine;
            Type = GetTypeFromMachine(machine);

            // ---- switch + when (pagal treniruoklio tipą) ----
            switch (machine)
            {
                // -------- Strength (reikia sets/reps/weight) --------
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

                // -------- Abs (reikia sets/reps, bet be weight) --------
                case ExerciseMachine.AbdominalCrunch:
                    Sets = sets;
                    Reps = reps;
                    Weight = null;
                    break;

                // -------- Cardio (reikia minutes + calories) --------
                case ExerciseMachine.StairStepper:
                case ExerciseMachine.Treadmill:
                case ExerciseMachine.Bike:
                    Minutes = minutes;
                    Calories = calories;
                    break;

                default:
                    throw new Exception("Unknown machine type.");
            }

            RecalculateEffort();
        }

        // ------------------ EFFORT LEVEL ------------------
        private void RecalculateEffort()
        {
            Effort = Weight switch
            {
                < 20 => EffortLevel.Low,
                >= 20 and < 40 => EffortLevel.Medium,
                >= 40 => EffortLevel.High,
                _ => EffortLevel.Low
            };
        }

        // ------------------ MACHINE → TYPE ------------------
        private ExerciseType GetTypeFromMachine(ExerciseMachine machine)
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
                    => ExerciseType.Legs,

                // Chest
                ExerciseMachine.ChestPress or ExerciseMachine.Pectoral
                    => ExerciseType.Chest,

                // Back
                ExerciseMachine.Row or ExerciseMachine.PullDown or
                ExerciseMachine.AssistedChinDip or ExerciseMachine.BackExtension
                    => ExerciseType.Back,

                // Arms
                ExerciseMachine.TricepsExtension or ExerciseMachine.BicepsCurl or
                ExerciseMachine.ArmCurl
                    => ExerciseType.Arms,

                // Shoulders
                ExerciseMachine.ShoulderPress or ExerciseMachine.DeltsMachine or
                ExerciseMachine.ReverseFly
                    => ExerciseType.Shoulders,

                // Abs
                ExerciseMachine.AbdominalCrunch
                    => ExerciseType.Abs,

                // Cardio
                ExerciseMachine.StairStepper or ExerciseMachine.Treadmill or ExerciseMachine.Bike
                    => ExerciseType.Cardio,

                _ => ExerciseType.Legs
            };
        }

        // ------------------ FORMATTING ------------------
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
            // Strength exercise
            if (Minutes == null)
                return $"{Machine} — {Sets}x{Reps} @ {Weight}kg — Effort: {Effort}";

            // Cardio exercise
            return $"{Machine} — {Minutes} min, {Calories} kcal — Effort: {Effort}";
        }
    }
}
