function mk(go) {
    return go.GraphObject.make;
}

window.renderDataFlowDiagram = function renderDataFlowDiagram() {
    if (window._flowInited) return;
    window._flowInited = true;

    const $ = mk(go);
    const canvas = document.getElementById("dataFlowDiagram");

    const diagram = $(go.Diagram, "dataFlowDiagram", {
        "undoManager.isEnabled": false,
        layout: $(go.LayeredDigraphLayout, {
            direction: 0,
            layerSpacing: 80,
            columnSpacing: 20
        }),
        "animationManager.isEnabled": false
    });

    // Clean node template for system flow
    diagram.nodeTemplate =
        $(go.Node, "Auto",
            {
                locationSpot: go.Spot.Center,
                selectable: true
            },
            $(go.Shape, "RoundedRectangle",
                {
                    strokeWidth: 2
                },
                new go.Binding("fill", "fill"),
                new go.Binding("stroke", "stroke")),
            $(go.TextBlock,
                {
                    margin: 12,
                    stroke: "#333",
                    font: "bold 11px Inter, sans-serif",
                    textAlign: "center",
                    maxSize: new go.Size(100, NaN)
                },
                new go.Binding("text", "key"))
        );

    // System flow links
    diagram.linkTemplate =
        $(go.Link,
            {
                routing: go.Link.AvoidsNodes,
                curve: go.Link.JumpGap,
                corner: 5
            },
            $(go.Shape, { strokeWidth: 3, stroke: "#2563eb" }),
            $(go.Shape, {
                toArrow: "Standard",
                fill: "#2563eb",
                stroke: "#2563eb",
                scale: 1.5
            })
        );

    // System architecture flow
    const nodes = [
        { key: "Blazor UI", fill: "#f3f4f6", stroke: "#6b7280" },
        { key: "Input DTO", fill: "#dbeafe", stroke: "#2563eb" },
        { key: "Service Layer", fill: "#dcfce7", stroke: "#16a34a" },
        { key: "Entity/Domain", fill: "#fef3c7", stroke: "#d97706" },
        { key: "SQLite/PostgreSQL", fill: "#f1f5f9", stroke: "#475569" },
        { key: "Output DTO", fill: "#dbeafe", stroke: "#2563eb" },
        { key: "UI Display", fill: "#f3f4f6", stroke: "#6b7280" }
    ];

    const links = [
        { from: "Blazor UI", to: "Input DTO" },
        { from: "Input DTO", to: "Service Layer" },
        { from: "Service Layer", to: "Entity/Domain" },
        { from: "Entity/Domain", to: "SQLite/PostgreSQL" },
        { from: "Service Layer", to: "Output DTO" },
        { from: "Output DTO", to: "UI Display" }
    ];

    diagram.model = new go.GraphLinksModel(nodes, links);
    canvas.classList.add('loaded');
};
