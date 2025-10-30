/**
 * Dashboard Section Controller
 * Handles the main dashboard view with progress overview and recent activity
 */

class DashboardSection {
    constructor() {
        this.sectionId = 'dashboard';
    }

    async render() {
        return `
            <div class="dashboard-container">
                <div class="dashboard-header">
                    <h1>Developer Dashboard</h1>
                    <p class="dashboard-subtitle">Your TankerMade development progress at a glance</p>
                </div>

                <!-- First Row - Overall Progress & Current Phase (unchanged) -->
                <div class="dashboard-grid-row">
                    <div class="card dashboard-card progress-card">
                        <div class="card-header">
                            <h2>Overall Progress</h2>
                        </div>
                        <div class="card-body">
                            <div class="progress-overview">
                                <div class="progress-stats">
                                    <span class="progress-stat">
                                        <span class="stat-value" id="total-completed">0</span>
                                        <span class="stat-label">/ <span id="total-tasks">196</span> tasks</span>
                                    </span>
                                    <span class="progress-stat">
                                        <span class="stat-value" id="current-week">Week 2</span>
                                        <span class="stat-label">of 21</span>
                                    </span>
                                </div>

                                <div class="progress-bar-container">
                                    <div class="progress-bar">
                                        <div class="progress-fill" id="overall-progress" style="width: 0%"></div>
                                    </div>
                                    <span class="progress-percentage" id="progress-percentage">0%</span>
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
                                <h3 class="phase-title" id="phase-title">Phase 1: Foundation</h3>
                                <p class="phase-description" id="phase-description">Basic infrastructure and authentication</p>

                                <div class="phase-progress">
                                    <div class="progress-bar">
                                        <div class="progress-fill" id="phase-progress" style="width: 0%"></div>
                                    </div>
                                    <span class="progress-text" id="phase-progress-text">0 / 23 tasks</span>
                                </div>

                                <button class="btn btn-primary phase-details-btn" onclick="window.TankerMadeRouter.navigate('dev-tracker')">
                                    View Details
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
                                        <div class="task-item">
                                            <div class="task-status"></div>
                                            <div class="task-info">
                                                <span class="task-title">Complete GitHub OAuth implementation</span>
                                                <span class="task-meta">Phase 1 • Authentication</span>
                                            </div>
                                        </div>
                                        <div class="task-item">
                                            <div class="task-status"></div>
                                            <div class="task-info">
                                                <span class="task-title">Set up progress tracking system</span>
                                                <span class="task-meta">Phase 1 • Data Management</span>
                                            </div>
                                        </div>
                                        <div class="task-item">
                                            <div class="task-status"></div>
                                            <div class="task-info">
                                                <span class="task-title">Implement responsive dashboard layout</span>
                                                <span class="task-meta">Phase 1 • UI/UX</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="focus-section">
                                    <h3 class="focus-section-title">📊 Phase Breakdown</h3>
                                    <div class="focus-content" id="phase-breakdown">
                                        <div class="phase-mini-card">
                                            <span class="phase-number">1</span>
                                            <div class="phase-mini-info">
                                                <span class="phase-mini-title">Foundation</span>
                                                <div class="phase-mini-progress">
                                                    <div class="progress-bar mini">
                                                        <div class="progress-fill" style="width: 12%"></div>
                                                    </div>
                                                    <span class="progress-text">3/23</span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="phase-mini-card upcoming">
                                            <span class="phase-number">2</span>
                                            <div class="phase-mini-info">
                                                <span class="phase-mini-title">Core Features</span>
                                                <div class="phase-mini-progress">
                                                    <div class="progress-bar mini">
                                                        <div class="progress-fill" style="width: 0%"></div>
                                                    </div>
                                                    <span class="progress-text">0/31</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Vertical Divider -->
                            <div class="focus-divider"></div>

                            <!-- Right Column -->
                            <div class="focus-column focus-right">
                                <div class="focus-section">
                                    <h3 class="focus-section-title">🔧 Dev Environment</h3>
                                    <div class="focus-content" id="dev-environment">
                                        <div class="env-status-item">
                                            <div class="status-indicator online"></div>
                                            <div class="status-info">
                                                <span class="status-title">GitHub Integration</span>
                                                <span class="status-meta">Connected as ProbablyBadAtThis</span>
                                            </div>
                                        </div>
                                        <div class="env-status-item">
                                            <div class="status-indicator online"></div>
                                            <div class="status-info">
                                                <span class="status-title">Cloudflare Pages</span>
                                                <span class="status-meta">Deployed • Functions Active</span>
                                            </div>
                                        </div>
                                        <div class="env-status-item">
                                            <div class="status-indicator online"></div>
                                            <div class="status-info">
                                                <span class="status-title">OAuth Service</span>
                                                <span class="status-meta">Operational</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="focus-section">
                                    <h3 class="focus-section-title">📈 This Week</h3>
                                    <div class="focus-content" id="week-summary">
                                        <div class="week-stat">
                                            <span class="week-stat-value">3</span>
                                            <span class="week-stat-label">Tasks Completed</span>
                                        </div>
                                        <div class="week-stat">
                                            <span class="week-stat-value">12</span>
                                            <span class="week-stat-label">Commits Made</span>
                                        </div>
                                        <div class="week-stat">
                                            <span class="week-stat-value">4.2</span>
                                            <span class="week-stat-label">Hours Focused</span>
                                        </div>
                                    </div>
                                </div>

                                <div class="focus-section">
                                    <h3 class="focus-section-title">🎯 Next Up</h3>
                                    <div class="focus-content" id="next-tasks">
                                        <div class="next-task">
                                            <span class="next-task-title">Dashboard UX improvements</span>
                                            <span class="next-task-phase">Phase 1</span>
                                        </div>
                                        <div class="next-task">
                                            <span class="next-task-title">Mobile responsive testing</span>
                                            <span class="next-task-phase">Phase 1</span>
                                        </div>
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
                                <div class="activity-item">
                                    <div class="activity-icon">✅</div>
                                    <div class="activity-content">
                                        <span class="activity-title">OAuth implementation complete</span>
                                        <span class="activity-time">2 hours ago</span>
                                    </div>
                                </div>

                                <div class="activity-item">
                                    <div class="activity-icon">🔧</div>
                                    <div class="activity-content">
                                        <span class="activity-title">Functions deployed to Cloudflare</span>
                                        <span class="activity-time">3 hours ago</span>
                                    </div>
                                </div>

                                <div class="activity-item">
                                    <div class="activity-icon">📝</div>
                                    <div class="activity-content">
                                        <span class="activity-title">Updated project structure</span>
                                        <span class="activity-time">4 hours ago</span>
                                    </div>
                                </div>

                                <div class="activity-item">
                                    <div class="activity-icon">🎨</div>
                                    <div class="activity-content">
                                        <span class="activity-title">Implemented auth-based navigation</span>
                                        <span class="activity-time">5 hours ago</span>
                                    </div>
                                </div>

                                <div class="activity-item">
                                    <div class="activity-icon">🔐</div>
                                    <div class="activity-content">
                                        <span class="activity-title">GitHub OAuth app configured</span>
                                        <span class="activity-time">6 hours ago</span>
                                    </div>
                                </div>

                                <div class="activity-item">
                                    <div class="activity-icon">📊</div>
                                    <div class="activity-content">
                                        <span class="activity-title">Dashboard layout redesigned</span>
                                        <span class="activity-time">1 day ago</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    async init() {
        console.log('🏠 Dashboard section initialized');

        // Load real progress data if authenticated
        if (window.TankerMadeAuth?.isAuthenticated()) {
            await this.loadProgressData();
        }

        // Set up real-time updates
        this.setupRealtimeUpdates();
    }

    async loadProgressData() {
        try {
            // This will be connected to GitHub API data
            console.log('📊 Loading dashboard progress data...');

            // Update UI with real data when available
            this.updateProgressDisplays();

        } catch (error) {
            console.warn('Failed to load dashboard data:', error);
        }
    }

    updateProgressDisplays() {
        // Update progress bars and statistics
        // This will be enhanced when GitHub API data is connected
    }

    setupRealtimeUpdates() {
        // Set up periodic updates for activity feed
        setInterval(() => {
            // Refresh activity data
        }, 30000); // Every 30 seconds
    }
}

// Export for app initialization
if (typeof module !== 'undefined' && module.exports) {
    module.exports = DashboardSection;
} else {
    window.DashboardSection = DashboardSection;
}