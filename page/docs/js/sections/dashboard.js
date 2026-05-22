/**
 * Dashboard Section Controller - Pure JavaScript Implementation
 * Handles complete dashboard rendering and real-time updates
 */

class DashboardSection {
    constructor() {
        this.sectionId = 'dashboard';
        this.data = {
            totalTasks: 65,
            completedTasks: 32,
            currentPhase: {
                number: "C",
                title: "Module Project Workspace",
                description: "Step progress, timers, completion, pieces, archive, and editing",
                completed: 1,
                total: 6
            }
        };
    }

    async render() {
        const progressPercentage = Math.round((this.data.completedTasks / this.data.totalTasks) * 100);
        const phasePercentage = Math.round((this.data.currentPhase.completed / this.data.currentPhase.total) * 100);
        const isAuthenticated = !!window.TankerMadeAuth?.isAuthenticated?.();

        return `
            <div class="dashboard-container">
                <div class="dashboard-header">
                    <h1>Developer Dashboard</h1>
                    <p class="dashboard-subtitle">${isAuthenticated
                        ? 'Module progress, implementation focus, and release health in one working surface.'
                        : 'A public glimpse of TankerMade progress. Sign in for tracker details, docs, architecture, and incidents.'}</p>
                </div>

                <!-- First Row - Overall Progress & Current Phase & Incidents -->
                <div class="dashboard-grid-row dashboard-top-row">
                    <div class="card dashboard-card progress-card">
                        <div class="card-header">
                            <h2>Overall Progress</h2>
                        </div>
                        <div class="card-body">
                            <div class="progress-overview">
                                <div class="progress-stats">
                                    <span class="progress-stat">
                                        <span class="stat-value" id="total-completed">${this.data.completedTasks}</span>
                                        <span class="stat-label">/ <span id="total-tasks">${this.data.totalTasks}</span> tasks</span>
                                    </span>
                                    <span class="progress-stat">
                                        <span class="stat-value" id="current-week">Phase ${this.data.currentPhase.number}</span>
                                        <span class="stat-label">current</span>
                                    </span>
                                </div>

                                <div class="progress-bar-container">
                                    <div class="progress-bar">
                                        <div class="progress-fill" id="overall-progress" style="width: ${progressPercentage}%"></div>
                                    </div>
                                    <span class="progress-percentage" id="progress-percentage">${progressPercentage}%</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card dashboard-card phase-card">
                        <div class="card-header">
                            <h2>Current Phase</h2>
                        </div>
                        <div class="card-body">
                            <div class="current-phase-info">
                                <h3 class="phase-title" id="phase-title">Phase ${this.data.currentPhase.number}: ${this.data.currentPhase.title}</h3>
                                <p class="phase-description" id="phase-description">${this.data.currentPhase.description}</p>

                                <div class="phase-progress">
                                    <div class="progress-bar">
                                        <div class="progress-fill" id="phase-progress" style="width: ${phasePercentage}%"></div>
                                    </div>
                                    <span class="progress-text" id="phase-progress-text">${this.data.currentPhase.completed} / ${this.data.currentPhase.total} tasks</span>
                                </div>

                                ${isAuthenticated
                                    ? `<button class="btn btn-primary phase-details-btn" onclick="window.TankerMadeRouter.navigate('dev-tracker')">View Details</button>`
                                    : `<button class="btn btn-primary phase-details-btn" onclick="window.TankerMadeAuth && window.TankerMadeAuth.login()">Sign in for Details</button>`}
                            </div>
                        </div>
                    </div>

                    ${isAuthenticated ? `<div class="card dashboard-card incidents-card">
                        <div class="card-header">
                            <h2>Incidents</h2>
                            <span class="badge badge-success" id="dashboard-incident-badge">
                                0 open
                            </span>
                        </div>
                        <div class="card-body">
                            <p class="text-secondary" id="dashboard-incident-text">
                                No open incidents
                            </p>
                            <div class="mt-4">
                                <button class="btn btn-secondary" onclick="window.TankerMadeRouter.navigate('incidents')">
                                    View Tracker
                                </button>
                            </div>
                        </div>
                    </div>` : this.renderPublicGlimpseCard()}
                </div>

                <div class="public-roadmap-section">
                    <div class="card dashboard-card focus-panel">
                        <div class="card-header">
                            <h2>Roadmap Pulse</h2>
                        </div>
                        <div class="card-body">
                            <div class="focus-content week-summary-grid" id="week-summary">
                                ${this.renderWeekSummary()}
                            </div>
                        </div>
                    </div>
                </div>

                ${isAuthenticated ? `
                <div class="dashboard-section-title">
                    <h2>Current Focus</h2>
                    <p>Open Phase C work split into scan-friendly panels.</p>
                </div>

                <div class="dashboard-focus-panels">
                    <div class="card dashboard-card focus-panel focus-panel-wide">
                        <div class="card-header">
                            <h2>Active Tasks</h2>
                        </div>
                        <div class="card-body">
                            <div class="focus-content" id="active-tasks">
                                ${this.renderActiveTasks()}
                            </div>
                        </div>
                    </div>

                    <div class="card dashboard-card focus-panel">
                        <div class="card-header">
                            <h2>Phase Breakdown</h2>
                        </div>
                        <div class="card-body">
                            <div class="focus-content" id="phase-breakdown">
                                ${this.renderPhaseBreakdown()}
                            </div>
                        </div>
                    </div>

                    <div class="card dashboard-card focus-panel">
                        <div class="card-header">
                            <h2>Dev Environment</h2>
                        </div>
                        <div class="card-body">
                            <div class="focus-content" id="dev-environment">
                                ${this.renderDevEnvironment()}
                            </div>
                        </div>
                    </div>

                    <div class="card dashboard-card focus-panel">
                        <div class="card-header">
                            <h2>Next Up</h2>
                        </div>
                        <div class="card-body">
                            <div class="focus-content" id="next-tasks">
                                ${this.renderNextTasks()}
                            </div>
                        </div>
                    </div>

                    <div class="card dashboard-card focus-panel recent-activity-card">
                        <div class="card-header">
                            <h2>Recent Activity</h2>
                        </div>
                        <div class="card-body recent-activity-body">
                            <div class="activity-feed" id="activity-feed">
                                ${this.renderRecentActivity()}
                            </div>
                        </div>
                    </div>
                </div>
                ` : ''}
            </div>
        `;
    }

