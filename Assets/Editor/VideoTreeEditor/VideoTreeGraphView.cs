using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class VideoTreeGraphView : GraphView
{
    private readonly VideoTreeEditorWindow _window;
    private readonly Dictionary<VideoNodeSO, VideoNodeView> _nodeViews = new();

    public System.Action<VideoNodeSO> OnNodeSelected;

    public VideoTreeGraphView(VideoTreeEditorWindow window)
    {
        _window = window;

        // Navigation
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Grid background
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // Stylesheet (optional)
        var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Assets/Editor/VideoTreeEditor/VideoTreeGraphView.uss");
        if (uss != null) styleSheets.Add(uss);

        // Context menu to create nodes
        this.AddManipulator(CreateContextualMenu());

        // Listen to graph changes (edge connect/disconnect)
        graphViewChanged += OnGraphViewChanged;
    }

    // ─── Load ─────────────────────────────────────────────────────────────────

    public void LoadTree(VideoNodeSO rootNode)
    {
        ClearGraph();
        if (rootNode == null) return;

        // Collect all nodes reachable from root
        var visited = new HashSet<VideoNodeSO>();
        CollectNodes(rootNode, visited);

        // Create views
        foreach (var node in visited)
            CreateNodeView(node);

        // Create edges
        foreach (var node in visited)
        {
            if (node.choices == null) continue;
            foreach (var choice in node.choices)
            {
                if (choice?.nextNode == null) continue;
                if (!_nodeViews.TryGetValue(node, out var fromView)) continue;
                if (!_nodeViews.TryGetValue(choice.nextNode, out var toView)) continue;

                var outPort = fromView.GetOutputPortForChoice(choice);
                var inPort = toView.InputPort;

                if (outPort != null && inPort != null)
                {
                    var edge = outPort.ConnectTo(inPort);
                    AddElement(edge);
                }
            }
        }
    }

    void CollectNodes(VideoNodeSO node, HashSet<VideoNodeSO> visited)
    {
        if (node == null || visited.Contains(node)) return;
        visited.Add(node);
        if (node.choices == null) return;
        foreach (var c in node.choices)
            if (c?.nextNode != null)
                CollectNodes(c.nextNode, visited);
    }

    void ClearGraph()
    {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        _nodeViews.Clear();
        graphViewChanged += OnGraphViewChanged;
    }

    // ─── Node creation ────────────────────────────────────────────────────────

    VideoNodeView CreateNodeView(VideoNodeSO node)
    {
        var view = new VideoNodeView(node);
        view.OnNodeSelected = v => OnNodeSelected?.Invoke(v.NodeData);
        _nodeViews[node] = view;
        AddElement(view);
        return view;
    }

    VideoNodeSO CreateNewNode(Vector2 graphPosition)
    {
        var node = ScriptableObject.CreateInstance<VideoNodeSO>();
        node.name = "VideoNode";
        node.graphPosition = graphPosition;

        // Save next to window's root asset, or in a default folder
        var path = GetSavePath("VideoNode");
        AssetDatabase.CreateAsset(node, path);
        AssetDatabase.SaveAssets();

        Undo.RegisterCreatedObjectUndo(node, "Create Video Node");
        CreateNodeView(node);
        return node;
    }

    string GetSavePath(string assetName, string subFolder = "VideoNodes")
    {
        var root = _window.RootNode;
        var baseFolder = root != null
            ? System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(root))
            : "Assets";

        var folder = $"{baseFolder}/{subFolder}";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder(baseFolder, subFolder);

        return AssetDatabase.GenerateUniqueAssetPath($"{folder}/{assetName}.asset");
    }

    // ─── Graph changes ────────────────────────────────────────────────────────

    GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        // Edge created → wire ChoiceSO.nextNode
        if (change.edgesToCreate != null)
        {
            foreach (var edge in change.edgesToCreate)
            {
                var choice = edge.output.userData as ChoiceSO;
                var toView = edge.input.node as VideoNodeView;
                if (choice != null && toView != null)
                {
                    Undo.RecordObject(choice, "Connect Choice");
                    choice.nextNode = toView.NodeData;
                    EditorUtility.SetDirty(choice);
                }
            }
        }

        // Edge removed → clear ChoiceSO.nextNode
        if (change.elementsToRemove != null)
        {
            foreach (var elem in change.elementsToRemove)
            {
                if (elem is Edge edge)
                {
                    var choice = edge.output?.userData as ChoiceSO;
                    if (choice != null)
                    {
                        Undo.RecordObject(choice, "Disconnect Choice");
                        choice.nextNode = null;
                        EditorUtility.SetDirty(choice);
                    }
                }
            }
        }

        return change;
    }

    // ─── Compatibility ────────────────────────────────────────────────────────

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.Where(p =>
            p.direction != startPort.direction &&
            p.node != startPort.node
        ).ToList();
    }

    // ─── Context menu ─────────────────────────────────────────────────────────

    IManipulator CreateContextualMenu()
    {
        return new ContextualMenuManipulator(evt =>
        {
            evt.menu.AppendAction("Créer un nœud vidéo", action =>
            {
                var worldPos = (action.eventInfo.localMousePosition);
                var graphPos = contentViewContainer.WorldToLocal(worldPos);
                CreateNewNode(graphPos);
            });
        });
    }

    // ─── Auto-layout ──────────────────────────────────────────────────────────

    public void AutoLayout(VideoNodeSO root)
    {
        if (root == null) return;
        var visited = new Dictionary<VideoNodeSO, int>(); // node → depth
        AssignDepths(root, 0, visited);

        var columns = new Dictionary<int, List<VideoNodeSO>>();
        foreach (var kv in visited)
        {
            if (!columns.ContainsKey(kv.Value)) columns[kv.Value] = new List<VideoNodeSO>();
            columns[kv.Value].Add(kv.Key);
        }

        const float colWidth = 280f;
        const float rowHeight = 160f;
        const float startX = 60f;
        const float startY = 60f;

        foreach (var col in columns)
        {
            for (int i = 0; i < col.Value.Count; i++)
            {
                var node = col.Value[i];
                var pos = new Vector2(startX + col.Key * colWidth, startY + i * rowHeight);
                if (_nodeViews.TryGetValue(node, out var view))
                    view.SetPosition(new Rect(pos, Vector2.zero));
            }
        }
    }

    void AssignDepths(VideoNodeSO node, int depth, Dictionary<VideoNodeSO, int> visited)
    {
        if (visited.ContainsKey(node)) return;
        visited[node] = depth;
        if (node.choices == null) return;
        foreach (var c in node.choices)
            if (c?.nextNode != null)
                AssignDepths(c.nextNode, depth + 1, visited);
    }
}