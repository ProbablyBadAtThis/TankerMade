/**
 * Dev Tracker Section Controller - Pure JavaScript Implementation
 * Recreates the original dev tracker layout with modern architecture
 */

class DevTrackerSection {
    constructor() {
        this.sectionId = 'dev-tracker';
        this.data = {
            totalTasks: 65,
            completedTasks: 37,
            totalPhases: 9,
            completedPhases: 3,
            currentPhase: {
                number: "D",
                title: "Inventory & Kits",
                description: "Yarn, tools, notions, lots, purchases, linking, and kits.",
                completed: 0,
                total: 7,
                color: "#f59e0b"
            },
            phases: [
                {
                    number: "A",
                    title: "Hardening, Module Host & Reference Module",
                    description: "Completed module-host foundation and bundled Crafting reference module.",
                    completed: 26,
                    total: 26,
                    color: "#2dd4bf",
                    status: "completed"
                },
                {
                    number: "B",
                    title: "Crafting Module V2",
                    description: "Pattern pieces, steps, reordering, aggregation, validation, and expanded module UI.",
                    completed: 5,
                    total: 5,
                    color: "#60a5fa",
                    status: "completed"
                },
                {
                    number: "C",
                    title: "Module Project Workspace",
                    description: "Step progress, timers, completion, pieces, archive, and editing.",
                    completed: 6,
                    total: 6,
                    color: "#a78bfa",
                    status: "completed"
                },
                {
                    number: "D",
                    title: "Inventory & Kits",
                    description: "Yarn, tools, notions, lot tracking, purchases, linking, and kits.",
                    completed: 0,
                    total: 7,
                    color: "#f59e0b",
                    status: "active"
                },
                {
                    number: "E",
                    title: "Reference Data Integration",
                    description: "Settings-backed reference data in forms and inline creation.",
                    completed: 0,
                    total: 3,
                    color: "#34d399",
                    status: "upcoming"
                },
                {
                    number: "F",
                    title: "Module Platform V1",
                    description: "Module contracts, discovery, UI extension points, and examples.",
                    completed: 0,
                    total: 5,
                    color: "#fb7185",
                    status: "upcoming"
                },
                {
                    number: "G",
                    title: "Images & Assets",
                    description: "Local file storage, thumbnails, and image pickers.",
                    completed: 0,
                    total: 3,
                    color: "#38bdf8",
                    status: "upcoming"
                },
                {
                    number: "H",
                    title: "Performance & Search",
                    description: "Indexes, filters, pagination, search, and caching.",
                    completed: 0,
                    total: 4,
                    color: "#818cf8",
                    status: "upcoming"
                },
                {
                    number: "I",
                    title: "Security, Ops & Cleanup",
                    description: "Secrets, HTTPS, keys, export/import, cleanup, and deployment guidance.",
                    completed: 0,
                    total: 6,
                    color: "#f472b6",
                    status: "upcoming"
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
        if (currentWeekEl) currentWeekEl.textContent = this.data.currentPhase.number;
        if (currentPhaseEl) currentPhaseEl.textContent = `Phase ${this.data.currentPhase.number}: ${this.data.currentPhase.title}`;
    }

    updateStatsGrid() {
        // Update all stat values
        const updates = {
            'completed-tasks': this.data.completedTasks,
            'total-tasks': this.data.totalTasks,
            'current-week-display': `Phase ${this.data.currentPhase.number}`,
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
        this.data.phases.forEach(phase => {
            const isCompleted = phase.completed === phase.total && phase.total > 0;
            html += `
                <div class="week-item ${isCompleted ? 'completed' : ''} ${phase.status === 'active' ? 'current' : ''}"
                     style="--phase-color: ${phase.color}">
                    <div class="week-number">${phase.number}</div>
                    <div class="week-label">Phase ${phase.number}</div>
                </div>
            `;
        });
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
                                <span class="stat-label">Status</span>
                                <span class="stat-value">${phase.status}</span>
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

    getPhaseForWeek(phaseNumber) {
        return this.data.phases.find(phase => phase.number === phaseNumber);
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

function getRoadmapPhase(phaseNumber) {
    const tracker = new DevTrackerSection();
    return tracker.data.phases.find(phase => String(phase.number) === String(phaseNumber)) || tracker.data.currentPhase;
}

const ROADMAP_PHASE_TASKS = {
    A: [
        { title: 'Stack decided: Blazor WASM + ASP.NET Core + SQLite + EF Core 10', status: 'done', area: 'Foundation' },
        { title: 'Solution structure: Core / Contracts / Application / Server / Client', status: 'done', area: 'Foundation' },
        { title: 'SQL Server to SQLite swap', status: 'done', area: 'Data' },
        { title: 'All projects upgraded to net10.0', status: 'done', area: 'Platform' },
        { title: 'Misplaced packages removed from Core, Application, Contracts', status: 'done', area: 'Cleanup' },
        { title: 'Swashbuckle replaced with Scalar', status: 'done', area: 'API Docs' },
        { title: 'IDesignTimeDbContextFactory added', status: 'done', area: 'EF Core' },
        { title: 'Auto-migration on startup', status: 'done', area: 'EF Core' },
        { title: 'InitialCreate migration: Users, Projects, Patterns, reference data seeded', status: 'done', area: 'Data' },
        { title: 'JWT auth + BCrypt password hashing wired', status: 'done', area: 'Auth' },
        { title: 'AuthController present with register/login', status: 'done', area: 'API' },
        { title: 'Server running, DB created, Scalar docs accessible', status: 'done', area: 'Verification' },
        { title: 'Modularity guardrail documented: base app is a module host, not a built-in craft app', status: 'done', area: 'Architecture' },
        { title: 'GitHub Actions CI workflow for restore/build/test', status: 'done', area: 'CI' },
        { title: 'JWT secret moved out of committed config', status: 'done', area: 'Security' },
        { title: 'Add Core module-host entities', status: 'done', area: 'Domain' },
        { title: 'Register module-host entities in DbContext and add migration', status: 'done', area: 'Data' },
        { title: 'Implement module discovery and activation services', status: 'done', area: 'Application' },
        { title: 'Add module host API endpoints', status: 'done', area: 'API' },
        { title: 'Scaffold first Crafting module as a separate module project', status: 'done', area: 'Modules' },
        { title: 'Extract project/pattern foundation into the Crafting module', status: 'done', area: 'Modules' },
        { title: 'Add module-owned project/pattern entities, services, and APIs', status: 'done', area: 'Modules' },
        { title: 'Add minimal module-provided navigation and UI surfaces', status: 'done', area: 'Client' },
        { title: 'Add module host and reference module service tests', status: 'done', area: 'Tests' },
        { title: 'Manual smoke test: auth, activation, module APIs, CRUD, ownership, and client nav', status: 'done', area: 'Verification' },
        { title: 'Patch module-owned partial update behavior', status: 'done', area: 'API' }
    ],
    B: [
        { title: 'Full CRUD and reorder for module-owned pattern pieces and steps', status: 'done', area: 'Patterns' },
        { title: 'Pattern detail page in module UI', status: 'done', area: 'Client' },
        { title: 'Step range display where relevant to the module', status: 'done', area: 'UX' },
        { title: 'Progress aggregation and validation where relevant to the module', status: 'done', area: 'Domain' },
        { title: 'Expand module-owned project workspace screens beyond the Phase A reference baseline', status: 'done', area: 'Projects' }
    ],
    C: [
        { title: 'Step checklist with ProjectStepProgress tracking', status: 'done', area: 'Projects' },
        { title: 'Per-step timers with play/pause, manual adjustment, and reset', status: 'done', area: 'Projects' },
        { title: 'Completion percentage from checked linked pattern steps', status: 'done', area: 'Progress' },
        { title: 'Piece selector on project detail', status: 'done', area: 'Client' },
        { title: 'Archive flag and archive flow', status: 'done', area: 'Projects' },
        { title: 'Non-destructive editing', status: 'done', area: 'Safety' }
    ],
    D: [
        { title: 'Craft module inventory: yarn, tools, notions, lots, purchase history, and sale price handling', status: 'todo', area: 'Inventory' },
        { title: '3D printing module inventory: materials, spools, printer/tooling needs, and purchase history', status: 'todo', area: 'Inventory' },
        { title: 'Module-owned filtering and reference data', status: 'todo', area: 'Filters' },
        { title: 'Purchase history per source and sale price handling', status: 'todo', area: 'Purchases' },
        { title: 'Module-owned project/inventory linking', status: 'todo', area: 'Relations' },
        { title: 'Module-owned kit/grouping behavior', status: 'todo', area: 'Kits' },
        { title: 'Module-owned kit/grouping to project flows', status: 'todo', area: 'Kits' }
    ],
    E: [
        { title: 'Wire Settings / ReferenceItem categories into forms', status: 'todo', area: 'Settings' },
        { title: 'Theme, Color, Source, Brand, FiberType selectable in UI', status: 'todo', area: 'Forms' },
        { title: 'Add/new option inline in dropdowns', status: 'todo', area: 'UX' }
    ],
    F: [
        { title: 'Define IModule contract and registration', status: 'todo', area: 'Modules' },
        { title: 'Module discovery from directory', status: 'todo', area: 'Modules' },
        { title: 'UI extension points via DynamicComponent', status: 'todo', area: 'Client' },
        { title: 'Crafting module extracted as first example', status: 'todo', area: 'Modules' },
        { title: '3D printing module scaffold', status: 'todo', area: 'Modules' }
    ],
    G: [
        { title: 'Local disk file storage', status: 'todo', area: 'Assets' },
        { title: 'Thumbnail generation', status: 'todo', area: 'Assets' },
        { title: 'Image pickers on Projects, Patterns, Inventory items', status: 'todo', area: 'Client' }
    ],
    H: [
        { title: 'DB indexes on commonly filtered columns', status: 'todo', area: 'Performance' },
        { title: 'Server-side filters and pagination on all list endpoints', status: 'todo', area: 'API' },
        { title: 'Full-text search with SQLite FTS if needed', status: 'todo', area: 'Search' },
        { title: 'Caching where beneficial', status: 'todo', area: 'Performance' }
    ],
    I: [
        { title: 'JWT secret properly managed', status: 'todo', area: 'Security' },
        { title: 'HTTPS enforced in production', status: 'todo', area: 'Ops' },
        { title: 'Data Protection key persistence', status: 'todo', area: 'Ops' },
        { title: 'Export/import round-trip tested and documented', status: 'todo', area: 'Admin' },
        { title: 'Legacy code removal', status: 'todo', area: 'Cleanup' },
        { title: 'Deployment guidance', status: 'todo', area: 'Docs' }
    ]
};

function renderRoadmapTaskList(tasks) {
    return tasks.map((task, index) => `
        <div class="roadmap-task ${task.status === 'done' ? 'is-done' : 'is-open'}">
            <div class="roadmap-task-check" aria-hidden="true">${task.status === 'done' ? '✓' : index + 1}</div>
            <div class="roadmap-task-body">
                <div class="roadmap-task-title">${task.title}</div>
                <div class="roadmap-task-meta">${task.area}</div>
            </div>
            <span class="badge badge-${task.status === 'done' ? 'success' : 'neutral'}">${task.status === 'done' ? 'Done' : 'Open'}</span>
        </div>
    `).join('');
}

function initDevTrackerPhase(params = {}) {
    const phaseNumber = params.phase || params.phaseId || 'A';
    const phase = getRoadmapPhase(phaseNumber);
    const tasks = ROADMAP_PHASE_TASKS[phase.number] || [];
    const progressPercent = Math.round((phase.completed / phase.total) * 100);
    const contentContainer = document.getElementById('content-container');

    if (!contentContainer) return;

    contentContainer.innerHTML = `
        <div class="dev-tracker-container">
            <div class="phase-detail-header mb-6">
                <div class="flex items-center justify-between gap-4">
                    <div>
                        <button class="btn btn-secondary mb-4" onclick="window.TankerMadeRouter.goToSection('dev-tracker')">
                            Back to Tracker
                        </button>
                        <h1>Phase ${phase.number}: ${phase.title}</h1>
                        <p class="text-secondary">${phase.description}</p>
                    </div>
                    <span class="badge badge-${phase.status === 'active' ? 'success' : 'neutral'}">${phase.status}</span>
                </div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-content">
                        <div class="stat-value">${phase.completed}</div>
                        <div class="stat-label">Completed</div>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-content">
                        <div class="stat-value">${phase.total}</div>
                        <div class="stat-label">Total Items</div>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-content">
                        <div class="stat-value">${progressPercent}%</div>
                        <div class="stat-label">Complete</div>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-content">
                        <div class="stat-value">${phase.status}</div>
                        <div class="stat-label">Status</div>
                    </div>
                </div>
            </div>

            <div class="phase-detail-grid mt-6">
                <section class="page-panel phase-task-panel">
                    <div class="phase-detail-panel-header">
                        <div>
                            <h2>Phase Tasks</h2>
                            <p>${tasks.length ? 'Mirrors docs/project/roadmap.md for this phase.' : 'No task list has been defined for this phase yet.'}</p>
                        </div>
                    </div>
                    <div class="roadmap-task-list">
                        ${tasks.length ? renderRoadmapTaskList(tasks) : '<p class="text-secondary">Tasks will appear here as this phase is planned.</p>'}
                    </div>
                </section>

                <aside class="page-panel phase-notes-panel">
                    <div class="phase-detail-panel-header">
                        <div>
                            <h2>Notes</h2>
                            <p>Current implementation state.</p>
                        </div>
                    </div>
                    <div class="phase-note-list">
                        <div class="phase-note">
                            <span class="phase-note-label">Source</span>
                            <span>docs/project/roadmap.md</span>
                        </div>
                        <div class="phase-note">
                            <span class="phase-note-label">Progress</span>
                            <span>${phase.completed} of ${phase.total}</span>
                        </div>
                        <div class="phase-note">
                            <span class="phase-note-label">Status</span>
                            <span>${phase.status}</span>
                        </div>
                    </div>
                    <button class="btn btn-primary w-full mt-4" onclick="window.TankerMadeRouter.goToSection('workbench')">Open Workbench</button>
                </aside>
            </div>
        </div>
    `;
}

// Export for app initialization
if (typeof module !== 'undefined' && module.exports) {
    module.exports = DevTrackerSection;
} else {
    window.DevTrackerSection = DevTrackerSection;
    window.initDevTrackerPhase = initDevTrackerPhase;
    window.initDevTracker = function initDevTracker() {
        const tracker = new DevTrackerSection();
        tracker.updateOverallProgress();
        tracker.updateStatsGrid();
        tracker.renderWeekTimeline();
        tracker.renderPhaseCards();
        tracker.setupEventHandlers();
        window.__currentDevTracker = tracker;
    };
}
