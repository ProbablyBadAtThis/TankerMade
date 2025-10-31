/**
 * Dev Tracker Section Controller - Pure JavaScript Implementation
 * Recreates the original dev tracker layout with modern architecture
 */

class DevTrackerSection {
    constructor() {
        this.sectionId = 'dev-tracker';
        this.data = {
            totalTasks: 196,
            completedTasks: 12,
            currentWeek: 2,
            totalWeeks: 21,
            totalPhases: 7,
            completedPhases: 0,
            currentPhase: {
                number: 1,
                title: "Foundation",
                description: "Basic infrastructure and authentication",
                completed: 3,
                total: 23,
                color: "#2563eb"
            },
            phases: [
                {
                    number: 1,
                    title: "Foundation",
                    description: "Basic infrastructure and authentication",
                    completed: 3,
                    total: 23,
                    color: "#2563eb",
                    status: "active",
                    weeks: [1, 2, 3]
                },
                {
                    number: 2,
                    title: "Core Features",
                    description: "Essential functionality and user management",
                    completed: 0,
                    total: 31,
                    color: "#7c3aed",
                    status: "upcoming",
                    weeks: [4, 5, 6, 7]
                },
                {
                    number: 3,
                    title: "Advanced Features",
                    description: "Enhanced functionality and integrations",
                    completed: 0,
                    total: 28,
                    color: "#059669",
                    status: "upcoming",
                    weeks: [8, 9, 10, 11]
                },
                {
                    number: 4,
                    title: "UI/UX Enhancement",
                    description: "Design improvements and user experience",
                    completed: 0,
                    total: 25,
                    color: "#dc2626",
                    status: "upcoming",
                    weeks: [12, 13, 14, 15]
                },
                {
                    number: 5,
                    title: "Testing & QA",
                    description: "Comprehensive testing and quality assurance",
                    completed: 0,
                    total: 22,
                    color: "#ea580c",
                    status: "upcoming",
                    weeks: [16, 17, 18]
                },
                {
                    number: 6,
                    title: "Performance Optimization",
                    description: "Speed and efficiency improvements",
                    completed: 0,
                    total: 18,
                    color: "#0891b2",
                    status: "upcoming",
                    weeks: [19, 20]
                },
                {
                    number: 7,
                    title: "Launch Preparation",
                    description: "Final preparations and deployment",
                    completed: 0,
                    total: 15,
                    color: "#be185d",
                    status: "upcoming",
                    weeks: [21]
                }
            ]
        };
    }

    async render() {
        // Return the HTML template (loaded from dev-tracker.html)
        return document.getElementById('dev-tracker-root').innerHTML;
    }

    async init() {
        console.log('📊 Dev Tracker section initializing...');

        // Get the root container
        const container = document.getElementById('dev-tracker-root');
        if (!container) {
            console.error('❌ Dev Tracker root container not found');
            return;
        }

        try {
            // Load the HTML template
            const response = await fetch('/sections/dev-tracker.html');
            const html = await response.text();
            container.innerHTML = html;

            console.log('✅ Dev Tracker HTML loaded');

            // Initialize all components
            this.updateOverallProgress();
            this.updateStatsGrid();
            this.renderWeekTimeline();
            this.renderPhaseCards();
            this.setupEventHandlers();

            console.log('✅ Dev Tracker initialization complete');

        } catch (error) {
            console.error('❌ Dev Tracker initialization failed:', error);
            container.innerHTML = `
                <div class="dev-tracker-error">
                    <h2>Dev Tracker Loading Error</h2>
                    <p>Failed to load dev tracker. Please refresh the page.</p>
                    <button onclick="location.reload()" class="btn btn-primary">Refresh</button>
                </div>
            `;
        }
    }

    updateOverallProgress() {
        const progressPercentage = Math.round((this.data.completedTasks / this.data.totalTasks) * 100);

        // Update progress circle
        const circle = document.getElementById('overall-progress-circle');
        const percentageEl = document.getElementById('overall-percentage');
        const currentWeekEl = document.getElementById('current-week');
        const currentPhaseEl = document.getElementById('current-phase');

        if (circle) {
            const progressFill = circle.querySelector('.progress-fill');
            progressFill.style.background = `conic-gradient(
                var(--color-primary) 0deg ${progressPercentage * 3.6}deg,
                var(--color-border) ${progressPercentage * 3.6}deg 360deg
            )`;
        }

        if (percentageEl) percentageEl.textContent = `${progressPercentage}%`;
        if (currentWeekEl) currentWeekEl.textContent = this.data.currentWeek;
        if (currentPhaseEl) currentPhaseEl.textContent = `Phase ${this.data.currentPhase.number}: ${this.data.currentPhase.title}`;
    }

