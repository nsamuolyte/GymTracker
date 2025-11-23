using GymTrackerApp.Models;
using GymTrackerApp.Exceptions;
using GymTrackerApp.Services;
using System.Linq;

var service = new WorkoutService();
var undoRedo = new UndoRedoService();

bool running = true;

while (running)
{
    Console.WriteLine("\n======= GYM TRACKER MENU =======");
    Console.WriteLine("1. Create new workout");
    Console.WriteLine("2. Add exercise to workout");
    Console.WriteLine("3. View all workouts");
    Console.WriteLine("4. Set exercise filter");
    Console.WriteLine("5. Clear filter");
    Console.WriteLine("6. Add group training");
    Console.WriteLine("7. Remove exercise");
    Console.WriteLine("8. Undo");
    Console.WriteLine("9. Redo");
    Console.WriteLine("10. Exit");
    Console.Write("Choose: ");

    string? choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1": CreateWorkout(service); break;
        case "2": AddExerciseToWorkout(service); break;
        case "3": ShowAllWorkouts(service); break;
        case "4": SetFilter(service); break;
        case "5": service.Filter = null; Console.WriteLine("Filter cleared!"); break;
        case "6": AddGroupTraining(service); break;
        case "7": RemoveExerciseFromWorkout(service); break;
        case "8": Undo(); break;
        case "9": Redo(); break;
        case "10": running = false; Console.WriteLine("Exiting..."); break;

        default: Console.WriteLine($"Unknown option: {choice}"); break;
    }
}

void CreateWorkout(WorkoutService service)
{
    Console.Write("Enter workout title: ");
    string? title = Console.ReadLine() ?? "Untitled";

    Console.Write("Enter workout date (yyyy-MM-dd): ");
    string? input = Console.ReadLine()?.Trim();

    DateTime date;
    while (!DateTime.TryParse(input, out date) || date > DateTime.Today)
    {
        Console.WriteLine("Invalid date! Cannot be in the future.");
        Console.Write("Enter workout date (yyyy-MM-dd): ");
        input = Console.ReadLine()?.Trim() ?? "";
    }

    undoRedo.SaveState(service.GetAllWorkouts());

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

    if (index < 0 || index >= workouts.Count)
    {
        Console.WriteLine("Workout does not exist!");
        return;
    }

    var workout = workouts[index];
    var allMachines = Enum.GetValues<ExerciseMachine>().ToList();

    // Filter
    if (service.Filter != null)
    {
        allMachines = allMachines
            .Where(m => service.Filter(new Exercise(m)))
            .ToList();
    }

    if (allMachines.Count == 0)
    {
        Console.WriteLine("No exercises match your filter.");
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

    undoRedo.SaveState(service.GetAllWorkouts());

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

    var ex = new Exercise(machine, s, r, w);
    workout.AddExercise(ex);

    service.SaveToFile();
    Console.WriteLine($"Exercise added! Effort: {ex.Effort}");
}

void RemoveExerciseFromWorkout(WorkoutService service)
{
    var workouts = service.GetAllWorkouts().ToList();
    if (workouts.Count == 0)
    {
        Console.WriteLine("No workouts found.");
        return;
    }

    Console.WriteLine("Choose workout:");
    for (int i = 0; i < workouts.Count; i++)
        Console.WriteLine($"{i + 1}. {workouts[i].Title}");

    int wIndex = GetNumberInput("Enter number: ") - 1;
    if (wIndex < 0 || wIndex >= workouts.Count)
    {
        Console.WriteLine("Invalid selection.");
        return;
    }

    var workout = workouts[wIndex];
    var exercises = workout.ToList();

    if (exercises.Count == 0)
    {
        Console.WriteLine("Workout has no exercises.");
        return;
    }

    Console.WriteLine("Choose exercise to remove:");
    for (int i = 0; i < exercises.Count; i++)
        Console.WriteLine($"{i + 1}. {exercises[i]}");

    int exIndex = GetNumberInput("Enter number: ") - 1;
    if (exIndex < 0 || exIndex >= exercises.Count)
    {
        Console.WriteLine("Invalid selection.");
        return;
    }

    undoRedo.SaveState(service.GetAllWorkouts());

    workout -= exercises[exIndex];

    service.SaveToFile();
    Console.WriteLine("Exercise removed.");
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
        if (w.GroupTrainings.Count > 0)
        {
            Console.WriteLine($"  Group trainings: {string.Join(", ", w.GroupTrainings)}");
        }


        MuscleGroup totalGroups = MuscleGroup.None;

        foreach (var ex in w)
        {
            if (service.Filter == null || service.Filter(ex))
                Console.WriteLine("  • " + ex.ToString("long", null));

            totalGroups |= ex.GetMuscleGroup();
        }

        Console.WriteLine($"Muscle groups trained: {totalGroups}\n");
    }
}

void Undo()
{
    var prev = undoRedo.Undo(service.GetAllWorkouts());
    if (prev == null)
    {
        Console.WriteLine("Nothing to undo.");
        return;
    }

    service.ReplaceWorkouts(prev);
    service.SaveToFile();
    Console.WriteLine("Undo complete.");
}

void Redo()
{
    var next = undoRedo.Redo(service.GetAllWorkouts());
    if (next == null)
    {
        Console.WriteLine("Nothing to redo.");
        return;
    }

    service.ReplaceWorkouts(next);
    service.SaveToFile();
    Console.WriteLine("Redo complete.");
}

int GetNumberInput(string message)
{
    Console.Write(message);
    return int.TryParse(Console.ReadLine(), out int value) ? value : -1;
}

double GetDoubleInput(string msg)
{
    Console.Write(msg);
    return double.TryParse(Console.ReadLine(), out double value) ? value : 0;
}

void AddGroupTraining(WorkoutService service)
{
    var workouts = service.GetAllWorkouts().ToList();

    if (workouts.Count == 0)
    {
        Console.WriteLine("No workouts found.");
        return;
    }

    Console.WriteLine("\nChoose workout:");
    for (int i = 0; i < workouts.Count; i++)
        Console.WriteLine($"{i + 1}. {workouts[i].Title}");

    int wIndex = GetNumberInput("Enter number: ") - 1;
    if (wIndex < 0 || wIndex >= workouts.Count)
    {
        Console.WriteLine("Invalid selection.");
        return;
    }

    var workout = workouts[wIndex];

    Console.WriteLine("\nChoose group training:");
    Console.WriteLine("1. Yoga");
    Console.WriteLine("2. Zumba");
    Console.WriteLine("3. Pilates");
    Console.WriteLine("4. Multiple (comma separated: yoga,zumba)");

    Console.Write("Choose: ");
    string? input = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Invalid.");
        return;
    }

    undoRedo.SaveState(service.GetAllWorkouts());

    string[] groups = input switch
    {
        "1" => new[] { "Yoga" },
        "2" => new[] { "Zumba" },
        "3" => new[] { "Pilates" },
        "4" => AskMultipleGroups(),
        _ => input.Split(',').Select(g => g.Trim()).ToArray()
    };

    workout.AddGroupTrainings(groups);

    service.SaveToFile();
    Console.WriteLine("Group training added.");
}

string[] AskMultipleGroups()
{
    Console.Write("Enter groups (comma separated): ");
    string? input = Console.ReadLine();

    return input?
        .Split(',')
        .Select(g => g.Trim())
        .Where(g => g.Length > 0)
        .ToArray()
        ?? Array.Empty<string>();
}

