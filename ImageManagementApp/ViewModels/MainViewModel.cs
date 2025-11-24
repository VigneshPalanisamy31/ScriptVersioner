using ImageManagementApp.Models;
using ImageManagementApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagementApp.ViewModels
{
    public class MainViewModel
    {
        private readonly ImageManager _imageManager;
        private readonly UndoRedoService _undoRedoService;
        private readonly ImageFileService _imageFileService;
        private Script _currentScript;
        private Step? _selectedStep;

        public ObservableCollection<Step> Steps { get; private set; }
        public int ActiveImagesCount => _imageManager.GetActiveImagesCount();
        public int PendingImagesCount => _imageManager.GetPendingDeletionCount();

        public Step? SelectedStep
        {
            get => _selectedStep;
            set => _selectedStep = value;
        }

        public MainViewModel()
        {
            _imageManager = new ImageManager();
            _undoRedoService = new UndoRedoService();
            _imageFileService = new ImageFileService();
            _currentScript = new Script();
            Steps = _currentScript.Steps;
        }

        public void AddStep()
        {
            _undoRedoService.SaveState(_currentScript);

            var step = new Step();
            step.ImagePath = _imageManager.CreateImage(step.Name);
            step.Status = StepStatus.Active;

            _currentScript.Steps.Add(step);
        }

        public void DeleteStep(Step step)
        {
            _undoRedoService.SaveState(_currentScript);

            if (_currentScript.Steps.Contains(step))
            {
                step.Status = StepStatus.PendingDeletion;
                _imageManager.MoveImageToPendingDeletion(step.ImagePath);

                // Remove from view immediately
                _currentScript.Steps.Remove(step);
                Steps.Remove(step);
            }
        }

        public bool Undo()
        {
            var previousState = _undoRedoService.Undo(_currentScript);
            if (previousState == null)
                return false;

            RestoreScriptState(previousState);
            return true;
        }

        public bool Redo()
        {
            var nextState = _undoRedoService.Redo();
            if (nextState == null)
                return false;

            RestoreScriptState(nextState);
            return true;
        }

        private void RestoreScriptState(Script newScript)
        {
            // Handle image restoration for undone deletions
            foreach (var newStep in newScript.Steps)
            {
                // If step exists in new state but not in current (was undone), restore image
                if (!_currentScript.Steps.Any(s => s.Id == newStep.Id))
                {
                    _imageManager.RestoreImageFromPendingDeletion(newStep.ImagePath);
                }
            }

            // Handle image movement for redone deletions
            foreach (var oldStep in _currentScript.Steps)
            {
                // If step was in current but not in new (deletion was redone), move image to pending
                if (!newScript.Steps.Any(s => s.Id == oldStep.Id))
                {
                    _imageManager.MoveImageToPendingDeletion(oldStep.ImagePath);
                }
            }

            _currentScript = newScript;
            Steps.Clear();
            foreach (var step in _currentScript.Steps)
            {
                Steps.Add(step);
            }
        }

        public void SaveScript()
        {
            // Only save the current state, don't delete anything
            // Pending deletion steps remain until script is closed
            _undoRedoService.SaveState(_currentScript);
            ScriptStatus = "Saved at " + DateTime.Now.ToString("HH:mm:ss");
        }

        public void CloseScript()
        {
            // Only on close, we clean up pending deletions
            var usedImages = _currentScript.Steps
                .Where(s => s.Status == StepStatus.Active)
                .Select(s => s.ImagePath)
                .ToList();

            _imageFileService.CleanupUnusedImages(usedImages);
            _imageManager.CleanupPendingDeletion();

            ScriptStatus = "Closed - Cleanup completed";
        }

        private string _scriptStatus = "Ready";
        public string ScriptStatus
        {
            get => _scriptStatus;
            set => _scriptStatus = value;
        }

        public List<string> GetGitStatus()
        {
            var status = new List<string>();
            status.Add("=== Git Status (Image Files) ===");
            status.AddRange(_imageManager.GetGitStatusForImages());

            if (status.Count == 1)
                status.Add("No changes detected");

            return status;
        }
    }
}
