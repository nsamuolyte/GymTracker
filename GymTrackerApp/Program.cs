using GymTrackerApp.Models;
using GymTrackerApp.Exceptions;
using GymTrackerApp.Services;
using System.Linq;

var service = new WorkoutService();

bool running = true;

while (running)
{
    Console.WriteLine("\n======= GYM TRACKER MENU =======");
    Console.WriteLine("1. Create new workout");
    Console.WriteLine("2. Add exercise to workout");
    Console.WriteLine("3. View all workouts");
    Console.WriteLine("4. Set exercise filter");
    Console.WriteLine("5. Clear filter");
    Console.WriteLine("6. Exit");
    Console.Write("Choose: ");

    string? choice = Console.ReadLine()?.Trim();

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
            SetFilter(service);
            break;

        case "5":
            service.Filter = null;
            Console.WriteLine("Filter cleared!");
            break;

        case "6":
            running = false;
            Console.WriteLine("Exiting...");
            break;

        default:
            Console.WriteLine($"Unknown option: {choice}");
            break;
    }
}

void CreateWorkout(WorkoutService service)
{
    Console.Write("Enter workout title: ");
    string? title = Console.ReadLine();
    title ??= "Untitled";

    Console.Write("Enter workout date (yyyy-MM-dd): ");
    string? input = Console.ReadLine()?.Trim();

    DateTime date;

    while (!DateTime.TryParse(input, out date) || date > DateTime.Today)
    {
        Console.WriteLine("Invalid date! Cannot be in the future.");
        Console.Write("Enter workout date (yyyy-MM-dd): ");

        input = Console.ReadLine()?.Trim();
        input ??= "";
    }

    var w = new Workout(date, title);
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

    try
    {
        if (index < 0 || index >= workouts.Count)
            throw new WorkoutNotFoundException("Workout does not exist!");
    }
    catch (WorkoutNotFoundException ex)
    {
        Console.WriteLine(ex.Message);
        return;
    }

    var workout = workouts[index];
    var allMachines = Enum.GetValues<ExerciseMachine>().ToList();

    if (service.Filter != null)
    {
        allMachines = allMachines
            .Where(m =>
            {
                var fake = new Exercise(m);
                return service.Filter(fake);
            })
            .ToList();
    }

    if (allMachines.Count == 0)
    {
        Console.WriteLine("No exercises match your filter. Clear filter first.");
        return;
    }

    Console.WriteLine("\nChoose machine:");
    for (int i = 0; i < allMachines.Count; i++)
        Console.WriteLine($"{i + 1}. {allMachines[i]}");

    int machineIndex = GetNumberInput("Enter number: ") - 1;

    if (machineIndex < 0 || machineIndex >= allMachines.Count)
    {
        Console.WriteLine("Invalid machine!");
        return;
    }

    var machine = allMachines[machineIndex];

    if (machine is ExerciseMachine.Bike
        or ExerciseMachine.Treadmill
        or ExerciseMachine.StairStepper)
    {
        int minutes = GetNumberInput("Enter time (minutes): ");
        int kcal = (int)GetDoubleInput("Enter calories burned: ");

        var cardio = new Exercise(machine, minutes: minutes, calories: kcal);
        workout.AddExercise(cardio);

        service.SaveToFile();
        Console.WriteLine($"Cardio added! Effort: {cardio.Effort}");
        return;
    }

    if (machine == ExerciseMachine.AbdominalCrunch)
    {
        int sets = GetNumberInput("Enter sets: ");
        int reps = GetNumberInput("Enter reps: ");

        var abs = new Exercise(machine, sets: sets, reps: reps);
        workout.AddExercise(abs);

        service.SaveToFile();
        Console.WriteLine($"Abs added! Effort: {abs.Effort}");
        return;
    }

    int s = GetNumberInput("Enter sets: ");
    int r = GetNumberInput("Enter reps: ");
    double w = GetDoubleInput("Enter weight (kg): ");

    var exercise = new Exercise(machine, s, r, w);
    workout.AddExercise(exercise);

    service.SaveToFile();
    Console.WriteLine($"Exercise added! Effort: {exercise.Effort}");
}

void SetFilter(WorkoutService service)
{
    Console.WriteLine("\nChoose filter:");
    Console.WriteLine("1. Only Chest");
    Console.WriteLine("2. Only Cardio");
    Console.WriteLine("3. Only Heavy (>40 kg)");
    Console.WriteLine("4. Only Abs");
    Console.Write("Choose: ");

    string? choice = Console.ReadLine()?.Trim();

    service.Filter = choice switch
    {
        "1" => ex => ex.Type == ExerciseType.Chest,
        "2" => ex => ex.Type == ExerciseType.Cardio,
        "3" => ex => ex.Weight is double w && w > 40,
        "4" => ex => ex.Type == ExerciseType.Abs,
        _ => null
    };

    Console.WriteLine("Filter applied!");
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
        var (date, title, count) = w;
        Console.WriteLine($"{date:yyyy-MM-dd} – {title} ({count} exercises)");

        MuscleGroup totalGroups = MuscleGroup.None;

        foreach (var ex in w)
        {
            if (service.Filter == null || service.Filter(ex))
            {
                Console.WriteLine($"  • {ex?.ToString("long", null) ?? "UNKNOWN EXERCISE"}");
            }

            totalGroups |= ex.GetMuscleGroup();
        }

        Console.WriteLine($"Muscle groups trained: {totalGroups}");
        Console.WriteLine();
    }
}

int GetNumberInput(string message)
{
    Console.Write(message);

    if (!int.TryParse(Console.ReadLine(), out int value))
        return -1;

    return value;
}

double GetDoubleInput(string msg)
{
    Console.Write(msg);

    if (!double.TryParse(Console.ReadLine(), out double value))
        return 0;

    return value;
}
