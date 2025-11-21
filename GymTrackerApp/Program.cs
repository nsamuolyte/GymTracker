using GymTrackerApp.Models;
using GymTrackerApp.Services;
using System.Linq;

// sukuria servisą
var service = new WorkoutService();

bool running = true;

while (running)
{
    Console.WriteLine("\n======= GYM TRACKER MENU =======");
    Console.WriteLine("1. Create new workout");
    Console.WriteLine("2. Add exercise to workout");
    Console.WriteLine("3. View all workouts");
    Console.WriteLine("4. Exit");
    Console.Write("Choose: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            CreateWorkout(service);
            break;

        case "2":
            AddExerciseToWorkout(service);
            break;

        case "3":
            ShowAllWorkouts(service);
            break;

        case "4":
            running = false;
            Console.WriteLine("Exiting...");
            break;

        default:
            Console.WriteLine("Unknown option!");
            break;
    }
}



///// --- FUNCTIONS --- /////


void CreateWorkout(WorkoutService service)
{
    Console.Write("Enter workout title: ");
    string title = Console.ReadLine() ?? "Untitled";

    var w = new Workout(DateTime.Now, title);
    service.AddWorkout(w);

    Console.WriteLine("Workout created!");
}

void AddExerciseToWorkout(WorkoutService service)
{
    var workouts = service.GetAllWorkouts().ToList();
    if (workouts.Count == 0)
    {
        Console.WriteLine("No workouts! Create one first.");
        return;
    }

    Console.WriteLine("\nChoose workout:");
    for (int i = 0; i < workouts.Count; i++)
        Console.WriteLine($"{i + 1}. {workouts[i].Title}");

    int index = GetNumberInput("Enter number: ") - 1;

    if (index < 0 || index >= workouts.Count)
    {
        Console.WriteLine("Invalid workout!");
        return;
    }

    var workout = workouts[index];

    Console.WriteLine("\nChoose machine:");
    var machines = Enum.GetValues<ExerciseMachine>();

    for (int i = 0; i < machines.Length; i++)
        Console.WriteLine($"{i + 1}. {machines[i]}");

    int machineIndex = GetNumberInput("Enter number: ") - 1;

    if (machineIndex < 0 || machineIndex >= machines.Length)
    {
        Console.WriteLine("Invalid machine!");
        return;
    }

    var machine = machines[machineIndex];

    // ================================
    //      CARDIO → time + kcal
    // ================================
    if (machine is ExerciseMachine.Bike
        or ExerciseMachine.Treadmill
        or ExerciseMachine.StairStepper)
    {
        int time = GetNumberInput("Enter time (minutes): ");
        double kcal = GetDoubleInput("Enter calories burned: ");

        var cardio = new Exercise(
            machine: machine,
            sets: time,     // TEMPORARY: using sets as time
            reps: 0,
            weight: kcal    // TEMPORARY: using weight as kcal
        );

        workout.AddExercise(cardio);
        Console.WriteLine($"Cardio added! Effort level: {cardio.Effort}");
        return;
    }

    // ================================
    //     ABDOMINAL → sets + reps
    // ================================
    if (machine is ExerciseMachine.AbdominalCrunch)
    {
        int sets = GetNumberInput("Enter sets: ");
        int reps = GetNumberInput("Enter reps: ");

        var abs = new Exercise(machine, sets, reps, weight: 0);
        workout.AddExercise(abs);

        Console.WriteLine($"Abs exercise added! Effort level: {abs.Effort}");
        return;
    }

    // ================================
    //       STRENGTH → sets/reps/kg
    // ================================
    int s = GetNumberInput("Enter sets: ");
    int r = GetNumberInput("Enter reps: ");
    double w = GetDoubleInput("Enter weight (kg): ");

    var ex = new Exercise(machine, s, r, w);
    workout.AddExercise(ex);

    Console.WriteLine($"Exercise added! Effort level: {ex.Effort}");
}



void ShowAllWorkouts(WorkoutService service)
{
    var workouts = service.GetAllWorkouts().ToList();

    if (workouts.Count == 0)
    {
        Console.WriteLine("No workouts found.");
        return;
    }

    Console.WriteLine("\n===== ALL WORKOUTS =====\n");

    foreach (var w in workouts)
    {
        Console.WriteLine(w);

        foreach (var ex in w)
            Console.WriteLine("  • " + ex.ToString("long", null));

        Console.WriteLine();
    }
}


// helpers
int GetNumberInput(string message)
{
    Console.Write(message);
    int.TryParse(Console.ReadLine(), out int value);
    return value;
}

double GetDoubleInput(string msg)
{
    Console.Write(msg);
    double.TryParse(Console.ReadLine(), out double value);
    return value;
}