    renderPublicGlimpseCard() {
        return `
            <div class="card dashboard-card public-glimpse-card">
                <div class="card-header">
                    <h2>Project Glimpse</h2>
                </div>
                <div class="card-body">
                    <p class="text-secondary">TankerMade is a local-first modular maker workbench. Phase B is complete; Phase C is deepening module-owned project workspaces.</p>
                    <div class="mt-4">
                        <button class="btn btn-primary" onclick="window.TankerMadeAuth && window.TankerMadeAuth.login()">
                            Sign in with GitHub
                        </button>
                    </div>
                </div>
            </div>
        `;
    }

    renderActiveTasks() {
        const tasks = [
            { title: "Add pattern pieces and steps", meta: "Phase B • Crafting module", status: "done" },
            { title: "Build pattern detail page", meta: "Phase B • Module UI", status: "done" },
            { title: "Add step range display", meta: "Phase B • UX", status: "done" },
            { title: "Add progress aggregation and validation", meta: "Phase B • Domain behavior", status: "done" },
            { title: "Expand module-owned project workspace screens", meta: "Phase B • Projects", status: "done" },
            { title: "Add module-owned step/checklist progress", meta: "Phase C • Projects", status: "done" },
            { title: "Add module-owned timers", meta: "Phase C • Projects", status: "active" }
        ];

        return tasks.map(task => `
            <div class="task-item">
                <div class="task-status ${task.status}"></div>
                <div class="task-info">
                    <span class="task-title">${task.title}</span>
                    <span class="task-meta">${task.meta}</span>
                </div>
            </div>
        `).join('');
    }

    renderPhaseBreakdown() {
        const phases = [
            { number: "A", title: "Module Host & Reference Module", completed: 26, total: 26, active: false },
            { number: "B", title: "Crafting Module V2", completed: 5, total: 5, active: false },
            { number: "C", title: "Module Project Workspace", completed: 1, total: 6, active: true }
        ];

        return phases.map(phase => {
            const percentage = Math.round((phase.completed / phase.total) * 100);
            return `
                <div class="phase-mini-card ${phase.active ? 'active' : 'upcoming'}">
                    <span class="phase-number">${phase.number}</span>
                    <div class="phase-mini-info">
                        <span class="phase-mini-title">${phase.title}</span>
                        <div class="phase-mini-progress">
                            <div class="progress-bar mini">
                                <div class="progress-fill" style="width: ${percentage}%"></div>
                            </div>
                            <span class="progress-text">${phase.completed}/${phase.total}</span>
                        </div>
                    </div>
                </div>
            `;
        }).join('');
    }

