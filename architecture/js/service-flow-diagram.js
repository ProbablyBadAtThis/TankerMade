function mk(go) { return go.GraphObject.make; }

window.renderServiceDiagram = function renderServiceDiagram() {
  if (window._svcInited) return;
  window._svcInited = true;

  const $ = mk(go);

  const diagram = $(go.Diagram, "serviceDiagram", {
    "undoManager.isEnabled": false,
    layout: $(go.LayeredDigraphLayout, { direction: 0, layerSpacing: 60 })
  });

  diagram.nodeTemplate =
    $(go.Node, "Auto",
      $(go.Shape, "RoundedRectangle",
        new go.Binding("fill", "fill"),
        { stroke: "#2b2f3a", strokeWidth: 1, portId: "", fromLinkable: true, toLinkable: true }),
      $(go.Panel, "Vertical", { margin: 8 },
        $(go.TextBlock, { font: "bold 12px sans-serif", stroke: "#e7e9ee" },
          new go.Binding("text", "key")),
        $(go.TextBlock, { font: "11px sans-serif", stroke: "#aeb3be" },
          new go.Binding("text", "kind"))
      )
    );

  diagram.linkTemplate =
    $(go.Link, { routing: go.Link.AvoidsNodes, curve: go.Link.JumpGap, corner: 6 },
      $(go.Shape, { stroke: "#2aa198" }),
      $(go.Shape, { toArrow: "Standard", fill: "#2aa198", stroke: "#2aa198" })
    );

  const data = window.TM;
  const nodes = [];
  const links = [];

  function addNode(key, kind, fill) {
    nodes.push({ key, kind, fill });
  }
  function link(a, b) { links.push({ from: a, to: b }); }

  // Add service nodes
  Object.keys(data.services).forEach(svc => addNode(svc, "(service)", "#123524"));

  // Add DTO nodes and link them to services
  const dtoFill = "#14213d";
  const seen = new Set();

  function ensureDto(name, kind) {
    if (seen.has(name)) return;
    seen.add(name);
    addNode(name, kind, dtoFill);
  }

  Object.entries(data.services).forEach(([svc, meta]) => {
    (meta.creates || []).forEach(dto => { ensureDto(dto, "(create)"); link(dto, svc); });
    (meta.updates || []).forEach(dto => { ensureDto(dto, "(update)"); link(dto, svc); });
    (meta.reads || []).forEach(dto => { ensureDto(dto, "(display)"); link(svc, dto); });
  });

  diagram.model = new go.GraphLinksModel(nodes, links);
};
