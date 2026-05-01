using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class VideoNodeView : Node
{
    public VideoNodeSO NodeData { get; private set; }
    public Port InputPort { get; private set; }
    public System.Action<VideoNodeView> OnNodeSelected;

    // One output port per choice
    private VisualElement _choicesContainer;

    public VideoNodeView(VideoNodeSO node) : base()
    {
        NodeData = node;
        title = node.name;

        SetPosition(new Rect(node.graphPosition, Vector2.zero));
        viewDataKey = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(node));

        var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Assets/Editor/VideoTreeEditor/VideoNodeView.uss");
        if (uss != null) styleSheets.Add(uss);

        BuildPorts();
        BuildBody();
        RefreshExpandedState();
        RefreshPorts();

        // Color end nodes differently
        if (node.isEndNode)
            AddToClassList("end-node");
    }

    // ─── Ports ────────────────────────────────────────────────────────────────

    void BuildPorts()
    {
        // Input (one per node — receives connections from choices)
        InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
            Port.Capacity.Multi, typeof(VideoNodeSO));
        InputPort.portName = "Entrée";
        inputContainer.Add(InputPort);

        // Output ports (one per choice)
        RefreshChoicePorts();
    }

    public void RefreshChoicePorts()
    {
        // Remove existing output ports
        outputContainer.Clear();

        if (NodeData.choices == null) return;

        foreach (var choice in NodeData.choices)
        {
            if (choice == null) continue;
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output,
                Port.Capacity.Single, typeof(VideoNodeSO));
            port.portName = string.IsNullOrEmpty(choice.label) ? "Choix" : choice.label;
            port.userData = choice;
            outputContainer.Add(port);
        }

        RefreshPorts();
        RefreshExpandedState();
    }

    // ─── Body ─────────────────────────────────────────────────────────────────

    void BuildBody()
    {
        // Video clip field
        var clipLabel = new Label(NodeData.videoClip != null
            ? $"🎬 {NodeData.videoClip.name}"
            : "🎬 (aucun clip)");
        clipLabel.AddToClassList("clip-label");
        extensionContainer.Add(clipLabel);

        // Description
        if (!string.IsNullOrEmpty(NodeData.description))
        {
            var desc = new Label(NodeData.description);
            desc.AddToClassList("description-label");
            extensionContainer.Add(desc);
        }

        // End node badge
        if (NodeData.isEndNode)
        {
            var badge = new Label("FIN");
            badge.AddToClassList("end-badge");
            titleContainer.Add(badge);
        }
    }

    // ─── Selection ────────────────────────────────────────────────────────────

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    // ─── Position persistence ─────────────────────────────────────────────────

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(NodeData, "Move Video Node");
        NodeData.graphPosition = newPos.position;
        EditorUtility.SetDirty(NodeData);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    public Port GetOutputPortForChoice(ChoiceSO choice)
    {
        return outputContainer.Children()
            .OfType<Port>()
            .FirstOrDefault(p => p.userData == choice);
    }
}