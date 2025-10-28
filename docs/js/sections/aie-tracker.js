/**
 * AIE Tracker Section - Incident monitoring and GitHub Issues integration
 */

function initAIETracker(params = {}) {
    console.log('🚨 Initializing AIE Tracker', params);

    if (params.incidentId) {
        loadIncidentDetails(params.incidentId);
    } else {
        loadAIETrackerOverview();
    }
}

function loadAIETrackerOverview() {
    // Load incidents from localStorage (placeholder for GitHub API)
    const incidents = getIncidentData();
    const stats = calculateIncidentStats(incidents);

    const trackerContent = `
        <div class="aie-tracker-header mb-6">
            <div class="flex items-center justify-between">
                <div>
                    <h1>AIE Tracker - Incident Monitoring</h1>
                    <p>Monitor and manage development incidents integrated with GitHub Issues</p>
                </div>
                <div class="flex gap-2">
                    <button class="btn btn-primary" onclick="createNewIncident()">
                        ➕ New Incident
                    </button>
                    <button class="btn btn-secondary" onclick="refreshIncidents()">
                        🔄 Refresh
                    </button>
                </div>
            </div>
        </div>

        <div class="grid grid-cols-5 mb-6">
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-primary">${stats.total}</div>
                    <div class="text-sm text-secondary">Total</div>
                </div>
            </div>
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-error">${stats.critical}</div>
                    <div class="text-sm text-secondary">Critical</div>
                </div>
            </div>
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-warning">${stats.high}</div>
                    <div class="text-sm text-secondary">High</div>
                </div>
            </div>
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-info">${stats.medium}</div>
                    <div class="text-sm text-secondary">Medium</div>
                </div>
            </div>
            <div class="card text-center">
                <div class="card-body">
                    <div class="text-2xl font-bold text-secondary">${stats.low}</div>
                    <div class="text-sm text-secondary">Low</div>
                </div>
            </div>
        </div>

        <div class="card mb-6">
            <div class="card-header">
                <h3 class="card-title">Filters</h3>
            </div>
            <div class="card-body">
                <div class="grid grid-cols-4 gap-4">
                    <select id="severity-filter" class="form-input">
                        <option value="">All Severities</option>
                        <option value="critical">Critical</option>
                        <option value="high">High</option>
                        <option value="medium">Medium</option>
                        <option value="low">Low</option>
                    </select>
                    <select id="status-filter" class="form-input">
                        <option value="">All Statuses</option>
                        <option value="open">Open</option>
                        <option value="in-progress">In Progress</option>
                        <option value="resolved">Resolved</option>
                        <option value="closed">Closed</option>
                    </select>
                    <select id="phase-filter" class="form-input">
                        <option value="">All Phases</option>
                        <option value="1">Phase 1</option>
                        <option value="2">Phase 2</option>
                        <option value="3">Phase 3</option>
                    </select>
                    <input type="search" id="search-incidents" class="form-input" placeholder="Search incidents...">
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                <h3 class="card-title">Incidents</h3>
            </div>
            <div class="card-body">
                <div id="incidents-list">
                    ${generateIncidentsList(incidents)}
                </div>
            </div>
        </div>
    `;

    const contentContainer = document.getElementById('content-container');
    if (contentContainer) {
        contentContainer.innerHTML = trackerContent;
        setupAIETrackerEvents();
    }
}

function generateIncidentsList(incidents) {
    if (incidents.length === 0) {
        return `
            <div class="text-center py-8">
                <div class="text-4xl mb-4">🎉</div>
                <h3 class="text-lg font-semibold mb-2">No Open Incidents</h3>
                <p class="text-secondary">Great job! All incidents are resolved.</p>
                <button class="btn btn-primary mt-4" onclick="createSampleIncident()">
                    Create Sample Incident
                </button>
            </div>
        `;
    }

    return incidents.map(incident => `
        <div class="incident-item border-b last:border-b-0 py-4" data-incident-id="${incident.id}">
            <div class="flex items-start justify-between">
                <div class="flex-1">
                    <div class="flex items-center gap-2 mb-2">
                        <span class="badge badge-${getSeverityColor(incident.severity)}">${incident.severity}</span>
                        <span class="badge badge-${getStatusColor(incident.status)}">${incident.status}</span>
                        ${incident.phase ? `<span class="badge badge-secondary">Phase ${incident.phase}</span>` : ''}
                    </div>
                    <h4 class="font-semibold mb-1">
                        <a href="#" onclick="viewIncidentDetails('${incident.id}')" class="text-primary hover:underline">
                            #${incident.id} - ${incident.title}
                        </a>
                    </h4>
                    <p class="text-secondary text-sm mb-2">${incident.description}</p>
                    <div class="flex items-center gap-4 text-xs text-secondary">
                        <span>Created: ${formatDate(incident.createdAt)}</span>
                        ${incident.assignee ? `<span>Assigned to: ${incident.assignee}</span>` : ''}
                        ${incident.githubUrl ? `<a href="${incident.githubUrl}" target="_blank" class="text-primary hover:underline">View on GitHub</a>` : ''}
                    </div>
                </div>
                <div class="flex gap-2">
                    <button class="btn btn-sm btn-secondary" onclick="editIncident('${incident.id}')">
                        Edit
                    </button>
                    <button class="btn btn-sm btn-success" onclick="resolveIncident('${incident.id}')">
                        Resolve
                    </button>
                </div>
            </div>
        </div>
    `).join('');
}

