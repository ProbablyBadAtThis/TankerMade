/**
 * Dashboard Section Controller - Pure JavaScript Implementation
 * Handles complete dashboard rendering and real-time updates
 */

class DashboardSection {
    constructor() {
        this.sectionId = 'dashboard';
        this.data = {
            totalTasks: 196,
            completedTasks: 12,
            currentWeek: 2,
            totalWeeks: 21,
            currentPhase: {
                number: 1,
                title: "Foundation",
                description: "Basic infrastructure and authentication",
                completed: 3,
                total: 23
            }
        };
    }

    async render() {
        const progressPercentage = Math.round((this.data.completedTasks / this.data.totalTasks) * 100);
        const phasePercentage = Math.round((this.data.currentPhase.completed / this.data.currentPhase.total) * 100);

        return `
            <div class="dashboard-container">
                <div class="dashboard-header">
                    <h1>Developer Dashboard</h1>
                    <p class="dashboard-subtitle">Your TankerMade development progress at a glance</p>
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
                                        <span class="stat-value" id="current-week">Week ${this.data.currentWeek}</span>
                                        <span class="stat-label">of ${this.data.totalWeeks}</span>
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

                                <button class="btn btn-primary phase-details-btn" onclick="window.TankerMadeRouter.navigate('dev-tracker')">
                                    View Details
                                </button>
                            </div>
                        </div>
                    </div>

                    <div class="card dashboard-card incidents-card">
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
                    </div>
                </div>

                <!-- Second Row - Current Focus (80%) & Recent Activity (20%) -->
                <div class="dashboard-grid-row dashboard-focus-row">
                    <!-- Current Focus Card - 80% width with two columns -->
                    <div class="card dashboard-card current-focus-card">
                        <div class="card-header">
                            <h2>Current Focus</h2>
                        </div>
                        <div class="card-body current-focus-body">
                            <!-- Left Column -->
                            <div class="focus-column focus-left">
                                <div class="focus-section">
                                    <h3 class="focus-section-title">🎯 Active Tasks</h3>
                                    <div class="focus-content" id="active-tasks">
                                        ${this.renderActiveTasks()}
                                    </div>
                                </div>

                                <div class="focus-section">
                                    <h3 class="focus-section-title">📊 Phase Breakdown</h3>
                                    <div class="focus-content" id="phase-breakdown">
                                        ${this.renderPhaseBreakdown()}
                                    </div>
                                </div>
                            </div>

                            <!-- Right Column - NO DIVIDER -->
                            <div class="focus-column focus-right">
                                <div class="focus-section">
                                    <h3 class="focus-section-title">🔧 Dev Environment</h3>
                                    <div class="focus-content" id="dev-environment">
                                        ${this.renderDevEnvironment()}
                                    </div>
                                </div>

                                <div class="focus-section">
                                    <h3 class="focus-section-title">📈 This Week</h3>
                                    <div class="focus-content" id="week-summary">
                                        ${this.renderWeekSummary()}
                                    </div>
                                </div>

                                <div class="focus-section">
                                    <h3 class="focus-section-title">🎯 Next Up</h3>
                                    <div class="focus-content" id="next-tasks">
                                        ${this.renderNextTasks()}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Recent Activity Card - 20% width -->
                    <div class="card dashboard-card recent-activity-card">
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
            </div>
        `;
    }

    renderActiveTasks() {
        const tasks = [
            { title: "Complete GitHub OAuth implementation", meta: "Phase 1 • Authentication", status: "active" },
            { title: "Set up progress tracking system", meta: "Phase 1 • Data Management", status: "active" },
            { title: "Implement responsive dashboard layout", meta: "Phase 1 • UI/UX", status: "active" }
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
            { number: 1, title: "Foundation", completed: 3, total: 23, active: true },
            { number: 2, title: "Core Features", completed: 0, total: 31, active: false }
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
            { title: "GitHub Integration", meta: "Connected as ProbablyBadAtThis", status: "online" },
            { title: "Cloudflare Pages", meta: "Deployed • Functions Active", status: "online" },
            { title: "OAuth Service", meta: "Operational", status: "online" }
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
            { value: "3", label: "Tasks Completed" },
            { value: "12", label: "Commits Made" },
            { value: "4.2", label: "Hours Focused" }
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
            { title: "Dashboard UX improvements", phase: "Phase 1" },
            { title: "Mobile responsive testing", phase: "Phase 1" }
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
            { icon: "✅", title: "OAuth implementation complete", time: "2 hours ago" },
            { icon: "🔧", title: "Functions deployed to Cloudflare", time: "3 hours ago" },
            { icon: "📝", title: "Updated project structure", time: "4 hours ago" },
            { icon: "🎨", title: "Implemented auth-based navigation", time: "5 hours ago" },
            { icon: "🔐", title: "GitHub OAuth app configured", time: "6 hours ago" },
            { icon: "📊", title: "Dashboard layout redesigned", time: "1 day ago" }
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