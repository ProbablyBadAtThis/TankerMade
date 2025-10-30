/**
 * TankerMade Developer Dashboard - Client-Side Router
 * Handles navigation between sections and URL management
 */

class TankerMadeRouter {
    constructor() {
        this.routes = new Map();
        this.currentSection = null;
        this.contentContainer = null;
        this.loadingIndicator = null;
        this.errorState = null;

        // Initialize after DOM is loaded
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        // Get DOM elements
        this.contentContainer = document.getElementById('content-container');
        this.loadingIndicator = document.getElementById('loading-indicator');
        this.errorState = document.getElementById('error-state');

        // Register routes
        this.registerRoutes();

        // Set up event listeners
        this.setupEventListeners();

        // Handle initial route
        this.handleInitialRoute();

        console.log('🚀 TankerMade Router initialized');
    }

    registerRoutes() {
        // Define all application routes
        this.routes.set('dashboard', {
            name: 'Dashboard',
            path: '/sections/dashboard.html',
            breadcrumb: ['Dashboard'],
            icon: '🏠',
            initFunction: 'initDashboard'
        });

        this.routes.set('dev-tracker', {
            name: 'Dev Tracker',
            path: '/sections/dev-tracker.html',
            breadcrumb: ['Dashboard', 'Dev Tracker'],
            icon: '📊',
            initFunction: 'initDevTracker'
        });

        this.routes.set('dev-tracker-phase', {
            name: 'Phase Details',
            path: '/sections/dev-tracker-phase.html',
            breadcrumb: ['Dashboard', 'Dev Tracker', 'Phase'],
            icon: '📊',
            initFunction: 'initDevTrackerPhase'
        });

        this.routes.set('workbench', {
            name: 'Workbench',
            path: '/sections/workbench.html',
            breadcrumb: ['Dashboard', 'Workbench'],
            icon: '📚',
            initFunction: 'initWorkbench'
        });

        this.routes.set('workbench-section', {
            name: 'Documentation',
            path: '/sections/workbench-section.html',
            breadcrumb: ['Dashboard', 'Workbench', 'Documentation'],
            icon: '📚',
            initFunction: 'initWorkbenchSection'
        });

        this.routes.set('architecture', {
            name: 'Architecture Visualizer',
            path: '/sections/architecture.html',
            breadcrumb: ['Dashboard', 'Architecture Visualizer'],
            icon: '🏗️',
            initFunction: 'initArchitecture'
        });

        this.routes.set('incidents', {
            name: 'AIE Tracker',
            path: '/sections/aie-tracker.html',
            breadcrumb: ['Dashboard', 'AIE Tracker'],
            icon: '🚨',
            initFunction: 'initAIETracker'
        });

        this.routes.set('incident-details', {
            name: 'Incident Details',
            path: '/sections/incident-details.html',
            breadcrumb: ['Dashboard', 'AIE Tracker', 'Incident'],
            icon: '🚨',
            initFunction: 'initIncidentDetails'
        });
    }

    setupEventListeners() {
        // Handle navigation clicks
        document.addEventListener('click', (e) => {
            const navItem = e.target.closest('[data-section]');
            if (navItem) {
                e.preventDefault();
                const section = navItem.getAttribute('data-section');
                const params = this.extractParams(navItem);
                this.navigate(section, params);
            }
        });

        // Handle browser back/forward buttons
        window.addEventListener('popstate', (e) => {
            if (e.state && e.state.section) {
                this.navigate(e.state.section, e.state.params, false);
            } else {
                this.navigate('dashboard', {}, false);
            }
        });

        // Handle hash changes for backward compatibility
        window.addEventListener('hashchange', () => {
            this.handleHashRoute();
        });

        // Retry button in error state
        const retryBtn = document.getElementById('retry-btn');
        if (retryBtn) {
            retryBtn.addEventListener('click', () => {
                if (this.currentSection) {
                    this.navigate(this.currentSection.key, this.currentSection.params);
                }
            });
        }
    }

    handleInitialRoute() {
        const hash = window.location.hash.slice(1);
        if (hash) {
            this.handleHashRoute();
        } else {
            this.navigate('dashboard');
        }
    }

