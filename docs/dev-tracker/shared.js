/* ============================================
   SHARED JAVASCRIPT FOR TANKERMADE DEV TRACKER
   ============================================ */

/**
 * Render shared header and navigation
 * Call this before initializePhaseTracker
 */
function renderHeaderAndNav(config) {
    const { phaseNumber, phaseTitle } = config;

    // Create header HTML
    const headerHTML = `
        <header>
            <div class="container">
                <div class="header-content">
                    <div class="logo">🛠️ TankerMade Dev Tracker</div>
                    <div class="phase-progress">
                        <span id="progress-text">0% Complete</span>
                        <div id="progress-circle" class="progress-circle" style="background: conic-gradient(var(--success-color) 0deg, var(--success-color) 0deg, rgba(255,255,255,0.3) 0deg);">0%</div>
                        <span id="task-count">0/0 tasks</span>
                    </div>
                </div>
            </div>
        </header>
        
        <nav>
            <div class="container">
                <div class="nav-content">
                    <a href="index.html" class="nav-link">Dashboard</a>
                    ${Array.from({ length: 10 }, (_, i) => {
        const num = i + 1;
        const isActive = num === phaseNumber;
        return `<a href="phase-${num}.html" class="nav-link ${isActive ? 'active' : ''}">Phase ${num}</a>`;
    }).join('')}
                </div>
            </div>
        </nav>
    `;

    // Find placeholder and replace with actual header
    const placeholder = document.getElementById('header-nav-placeholder');
    if (placeholder) {
        placeholder.outerHTML = headerHTML;
    } else {
        // If no placeholder, insert at beginning of body
        document.body.insertAdjacentHTML('afterbegin', headerHTML);
    }
}

/**
 * Initialize page with proper checkbox state handling
 * Call this from each phase page with phase-specific config
 */
