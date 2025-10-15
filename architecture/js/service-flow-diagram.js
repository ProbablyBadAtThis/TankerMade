function mk(go) {
    return go.GraphObject.make;
}

window.renderServiceDiagram = function renderServiceDiagram() {
    if (window._svcInited) return;
    window._svcInited = true;

    const $ = mk(go);
    const canvas = document.getElementById("serviceDiagram");

    const diagram = $(go.Diagram, "serviceDiagram", {
        "undoManager.isEnabled": false,
        layout: $(go.LayeredDigraphLayout, {
            direction: 0,
            layerSpacing: 100,
            columnSpacing: 50
        }),
        "animationManager.isEnabled": false
    });

    // Node template with service/DTO differentiation
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
                    toLinkable: true
                },
                new go.Binding("fill", "fill"),
                new go.Binding("stroke", "stroke")),
            $(go.Panel, "Vertical",
                { margin: 10 },
                $(go.TextBlock,
                    {
                        font: "bold 12px Inter, sans-serif",
                        stroke: "#333",
                        maxSize: new go.Size(120, NaN),
                        textAlign: "center"
                    },
                    new go.Binding("text", "key")),
                $(go.TextBlock,
                    {
                        font: "10px Inter, sans-serif",
                        stroke: "#666",
                        maxSize: new go.Size(120, NaN),
                        textAlign: "center"
                    },
                    new go.Binding("text", "kind"))
            )
        );

    // Link template with directional arrows
    diagram.linkTemplate =
        $(go.Link,
            {
                routing: go.Link.AvoidsNodes,
                curve: go.Link.JumpGap,
                corner: 6
            },
            $(go.Shape, { strokeWidth: 2, stroke: "#10b981" }),
            $(go.Shape, {
                toArrow: "Standard",
                fill: "#10b981",
                stroke: "#10b981",
                scale: 1.1
            })
        );

    const data = window.TM;
    const nodes = [];
    const links = [];

    function addNode(key, kind, fill, stroke) {
        nodes.push({ key, kind, fill, stroke });
    }

    function link(a, b) {
        links.push({ from: a, to: b });
    }

    // Add service nodes (green theme)
    Object.keys(data.services).forEach(svc =>
        addNode(svc, "(service)", "#dcfce7", "#16a34a")
    );

    // Add DTO nodes and create relationships
    const seen = new Set();

    function ensureDto(name, kind) {
        if (seen.has(name)) return;
        seen.add(name);

        // Color DTOs by domain
        let fill = "#dbeafe", stroke = "#2563eb";
        if (name.includes("Pattern")) { fill = "#fef3c7"; stroke = "#d97706"; }
        else if (name.includes("User")) { fill = "#fce7f3"; stroke = "#ec4899"; }

        addNode(name, kind, fill, stroke);
    }

    // Build service-DTO relationships
    Object.entries(data.services).forEach(([svc, meta]) => {
        // Input DTOs flow into services
        (meta.creates || []).forEach(dto => {
            ensureDto(dto, "(create)");
            link(dto, svc);
        });

        (meta.updates || []).forEach(dto => {
            ensureDto(dto, "(update)");
            link(dto, svc);
        });

        // Services produce output DTOs
        (meta.reads || []).forEach(dto => {
            ensureDto(dto, "(display)");
            link(svc, dto);
        });
    });

    diagram.model = new go.GraphLinksModel(nodes, links);
    canvas.classList.add('loaded');

    // Add interactivity
    diagram.addDiagramListener("ObjectSingleClicked", function (e) {
        var part = e.subject.part;
        if (part instanceof go.Node) {
            console.log("Clicked service component:", part.data.key, part.data.kind);
        }
    });
};
