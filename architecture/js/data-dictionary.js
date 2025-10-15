// Enhanced data dictionary with advanced filtering and search
function badge(control) {
    const configs = {
        user: { class: "user", label: "User", icon: "🔵" },
        system: { class: "system", label: "System", icon: "🔴" },
        admin: { class: "admin", label: "Admin", icon: "🟡" },
        computed: { class: "computed", label: "Computed", icon: "🟢" }
    };

    const config = configs[control] || { class: "user", label: "Unknown", icon: "⚪" };
    return `<span class="control-badge ${config.class}">${config.icon} ${config.label}</span>`;
}

function matches(text, query) {
    if (!query) return true;
    return (text || "").toLowerCase().includes(query.toLowerCase());
}

function highlightMatch(text, query) {
    if (!query || !text) return text;
    const regex = new RegExp(`(${query})`, 'gi');
    return text.replace(regex, '<mark>$1</mark>');
}

window.renderDataDictionary = function renderDataDictionary() {
    if (window._dictInited) {
        applyDictFilters();
        return;
    }
    window._dictInited = true;

    const search = document.getElementById("dictSearch");
    const control = document.getElementById("dictControlFilter");

    // Enhanced search with debouncing
    let searchTimeout;
    search.addEventListener("input", function () {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(applyDictFilters, 300);
    });

    control.addEventListener("change", applyDictFilters);

    // Initial render
    applyDictFilters();

    console.log('Data Dictionary initialized with', window.TM.fields.length, 'fields');
};

function applyDictFilters() {
    const body = document.getElementById("dictBody");
    const query = (document.getElementById("dictSearch").value || "").trim();
    const controlFilter = document.getElementById("dictControlFilter").value;

    // Multi-field search
    const filteredFields = window.TM.fields.filter(field => {
        // Control type filter
        if (controlFilter && field.control !== controlFilter) return false;

        // Search across multiple fields
        if (query) {
            return matches(field.name, query) ||
                matches(field.type, query) ||
                matches(field.purpose, query) ||
                matches(field.validation, query) ||
                matches(field.security, query) ||
                (field.usedIn || []).some(usage => matches(usage, query));
        }

        return true;
    });

    // Generate table rows
    const rows = filteredFields.map(field => {
        const usedInList = (field.usedIn || [])
            .map(usage => `<code>${highlightMatch(usage, query)}</code>`)
            .join(", ");

        return `
      <tr data-control="${field.control}">
        <td>
          <span class="field-name ${field.control}">
            ${highlightMatch(field.name, query)}
          </span>
        </td>
        <td><code>${highlightMatch(field.type, query)}</code></td>
        <td>${badge(field.control)}</td>
        <td>${highlightMatch(field.purpose, query)}</td>
        <td>${usedInList}</td>
        <td>${highlightMatch(field.validation || "", query)}</td>
        <td>${highlightMatch(field.security || "", query)}</td>
      </tr>
    `;
    }).join("");

    // Update table with results or no-results message
    body.innerHTML = rows || `
    <tr>
      <td colspan="7" style="color: var(--text-muted); text-align: center; padding: 2rem;">
        No fields match your current filters. Try adjusting your search terms.
      </td>
    </tr>
  `;

    // Update search results info
    updateResultsInfo(filteredFields.length, window.TM.fields.length, query, controlFilter);
}

function updateResultsInfo(filtered, total, query, controlFilter) {
    // Add results summary if it doesn't exist
    let summary = document.getElementById("results-summary");
    if (!summary) {
        summary = document.createElement("div");
        summary.id = "results-summary";
        summary.style.cssText = `
      color: var(--text-muted);
      font-size: 0.85rem;
      margin-bottom: 0.5rem;
      padding: 0.5rem 0;
      border-bottom: 1px solid var(--border-light);
    `;

        const tableWrapper = document.querySelector(".table-wrapper");
        tableWrapper.parentNode.insertBefore(summary, tableWrapper);
    }

    // Update summary text
    let summaryText = `Showing ${filtered} of ${total} fields`;
    if (query) summaryText += ` matching "${query}"`;
    if (controlFilter) summaryText += ` (${controlFilter} only)`;

    summary.textContent = summaryText;
}

// Export for potential external use
window.TM.searchFields = function (query, controlType = null) {
    return window.TM.fields.filter(field => {
        if (controlType && field.control !== controlType) return false;
        if (!query) return true;

        return matches(field.name, query) ||
            matches(field.purpose, query) ||
            (field.usedIn || []).some(usage => matches(usage, query));
    });
};

// Add CSS for search highlighting
const style = document.createElement('style');
style.textContent = `
  mark {
    background: #fef08a;
    color: #92400e;
    padding: 0.1em 0.2em;
    border-radius: 2px;
    font-weight: 600;
  }
`;
document.head.appendChild(style);
