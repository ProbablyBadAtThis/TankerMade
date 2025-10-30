/**
 * TankerMade Developer Dashboard - Main Application Logic
 * Handles global functionality, state management, and UI interactions
 */

class TankerMadeApp {
    constructor() {
    this.state = {
        currentPhase: 1,
        totalPhases: 10,
        currentWeek: 2,
        totalWeeks: 21,
        overallProgress: 12, // Updated to match your screenshot
        completedTasks: 0,
        totalTasks: 0,
        openIncidents: 0,
        lastUpdated: new Date().toLocaleDateString()
    };

    this.eventListeners = new Map();
    this.searchIndex = new Map();
    this.isAuthenticated = false;

    // Initialize after DOM is loaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => this.init());
    } else {
        this.init();
    }
}
    async init() {
    console.log('🧶 TankerMade Developer Dashboard starting...');

    // Initialize authentication first
    const auth = await window.TankerMadeAuth.init();
    this.isAuthenticated = !!auth;

    // Initialize GitHub data store if authenticated
    if (this.isAuthenticated) {
        window.TankerMadeData = new GitHubDataStore(window.TankerMadeAuth);
        await window.TankerMadeData.init();
    }

    // Initialize core functionality
    this.setupEventListeners();
    this.updateHeaderStatus();
    this.setupMobileNavigation();
    this.initializeSearch();
    this.setupKeyboardShortcuts();

    // Load initial data
    await this.loadProgressData();
    this.loadIncidentData();

    console.log('✅ TankerMade Application initialized');
}

    setupEventListeners() {
        // Global search functionality
        const searchBtn = document.getElementById('search-btn');
        const searchModal = document.getElementById('search-modal');
        const searchInput = document.getElementById('search-input');

        if (searchBtn && searchModal) {
            searchBtn.addEventListener('click', () => this.openSearch());
        }

        if (searchModal) {
            searchModal.addEventListener('click', (e) => {
                if (e.target === searchModal) {
                    this.closeSearch();
                }
            });

            const closeBtn = searchModal.querySelector('.modal-close');
            if (closeBtn) {
                closeBtn.addEventListener('click', () => this.closeSearch());
            }
        }

        if (searchInput) {
            searchInput.addEventListener('input', (e) => this.handleSearch(e.target.value));
            searchInput.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    this.closeSearch();
                }
            });
        }

        // Export/Import functionality
        const exportBtn = document.getElementById('export-btn');
        const exportProgressBtn = document.getElementById('export-progress');
        const importProgressBtn = document.getElementById('import-progress');

        if (exportBtn) {
            exportBtn.addEventListener('click', () => this.exportProgress());
        }
        if (exportProgressBtn) {
            exportProgressBtn.addEventListener('click', () => this.exportProgress());
        }
        if (importProgressBtn) {
            importProgressBtn.addEventListener('click', () => this.importProgress());
        }

        // Incidents functionality
        const incidentsBtn = document.getElementById('incidents-btn');
        if (incidentsBtn) {
            incidentsBtn.addEventListener('click', () => {
                window.TankerMadeRouter.goToSection('incidents');
            });
        }

        // Settings functionality
        const settingsBtn = document.getElementById('settings-btn');
        const appSettingsBtn = document.getElementById('app-settings');

        if (settingsBtn) {
            settingsBtn.addEventListener('click', () => this.openSettings());
        }
        if (appSettingsBtn) {
            appSettingsBtn.addEventListener('click', () => this.openSettings());
        }

        // Global search in navigation
        const globalSearchBtn = document.getElementById('global-search');
        if (globalSearchBtn) {
            globalSearchBtn.addEventListener('click', () => this.openSearch());
        }
    }

    setupMobileNavigation() {
        const mobileMenuToggle = document.getElementById('mobile-menu-toggle');
        const sidebar = document.getElementById('sidebar');

        if (mobileMenuToggle && sidebar) {
            mobileMenuToggle.addEventListener('click', () => {
                this.toggleMobileSidebar();
            });

            // Create overlay for mobile sidebar
            const overlay = document.createElement('div');
            overlay.className = 'sidebar-overlay';
            overlay.id = 'sidebar-overlay';
            overlay.addEventListener('click', () => this.closeMobileSidebar());
            document.body.appendChild(overlay);
        }
    }

    toggleMobileSidebar() {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.getElementById('sidebar-overlay');

        if (sidebar && overlay) {
            const isOpen = sidebar.classList.contains('open');

            if (isOpen) {
                this.closeMobileSidebar();
            } else {
                this.openMobileSidebar();
            }
        }
    }

    openMobileSidebar() {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.getElementById('sidebar-overlay');

        if (sidebar && overlay) {
            sidebar.classList.add('open');
            overlay.classList.add('active');
            document.body.style.overflow = 'hidden';
        }
    }

    closeMobileSidebar() {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.getElementById('sidebar-overlay');

        if (sidebar && overlay) {
            sidebar.classList.remove('open');
            overlay.classList.remove('active');
            document.body.style.overflow = '';
        }
    }

    setupKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Global shortcuts
            if (e.ctrlKey || e.metaKey) {
                switch (e.key) {
                    case 'k':
                        e.preventDefault();
                        this.openSearch();
                        break;
                    case 'e':
                        e.preventDefault();
                        this.exportProgress();
                        break;
                }
            }

            // Alt + number shortcuts for navigation
            if (e.altKey && !e.ctrlKey && !e.metaKey) {
                const shortcuts = {
                    '1': 'dashboard',
                    '2': 'dev-tracker',
                    '3': 'workbench',
                    '4': 'architecture',
                    '5': 'incidents'
                };

                if (shortcuts[e.key]) {
                    e.preventDefault();
                    window.TankerMadeRouter.goToSection(shortcuts[e.key]);
                }
            }

            // Escape key shortcuts
            if (e.key === 'Escape') {
                this.closeSearch();
                this.closeMobileSidebar();
            }
        });
    }

    loadApplicationState() {
        // Load saved application state from localStorage
        const savedState = localStorage.getItem('tankermade-app-state');
        if (savedState) {
            try {
                const parsed = JSON.parse(savedState);
                this.state = { ...this.state, ...parsed };
            } catch (error) {
                console.warn('Failed to load application state:', error);
            }
        }
    }

    saveApplicationState() {
        // Save current application state to localStorage
        try {
            localStorage.setItem('tankermade-app-state', JSON.stringify(this.state));
        } catch (error) {
            console.warn('Failed to save application state:', error);
        }
    }

    updateHeaderStatus() {
        // Update header status indicators
        const currentPhaseEl = document.getElementById('current-phase');
        const headerProgressEl = document.getElementById('header-progress');
        const progressTextEl = document.getElementById('progress-text');
        const currentWeekEl = document.getElementById('current-week');
        const lastUpdatedEl = document.getElementById('last-updated');
        const nextTaskEl = document.getElementById('next-task');

        if (currentPhaseEl) {
            currentPhaseEl.textContent = `Phase ${this.state.currentPhase}: Foundation`;
        }

        if (headerProgressEl) {
            headerProgressEl.style.width = `${this.state.overallProgress}%`;
        }

        if (progressTextEl) {
            progressTextEl.textContent = `${this.state.overallProgress}% Complete`;
        }

        if (currentWeekEl) {
            currentWeekEl.textContent = this.state.currentWeek;
        }

        if (lastUpdatedEl) {
            lastUpdatedEl.textContent = this.state.lastUpdated;
        }

        if (nextTaskEl) {
            nextTaskEl.textContent = this.getNextTask();
        }
    }

    async loadProgressData() {
    try {
        if (this.isAuthenticated && window.TankerMadeData) {
            // Use GitHub API data
            const phases = await window.TankerMadeData.getAllPhaseProgress();

            let totalTasks = 0;
            let completedTasks = 0;

            phases.forEach(phase => {
                completedTasks += phase.completed || 0;
                totalTasks += phase.total || 0;
            });

            this.state.totalTasks = totalTasks;
            this.state.completedTasks = completedTasks;
            this.state.overallProgress = totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;

        } else {
            // Fallback to localStorage for non-authenticated users
            const phases = [
                { id: 1, taskCount: 23 }, { id: 2, taskCount: 31 }, { id: 3, taskCount: 18 },
                { id: 4, taskCount: 15 }, { id: 5, taskCount: 25 }, { id: 6, taskCount: 12 },
                { id: 7, taskCount: 14 }, { id: 8, taskCount: 19 }, { id: 9, taskCount: 17 },
                { id: 10, taskCount: 22 }
            ];

            let totalTasks = 0;
            let completedTasks = 0;

            phases.forEach(phase => {
                const phaseKey = `phase-${phase.id}-progress`;
                const phaseData = localStorage.getItem(phaseKey);

                if (phaseData) {
                    const parsed = JSON.parse(phaseData);
                    completedTasks += parsed.completed || 0;
                }

                totalTasks += phase.taskCount;
            });

            this.state.totalTasks = totalTasks;
            this.state.completedTasks = completedTasks;
            this.state.overallProgress = totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;
        }

        this.updateHeaderStatus();
        this.saveApplicationState();

    } catch (error) {
        console.warn('Failed to load progress data:', error);
    }
}

    loadIncidentData() {
        // Load incident count (placeholder - will be replaced with actual GitHub API call)
        const savedIncidents = localStorage.getItem('tankermade-incidents');
        if (savedIncidents) {
            try {
                const incidents = JSON.parse(savedIncidents);
                this.state.openIncidents = incidents.filter(i => i.status === 'open').length;
            } catch (error) {
                console.warn('Failed to load incident data:', error);
            }
        }

        this.updateIncidentBadges();
    }

    updateIncidentBadges() {
        // Update incident count badges
        const badges = [
            document.getElementById('incident-count'),
            document.getElementById('sidebar-incident-count'),
            document.getElementById('bottom-incident-count')
        ];

        badges.forEach(badge => {
            if (badge) {
                badge.textContent = this.state.openIncidents;
                badge.style.display = this.state.openIncidents > 0 ? 'flex' : 'none';
            }
        });
    }

    getNextTask() {
        // Get next task based on current progress
        const nextTasks = [
            'Complete auth service',
            'Implement project CRUD',
            'Add validation rules',
            'Create unit tests',
            'Set up CI/CD pipeline'
        ];

        return nextTasks[Math.floor(Math.random() * nextTasks.length)];
    }

    // Search functionality
    initializeSearch() {
        // Build search index (placeholder - will be enhanced with actual content)
        this.searchIndex.set('dashboard', {
            title: 'Dashboard',
            content: 'Project status overview, quick actions, recent activity',
            section: 'Dashboard',
            url: '#dashboard'
        });

        this.searchIndex.set('dev-tracker', {
            title: 'Development Tracker',
            content: 'Phase progress, task management, timeline tracking',
            section: 'Dev Tracker',
            url: '#dev-tracker'
        });

        this.searchIndex.set('workbench', {
            title: 'Documentation Workbench',
            content: 'Architecture docs, domain model, development guidelines',
            section: 'Workbench',
            url: '#workbench'
        });

        this.searchIndex.set('architecture', {
            title: 'Architecture Visualizer',
            content: 'Entity diagrams, data flow, system architecture',
            section: 'Architecture',
            url: '#architecture'
        });

        this.searchIndex.set('incidents', {
            title: 'AIE Tracker',
            content: 'Incident monitoring, GitHub issues, problem tracking',
            section: 'AIE Tracker',
            url: '#incidents'
        });
    }

    openSearch() {
        const searchModal = document.getElementById('search-modal');
        const searchInput = document.getElementById('search-input');

        if (searchModal) {
            searchModal.classList.add('active');
            searchModal.setAttribute('aria-hidden', 'false');

            if (searchInput) {
                searchInput.focus();
            }
        }
    }

    closeSearch() {
        const searchModal = document.getElementById('search-modal');

        if (searchModal) {
            searchModal.classList.remove('active');
            searchModal.setAttribute('aria-hidden', 'true');
        }
    }

    handleSearch(query) {
        const searchResults = document.getElementById('search-results');
        if (!searchResults) return;

        if (!query.trim()) {
            searchResults.innerHTML = '';
            return;
        }

        const results = this.performSearch(query);
        this.displaySearchResults(results);
    }

    performSearch(query) {
        const results = [];
        const queryLower = query.toLowerCase();

        this.searchIndex.forEach((item, key) => {
            const titleMatch = item.title.toLowerCase().includes(queryLower);
            const contentMatch = item.content.toLowerCase().includes(queryLower);

            if (titleMatch || contentMatch) {
                results.push({
                    ...item,
                    key,
                    relevance: titleMatch ? 2 : 1
                });
            }
        });

        return results.sort((a, b) => b.relevance - a.relevance);
    }

    displaySearchResults(results) {
        const searchResults = document.getElementById('search-results');
        if (!searchResults) return;

        if (results.length === 0) {
            searchResults.innerHTML = '<div class="search-result"><div class="search-result-title">No results found</div></div>';
            return;
        }

        const html = results.map(result => `
            <div class="search-result" onclick="app.navigateFromSearch('${result.key}')">
                <div class="search-result-title">${result.title}</div>
                <div class="search-result-excerpt">${result.content}</div>
                <div class="search-result-section">${result.section}</div>
            </div>
        `).join('');

        searchResults.innerHTML = html;
    }

    navigateFromSearch(sectionKey) {
        this.closeSearch();
        window.TankerMadeRouter.goToSection(sectionKey);
    }

    // Export/Import functionality
    exportProgress() {
        try {
            const data = {
                version: '1.0',
                timestamp: new Date().toISOString(),
                state: this.state,
                phases: this.getPhaseData(),
                incidents: this.getIncidentData()
            };

            const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `tankermade-progress-${new Date().toISOString().split('T')[0]}.json`;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);

            this.showNotification('Progress exported successfully!', 'success');

        } catch (error) {
            console.error('Export failed:', error);
            this.showNotification('Export failed. Please try again.', 'error');
        }
    }

    importProgress() {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = '.json';
        input.onchange = (e) => {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    try {
                        const data = JSON.parse(e.target.result);
                        this.processImportData(data);
                        this.showNotification('Progress imported successfully!', 'success');
                        window.location.reload(); // Reload to apply changes
                    } catch (error) {
                        console.error('Import failed:', error);
                        this.showNotification('Import failed. Please check the file format.', 'error');
                    }
                };
                reader.readAsText(file);
            }
        };
        input.click();
    }

    processImportData(data) {
        // Restore application state
        if (data.state) {
            this.state = { ...this.state, ...data.state };
            this.saveApplicationState();
        }

        // Restore phase data
        if (data.phases) {
            Object.entries(data.phases).forEach(([key, value]) => {
                localStorage.setItem(key, JSON.stringify(value));
            });
        }

        // Restore incident data
        if (data.incidents) {
            localStorage.setItem('tankermade-incidents', JSON.stringify(data.incidents));
        }
    }

    getPhaseData() {
        const phaseData = {};
        for (let i = 1; i <= 10; i++) {
            const key = `phase-${i}-progress`;
            const data = localStorage.getItem(key);
            if (data) {
                phaseData[key] = JSON.parse(data);
            }
        }
        return phaseData;
    }

    getIncidentData() {
        const incidentData = localStorage.getItem('tankermade-incidents');
        return incidentData ? JSON.parse(incidentData) : [];
    }

    // Settings functionality
    openSettings() {
        // Placeholder for settings modal
        this.showNotification('Settings panel coming soon!', 'info');
    }

    // Utility methods
    showNotification(message, type = 'info') {
        // Create and show notification toast
        const notification = document.createElement('div');
        notification.className = `alert alert-${type}`;
        notification.style.position = 'fixed';
        notification.style.top = '80px';
        notification.style.right = '20px';
        notification.style.zIndex = '1000';
        notification.style.minWidth = '300px';
        notification.textContent = message;

        document.body.appendChild(notification);

        setTimeout(() => {
            notification.remove();
        }, 5000);
    }

    // Public API methods
    updateProgress(phaseId, completed, total) {
        // Update progress for a specific phase
        const key = `phase-${phaseId}-progress`;
        const data = { completed, total, lastUpdated: new Date().toISOString() };
        localStorage.setItem(key, JSON.stringify(data));

        this.loadProgressData();
    }

    addIncident(incident) {
        // Add new incident
        const incidents = this.getIncidentData();
        incidents.push({ ...incident, id: Date.now(), createdAt: new Date().toISOString() });
        localStorage.setItem('tankermade-incidents', JSON.stringify(incidents));

        this.loadIncidentData();
    }

    getState() {
        return { ...this.state };
    }
}

// Initialize application
const app = new TankerMadeApp();

// Export for global access
window.TankerMadeApp = app;
