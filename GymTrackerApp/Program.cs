// See https://aka.ms/new-console-template for more information
using GymTrackerApp.Models;

var ex = new Exercise
{
    Name = "Chest Press",
    Sets = 5,
    Reps = 15,
    Weight = 15
};

// Testuojame paprastą ToString()
Console.WriteLine(ex);

// Testuojame IFormattable
Console.WriteLine(ex.ToString("short", null));
Console.WriteLine(ex.ToString("long", null));

Console.ReadLine();