function setupAIETrackerEvents() {
    // Set up filter event listeners
    const severityFilter = document.getElementById('severity-filter');
    const statusFilter = document.getElementById('status-filter');
    const phaseFilter = document.getElementById('phase-filter');
    const searchInput = document.getElementById('search-incidents');

    [severityFilter, statusFilter, phaseFilter].forEach(filter => {
        if (filter) {
            filter.addEventListener('change', applyFilters);
        }
    });

    if (searchInput) {
        searchInput.addEventListener('input', debounce(applyFilters, 300));
    }
}

function applyFilters() {
    const severity = document.getElementById('severity-filter')?.value || '';
    const status = document.getElementById('status-filter')?.value || '';
    const phase = document.getElementById('phase-filter')?.value || '';
    const search = document.getElementById('search-incidents')?.value.toLowerCase() || '';

    const incidents = getIncidentData();
    const filtered = incidents.filter(incident => {
        const matchesSeverity = !severity || incident.severity === severity;
        const matchesStatus = !status || incident.status === status;
        const matchesPhase = !phase || incident.phase === parseInt(phase);
        const matchesSearch = !search || 
            incident.title.toLowerCase().includes(search) ||
            incident.description.toLowerCase().includes(search);

        return matchesSeverity && matchesStatus && matchesPhase && matchesSearch;
    });

    const incidentsList = document.getElementById('incidents-list');
    if (incidentsList) {
        incidentsList.innerHTML = generateIncidentsList(filtered);
    }
}

function getIncidentData() {
    const saved = localStorage.getItem('tankermade-incidents');
    return saved ? JSON.parse(saved) : [];
}

function calculateIncidentStats(incidents) {
    return {
        total: incidents.length,
        critical: incidents.filter(i => i.severity === 'critical').length,
        high: incidents.filter(i => i.severity === 'high').length,
        medium: incidents.filter(i => i.severity === 'medium').length,
        low: incidents.filter(i => i.severity === 'low').length
    };
}

function getSeverityColor(severity) {
    const colors = {
        critical: 'error',
        high: 'warning',
        medium: 'info',
        low: 'secondary'
    };
    return colors[severity] || 'secondary';
}

function getStatusColor(status) {
    const colors = {
        open: 'error',
        'in-progress': 'warning',
        resolved: 'success',
        closed: 'secondary'
    };
    return colors[status] || 'secondary';
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function createNewIncident() {
    // Show create incident modal (placeholder)
    const title = prompt('Incident Title:');
    if (title) {
        const incident = {
            id: Date.now(),
            title,
            description: prompt('Description:') || '',
            severity: 'medium',
            status: 'open',
            createdAt: new Date().toISOString(),
            assignee: null,
            phase: null,
            githubUrl: null
        };

        const incidents = getIncidentData();
        incidents.push(incident);
        localStorage.setItem('tankermade-incidents', JSON.stringify(incidents));

        window.TankerMadeApp.showNotification('Incident created successfully!', 'success');
        loadAIETrackerOverview();
    }
}

function createSampleIncident() {
    const sampleIncident = {
        id: Date.now(),
        title: 'Authentication token expiry causes 401 loops',
        description: 'Users are getting stuck in authentication loops when tokens expire during active sessions.',
        severity: 'high',
        status: 'open',
        createdAt: new Date().toISOString(),
        assignee: 'developer',
        phase: 1,
        githubUrl: 'https://github.com/ProbablyBadAtThis/TankerMade/issues/123'
    };

    const incidents = getIncidentData();
    incidents.push(sampleIncident);
    localStorage.setItem('tankermade-incidents', JSON.stringify(incidents));

    window.TankerMadeApp.showNotification('Sample incident created!', 'success');
    loadAIETrackerOverview();
}

function viewIncidentDetails(incidentId) {
    window.TankerMadeRouter.goToSection('incident-details', { incidentId });
}

function editIncident(incidentId) {
    window.TankerMadeApp.showNotification('Edit incident feature coming soon!', 'info');
}

function resolveIncident(incidentId) {
    const incidents = getIncidentData();
    const incident = incidents.find(i => i.id == incidentId);

    if (incident) {
        incident.status = 'resolved';
        incident.resolvedAt = new Date().toISOString();

        localStorage.setItem('tankermade-incidents', JSON.stringify(incidents));
        window.TankerMadeApp.showNotification('Incident resolved!', 'success');
        loadAIETrackerOverview();
    }
}

function refreshIncidents() {
    window.TankerMadeApp.showNotification('Refreshing incidents...', 'info');
    loadAIETrackerOverview();
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Export functions
window.initAIETracker = initAIETracker;
window.createNewIncident = createNewIncident;
window.createSampleIncident = createSampleIncident;
window.viewIncidentDetails = viewIncidentDetails;
window.editIncident = editIncident;
window.resolveIncident = resolveIncident;
window.refreshIncidents = refreshIncidents;