    renderDevEnvironment() {
        const services = [
            { title: "GitHub OAuth", meta: "Client ID and callback flow configured", status: "online" },
            { title: "Cloudflare Pages", meta: "Static site plus Functions token exchange", status: "online" },
            { title: "Local Preview", meta: "Serving current Pages build", status: "online" }
        ];

        return services.map(service => `
            <div class="env-status-item">
                <div class="status-indicator ${service.status}"></div>
                <div class="status-info">
                    <span class="status-title">${service.title}</span>
                    <span class="status-meta">${service.meta}</span>
                </div>
            </div>
        `).join('');
    }

    renderWeekSummary() {
        const stats = [
            { value: "32", label: "Done" },
            { value: "5", label: "Open in Phase C" },
            { value: "9", label: "Roadmap Phases" }
        ];

        return stats.map(stat => `
            <div class="week-stat">
                <span class="week-stat-value">${stat.value}</span>
                <span class="week-stat-label">${stat.label}</span>
            </div>
        `).join('');
    }

    renderNextTasks() {
        const tasks = [
            { title: "Module-owned timers with play/pause", phase: "Phase C" },
            { title: "Module-specific completion percentage logic", phase: "Phase C" },
            { title: "Module-specific piece/section selector", phase: "Phase C" }
        ];

        return tasks.map(task => `
            <div class="next-task">
                <span class="next-task-title">${task.title}</span>
                <span class="next-task-phase">${task.phase}</span>
            </div>
        `).join('');
    }

    renderRecentActivity() {
        const activities = [
            { icon: "", title: "Phase A smoke test passed", time: "Today" },
            { icon: "", title: "Crafting module activation verified in client", time: "Today" },
            { icon: "", title: "Pattern and project CRUD verified", time: "Today" },
            { icon: "", title: "Cross-user ownership check passed", time: "Today" },
            { icon: "", title: "Module migration metadata repaired", time: "Today" },
            { icon: "", title: "Partial update behavior patched", time: "Today" }
        ];

        return activities.map(activity => `
            <div class="activity-item">
                <div class="activity-icon">${activity.icon}</div>
                <div class="activity-content">
                    <span class="activity-title">${activity.title}</span>
                    <span class="activity-time">${activity.time}</span>
                </div>
            </div>
        `).join('');
    }

    async init() {
        console.log('🏠 Dashboard section initializing...');

        // Get the root container
        const container = document.getElementById('dashboard-root');
        if (!container) {
            console.error('❌ Dashboard root container not found');
            return;
        }

        try {
            // Render complete dashboard
            const dashboardHTML = await this.render();
            container.innerHTML = dashboardHTML;

            console.log('✅ Dashboard rendered successfully');

            // Load real-time data if authenticated
            if (window.TankerMadeAuth?.isAuthenticated()) {
                await this.loadProgressData();
            }

            // Set up real-time updates
            this.setupRealtimeUpdates();

            // Set up click handlers
            this.setupEventHandlers();

        } catch (error) {
            console.error('❌ Dashboard rendering failed:', error);
            container.innerHTML = `
                <div class="dashboard-error">
                    <h2>Dashboard Loading Error</h2>
                    <p>Failed to load dashboard. Please refresh the page.</p>
                    <button onclick="location.reload()" class="btn btn-primary">Refresh</button>
                </div>
            `;
        }
    }

    setupEventHandlers() {
        // Add any additional click handlers here
        console.log('🔧 Dashboard event handlers set up');
    }

    async loadProgressData() {
        try {
            console.log('📊 Loading real-time progress data...');

            // Future: Connect to GitHub API for real data
            // const progressData = await window.TankerMadeGitHub.getProgressData();
            // this.updateProgressDisplays(progressData);

            this.updateProgressDisplays();

        } catch (error) {
            console.warn('⚠️ Failed to load progress data:', error);
        }
    }

    updateProgressDisplays(data = null) {
        if (data) {
            // Update with real data when available
            this.data = { ...this.data, ...data };
        }

        // Update any dynamic elements
        console.log('📈 Progress displays updated');
    }

    setupRealtimeUpdates() {
        // Set up periodic updates for activity feed and progress
        this.updateInterval = setInterval(async () => {
            if (window.TankerMadeAuth?.isAuthenticated()) {
                await this.loadProgressData();
            }
        }, 60000); // Every minute

        console.log('⏰ Real-time updates configured');
    }

    cleanup() {
        // Clean up intervals when section is destroyed
        if (this.updateInterval) {
            clearInterval(this.updateInterval);
        }
    }
}

// Export for app initialization
if (typeof module !== 'undefined' && module.exports) {
    module.exports = DashboardSection;
} else {
    window.DashboardSection = DashboardSection;
}
