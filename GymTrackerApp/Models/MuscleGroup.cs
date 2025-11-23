namespace GymTrackerApp.Models
{
    [Flags]
    public enum MuscleGroup
    {
        None     = 0,
        Legs     = 1 << 0,  // 1
        Chest    = 1 << 1,  // 2
        Back     = 1 << 2,  // 4
        Arms     = 1 << 3,  // 8
        Shoulders= 1 << 4,  // 16
        Abs      = 1 << 5,  // 32
        Cardio   = 1 << 6,  // 64

        Core     = Back | Abs, 
        UpperBody= Chest | Back | Arms | Shoulders,
        LowerBody= Legs,
        FullBody = Legs | Chest | Back | Abs | Arms | Shoulders
    }
}
