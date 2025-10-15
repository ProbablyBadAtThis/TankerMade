function mk(go) {
  return go.GraphObject.make;
}

window.renderDtoDiagram = function renderDtoDiagram() {
  if (window._dtoInited) return;
  window._dtoInited = true;

  const $ = mk(go);

  const diagram = $(go.Diagram, "dtoDiagram", {
    "undoManager.isEnabled": false,
    layout: $(go.LayeredDigraphLayout, { direction: 0, layerSpacing: 50 })
  });

  diagram.nodeTemplate =
    $(go.Node, "Auto",
      $(go.Shape, "RoundedRectangle",
        new go.Binding("fill", "fill"),
        { stroke: "#2b2f3a", strokeWidth: 1, portId: "", fromLinkable: true, toLinkable: true }),
      $(go.Panel, "Vertical",
        { margin: 8 },
        $(go.TextBlock, { font: "bold 12px sans-serif", stroke: "#e7e9ee" },
          new go.Binding("text", "key")),
        $(go.TextBlock, { font: "11px sans-serif", stroke: "#aeb3be" },
          new go.Binding("text", "kind"))
      )
    );

  diagram.linkTemplate =
    $(go.Link,
      { routing: go.Link.AvoidsNodes, curve: go.Link.JumpGap, corner: 6 },
      $(go.Shape, { stroke: "#3a6ea5" }),
      $(go.Shape, { toArrow: "Standard", fill: "#3a6ea5", stroke: "#3a6ea5" })
    );

  const data = window.TM;
  const nodes = [];
  const links = [];

  function addDtoNode(name, kind) {
    nodes.push({ key: name, kind, fill: "#14213d" });
  }
  function link(a, b) { links.push({ from: a, to: b }); }

  // Projects
  addDtoNode(data.dtos.projects.display, "(display)");
  addDtoNode(data.dtos.projects.create, "(create)");
  addDtoNode(data.dtos.projects.update, "(update)");

  // Patterns
  addDtoNode(data.dtos.patterns.display, "(display)");
  addDtoNode(data.dtos.patterns.create, "(create)");
  addDtoNode(data.dtos.patterns.update, "(update)");

  // Users
  addDtoNode(data.dtos.users.display, "(display)");
  addDtoNode(data.dtos.users.create, "(create)");
  addDtoNode(data.dtos.users.update, "(update)");

  // Simple logical links: create/update → display per domain
  link(data.dtos.projects.create, data.dtos.projects.display);
  link(data.dtos.projects.update, data.dtos.projects.display);

  link(data.dtos.patterns.create, data.dtos.patterns.display);
  link(data.dtos.patterns.update, data.dtos.patterns.display);

  link(data.dtos.users.create, data.dtos.users.display);
  link(data.dtos.users.update, data.dtos.users.display);

  diagram.model = new go.GraphLinksModel(nodes, links);
};
