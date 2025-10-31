/**
 * Progress Management System
 * Handles persistent task progress tracking via GitHub integration
 */

class ProgressManager {
    constructor() {
        this.gistId = null; // Will be set after first creation
        this.progressData = {
            phases: {},
            lastUpdated: null,
            version: '1.0'
        };
        this.isDirty = false;
        this.saveTimeout = null;
    }

    async init() {
        console.log('📊 Initializing Progress Manager...');

        // Try to load existing progress
        await this.loadProgress();

        // Set up auto-save
        this.setupAutoSave();

        console.log('✅ Progress Manager initialized');
    }

    async loadProgress() {
        try {
            // Try to load from GitHub Gist first
            const gistData = await this.loadFromGist();
            if (gistData) {
                this.progressData = gistData;
                console.log('📥 Progress loaded from GitHub Gist');
                return;
            }

            // Fallback to localStorage
            const localData = localStorage.getItem('tankermade-progress');
            if (localData) {
                this.progressData = JSON.parse(localData);
                console.log('📥 Progress loaded from localStorage');

                // Sync to GitHub for future use
                await this.saveToGist();
            }

        } catch (error) {
            console.warn('⚠️ Failed to load progress:', error);
            // Use default empty progress
        }
    }

    async loadFromGist() {
        if (!window.TankerMadeAuth?.isAuthenticated()) {
            return null;
        }

        try {
            const user = window.TankerMadeAuth.getUser();
            const token = window.TankerMadeAuth.getToken();

            // First, try to find existing progress gist
            const gistsResponse = await fetch('https://api.github.com/gists', {
                headers: {
                    'Authorization': `token ${token}`,
                    'Accept': 'application/vnd.github.v3+json'
                }
            });

            if (gistsResponse.ok) {
                const gists = await gistsResponse.json();
                const progressGist = gists.find(g => 
                    g.description === 'TankerMade Development Progress' ||
                    g.files['tankermade-progress.json']
                );

                if (progressGist) {
                    this.gistId = progressGist.id;
                    const fileContent = progressGist.files['tankermade-progress.json'];
                    if (fileContent && fileContent.content) {
                        return JSON.parse(fileContent.content);
                    }
                }
            }

            return null;

        } catch (error) {
            console.warn('⚠️ Failed to load from Gist:', error);
            return null;
        }
    }

    async saveToGist() {
        if (!window.TankerMadeAuth?.isAuthenticated()) {
            console.warn('⚠️ Not authenticated, cannot save to Gist');
            return false;
        }

        try {
            const token = window.TankerMadeAuth.getToken();
            const progressJson = JSON.stringify(this.progressData, null, 2);

            const gistData = {
                description: 'TankerMade Development Progress',
                public: false,
                files: {
                    'tankermade-progress.json': {
                        content: progressJson
                    }
                }
            };

            let url = 'https://api.github.com/gists';
            let method = 'POST';

            // If we have an existing gist, update it
            if (this.gistId) {
                url = `https://api.github.com/gists/${this.gistId}`;
                method = 'PATCH';
            }

            const response = await fetch(url, {
                method: method,
                headers: {
                    'Authorization': `token ${token}`,
                    'Accept': 'application/vnd.github.v3+json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(gistData)
            });

            if (response.ok) {
                const result = await response.json();
                this.gistId = result.id;
                this.progressData.lastUpdated = new Date().toISOString();
                console.log('💾 Progress saved to GitHub Gist');
                return true;
            } else {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

        } catch (error) {
            console.error('❌ Failed to save to Gist:', error);
            return false;
        }
    }

    // Mark task as complete/incomplete
    setTaskComplete(phaseId, taskId, isComplete) {
        if (!this.progressData.phases[phaseId]) {
            this.progressData.phases[phaseId] = {};
        }

        this.progressData.phases[phaseId][taskId] = {
            completed: isComplete,
            completedAt: isComplete ? new Date().toISOString() : null
        };

        this.markDirty();

        console.log(`📝 Task ${taskId} in Phase ${phaseId}: ${isComplete ? 'completed' : 'uncompleted'}`);

        // Trigger UI updates
        this.notifyProgressChange(phaseId);
    }

    // Get task completion status
    isTaskComplete(phaseId, taskId) {
        return this.progressData.phases[phaseId]?.[taskId]?.completed || false;
    }

    // Get phase completion stats
    getPhaseStats(phaseId, taskList) {
        if (!taskList || taskList.length === 0) {
            return { completed: 0, total: 0, percentage: 0 };
        }

        const completed = taskList.filter(task => 
            this.isTaskComplete(phaseId, task.id)
        ).length;

        return {
            completed,
            total: taskList.length,
            percentage: Math.round((completed / taskList.length) * 100)
        };
    }

    // Get overall progress across all phases
    getOverallStats(allPhases) {
        let totalTasks = 0;
        let completedTasks = 0;

        allPhases.forEach(phase => {
            if (phase.tasks) {
                totalTasks += phase.tasks.length;
                completedTasks += phase.tasks.filter(task => 
                    this.isTaskComplete(phase.id, task.id)
                ).length;
            }
        });

        return {
            completed: completedTasks,
            total: totalTasks,
            percentage: totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0
        };
    }

    markDirty() {
        this.isDirty = true;

        // Save to localStorage immediately for quick access
        localStorage.setItem('tankermade-progress', JSON.stringify(this.progressData));

        // Debounced save to GitHub
        if (this.saveTimeout) {
            clearTimeout(this.saveTimeout);
        }

        this.saveTimeout = setTimeout(() => {
            this.saveToGist();
            this.isDirty = false;
        }, 2000); // Save 2 seconds after last change
    }

    setupAutoSave() {
        // Save every 5 minutes if dirty
        setInterval(() => {
            if (this.isDirty) {
                this.saveToGist();
                this.isDirty = false;
            }
        }, 5 * 60 * 1000);

        // Save when page is about to close
        window.addEventListener('beforeunload', () => {
            if (this.isDirty) {
                // Synchronous save for page unload
                const progressJson = JSON.stringify(this.progressData);
                localStorage.setItem('tankermade-progress', progressJson);
            }
        });
    }

    // Notify other components of progress changes
    notifyProgressChange(phaseId) {
        const event = new CustomEvent('progress-updated', {
            detail: { phaseId, progressData: this.progressData }
        });
        document.dispatchEvent(event);
    }

    // Export progress for debugging
    exportProgress() {
        return JSON.stringify(this.progressData, null, 2);
    }

    // Admin function to reset all progress (use carefully!)
    async resetAllProgress() {
        if (!confirm('Are you sure you want to reset ALL progress? This cannot be undone.')) {
            return false;
        }

        this.progressData = {
            phases: {},
            lastUpdated: new Date().toISOString(),
            version: '1.0'
        };

        this.markDirty();
        await this.saveToGist();

        // Notify all components
        document.dispatchEvent(new CustomEvent('progress-reset'));

        console.log('🔄 All progress reset');
        return true;
    }
}

// Global instance
window.TankerMadeProgress = new ProgressManager();

// Auto-initialize when auth is ready
document.addEventListener('auth-state-changed', async (event) => {
    if (event.detail.authenticated) {
        await window.TankerMadeProgress.init();
    }
});