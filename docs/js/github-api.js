/**
 * GitHub API Data Store - Replaces localStorage with GitHub repository storage
 */

class GitHubDataStore {
    constructor(auth) {
        this.auth = auth;
        this.repo = 'ProbablyBadAtThis/TankerMade';
        this.dataPath = 'data';
        this.cache = new Map();
    }

    // Initialize data store
    async init() {
        console.log('📁 Initializing GitHub Data Store...');

        if (!this.auth.isAuthenticated()) {
            console.warn('GitHub Data Store requires authentication');
            return false;
        }

        try {
            // Ensure data directory exists
            await this.ensureDataDirectory();
            return true;
        } catch (error) {
            console.error('Failed to initialize GitHub Data Store:', error);
            return false;
        }
    }

    // Ensure the data directory exists in the repository
    async ensureDataDirectory() {
        try {
            await this.auth.apiRequest(`/repos/${this.repo}/contents/${this.dataPath}`);
        } catch (error) {
            if (error.status === 404) {
                // Create .gitkeep file to establish directory
                await this.auth.apiRequest(`/repos/${this.repo}/contents/${this.dataPath}/.gitkeep`, {
                    method: 'PUT',
                    body: JSON.stringify({
                        message: 'Initialize data directory for progress tracking',
                        content: btoa('# Data directory for TankerMade progress tracking\n'),
                        branch: 'master'
                    })
                });
            } else {
                throw error;
            }
        }
    }

    // Get phase progress data
    async getPhaseProgress(phaseId) {
        const cacheKey = `phase-${phaseId}-progress`;

        // Check cache first
        if (this.cache.has(cacheKey)) {
            return this.cache.get(cacheKey);
        }

        const path = `${this.dataPath}/phase-${phaseId}-progress.json`;

        try {
            const response = await this.auth.apiRequest(`/repos/${this.repo}/contents/${path}`);
            const content = JSON.parse(atob(response.content));

            // Cache the result
            this.cache.set(cacheKey, content);
            return content;
        } catch (error) {
            if (error.status === 404) {
                // File doesn't exist, return default
                const defaultData = { 
                    completed: 0, 
                    total: this.getPhaseTaskCount(phaseId),
                    tasks: {},
                    lastUpdated: new Date().toISOString()
                };
                return defaultData;
            }
            throw error;
        }
    }

    // Update phase progress data
    async updatePhaseProgress(phaseId, progressData) {
        const path = `${this.dataPath}/phase-${phaseId}-progress.json`;
        const content = btoa(JSON.stringify(progressData, null, 2));
        const cacheKey = `phase-${phaseId}-progress`;

        try {
            let sha = null;

            // Get current file SHA for updates
            try {
                const existing = await this.auth.apiRequest(`/repos/${this.repo}/contents/${path}`);
                sha = existing.sha;
            } catch (error) {
                if (error.status !== 404) throw error;
            }

            const payload = {
                message: `Update Phase ${phaseId} progress: ${progressData.completed}/${progressData.total} tasks`,
                content: content,
                branch: 'master'
            };

            if (sha) {
                payload.sha = sha;
            }

            await this.auth.apiRequest(`/repos/${this.repo}/contents/${path}`, {
                method: 'PUT',
                body: JSON.stringify(payload)
            });

            // Update cache
            this.cache.set(cacheKey, progressData);

            return progressData;

        } catch (error) {
            console.error(`Failed to update phase ${phaseId} progress:`, error);
            throw error;
        }
    }

    // Update individual task status
    async updateTaskStatus(phaseId, taskId, completed) {
        try {
            const progress = await this.getPhaseProgress(phaseId);
            const tasks = progress.tasks || {};

            tasks[taskId] = {
                completed,
                timestamp: new Date().toISOString(),
                user: this.auth.getCurrentUser()?.login
            };

            const completedCount = Object.values(tasks).filter(t => t.completed).length;

            const updatedProgress = {
                ...progress,
                tasks,
                completed: completedCount,
                lastUpdated: new Date().toISOString()
            };

            await this.updatePhaseProgress(phaseId, updatedProgress);

            // Update UI immediately
            this.updateProgressDisplays();

            // Show success feedback
            window.TankerMadeApp?.showNotification(
                `Task ${completed ? 'completed' : 'unchecked'}`, 
                'success'
            );

            return updatedProgress;

        } catch (error) {
            console.error('Failed to update task status:', error);
            window.TankerMadeApp?.showNotification('Failed to update task', 'error');
            throw error;
        }
    }

    // Get all phase data for overview calculations
    async getAllPhaseProgress() {
        const phases = [];

        for (let i = 1; i <= 10; i++) {
            try {
                const progress = await this.getPhaseProgress(i);
                phases.push({ id: i, ...progress });
            } catch (error) {
                console.warn(`Failed to load phase ${i} progress:`, error);
                phases.push({ 
                    id: i, 
                    completed: 0, 
                    total: this.getPhaseTaskCount(i),
                    tasks: {}
                });
            }
        }

        return phases;
    }

    // Update progress displays in UI
    updateProgressDisplays() {
        // Trigger app to recalculate and update displays
        if (window.TankerMadeApp) {
            window.TankerMadeApp.loadProgressData();
        }
    }

    // Get task count for each phase (from existing dev-tracker.js)
    getPhaseTaskCount(phaseId) {
        const taskCounts = {
            1: 23, 2: 31, 3: 18, 4: 15, 5: 25,
            6: 12, 7: 14, 8: 19, 9: 17, 10: 22
        };
        return taskCounts[phaseId] || 0;
    }

    // Clear cache (useful for testing/debugging)
    clearCache() {
        this.cache.clear();
    }

    // Export all data for backup
    async exportAllData() {
        const data = {
            version: '2.0',
            timestamp: new Date().toISOString(),
            user: this.auth.getCurrentUser(),
            phases: await this.getAllPhaseProgress()
        };

        return data;
    }

    // Setup task checkboxes with GitHub API integration
    setupTaskCheckboxes(phaseId) {
        const checkboxes = document.querySelectorAll(`[data-phase="${phaseId}"] input[type="checkbox"]`);

        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', async (e) => {
                const taskId = e.target.dataset.taskId;
                const completed = e.target.checked;

                // Show loading state
                e.target.disabled = true;

                try {
                    await this.updateTaskStatus(phaseId, taskId, completed);

                    // Show success feedback
                    this.showTaskUpdateFeedback(e.target, 'success');

                } catch (error) {
                    // Revert checkbox state
                    e.target.checked = !completed;
                    this.showTaskUpdateFeedback(e.target, 'error');
                    console.error('Failed to update task:', error);
                } finally {
                    e.target.disabled = false;
                }
            });
        });
    }

    // Show visual feedback for task updates
    showTaskUpdateFeedback(checkbox, type) {
        const feedback = document.createElement('span');
        feedback.className = `task-feedback ${type}`;
        feedback.innerHTML = type === 'success' ? '✓' : '✗';

        // Position relative to checkbox
        checkbox.parentNode.style.position = 'relative';
        checkbox.parentNode.appendChild(feedback);

        setTimeout(() => feedback.remove(), 2000);
    }
}

// Create global data store instance (will be initialized after auth)
window.TankerMadeData = null;