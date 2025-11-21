using GymTrackerApp.Models;
using GymTrackerApp.Services;

// sukuriame servisą
var service = new WorkoutService();

// Sukuriame treniruotę
var w = new Workout(DateTime.Now, "Pirmadienio treniruotė");

// Įrašome pratimus naudojant ENUM'us
w.AddExercise(new Exercise(
    ExerciseMachine.LegPress,
    ExerciseType.Legs,
    3,
    10,
    40));

w.AddExercise(new Exercise(
    ExerciseMachine.ChestPress,
    ExerciseType.Chest,
    4,
    12,
    25));

// Įdedame treniruotę į servisą
service.AddWorkout(w);

// ---- Išvedimas ----
Console.WriteLine("==== VISOS TRENIRUOTĖS ====");

foreach (var workout in service.GetAllWorkouts())
{
    Console.WriteLine(workout);

    foreach (var ex in workout)
        Console.WriteLine("  " + ex.ToString("long", null));
}
