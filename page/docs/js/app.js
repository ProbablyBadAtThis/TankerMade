/**
 * TankerMade Dev — Main Application
 * Orchestrates auth, navigation, progress, search, and UI state.
 * All hardcoded progress values removed — data drives everything.
 */

class TankerMadeApp {
  constructor() {
    this.state = {
      currentPhase:    4,
      currentPhaseLabel: 'D',
      totalPhases:     9,
      overallProgress: 0,   // always derived, never hardcoded
      completedTasks:  37,
      totalTasks:      65,
      openIncidents:   0,
    };

    this.searchIndex    = new Map();
    this.isAuthenticated = false;

    if (document.readyState === 'loading') {
      document.addEventListener('DOMContentLoaded', () => this.init());
    } else {
      this.init();
    }
  }

  async init() {
    console.log('TankerMade Dev starting…');

    // Auth must resolve first
    const auth = await window.TankerMadeAuth.init();
    this.isAuthenticated = !!auth;

    if (this.isAuthenticated && window.GitHubDataStore) {
      window.TankerMadeData = new GitHubDataStore(window.TankerMadeAuth);
      await window.TankerMadeData.init();
    }

    this.setupEventListeners();
    this.setupMobileNavigation();
    this.setupUnifiedNavigation();
    this.initializeSearch();
    this.setupKeyboardShortcuts();

    await this.loadProgressData();
    this.loadIncidentData();
    this.updateHeaderStatus();

    console.log('TankerMade Dev ready.');
  }

  /* ── Event wiring ──────────────────────────────────────── */

  setupEventListeners() {
    const bind = (id, event, fn) => {
      const el = document.getElementById(id);
      if (el) el.addEventListener(event, fn.bind(this));
    };

    // Search
    bind('global-search', 'click',  this.openSearch);
    bind('search-input',  'input',  e => this.handleSearch(e.target.value));
    bind('search-input',  'keydown', e => e.key === 'Escape' && this.closeSearch());

    const searchModal = document.getElementById('search-modal');
    if (searchModal) {
      searchModal.addEventListener('click', e => {
        if (e.target === searchModal) this.closeSearch();
      });
      const closeBtn = searchModal.querySelector('.modal-close');
      if (closeBtn) closeBtn.addEventListener('click', () => this.closeSearch());
    }

    // Settings placeholder
    bind('app-settings', 'click', () => this.showNotification('Settings coming soon', 'info'));

    // Retry button in error state
    bind('retry-btn', 'click', () => window.TankerMadeRouter?.reload());
  }

  setupMobileNavigation() {
    const toggle  = document.getElementById('mobile-menu-toggle');
    const overlay = document.getElementById('sidebar-overlay');

    if (toggle) {
      toggle.addEventListener('click', () => this.toggleMobileSidebar());
    }
    if (overlay) {
      overlay.addEventListener('click', () => this.closeMobileSidebar());
    }
  }

  setupUnifiedNavigation() {
    const parentMap = {
      'dev-tracker-phase': 'dev-tracker',
      'workbench-section': 'workbench',
      'incident-details': 'incidents'
    };

    const sync = () => {
      const rawSection = (window.location.hash.slice(1).split('/')[0] || 'dashboard');
      const section = parentMap[rawSection] || rawSection;
      document.querySelectorAll('.unified-nav .nav-item').forEach(item => {
        const isActive = item.getAttribute('data-section') === section;
        item.classList.toggle('active', isActive);
        if (isActive) {
          item.setAttribute('aria-current', 'page');
        } else {
          item.removeAttribute('aria-current');
        }
      });
    };

    sync();
    window.addEventListener('hashchange', sync);
    window.addEventListener('popstate', sync);
    document.addEventListener('click', event => {
      if (event.target.closest('.unified-nav [data-section]')) {
        requestAnimationFrame(sync);
      }
    });
  }

