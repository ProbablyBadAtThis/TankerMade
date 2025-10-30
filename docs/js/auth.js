/**
 * GitHub OAuth Authentication Handler
 */

class GitHubAuth {
    constructor() {
        this.clientId = 'YOUR_GITHUB_CLIENT_ID'; // Replace with your actual client ID
        this.scopes = 'repo read:user';
        this.redirectUri = window.location.origin + '/auth/callback.html';
        this.apiBase = 'https://api.github.com';
        this.user = null;
        this.token = null;
    }

    // Initialize authentication on app startup
    async init() {
        console.log('🔐 Initializing GitHub Authentication...');

        const auth = await this.checkAuth();
        if (auth) {
            this.token = auth.token;
            this.user = auth.user;
            this.showAuthenticatedState();
        } else {
            this.showUnauthenticatedState();
        }

        this.setupEventListeners();
        return auth;
    }

    // Check if user is already authenticated
    async checkAuth() {
        const token = localStorage.getItem('github-token');
        if (!token) return null;

        try {
            const user = await this.getUser(token);
            return { token, user };
        } catch (error) {
            console.warn('Token validation failed:', error);
            localStorage.removeItem('github-token');
            return null;
        }
    }

    // Get user information from GitHub API
    async getUser(token) {
        const response = await fetch(`${this.apiBase}/user`, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': 'application/vnd.github.v3+json'
            }
        });

        if (!response.ok) {
            throw new Error('Failed to fetch user information');
        }

        return await response.json();
    }

    // Generate random state for OAuth security
    generateState() {
        return Math.random().toString(36).substring(2, 15) + 
               Math.random().toString(36).substring(2, 15);
    }

    // Initiate GitHub OAuth flow
    login() {
        const state = this.generateState();
        localStorage.setItem('oauth-state', state);

        const params = new URLSearchParams({
            client_id: this.clientId,
            scope: this.scopes,
            state: state,
            redirect_uri: this.redirectUri
        });

        window.location.href = `https://github.com/login/oauth/authorize?${params}`;
    }

    // Sign out user
    logout() {
        localStorage.removeItem('github-token');
        localStorage.removeItem('oauth-state');
        this.token = null;
        this.user = null;
        this.showUnauthenticatedState();

        // Reload to clear any cached data
        window.location.reload();
    }

    // Make authenticated API request
    async apiRequest(endpoint, options = {}) {
        if (!this.token) {
            throw new Error('No authentication token available');
        }

        const response = await fetch(`${this.apiBase}${endpoint}`, {
            ...options,
            headers: {
                'Authorization': `Bearer ${this.token}`,
                'Accept': 'application/vnd.github.v3+json',
                'Content-Type': 'application/json',
                ...options.headers
            }
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw Object.assign(new Error(error.message || 'API request failed'), {
                status: response.status,
                response: error
            });
        }

        return await response.json();
    }

    // Setup event listeners for auth buttons
    setupEventListeners() {
        const loginBtn = document.getElementById('github-login-btn');
        const logoutBtn = document.getElementById('logout-btn');

        if (loginBtn) {
            loginBtn.addEventListener('click', () => this.login());
        }

        if (logoutBtn) {
            logoutBtn.addEventListener('click', () => this.logout());
        }
    }

    // Show login button and hide protected navigation when not authenticated
    showUnauthenticatedState() {
        // Header auth section
        const loginBtn = document.getElementById('github-login-btn');
        const userInfo = document.getElementById('user-info');

        if (loginBtn) loginBtn.style.display = 'flex';
        if (userInfo) userInfo.style.display = 'none';

        // Hide protected navigation sections
        this.hideProtectedNavigation();
    }

    // Show user info and protected navigation when authenticated
    showAuthenticatedState() {
        // Header auth section
        const loginBtn = document.getElementById('github-login-btn');
        const userInfo = document.getElementById('user-info');
        const userAvatar = document.getElementById('user-avatar');
        const userName = document.getElementById('user-name');

        if (loginBtn) loginBtn.style.display = 'none';
        if (userInfo) userInfo.style.display = 'flex';

        if (userAvatar && this.user) {
            userAvatar.src = this.user.avatar_url;
            userAvatar.alt = `${this.user.login} avatar`;
        }

        if (userName && this.user) {
            userName.textContent = this.user.login;
        }

        // Show protected navigation sections
        this.showProtectedNavigation();
    }

    // Hide protected navigation elements
    hideProtectedNavigation() {
        // Hide sidebar protected sections
        const protectedSections = document.querySelectorAll('.nav-protected');
        protectedSections.forEach(section => {
            section.style.display = 'none';
        });

        // Show auth prompt
        const authPrompt = document.getElementById('nav-auth-prompt');
        if (authPrompt) {
            authPrompt.style.display = 'block';
        }

        // Hide mobile bottom nav protected items
        const bottomNavProtected = document.getElementById('bottom-nav-protected');
        if (bottomNavProtected) {
            bottomNavProtected.style.display = 'none';
        }
    }

    // Show protected navigation elements
    showProtectedNavigation() {
        // Show sidebar protected sections
        const protectedSections = document.querySelectorAll('.nav-protected');
        protectedSections.forEach(section => {
            section.style.display = 'block';
        });

        // Hide auth prompt
        const authPrompt = document.getElementById('nav-auth-prompt');
        if (authPrompt) {
            authPrompt.style.display = 'none';
        }

        // Show mobile bottom nav protected items
        const bottomNavProtected = document.getElementById('bottom-nav-protected');
        if (bottomNavProtected) {
            bottomNavProtected.style.display = 'flex';
        }
    }

    // Check if user is authenticated
    isAuthenticated() {
        return this.token !== null && this.user !== null;
    }

    // Get current user
    getCurrentUser() {
        return this.user;
    }

    // Get current token
    getToken() {
        return this.token;
    }
}

// Create global auth instance
window.TankerMadeAuth = new GitHubAuth();