function initializePhaseTracker(config) {
    const { phaseNumber, phaseTitle, taskIds, criteriaIds } = config;
    const STORAGE_KEY = `phase-${phaseNumber}-tasks`;
    const CRITERIA_KEY = `phase-${phaseNumber}-criteria`;

    // Wait for DOM to be fully loaded
    document.addEventListener('DOMContentLoaded', function () {
        // Render header/nav first
        renderHeaderAndNav({ phaseNumber, phaseTitle });

        // Then initialize tracker
        loadProgress();
        updateAllProgress();
        setupEventListeners();
        applyInitialCompletedStyles();

        // Show page after everything is loaded
        document.body.classList.add('loaded');
    });

    /**
     * CRITICAL: Apply completed styling to checkboxes that are already checked in HTML
     * This fixes the strikethrough issue on page load
     */
    function applyInitialCompletedStyles() {
        // Task checkboxes
        const checkedTasks = document.querySelectorAll('.task-checkbox:checked');
        checkedTasks.forEach(checkbox => {
            const taskItem = checkbox.closest('.task-item');
            if (taskItem) {
                taskItem.classList.add('completed');
            }
        });

        // Criteria checkboxes
        const checkedCriteria = document.querySelectorAll('.criteria-checkbox:checked');
        checkedCriteria.forEach(checkbox => {
            const criteriaItem = checkbox.closest('.criteria-item');
            if (criteriaItem) {
                criteriaItem.classList.add('completed');
            }
        });
    }

    function setupEventListeners() {
        // Task checkboxes
        taskIds.forEach(taskId => {
            const checkbox = document.querySelector(`[data-task="${taskId}"]`);
            if (checkbox) {
                checkbox.addEventListener('change', function () {
                    saveProgress();
                    updateAllProgress();
                });
            }
        });

        // Criteria checkboxes
        if (criteriaIds && criteriaIds.length > 0) {
            criteriaIds.forEach(criteriaId => {
                const checkbox = document.querySelector(`[data-criteria="${criteriaId}"]`);
                if (checkbox) {
                    checkbox.addEventListener('change', function () {
                        saveCriteria();
                        updateAllProgress();
                    });
                }
            });
        }
    }

    function loadProgress() {
        // Load task progress from localStorage
        const saved = localStorage.getItem(STORAGE_KEY);
        if (saved) {
            const completedTasks = JSON.parse(saved);
            taskIds.forEach(taskId => {
                const checkbox = document.querySelector(`[data-task="${taskId}"]`);
                if (checkbox && !checkbox.hasAttribute('checked')) {
                    // Only apply localStorage if HTML doesn't have checked attribute
                    checkbox.checked = completedTasks.includes(taskId);
                    const taskItem = checkbox.closest('.task-item');
                    if (taskItem && checkbox.checked) {
                        taskItem.classList.add('completed');
                    }
                }
            });
        }

        // Load criteria progress from localStorage
        if (criteriaIds && criteriaIds.length > 0) {
            const savedCriteria = localStorage.getItem(CRITERIA_KEY);
            if (savedCriteria) {
                const completedCriteria = JSON.parse(savedCriteria);
                criteriaIds.forEach(criteriaId => {
                    const checkbox = document.querySelector(`[data-criteria="${criteriaId}"]`);
                    if (checkbox && !checkbox.hasAttribute('checked')) {
                        checkbox.checked = completedCriteria.includes(criteriaId);
                        const criteriaItem = checkbox.closest('.criteria-item');
                        if (criteriaItem && checkbox.checked) {
                            criteriaItem.classList.add('completed');
                        }
                    }
                });
            }
        }
    }

    function saveProgress() {
        const completedTasks = taskIds.filter(taskId => {
            const checkbox = document.querySelector(`[data-task="${taskId}"]`);
            return checkbox && checkbox.checked;
        });
        localStorage.setItem(STORAGE_KEY, JSON.stringify(completedTasks));

        // Update visual state
        taskIds.forEach(taskId => {
            const checkbox = document.querySelector(`[data-task="${taskId}"]`);
            if (checkbox) {
                const taskItem = checkbox.closest('.task-item');
                if (taskItem) {
                    if (checkbox.checked) {
                        taskItem.classList.add('completed');
                    } else {
                        taskItem.classList.remove('completed');
                    }
                }
            }
        });

        // Update parent dashboard
        updateParentProgress();
    }

    function saveCriteria() {
        if (!criteriaIds || criteriaIds.length === 0) return;

        const completedCriteria = criteriaIds.filter(criteriaId => {
            const checkbox = document.querySelector(`[data-criteria="${criteriaId}"]`);
            return checkbox && checkbox.checked;
        });
        localStorage.setItem(CRITERIA_KEY, JSON.stringify(completedCriteria));

        // Update visual state
        criteriaIds.forEach(criteriaId => {
            const checkbox = document.querySelector(`[data-criteria="${criteriaId}"]`);
            if (checkbox) {
                const criteriaItem = checkbox.closest('.criteria-item');
                if (criteriaItem) {
                    if (checkbox.checked) {
                        criteriaItem.classList.add('completed');
                    } else {
                        criteriaItem.classList.remove('completed');
                    }
                }
            }
        });
    }

    function updateAllProgress() {
        const totalTasks = taskIds.length;
        const completedTasks = taskIds.filter(taskId => {
            const checkbox = document.querySelector(`[data-task="${taskId}"]`);
            return checkbox && checkbox.checked;
        }).length;

        const percentage = totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;

        // Update header progress
        const progressText = document.getElementById('progress-text');
        const taskCount = document.getElementById('task-count');
        const progressCircle = document.getElementById('progress-circle');

        if (progressText) progressText.textContent = `${percentage}% Complete`;
        if (taskCount) taskCount.textContent = `${completedTasks}/${totalTasks} tasks`;
        if (progressCircle) {
            progressCircle.textContent = `${percentage}%`;
            const degrees = (percentage / 100) * 360;
            progressCircle.style.background = `conic-gradient(var(--success-color) 0deg, var(--success-color) ${degrees}deg, rgba(255,255,255,0.3) ${degrees}deg)`;
        }

        // Update section-specific progress if exists
        updateSectionProgress();
    }

    function updateSectionProgress() {
        // Look for section progress elements and update them
        const sections = document.querySelectorAll('[data-section-tasks]');
        sections.forEach(section => {
            const sectionTaskIds = section.dataset.sectionTasks.split(',');
            const completed = sectionTaskIds.filter(taskId => {
                const checkbox = document.querySelector(`[data-task="${taskId}"]`);
                return checkbox && checkbox.checked;
            }).length;

            const progressElement = section.querySelector('.section-progress');
            if (progressElement) {
                progressElement.textContent = `${completed}/${sectionTaskIds.length} tasks`;
            }
        });
    }

    function updateParentProgress() {
        // Notify parent dashboard about progress change
        const totalTasks = taskIds.length;
        const completedTasks = taskIds.filter(taskId => {
            const checkbox = document.querySelector(`[data-task="${taskId}"]`);
            return checkbox && checkbox.checked;
        }).length;

        const progressData = {
            completed: completedTasks,
            total: totalTasks
        };

        localStorage.setItem(`phase-${phaseNumber}-progress`, JSON.stringify(progressData));
    }

    // Global functions for button actions
    window.markAllComplete = function () {
        if (!confirm("Mark all tasks as complete? This will check every checkbox in this phase.")) return;

        taskIds.forEach(taskId => {
            const checkbox = document.querySelector(`[data-task="${taskId}"]`);
            if (checkbox) checkbox.checked = true;
        });

        if (criteriaIds && criteriaIds.length > 0) {
            criteriaIds.forEach(criteriaId => {
                const checkbox = document.querySelector(`[data-criteria="${criteriaId}"]`);
                if (checkbox) checkbox.checked = true;
            });
        }

        saveProgress();
        saveCriteria();
        updateAllProgress();
    };

    window.markAllIncomplete = function () {
        if (!confirm("Mark all tasks as incomplete? This will uncheck every checkbox in this phase.")) return;

        taskIds.forEach(taskId => {
            const checkbox = document.querySelector(`[data-task="${taskId}"]`);
            if (checkbox) checkbox.checked = false;
        });

        if (criteriaIds && criteriaIds.length > 0) {
            criteriaIds.forEach(criteriaId => {
                const checkbox = document.querySelector(`[data-criteria="${criteriaId}"]`);
                if (checkbox) checkbox.checked = false;
            });
        }

        saveProgress();
        saveCriteria();
        updateAllProgress();
    };

    window.exportPhaseProgress = function () {
        const data = {
            phase: phaseNumber,
            title: phaseTitle,
            tasks: localStorage.getItem(STORAGE_KEY),
            criteria: localStorage.getItem(CRITERIA_KEY),
            exportDate: new Date().toISOString()
        };

        const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `phase-${phaseNumber}-progress-${new Date().toISOString().split('T')[0]}.json`;
        a.click();
        URL.revokeObjectURL(url);
    };

    window.resetPhaseProgress = function () {
        if (!confirm("Reset all progress for this phase? This cannot be undone.")) return;

        localStorage.removeItem(STORAGE_KEY);
        localStorage.removeItem(CRITERIA_KEY);
        localStorage.removeItem(`phase-${phaseNumber}-progress`);
        location.reload();
    };

    // Initialize progress on load
    updateParentProgress();
}
