function mk(go) { return go.GraphObject.make; }

window.renderDataFlowDiagram = function renderDataFlowDiagram() {
  if (window._flowInited) return;
  window._flowInited = true;

  const $ = mk(go);
  const d = $(go.Diagram, "dataFlowDiagram", {
    "undoManager.isEnabled": false,
    layout: $(go.LayeredDigraphLayout, { direction: 0, layerSpacing: 70 })
  });

  d.nodeTemplate =
    $(go.Node, "Auto",
      $(go.Shape, "RoundedRectangle",
        new go.Binding("fill", "fill"),
        { stroke: "#2b2f3a", strokeWidth: 1 }),
      $(go.TextBlock, { margin: 10, stroke: "#e7e9ee", font: "bold 12px sans-serif" },
        new go.Binding("text", "key"))
    );

  d.linkTemplate =
    $(go.Link, { routing: go.Link.AvoidsNodes, curve: go.Link.JumpGap, corner: 6 },
      $(go.Shape, { stroke: "#8892b0" }),
      $(go.Shape, { toArrow: "Standard", fill: "#8892b0", stroke: "#8892b0" })
    );

  const nodes = [
    { key: "UI (Client)", fill: "#263238" },
    { key: "DTO (Input)", fill: "#14213d" },
    { key: "Service Layer", fill: "#123524" },
    { key: "Entity/Domain", fill: "#3a2a18" },
    { key: "Database", fill: "#1f2430" },
    { key: "DTO (Output)", fill: "#14213d" },
    { key: "UI (Client View)", fill: "#263238" }
  ];

  const links = [
    { from: "UI (Client)", to: "DTO (Input)" },
    { from: "DTO (Input)", to: "Service Layer" },
    { from: "Service Layer", to: "Entity/Domain" },
    { from: "Entity/Domain", to: "Database" },
    { from: "Service Layer", to: "DTO (Output)" },
    { from: "DTO (Output)", to: "UI (Client View)" }
  ];

  d.model = new go.GraphLinksModel(nodes, links);
};
