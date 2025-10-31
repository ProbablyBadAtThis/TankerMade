/**
 * Checkbox Helper for Progress Tracking
 * Automatically syncs checkbox states with ProgressManager
 */

class CheckboxHelper {
    static setupPhaseCheckboxes(phaseId, container) {
        if (!container) return;

        const checkboxes = container.querySelectorAll('input[type="checkbox"][data-task-id]');

        checkboxes.forEach(checkbox => {
            const taskId = checkbox.dataset.taskId;

            // Set initial state from saved progress
            if (window.TankerMadeProgress) {
                checkbox.checked = window.TankerMadeProgress.isTaskComplete(phaseId, taskId);
            }

            // Listen for changes
            checkbox.addEventListener('change', (e) => {
                const isChecked = e.target.checked;

                if (window.TankerMadeProgress) {
                    window.TankerMadeProgress.setTaskComplete(phaseId, taskId, isChecked);
                }

                // Update visual feedback
                CheckboxHelper.updateTaskVisuals(e.target, isChecked);

                console.log(`✅ Task ${taskId} ${isChecked ? 'completed' : 'uncompleted'}`);
            });

            // Set initial visuals
            CheckboxHelper.updateTaskVisuals(checkbox, checkbox.checked);
        });
    }

    static updateTaskVisuals(checkbox, isComplete) {
        const taskElement = checkbox.closest('.task-item, .task-card, li');
        if (taskElement) {
            if (isComplete) {
                taskElement.classList.add('task-completed');
            } else {
                taskElement.classList.remove('task-completed');
            }
        }
    }

    // Create a progress-enabled checkbox
    static createCheckbox(phaseId, taskId, label, isRequired = false) {
        const isComplete = window.TankerMadeProgress?.isTaskComplete(phaseId, taskId) || false;

        return `
            <label class="task-checkbox ${isComplete ? 'task-completed' : ''}">
                <input type="checkbox" 
                       data-task-id="${taskId}" 
                       ${isComplete ? 'checked' : ''}
                       ${isRequired ? 'required' : ''}>
                <span class="checkmark"></span>
                <span class="task-label">${label}</span>
            </label>
        `;
    }
}

// Make globally available
window.CheckboxHelper = CheckboxHelper;