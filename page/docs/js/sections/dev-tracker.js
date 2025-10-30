/**
 * Dev Tracker Section - Phase management and task tracking
 */

function initDevTracker(params = {}) {
    console.log('📊 Initializing Dev Tracker', params);

    if (params.phase) {
        loadPhaseDetails(parseInt(params.phase));
    } else {
        loadDevTrackerOverview();
    }
}

function loadDevTrackerOverview() {
    const phases = [
        { id: 1, title: "Foundation", duration: "Weeks 1-3", taskCount: 23, color: "#2563eb" },
        { id: 2, title: "Core Features", duration: "Weeks 4-6", taskCount: 31, color: "#059669" },
        { id: 3, title: "Pattern Details", duration: "Weeks 7-8", taskCount: 18, color: "#7c3aed" },
        { id: 4, title: "Timers & Sessions", duration: "Weeks 9-10", taskCount: 15, color: "#dc2626" },
        { id: 5, title: "Gamification", duration: "Weeks 11-13", taskCount: 25, color: "#ea580c" },
        { id: 6, title: "Theme & Settings", duration: "Week 14", taskCount: 12, color: "#0891b2" },
        { id: 7, title: "Admin & Operations", duration: "Week 15", taskCount: 14, color: "#65a30d" },
        { id: 8, title: "Module System", duration: "Weeks 16-17", taskCount: 19, color: "#c026d3" },
        { id: 9, title: "Search & Performance", duration: "Weeks 18-19", taskCount: 17, color: "#e11d48" },
        { id: 10, title: "Polish & Testing", duration: "Weeks 20-21", taskCount: 22, color: "#0d9488" }
    ];

    const state = window.TankerMadeApp.getState();
    let totalCompleted = 0;
    let totalTasks = 0;

    // Calculate progress for each phase
    const phaseProgress = phases.map(phase => {
        const progressData = getPhaseProgress(phase.id);
        const completed = progressData.completed || 0;
        const total = progressData.total || phase.taskCount;
        const percentage = total > 0 ? Math.round((completed / total) * 100) : 0;

        totalCompleted += completed;
        totalTasks += total;

        return { ...phase, completed, total, percentage };
    });

    const overallPercentage = totalTasks > 0 ? Math.round((totalCompleted / totalTasks) * 100) : 0;

    const trackerContent = `
        <div class="dev-tracker-header mb-6">
            <h1>Development Progress Dashboard</h1>
            <p>Track your progress through the complete development roadmap</p>
        </div>

        <div class="grid grid-cols-4 mb-6">
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-primary">${totalCompleted}</div>
                    <div class="text-sm text-secondary">Tasks Completed</div>
                </div>
            </div>
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-primary">${totalTasks}</div>
                    <div class="text-sm text-secondary">Total Tasks</div>
                </div>
            </div>
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-primary">${phaseProgress.filter(p => p.percentage === 100).length}</div>
                    <div class="text-sm text-secondary">Phases Completed</div>
                </div>
            </div>
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-primary">${overallPercentage}%</div>
                    <div class="text-sm text-secondary">Overall Progress</div>
                </div>
            </div>
        </div>

        <div class="card mb-6">
            <div class="card-header">
                <h3 class="card-title">21-Week Timeline</h3>
            </div>
            <div class="card-body">
                <div class="timeline-weeks">
                    ${generateWeekTimeline(phaseProgress)}
                </div>
            </div>
        </div>

        <div class="phases-grid">
            ${phaseProgress.map(phase => generatePhaseCard(phase)).join('')}
        </div>

        <div class="mt-6 text-center">
            <button class="btn btn-primary mr-2" onclick="exportDevTrackerProgress()">
                📁 Export Progress
            </button>
            <button class="btn btn-secondary mr-2" onclick="importDevTrackerProgress()">
                📂 Import Progress
            </button>
            <button class="btn btn-error" onclick="resetAllProgress()">
                🔄 Reset All Progress
            </button>
        </div>
    `;

    const contentContainer = document.getElementById('content-container');
    if (contentContainer) {
        contentContainer.innerHTML = trackerContent;
    }
}

function generateWeekTimeline(phases) {
    let html = '<div class="flex flex-wrap gap-2">';

    for (let week = 1; week <= 21; week++) {
        let status = 'upcoming';
        let phase = null;

        // Find which phase this week belongs to
        for (const p of phases) {
            const weekRanges = {
                1: [1, 2, 3], 2: [4, 5, 6], 3: [7, 8], 4: [9, 10], 5: [11, 12, 13],
                6: [14], 7: [15], 8: [16, 17], 9: [18, 19], 10: [20, 21]
            };

            if (weekRanges[p.id] && weekRanges[p.id].includes(week)) {
                phase = p;
                if (p.percentage === 100) status = 'completed';
                else if (week <= 2) status = 'current'; // Assuming we're in week 2
                break;
            }
        }

        const statusClass = status === 'completed' ? 'bg-success text-white' : 
                           status === 'current' ? 'bg-warning text-white' : 
                           'bg-secondary text-secondary';

        html += `
            <div class="week-item ${statusClass}" title="Week ${week}${phase ? ' - ' + phase.title : ''}">
                <div class="text-xs font-semibold">W${week}</div>
            </div>
        `;
    }

    html += '</div>';
    return html;
}

function generatePhaseCard(phase) {
    const isCompleted = phase.percentage === 100;
    const cardClass = isCompleted ? 'border-success' : '';

    return `
        <div class="card phase-card ${cardClass}" onclick="loadPhaseDetails(${phase.id})">
            <div class="card-header">
                <div class="flex items-center gap-3">
                    <div class="phase-number" style="background-color: ${phase.color}">
                        ${phase.id}
                    </div>
                    <div>
                        <h3 class="card-title">${phase.title}</h3>
                        <p class="text-sm text-secondary">${phase.duration}</p>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="progress-bar mb-2">
                    <div class="progress-bar-fill" style="width: ${phase.percentage}%; background-color: ${phase.color}"></div>
                </div>
                <div class="progress-bar-text">
                    <span>${phase.completed} / ${phase.total} tasks</span>
                    <span>${phase.percentage}%</span>
                </div>
            </div>
        </div>
    `;
}

function loadPhaseDetails(phaseId) {
    // Navigate to specific phase details
    window.TankerMadeRouter.goToSection('dev-tracker-phase', { phaseId });
}

function getPhaseProgress(phaseId) {
    const key = `phase-${phaseId}-progress`;
    const saved = localStorage.getItem(key);
    return saved ? JSON.parse(saved) : { completed: 0, total: 0 };
}

function exportDevTrackerProgress() {
    window.TankerMadeApp.exportProgress();
}

function importDevTrackerProgress() {
    window.TankerMadeApp.importProgress();
}

function resetAllProgress() {
    if (confirm('Are you sure you want to reset ALL progress? This cannot be undone.')) {
        for (let i = 1; i <= 10; i++) {
            localStorage.removeItem(`phase-${i}-progress`);
            localStorage.removeItem(`phase-${i}-tasks`);
        }
        window.TankerMadeApp.showNotification('All progress has been reset.', 'info');
        loadDevTrackerOverview();
    }
}

// Export functions
window.initDevTracker = initDevTracker;
window.loadPhaseDetails = loadPhaseDetails;
window.exportDevTrackerProgress = exportDevTrackerProgress;
window.importDevTrackerProgress = importDevTrackerProgress;
window.resetAllProgress = resetAllProgress;
