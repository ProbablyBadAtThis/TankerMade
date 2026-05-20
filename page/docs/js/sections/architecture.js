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
                        <p id="architecture-view-description">Interactive ERD showing data model relationships.</p>
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
                        <h3>Entity Relationship Diagram</h3>
                        <p>Interactive diagram content will render here as the visualizer matures.</p>
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
    setArchitectureHeader('Entity Relationships', 'Interactive ERD showing data model relationships.');
    const container = document.getElementById('visualization-container');
    if (container) {
        container.innerHTML = `
            <div>
                <h3>Entity Relationship Diagram</h3>
                <p>The interactive ERD will live here once the visualizer content is migrated.</p>
                <button class="btn btn-primary mt-4" onclick="loadExistingArchitecture()">
                    Load Existing Visualizer
                </button>
            </div>
        `;
    }
}

function showDataFlow() {
    setArchitectureTab('Data Flow');
    setArchitectureHeader('Data Flow', 'System interaction maps and data flow diagrams.');
    const container = document.getElementById('visualization-container');
    if (container) {
        container.innerHTML = `
            <div>
                <h3>Data Flow Diagram</h3>
                <p>Data flow visualizations will be available after content migration.</p>
            </div>
        `;
    }
}

function showSystemArchitecture() {
    setArchitectureTab('System Architecture');
    setArchitectureHeader('System Architecture', 'Component dependencies and system overview.');
    const container = document.getElementById('visualization-container');
    if (container) {
        container.innerHTML = `
            <div>
                <h3>System Architecture</h3>
                <p>System architecture diagrams will be integrated after content migration.</p>
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
