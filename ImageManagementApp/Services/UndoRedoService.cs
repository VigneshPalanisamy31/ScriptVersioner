using ImageManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagementApp.Services
{
    public class UndoRedoService
    {
        private readonly Stack<Script> _undoStack = new();
        private readonly Stack<Script> _redoStack = new();

        public void SaveState(Script script)
        {
            _undoStack.Push(script.DeepCopy());
            _redoStack.Clear();
        }

        public Script? Undo(Script currentScript)
        {
            if (_undoStack.Count == 0)
                return null;

            _redoStack.Push(currentScript.DeepCopy());
            return _undoStack.Pop();
        }

        public Script? Redo()
        {
            if (_redoStack.Count == 0)
                return null;

            return _redoStack.Pop();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }
}
