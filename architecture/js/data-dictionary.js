function badge(control) {
  const cls = control === "user" ? "user" :
              control === "system" ? "system" :
              control === "admin" ? "admin" : "computed";
  const label = control.charAt(0).toUpperCase() + control.slice(1);
  return `<span class="badge ${cls}">${label}</span>`;
}

function matches(text, q) {
  return (text || "").toLowerCase().includes(q);
}

window.renderDataDictionary = function renderDataDictionary() {
  if (window._dictInited) {
    // reapply filter on revisit
    applyDictFilters();
    return;
  }
  window._dictInited = true;

  const search = document.getElementById("dictSearch");
  const control = document.getElementById("dictControlFilter");
  search.addEventListener("input", applyDictFilters);
  control.addEventListener("change", applyDictFilters);

  applyDictFilters();
};

function applyDictFilters() {
  const body = document.getElementById("dictBody");
  const q = (document.getElementById("dictSearch").value || "").toLowerCase();
  const ctl = document.getElementById("dictControlFilter").value;

  const rows = window.TM.fields
    .filter(f => (ctl ? f.control === ctl : true))
    .filter(f =>
      matches(f.name, q) ||
      matches(f.type, q) ||
      matches(f.purpose, q) ||
      matches((f.usedIn || []).join(", "), q)
    )
    .map(f => `
      <tr>
        <td><code>${f.name}</code></td>
        <td>${f.type}</td>
        <td>${badge(f.control)}</td>
        <td>${f.purpose}</td>
        <td>${(f.usedIn || []).map(u => `<code>${u}</code>`).join(", ")}</td>
        <td>${f.validation || ""}</td>
        <td>${f.security || ""}</td>
      </tr>
    `)
    .join("");

  body.innerHTML = rows || `<tr><td colspan="7" style="color:#aeb3be">No fields match your filters.</td></tr>`;
}
