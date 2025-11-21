using System;

namespace GymTrackerApp.Models
{
    public class Exercise : IFormattable
    {
        public EffortLevel Effort { get; private set; }
        public ExerciseMachine Machine { get; }
        public ExerciseType Type { get; }
        public int Sets { get; }
        public int Reps { get; }
        public double Weight { get; }

        public Exercise(
            ExerciseMachine machine,
            int sets,
            int reps,
            double weight)
        {
            Machine = machine;
            Type = GetTypeFromMachine(machine);
            Sets = sets;
            Reps = reps;
            Weight = weight;

            RecalculateEffort(); // ← AUTOMATIŠKAI APSKAIČIUOJAME
        }

        // ------------ Effort logika (switch + when) ------------
        private void RecalculateEffort()
        {
            Effort = Weight switch
            {
                < 20                     => EffortLevel.Low,
                >= 20 and < 40           => EffortLevel.Medium,
                >= 40                    => EffortLevel.High,
                _                        => EffortLevel.Low
            };
        }

        // ------------ Machine → Type automatinis priskyrimas ------------
        private ExerciseType GetTypeFromMachine(ExerciseMachine machine)
        {
            return machine switch
            {
                ExerciseMachine.LegPress => ExerciseType.Legs,
                ExerciseMachine.LegCurl => ExerciseType.Legs,
                ExerciseMachine.LegExtension => ExerciseType.Legs,
                ExerciseMachine.Abductor => ExerciseType.Legs,
                ExerciseMachine.Adductor => ExerciseType.Legs,
                ExerciseMachine.MultiHip => ExerciseType.Legs,

                ExerciseMachine.ChestPress => ExerciseType.Chest,
                ExerciseMachine.Pectoral => ExerciseType.Chest,

                ExerciseMachine.Row => ExerciseType.Back,
                ExerciseMachine.PullDown => ExerciseType.Back,
                ExerciseMachine.AssistedChinDip => ExerciseType.Back,
                ExerciseMachine.BackExtension => ExerciseType.Back,

                ExerciseMachine.TricepsExtension => ExerciseType.Arms,
                ExerciseMachine.BicepsCurl => ExerciseType.Arms,
                ExerciseMachine.ArmCurl => ExerciseType.Arms,

                ExerciseMachine.ShoulderPress => ExerciseType.Shoulders,
                ExerciseMachine.DeltsMachine => ExerciseType.Shoulders,
                ExerciseMachine.ReverseFly => ExerciseType.Shoulders,

                ExerciseMachine.AbdominalCrunch => ExerciseType.Abs,

                ExerciseMachine.StairStepper => ExerciseType.Cardio,
                ExerciseMachine.Treadmill => ExerciseType.Cardio,
                ExerciseMachine.Bike => ExerciseType.Cardio,

                _ => ExerciseType.Legs // default
            };
        }

        // ------------ IFormattable ------------
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return format switch
            {
                "short" => $"{Machine}: {Sets}x{Reps}",
                "long" =>
                    $"{Machine} ({Type}) – {Sets} sets × {Reps} reps @ {Weight}kg — Effort: {Effort}",
                _ => ToString()
            };
        }

        public override string ToString()
            => $"{Machine} – {Sets}x{Reps} @ {Weight}kg — Effort: {Effort}";
    }
}
