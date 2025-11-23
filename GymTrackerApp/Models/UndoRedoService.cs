using GymTrackerApp.Models;

namespace GymTrackerApp.Services
{
    public class UndoRedoService
    {
        private readonly Stack<List<Workout>> _undo = new();
        private readonly Stack<List<Workout>> _redo = new();

        /// Save copy of current workouts
        public void SaveState(IEnumerable<Workout> workouts)
        {
            _undo.Push(workouts.Select(w => new Workout(w)).ToList());
            _redo.Clear();
        }

        public List<Workout>? Undo(IEnumerable<Workout> current)
        {
            if (_undo.Count == 0)
                return null;

            _redo.Push(current.Select(w => new Workout(w)).ToList());

            return _undo.Pop();
        }

        public List<Workout>? Redo(IEnumerable<Workout> current)
        {
            if (_redo.Count == 0)
                return null;

            _undo.Push(current.Select(w => new Workout(w)).ToList());

            return _redo.Pop();
        }
    }
}
