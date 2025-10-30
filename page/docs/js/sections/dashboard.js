/**
 * Dashboard Section - Main overview and quick actions
 */

function initDashboard(params = {}) {
    console.log('📊 Initializing Dashboard');

    // Update dashboard content with real-time data
    updateDashboardStats();
    updateRecentActivity();
    updateCurrentFocus();

    // Set up dashboard-specific event listeners
    setupDashboardEvents();
}

function updateDashboardStats() {
    const state = window.TankerMadeApp.getState();

    // Create dashboard content with current stats
    const dashboardContent = `
        <div class="dashboard-header">
            <h1>Developer Dashboard</h1>
            <p>Your TankerMade development progress at a glance</p>
        </div>

        <div class="grid grid-cols-3 mb-6">
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Overall Progress</h3>
                </div>
                <div class="card-body">
                    <div class="progress-bar mb-4">
                        <div class="progress-bar-fill" style="width: ${state.overallProgress}%"></div>
                    </div>
                    <div class="progress-bar-text">
                        <span>${state.completedTasks} / ${state.totalTasks} tasks</span>
                        <span>${state.overallProgress}%</span>
                    </div>
                    <p class="text-sm text-secondary mt-2">Week ${state.currentWeek} of ${state.totalWeeks}</p>
                </div>
            </div>

            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Current Phase</h3>
                </div>
                <div class="card-body">
                    <h4 class="font-semibold">Phase ${state.currentPhase}: Foundation</h4>
                    <p class="text-secondary">Basic infrastructure and authentication</p>
                    <div class="mt-4">
                        <button class="btn btn-primary" onclick="window.TankerMadeRouter.goToSection('dev-tracker')">
                            View Details
                        </button>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Incidents</h3>
                    <span class="badge badge-${state.openIncidents > 0 ? 'error' : 'success'}">
                        ${state.openIncidents} open
                    </span>
                </div>
                <div class="card-body">
                    <p class="text-secondary">
                        ${state.openIncidents === 0 ? 'No open incidents' : `${state.openIncidents} incidents need attention`}
                    </p>
                    <div class="mt-4">
                        <button class="btn btn-secondary" onclick="window.TankerMadeRouter.goToSection('incidents')">
                            ${state.openIncidents > 0 ? 'View Incidents' : 'View Tracker'}
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div class="grid grid-cols-2 mb-6">
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Quick Actions</h3>
                </div>
                <div class="card-body">
                    <div class="grid grid-cols-2">
                        <button class="btn btn-primary mb-2" onclick="window.TankerMadeRouter.goToSection('dev-tracker')">
                            📊 Dev Tracker
                        </button>
                        <button class="btn btn-secondary mb-2" onclick="window.TankerMadeRouter.goToSection('workbench')">
                            📚 Workbench
                        </button>
                        <button class="btn btn-secondary mb-2" onclick="window.TankerMadeRouter.goToSection('architecture')">
                            🏗️ Architecture
                        </button>
                        <button class="btn btn-secondary" onclick="window.TankerMadeApp.exportProgress()">
                            📤 Export Progress
                        </button>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Recent Activity</h3>
                </div>
                <div class="card-body" id="recent-activity">
                    <div class="loading-indicator">Loading recent activity...</div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                <h3 class="card-title">Current Focus</h3>
            </div>
            <div class="card-body" id="current-focus">
                <div class="loading-indicator">Loading current tasks...</div>
            </div>
        </div>
    `;

    const contentContainer = document.getElementById('content-container');
    if (contentContainer) {
        contentContainer.innerHTML = dashboardContent;
    }
}

function updateRecentActivity() {
    // Simulate recent activity data
    const activities = [
        { type: 'success', icon: '✅', text: 'Authentication service completed', time: '2 hours ago' },
        { type: 'info', icon: '📝', text: 'Updated domain model documentation', time: '4 hours ago' },
        { type: 'warning', icon: '🔧', text: 'Fixed build pipeline configuration', time: '6 hours ago' },
        { type: 'success', icon: '📊', text: 'Phase 1 progress: 30% → 35%', time: '1 day ago' },
        { type: 'info', icon: '🚨', text: 'Resolved incident #122', time: '2 days ago' }
    ];

    const activityHtml = activities.map(activity => `
        <div class="flex items-center gap-3 mb-3 last:mb-0">
            <span class="status-dot ${activity.type}"></span>
            <div class="flex-1">
                <p class="text-sm">${activity.text}</p>
                <p class="text-xs text-secondary">${activity.time}</p>
            </div>
        </div>
    `).join('');

    const activityContainer = document.getElementById('recent-activity');
    if (activityContainer) {
        activityContainer.innerHTML = activityHtml;
    }
}

function updateCurrentFocus() {
    // Get current focus tasks
    const focusTasks = [
        { text: 'Complete Project CRUD operations', priority: 'high', estimated: '4 hours' },
        { text: 'Implement validation rules', priority: 'medium', estimated: '2 hours' },
        { text: 'Create unit tests for auth service', priority: 'medium', estimated: '3 hours' },
        { text: 'Set up CI/CD pipeline configuration', priority: 'low', estimated: '1 hour' }
    ];

    const focusHtml = `
        <div class="mb-4">
            <p class="text-secondary">Next tasks to focus on for Phase 1 completion:</p>
        </div>
        <div class="space-y-3">
            ${focusTasks.map(task => `
                <div class="flex items-center justify-between p-3 bg-secondary rounded-lg">
                    <div class="flex-1">
                        <p class="font-medium">${task.text}</p>
                        <p class="text-sm text-secondary">Estimated: ${task.estimated}</p>
                    </div>
                    <div class="flex items-center gap-2">
                        <span class="badge badge-${task.priority === 'high' ? 'error' : task.priority === 'medium' ? 'warning' : 'secondary'}">
                            ${task.priority}
                        </span>
                        <button class="btn btn-sm btn-primary" onclick="markTaskComplete(this)">
                            Complete
                        </button>
                    </div>
                </div>
            `).join('')}
        </div>
        <div class="mt-4">
            <button class="btn btn-primary" onclick="window.TankerMadeRouter.goToSection('dev-tracker', {phase: 1})">
                View All Phase 1 Tasks
            </button>
        </div>
    `;

    const focusContainer = document.getElementById('current-focus');
    if (focusContainer) {
        focusContainer.innerHTML = focusHtml;
    }
}

function setupDashboardEvents() {
    // Set up any dashboard-specific event listeners
    console.log('Dashboard events initialized');
}

function markTaskComplete(button) {
    // Mark a task as complete and update the UI
    const taskRow = button.closest('.flex');
    if (taskRow) {
        taskRow.style.opacity = '0.6';
        button.textContent = 'Completed';
        button.disabled = true;
        button.classList.remove('btn-primary');
        button.classList.add('btn-success');

        // Show success notification
        window.TankerMadeApp.showNotification('Task marked as complete!', 'success');

        // Update progress after a short delay
        setTimeout(() => {
            window.TankerMadeApp.loadProgressData();
        }, 1000);
    }
}

// Export functions for global access
window.initDashboard = initDashboard;
window.markTaskComplete = markTaskComplete;