    handleHashRoute() {
        const hash = window.location.hash.slice(1);
        const [section, ...paramParts] = hash.split('/');

        if (section && this.routes.has(section)) {
            const params = this.parseParamParts(paramParts);
            this.navigate(section, params, false);
        } else {
            this.navigate('dashboard', {}, false);
        }
    }

    parseParamParts(paramParts) {
        const params = {};
        for (let i = 0; i < paramParts.length; i += 2) {
            if (paramParts[i] && paramParts[i + 1]) {
                params[paramParts[i]] = paramParts[i + 1];
            }
        }
        return params;
    }

    extractParams(element) {
        const params = {};
        // Extract data attributes as parameters
        for (const attr of element.attributes) {
            if (attr.name.startsWith('data-param-')) {
                const paramName = attr.name.replace('data-param-', '');
                params[paramName] = attr.value;
            }
        }
        return params;
    }

    async navigate(sectionKey, params = {}, updateHistory = true) {
        const route = this.routes.get(sectionKey);
        if (!route) {
            console.error(`Route not found: ${sectionKey}`);
            return;
        }

        // Show loading state
        this.showLoading();

        try {
            // Load content
            const content = await this.loadContent(route.path, params);

            // Update DOM
            this.updateContent(content);
            this.updateNavigation(sectionKey);
            this.updateBreadcrumb(route.breadcrumb, params);
            this.updatePageTitle(route.name);

            // Update URL and history
            if (updateHistory) {
                this.updateHistory(sectionKey, params);
            }

            // Initialize section-specific functionality
            this.initializeSection(route.initFunction, params, sectionKey);

            // Store current section
            this.currentSection = { key: sectionKey, params, route };

            // Hide loading state
            this.hideLoading();

            // Focus management for accessibility
            this.manageFocus();

            console.log(`📍 Navigated to: ${route.name}`, params);

        } catch (error) {
            console.error('Navigation error:', error);
            this.showError();
        }
    }

    async loadContent(path, params = {}) {
        // Try to load from sections directory first
        let fullPath = path;

        // If it's a relative path, make it absolute
        if (!path.startsWith('http') && !path.startsWith('/')) {
            fullPath = `/sections/${path}`;
        }

        const response = await fetch(fullPath);

        if (!response.ok) {
            // Fallback: try to load from current directory structure
            const fallbackPath = path.replace('/sections/', '');
            const fallbackResponse = await fetch(fallbackPath);

            if (!fallbackResponse.ok) {
                throw new Error(`Failed to load content: ${response.status}`);
            }

            return await fallbackResponse.text();
        }

        return await response.text();
    }

    updateContent(content) {
        if (this.contentContainer) {
            this.contentContainer.innerHTML = content;
        }
    }

    updateNavigation(sectionKey) {
        // Update sidebar navigation
        const sidebarItems = document.querySelectorAll('.sidebar .nav-item');
        sidebarItems.forEach(item => {
            item.classList.remove('active');
            if (item.getAttribute('data-section') === sectionKey) {
                item.classList.add('active');
            }
        });

        // Update bottom navigation
        const bottomNavItems = document.querySelectorAll('.bottom-nav .bottom-nav-item');
        bottomNavItems.forEach(item => {
            item.classList.remove('active');
            if (item.getAttribute('data-section') === sectionKey) {
                item.classList.add('active');
            }
        });
    }

