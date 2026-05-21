/**
 * Architecture Visualizer Section - System diagrams and visualizations
 */

function initArchitecture(params = {}) {
    console.log('🏗️ Initializing Architecture Visualizer', params);
    loadArchitectureVisualizer();
}

function loadArchitectureVisualizer() {
    const architectureContent = `
        <div class="architecture-container">
            <div class="architecture-header mb-6">
                <h1>Architecture Visualizer</h1>
                <p>Entity relationships, data movement, and system boundaries for the local-first maker workbench.</p>
            </div>

            <div class="section-tabs" role="tablist" aria-label="Architecture views">
                <button class="section-tab active" type="button" role="tab" aria-selected="true" onclick="showEntityDiagram()">Entity Relationships</button>
                <button class="section-tab" type="button" role="tab" aria-selected="false" onclick="showDataFlow()">Data Flow</button>
                <button class="section-tab" type="button" role="tab" aria-selected="false" onclick="showSystemArchitecture()">System Architecture</button>
            </div>

            <section class="page-panel architecture-workspace">
                <div class="architecture-toolbar">
                    <div>
                        <h2 id="architecture-view-title">Entity Relationships</h2>
                        <p id="architecture-view-description">Current persisted entities and planned model expansion.</p>
                    </div>
                    <div class="architecture-actions" id="visualization-controls">
                        <button class="btn btn-sm btn-secondary" onclick="zoomOut()" aria-label="Zoom out">−</button>
                        <button class="btn btn-sm btn-secondary" onclick="resetZoom()" aria-label="Reset zoom">Reset</button>
                        <button class="btn btn-sm btn-secondary" onclick="zoomIn()" aria-label="Zoom in">+</button>
                        <button class="btn btn-sm btn-primary" onclick="exportDiagram()" aria-label="Export diagram">Export</button>
                    </div>
                </div>

                <div id="visualization-container" class="architecture-canvas">
                    <div>
                        <h3>Entity Relationship Map</h3>
                        <p>Current model: Users, Projects, Patterns, Themes, Colors, Sources, and Brands. Planned model details are tracked in Phase A-D.</p>
                        <div class="architecture-mini-map" aria-label="Current entity overview">
                            <span>User</span>
                            <span>Project</span>
                            <span>Pattern</span>
                            <span>Theme</span>
                            <span>Source</span>
                            <span>Brand</span>
                            <span>Color</span>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    `;

    const contentContainer = document.getElementById('content-container');
    if (contentContainer) {
        contentContainer.innerHTML = architectureContent;
    }
}

function setArchitectureTab(label) {
    document.querySelectorAll('.section-tab').forEach(tab => {
        const isActive = tab.textContent.trim() === label;
        tab.classList.toggle('active', isActive);
        tab.setAttribute('aria-selected', isActive ? 'true' : 'false');
    });
}

function setArchitectureHeader(title, description) {
    const titleEl = document.getElementById('architecture-view-title');
    const descriptionEl = document.getElementById('architecture-view-description');
    if (titleEl) titleEl.textContent = title;
    if (descriptionEl) descriptionEl.textContent = description;
}

function showEntityDiagram() {
    setArchitectureTab('Entity Relationships');
    setArchitectureHeader('Entity Relationships', 'Current persisted entities and planned model expansion.');
    const container = document.getElementById('visualization-container');
    if (container) {
        container.innerHTML = `
            <div>
                <h3>Entity Relationship Map</h3>
                <p>Current persisted model: Users, Projects, Patterns, and shared reference data. Piece, step, timer, inventory, kit, and asset entities are still roadmap work.</p>
                <div class="architecture-mini-map" aria-label="Current entity overview">
                    <span>User</span>
                    <span>Project</span>
                    <span>Pattern</span>
                    <span>Theme</span>
                    <span>Source</span>
                    <span>Brand</span>
                    <span>Color</span>
                </div>
            </div>
        `;
    }
}

function showDataFlow() {
    setArchitectureTab('Data Flow');
    setArchitectureHeader('Data Flow', 'Current request flow and planned tracking areas.');
    const container = document.getElementById('visualization-container');
    if (container) {
        container.innerHTML = `
            <div>
                <h3>Current Flow</h3>
                <p>Blazor WASM client talks to the ASP.NET Core API, which uses application services and EF Core against SQLite. GitHub OAuth is only for this Pages tracker.</p>
                <div class="architecture-flow-list">
                    <span>Client</span>
                    <span>API</span>
                    <span>Services</span>
                    <span>EF Core</span>
                    <span>SQLite</span>
                </div>
            </div>
        `;
    }
}

function showSystemArchitecture() {
    setArchitectureTab('System Architecture');
    setArchitectureHeader('System Architecture', 'Solution boundaries and dependency direction.');
    const container = document.getElementById('visualization-container');
    if (container) {
        container.innerHTML = `
            <div>
                <h3>Solution Shape</h3>
                <p>Core stays dependency-light, Contracts references Core, Application implements service behavior, Server hosts API/EF/auth, and Client references Contracts.</p>
                <div class="architecture-flow-list">
                    <span>Core</span>
                    <span>Contracts</span>
                    <span>Application</span>
                    <span>Server</span>
                    <span>Client</span>
                </div>
            </div>
        `;
    }
}

function loadExistingArchitecture() {
    window.TankerMadeApp.showNotification('Architecture loading feature coming soon!', 'info');
}

// Export functions
window.initArchitecture = initArchitecture;
window.showEntityDiagram = showEntityDiagram;
window.showDataFlow = showDataFlow;
window.showSystemArchitecture = showSystemArchitecture;
window.loadExistingArchitecture = loadExistingArchitecture;
