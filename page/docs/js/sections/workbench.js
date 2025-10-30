/**
 * Workbench Section - Documentation and development resources
 */

function initWorkbench(params = {}) {
    console.log('📚 Initializing Workbench', params);

    if (params.section) {
        loadWorkbenchSection(params.section);
    } else {
        loadWorkbenchOverview();
    }
}

function loadWorkbenchOverview() {
    const workbenchContent = `
        <div class="workbench-header mb-6">
            <h1>Documentation Workbench</h1>
            <p>Architecture decisions, domain model documentation, development guidelines, and technical specifications</p>
        </div>

        <div class="grid grid-cols-2 gap-6">
            <div class="card" onclick="navigateToWorkbenchSection('architecture')">
                <div class="card-header">
                    <h3 class="card-title">🏗️ Architecture</h3>
                </div>
                <div class="card-body">
                    <p class="text-secondary">System architecture, patterns, and design decisions</p>
                </div>
            </div>

            <div class="card" onclick="navigateToWorkbenchSection('domain-model')">
                <div class="card-header">
                    <h3 class="card-title">🗄️ Domain Model</h3>
                </div>
                <div class="card-body">
                    <p class="text-secondary">Entity specifications and data relationships</p>
                </div>
            </div>

            <div class="card" onclick="navigateToWorkbenchSection('guidelines')">
                <div class="card-header">
                    <h3 class="card-title">📋 Development Guidelines</h3>
                </div>
                <div class="card-body">
                    <p class="text-secondary">Coding standards, best practices, and workflows</p>
                </div>
            </div>

            <div class="card" onclick="navigateToWorkbenchSection('technical-specs')">
                <div class="card-header">
                    <h3 class="card-title">⚙️ Technical Specifications</h3>
                </div>
                <div class="card-body">
                    <p class="text-secondary">Detailed technical requirements and specifications</p>
                </div>
            </div>

            <div class="card" onclick="navigateToWorkbenchSection('ui-ux-specs')">
                <div class="card-header">
                    <h3 class="card-title">🎨 UI/UX Specifications</h3>
                </div>
                <div class="card-body">
                    <p class="text-secondary">User interface and experience requirements</p>
                </div>
            </div>

            <div class="card" onclick="navigateToWorkbenchSection('deployment')">
                <div class="card-header">
                    <h3 class="card-title">🚀 Deployment</h3>
                </div>
                <div class="card-body">
                    <p class="text-secondary">Deployment guides and infrastructure setup</p>
                </div>
            </div>
        </div>
    `;

    const contentContainer = document.getElementById('content-container');
    if (contentContainer) {
        contentContainer.innerHTML = workbenchContent;
    }
}

function navigateToWorkbenchSection(section) {
    window.TankerMadeRouter.goToSection('workbench-section', { section });
}

function loadWorkbenchSection(section) {
    const sectionContent = `
        <div class="workbench-section-header mb-6">
            <div class="flex items-center gap-4">
                <button class="btn btn-secondary" onclick="window.TankerMadeRouter.goToSection('workbench')">
                    ← Back to Workbench
                </button>
                <div>
                    <h1>${getSectionTitle(section)}</h1>
                    <p class="text-secondary">Detailed documentation and specifications</p>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <div class="alert alert-info">
                    <p><strong>Content Migration Notice:</strong> This section will display content from the existing ${section}.html file once content migration is complete.</p>
                </div>
                <div class="mt-6">
                    <button class="btn btn-primary" onclick="loadExistingContent('${section}')">
                        Load Existing Content
                    </button>
                </div>
            </div>
        </div>
    `;

    const contentContainer = document.getElementById('content-container');
    if (contentContainer) {
        contentContainer.innerHTML = sectionContent;
    }
}

function getSectionTitle(section) {
    const titles = {
        'architecture': 'Architecture Documentation',
        'domain-model': 'Domain Model',
        'guidelines': 'Development Guidelines',
        'technical-specs': 'Technical Specifications',
        'ui-ux-specs': 'UI/UX Specifications',
        'deployment': 'Deployment Guide'
    };
    return titles[section] || 'Documentation';
}

function loadExistingContent(section) {
    window.TankerMadeApp.showNotification('Content loading feature coming soon!', 'info');
}

// Export functions
window.initWorkbench = initWorkbench;
window.navigateToWorkbenchSection = navigateToWorkbenchSection;
window.loadExistingContent = loadExistingContent;
