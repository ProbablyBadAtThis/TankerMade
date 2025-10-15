function mk(go) {
    return go.GraphObject.make;
}

window.renderDtoDiagram = function renderDtoDiagram() {
    if (window._dtoInited) return;
    window._dtoInited = true;

    const $ = mk(go);
    const canvas = document.getElementById("dtoDiagram");

    const diagram = $(go.Diagram, "dtoDiagram", {
        "undoManager.isEnabled": false,
        layout: $(go.LayeredDigraphLayout, {
            direction: 0,
            layerSpacing: 80,
            columnSpacing: 40,
            setsPortSpots: false
        }),
        "animationManager.isEnabled": false
    });

    // Custom node template with better styling
    diagram.nodeTemplate =
        $(go.Node, "Auto",
            {
                locationSpot: go.Spot.Center,
                selectable: true,
                cursor: "pointer"
            },
            $(go.Shape, "RoundedRectangle",
                {
                    strokeWidth: 2,
                    portId: "",
                    fromLinkable: true,
                    toLinkable: true,
                    cursor: "pointer"
                },
                new go.Binding("fill", "fill"),
                new go.Binding("stroke", "stroke")),
            $(go.Panel, "Vertical",
                { margin: 12 },
                $(go.TextBlock,
                    {
                        font: "bold 13px Inter, sans-serif",
                        stroke: "#333",
                        maxSize: new go.Size(140, NaN),
                        textAlign: "center"
                    },
                    new go.Binding("text", "key")),
                $(go.TextBlock,
                    {
                        font: "11px Inter, sans-serif",
                        stroke: "#666",
                        maxSize: new go.Size(140, NaN),
                        textAlign: "center"
                    },
                    new go.Binding("text", "kind"))
            )
        );

    // Custom link template
    diagram.linkTemplate =
        $(go.Link,
            {
                routing: go.Link.AvoidsNodes,
                curve: go.Link.JumpGap,
                corner: 8,
                selectable: true,
                cursor: "pointer"
            },
            $(go.Shape, { strokeWidth: 2, stroke: "#2563eb" }),
            $(go.Shape, {
                toArrow: "Standard",
                fill: "#2563eb",
                stroke: "#2563eb",
                scale: 1.2
            })
        );

    // Build the data model
    const data = window.TM;
    const nodes = [];
    const links = [];

    function addDtoNode(name, kind, domain) {
        const colors = {
            "projects": { fill: "#dbeafe", stroke: "#2563eb" },
            "patterns": { fill: "#dcfce7", stroke: "#16a34a" },
            "users": { fill: "#fef3c7", stroke: "#d97706" }
        };

        nodes.push({
            key: name,
            kind: kind,
            fill: colors[domain]?.fill || "#f3f4f6",
            stroke: colors[domain]?.stroke || "#6b7280"
        });
    }

    function link(a, b) {
        links.push({ from: a, to: b });
    }

    // Add all DTOs with domain-based coloring
    Object.entries(data.dtos).forEach(([domain, dtos]) => {
        addDtoNode(dtos.display, "(display)", domain);
        addDtoNode(dtos.create, "(create)", domain);
        addDtoNode(dtos.update, "(update)", domain);

        // Create logical relationships
        link(dtos.create, dtos.display);
        link(dtos.update, dtos.display);
    });

    // Set model and mark as loaded
    diagram.model = new go.GraphLinksModel(nodes, links);
    canvas.classList.add('loaded');

    // Add click handler for interactivity
    diagram.addDiagramListener("ObjectSingleClicked", function (e) {
        var part = e.subject.part;
        if (part instanceof go.Node) {
            console.log("Clicked DTO:", part.data.key);
        }
    });
};