    updateBreadcrumb(breadcrumb, params = {}) {
        const breadcrumbList = document.getElementById('breadcrumb-list');
        if (!breadcrumbList) return;

        breadcrumbList.innerHTML = '';

        breadcrumb.forEach((crumb, index) => {
            const li = document.createElement('li');

            if (index === breadcrumb.length - 1) {
                // Last item (current page)
                li.setAttribute('aria-current', 'page');
                li.textContent = this.formatBreadcrumbText(crumb, params);
            } else {
                // Clickable breadcrumb item
                const a = document.createElement('a');
                a.href = '#';
                a.textContent = crumb;

                // Add click handler for navigation
                a.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.handleBreadcrumbClick(index, breadcrumb);
                });

                li.appendChild(a);
            }

            breadcrumbList.appendChild(li);
        });
    }

    formatBreadcrumbText(text, params) {
        // Replace placeholders with actual values
        if (params.phaseId) {
            return text.replace('Phase', `Phase ${params.phaseId}`);
        }
        if (params.incidentId) {
            return text.replace('Incident', `Incident #${params.incidentId}`);
        }
        return text;
    }

    handleBreadcrumbClick(index, breadcrumb) {
        // Navigate to parent level based on breadcrumb index
        if (index === 0) {
            this.navigate('dashboard');
        } else if (breadcrumb[index] === 'Dev Tracker') {
            this.navigate('dev-tracker');
        } else if (breadcrumb[index] === 'Workbench') {
            this.navigate('workbench');
        } else if (breadcrumb[index] === 'AIE Tracker') {
            this.navigate('incidents');
        }
    }

    updatePageTitle(sectionName) {
        document.title = `${sectionName} - TankerMade Developer Dashboard`;
    }

    updateHistory(sectionKey, params) {
        const url = this.buildUrl(sectionKey, params);
        const state = { section: sectionKey, params };

        history.pushState(state, '', url);
    }

    buildUrl(sectionKey, params) {
        let url = `#${sectionKey}`;

        // Add parameters to URL
        Object.entries(params).forEach(([key, value]) => {
            url += `/${key}/${value}`;
        });

        return url;
    }

    initializeSection(initFunction, params, sectionKey) {
        // Special handling for Dashboard section
        if (sectionKey === 'dashboard') {
            // Wait for the DOM to be ready, then initialize DashboardSection
            setTimeout(async () => {
                try {
                    const dashboardRoot = document.getElementById('dashboard-root');
                    if (!dashboardRoot) {
                        console.warn('⚠️ dashboard-root not found, retrying...');
                        setTimeout(() => this.initializeSection(initFunction, params, sectionKey), 100);
                        return;
                    }

                    if (window.DashboardSection) {
                        console.log('🏠 Initializing DashboardSection...');
                        const dashboardInstance = new window.DashboardSection();
                        await dashboardInstance.init();

                        // Store reference for later use
                        window.__currentDashboard = dashboardInstance;

                        console.log('✅ Dashboard initialization complete');
                    } else {
                        console.error('❌ DashboardSection class not found. Check if js/sections/dashboard.js is loaded.');
                    }
                } catch (error) {
                    console.error('❌ Dashboard initialization failed:', error);
                }
            }, 50); // Small delay to ensure DOM is ready
            return;
        }

        // Call section-specific initialization function if it exists
        if (initFunction && typeof window[initFunction] === 'function') {
            try {
                window[initFunction](params);
            } catch (error) {
                console.warn(`Failed to initialize section: ${initFunction}`, error);
            }
        }
    }

    showLoading() {
        if (this.loadingIndicator) {
            this.loadingIndicator.style.display = 'flex';
        }
        if (this.errorState) {
            this.errorState.style.display = 'none';
        }
    }

    hideLoading() {
        if (this.loadingIndicator) {
            this.loadingIndicator.style.display = 'none';
        }
    }

    showError() {
        if (this.errorState) {
            this.errorState.style.display = 'flex';
        }
        if (this.loadingIndicator) {
            this.loadingIndicator.style.display = 'none';
        }
    }

    manageFocus() {
        // Focus management for accessibility
        const mainContent = document.getElementById('main-content');
        if (mainContent) {
            mainContent.focus();
        }
    }

    // Public methods for external use
    getCurrentSection() {
        return this.currentSection;
    }

    reload() {
        if (this.currentSection) {
            this.navigate(this.currentSection.key, this.currentSection.params);
        }
    }

    goBack() {
        history.back();
    }

    goToSection(sectionKey, params = {}) {
        this.navigate(sectionKey, params);
    }
}

// Initialize router when script loads
const router = new TankerMadeRouter();

// Export for global access
window.TankerMadeRouter = router;