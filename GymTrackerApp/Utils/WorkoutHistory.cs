using System.Collections.Generic;

namespace GymTrackerApp.Utils
{
    public class WorkoutHistory<T> where T : ICloneable
    {
        private readonly Stack<T> _undoStack = new();
        private readonly Stack<T> _redoStack = new();

        public void AddState(T state)
        {
            if (state == null) return;

            _undoStack.Push((T)state.Clone());
            _redoStack.Clear();
        }

        public T? Undo()
        {
            if (_undoStack.Count <= 1)
                return default;

            var popped = _undoStack.Pop();
            _redoStack.Push(popped);

            return _undoStack.Peek(); // return previous state
        }

        public T? Redo()
        {
            if (_redoStack.Count == 0)
                return default;

            var restored = _redoStack.Pop();
            _undoStack.Push(restored);

            return restored;
        }
    }
}