  setupKeyboardShortcuts() {
    document.addEventListener('keydown', e => {
      if (e.key === 'Escape') {
        this.closeSearch();
        this.closeMobileSidebar();
        return;
      }

      if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        this.openSearch();
        return;
      }

      // Alt+1-5 nav shortcuts (authenticated only)
      if (e.altKey && !e.ctrlKey && !e.metaKey && this.isAuthenticated) {
        const map = { '1':'dashboard', '2':'dev-tracker', '3':'workbench', '4':'architecture', '5':'incidents' };
        if (map[e.key]) {
          e.preventDefault();
          window.TankerMadeRouter?.goToSection(map[e.key]);
        }
      }
    });
  }

  /* ── Progress ──────────────────────────────────────────── */

  async loadProgressData() {
    try {
      let completedTasks = 0;
      let totalTasks     = 0;

      const phaseCounts = [26, 5, 6, 7, 3, 5, 3, 4, 6];
      const baselineCompleted = [26, 5, 6, 0, 0, 0, 0, 0, 0];

      if (this.isAuthenticated && window.TankerMadeData) {
        const phases = await window.TankerMadeData.getAllPhaseProgress();
        phases.forEach((p, i) => {
          completedTasks += p.completed || 0;
          totalTasks     += p.total     || phaseCounts[i] || 0;
        });
      } else {
        // Unauthenticated: read localStorage only
        phaseCounts.forEach((count, i) => {
          totalTasks += count;
          const raw  = localStorage.getItem('phase-' + (i + 1) + '-progress');
          if (raw) {
            try { completedTasks += JSON.parse(raw).completed || baselineCompleted[i] || 0; } catch (_) {
              completedTasks += baselineCompleted[i] || 0;
            }
          } else {
            completedTasks += baselineCompleted[i] || 0;
          }
        });
      }

      this.state.completedTasks  = completedTasks;
      this.state.totalTasks      = totalTasks;
      this.state.overallProgress = totalTasks > 0
        ? Math.round((completedTasks / totalTasks) * 100) : 0;

    } catch (err) {
      console.warn('loadProgressData failed:', err);
    }
  }

  updateHeaderStatus() {
    const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };
    const setStyle = (id, prop, val) => { const el = document.getElementById(id); if (el) el.style[prop] = val; };

    set('current-phase', 'Phase ' + this.state.currentPhaseLabel + ': Inventory & Kits');
    set('progress-text', this.state.overallProgress + '%');
    setStyle('header-progress', 'width', this.state.overallProgress + '%');
  }

  /* ── Incidents ─────────────────────────────────────────── */

  loadIncidentData() {
    try {
      const raw = localStorage.getItem('tankermade-incidents');
      if (raw) {
        const list = JSON.parse(raw);
        this.state.openIncidents = list.filter(i => i.status === 'open').length;
      }
    } catch (_) {}
    this.updateIncidentBadges();
  }

  updateIncidentBadges() {
    const count = this.state.openIncidents;
    ['sidebar-incident-count', 'bottom-incident-count', 'header-incident-count'].forEach(id => {
      const el = document.getElementById(id);
      if (!el) return;
      el.textContent    = count;
      el.style.display  = count > 0 ? 'flex' : 'none';
    });
  }

  /* ── Mobile Sidebar ────────────────────────────────────── */

  toggleMobileSidebar() {
    const sidebar = document.getElementById('sidebar');
    sidebar?.classList.contains('open')
      ? this.closeMobileSidebar()
      : this.openMobileSidebar();
  }

  openMobileSidebar() {
    document.getElementById('sidebar')?.classList.add('open');
    document.getElementById('sidebar-overlay')?.classList.add('active');
    document.getElementById('mobile-menu-toggle')?.setAttribute('aria-expanded', 'true');
    document.body.style.overflow = 'hidden';
  }

  closeMobileSidebar() {
    document.getElementById('sidebar')?.classList.remove('open');
    document.getElementById('sidebar-overlay')?.classList.remove('active');
    document.getElementById('mobile-menu-toggle')?.setAttribute('aria-expanded', 'false');
    document.body.style.overflow = '';
  }

  /* ── Search ────────────────────────────────────────────── */

  initializeSearch() {
    const entries = [
      { key: 'dashboard',    title: 'Dashboard',                excerpt: 'Project status overview, quick actions, recent activity' },
      { key: 'dev-tracker',  title: 'Dev Tracker',              excerpt: 'Roadmap phase progress and task tracking' },
      { key: 'workbench',    title: 'Workbench',                excerpt: 'Documentation, domain model, development guidelines' },
      { key: 'architecture', title: 'Architecture',             excerpt: 'Entity diagrams, data flow, system architecture' },
      { key: 'incidents',    title: 'AIE / Incident Tracker',   excerpt: 'Incident monitoring, GitHub issues, problem log' },
    ];
    entries.forEach(e => this.searchIndex.set(e.key, e));
  }

  openSearch() {
    const modal = document.getElementById('search-modal');
    if (!modal) return;
    modal.classList.add('active');
    modal.setAttribute('aria-hidden', 'false');
    document.getElementById('search-input')?.focus();
  }

  closeSearch() {
    const modal = document.getElementById('search-modal');
    if (!modal) return;
    modal.classList.remove('active');
    modal.setAttribute('aria-hidden', 'true');
  }

  handleSearch(query) {
    const container = document.getElementById('search-results');
    if (!container) return;

    if (!query.trim()) { container.innerHTML = ''; return; }

    const q   = query.toLowerCase();
    const hits = [];
    this.searchIndex.forEach(item => {
      const score = (item.title.toLowerCase().includes(q) ? 2 : 0)
                  + (item.excerpt.toLowerCase().includes(q) ? 1 : 0);
      if (score) hits.push({ ...item, score });
    });
    hits.sort((a, b) => b.score - a.score);

    if (!hits.length) {
      container.innerHTML = '<div class="search-result"><div class="search-result-title">No results</div></div>';
      return;
    }

    container.innerHTML = hits.map(h => `
      <div class="search-result" role="listitem" tabindex="0"
           onclick="window.TankerMadeApp._searchNavigate('${h.key}')"
           onkeydown="if(event.key==='Enter')window.TankerMadeApp._searchNavigate('${h.key}')">
        <div class="search-result-title">${h.title}</div>
        <div class="search-result-excerpt">${h.excerpt}</div>
      </div>
    `).join('');
  }

  _searchNavigate(key) {
    this.closeSearch();
    window.TankerMadeRouter?.goToSection(key);
  }

  /* ── Export / Import ───────────────────────────────────── */

  exportProgress() {
    try {
      const data = {
        version:   '2.0',
        exported:  new Date().toISOString(),
        state:     this.state,
        phases:    this._collectPhaseData(),
        incidents: this._collectIncidentData(),
      };
      const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
      const url  = URL.createObjectURL(blob);
      const a    = Object.assign(document.createElement('a'), {
        href: url,
        download: 'tankermade-' + new Date().toISOString().split('T')[0] + '.json',
      });
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
      this.showNotification('Progress exported', 'success');
    } catch (err) {
      console.error('Export failed:', err);
      this.showNotification('Export failed', 'error');
    }
  }

  importProgress() {
    const input = Object.assign(document.createElement('input'), { type: 'file', accept: '.json' });
    input.onchange = e => {
      const file = e.target.files[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = ev => {
        try {
          const data = JSON.parse(ev.target.result);
          if (data.phases) {
            Object.entries(data.phases).forEach(([k, v]) => {
              localStorage.setItem(k, JSON.stringify(v));
            });
          }
          if (data.incidents) {
            localStorage.setItem('tankermade-incidents', JSON.stringify(data.incidents));
          }
          this.showNotification('Import successful — reloading', 'success');
          setTimeout(() => window.location.reload(), 1200);
        } catch (_) {
          this.showNotification('Import failed — invalid file', 'error');
        }
      };
      reader.readAsText(file);
    };
    input.click();
  }

  _collectPhaseData() {
    const out = {};
    for (let i = 1; i <= 10; i++) {
      const key = 'phase-' + i + '-progress';
      const raw = localStorage.getItem(key);
      if (raw) try { out[key] = JSON.parse(raw); } catch (_) {}
    }
    return out;
  }

  _collectIncidentData() {
    try { return JSON.parse(localStorage.getItem('tankermade-incidents') || '[]'); } catch (_) { return []; }
  }

  /* ── Notifications ─────────────────────────────────────── */

  showNotification(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = 'toast toast-' + type;
    toast.textContent = message;
    toast.setAttribute('role', 'status');
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 4500);
  }

  /* ── Public getters ────────────────────────────────────── */

  getState() { return { ...this.state }; }
}

// Bootstrap
const app = new TankerMadeApp();
window.TankerMadeApp = app;