    updateStatsGrid() {
        // Update all stat values
        const updates = {
            'completed-tasks': this.data.completedTasks,
            'total-tasks': this.data.totalTasks,
            'current-week-display': `Week ${this.data.currentWeek}`,
            'completed-phases': this.data.completedPhases,
            'total-phases': this.data.totalPhases,
            'current-phase-progress': `${Math.round((this.data.currentPhase.completed / this.data.currentPhase.total) * 100)}%`
        };

        Object.entries(updates).forEach(([id, value]) => {
            const element = document.getElementById(id);
            if (element) element.textContent = value;
        });
    }

    renderWeekTimeline() {
        const weekTracker = document.getElementById('week-tracker');
        if (!weekTracker) return;

        let html = '';
        for (let week = 1; week <= this.data.totalWeeks; week++) {
            const isCompleted = week < this.data.currentWeek;
            const isCurrent = week === this.data.currentWeek;
            const phase = this.getPhaseForWeek(week);

            html += `
                <div class="week-item ${isCompleted ? 'completed' : ''} ${isCurrent ? 'current' : ''}"
                     style="--phase-color: ${phase?.color || 'var(--color-border)'}">
                    <div class="week-number">${week}</div>
                    <div class="week-label">Week ${week}</div>
                </div>
            `;
        }
        weekTracker.innerHTML = html;
    }

    renderPhaseCards() {
        const phasesGrid = document.getElementById('phases-grid');
        if (!phasesGrid) return;

        let html = '';
        this.data.phases.forEach(phase => {
            const progressPercent = Math.round((phase.completed / phase.total) * 100);

            html += `
                <div class="phase-card ${phase.status}" 
                     style="--phase-color: ${phase.color}"
                     data-section="dev-tracker-phase" 
                     data-param-phase="${phase.number}">
                    <div class="phase-header">
                        <div class="phase-number">${phase.number}</div>
                        <div class="phase-title">${phase.title}</div>
                        <div class="phase-status-badge ${phase.status}">${phase.status}</div>
                    </div>
                    <div class="phase-body">
                        <p class="phase-description">${phase.description}</p>
                        <div class="phase-stats">
                            <div class="stat">
                                <span class="stat-label">Tasks</span>
                                <span class="stat-value">${phase.completed}/${phase.total}</span>
                            </div>
                            <div class="stat">
                                <span class="stat-label">Weeks</span>
                                <span class="stat-value">${phase.weeks.join(', ')}</span>
                            </div>
                        </div>
                        <div class="phase-progress">
                            <div class="progress-bar">
                                <div class="progress-fill" style="width: ${progressPercent}%; background: ${phase.color}"></div>
                            </div>
                            <span class="progress-text">${progressPercent}%</span>
                        </div>
                    </div>
                    <div class="phase-footer">
                        <button class="btn btn-outline phase-btn">
                            ${phase.status === 'active' ? 'Continue' : 'View Details'}
                        </button>
                    </div>
                </div>
            `;
        });
        phasesGrid.innerHTML = html;
    }

    getPhaseForWeek(week) {
        return this.data.phases.find(phase => phase.weeks.includes(week));
    }

    setupEventHandlers() {
        // Phase cards are already set up with data-section attributes
        // Router will handle the navigation automatically
        console.log('📊 Dev Tracker event handlers set up');
    }

    // Public methods for data updates
    updateProgress(completed, total) {
        this.data.completedTasks = completed;
        this.data.totalTasks = total;
        this.updateOverallProgress();
        this.updateStatsGrid();
    }

    setCurrentWeek(week) {
        this.data.currentWeek = week;
        this.updateOverallProgress();
        this.updateStatsGrid();
        this.renderWeekTimeline();
    }

    updatePhaseProgress(phaseNumber, completed, total) {
        const phase = this.data.phases.find(p => p.number === phaseNumber);
        if (phase) {
            phase.completed = completed;
            phase.total = total;

            if (phaseNumber === this.data.currentPhase.number) {
                this.data.currentPhase.completed = completed;
                this.data.currentPhase.total = total;
            }

            this.updateStatsGrid();
            this.renderPhaseCards();
        }
    }

    cleanup() {
        // Clean up any intervals or event listeners
        console.log('📊 Dev Tracker cleanup complete');
    }
}

// Export for app initialization
if (typeof module !== 'undefined' && module.exports) {
    module.exports = DevTrackerSection;
} else {
    window.DevTrackerSection = DevTrackerSection;
}