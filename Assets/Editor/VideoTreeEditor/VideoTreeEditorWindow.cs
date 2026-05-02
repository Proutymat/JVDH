using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class VideoTreeEditorWindow : EditorWindow
{
    public VideoNodeSO RootNode { get; private set; }

    private VideoTreeGraphView _graphView;
    private InspectorPanel _inspectorPanel;
    private Label _titleLabel;

    // ─── Open ─────────────────────────────────────────────────────────────────

    [MenuItem("Tools/Video Decision Tree")]
    public static void Open()
    {
        var window = GetWindow<VideoTreeEditorWindow>();
        window.titleContent = new GUIContent("Video Decision Tree");
        window.minSize = new Vector2(900, 600);
    }

    // Double-click on a VideoNodeSO asset opens the editor
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (EditorUtility.InstanceIDToObject(instanceId) is VideoNodeSO node)
        {
            var window = GetWindow<VideoTreeEditorWindow>();
            window.titleContent = new GUIContent("Video Decision Tree");
            window.SetRootNode(node);
            return true;
        }
        return false;
    }

    // ─── Build UI ─────────────────────────────────────────────────────────────

    void CreateGUI()
    {
        var root = rootVisualElement;

        // Toolbar
        var toolbar = new VisualElement();
        toolbar.AddToClassList("toolbar");
        root.Add(toolbar);

        // Root node picker
        var rootLabel = new Label("Nœud racine :");
        rootLabel.AddToClassList("toolbar-label");
        toolbar.Add(rootLabel);

        var rootField = new UnityEditor.UIElements.ObjectField();
        rootField.objectType = typeof(VideoNodeSO);
        rootField.style.width = 220;
        rootField.value = RootNode;
        rootField.RegisterValueChangedCallback(evt =>
            SetRootNode(evt.newValue as VideoNodeSO));
        toolbar.Add(rootField);

        // Spacer
        var spacer = new VisualElement();
        spacer.style.flexGrow = 1;
        toolbar.Add(spacer);

        // Auto-layout button
        var layoutBtn = new Button(() =>
        {
            _graphView?.AutoLayout(RootNode);
        }) { text = "Auto-layout" };
        layoutBtn.AddToClassList("toolbar-btn");
        toolbar.Add(layoutBtn);

        // Refresh button
        var refreshBtn = new Button(() =>
        {
            _graphView?.LoadTree(RootNode);
        }) { text = "Recharger" };
        refreshBtn.AddToClassList("toolbar-btn");
        toolbar.Add(refreshBtn);

        // Main area: inspector + graph side by side
        var mainArea = new VisualElement();
        mainArea.style.flexDirection = FlexDirection.Row;
        mainArea.style.flexGrow = 1;
        root.Add(mainArea);

        // Inspector panel (left, fixed width)
        _inspectorPanel = new InspectorPanel();
        _inspectorPanel.style.width = 280;
        _inspectorPanel.style.minWidth = 280;
        _inspectorPanel.style.maxWidth = 280;
        _inspectorPanel.style.borderRightWidth = 1;
        _inspectorPanel.style.borderRightColor = new Color(0, 0, 0, 0.4f);
        mainArea.Add(_inspectorPanel);

        // Graph (right, takes remaining space)
        _graphView = new VideoTreeGraphView(this);
        _graphView.style.flexGrow = 1;
        _graphView.style.flexShrink = 1;
        _graphView.OnNodeSelected = node => _inspectorPanel.ShowNode(node, _graphView);
        mainArea.Add(_graphView);

        if (RootNode != null)
            _graphView.LoadTree(RootNode);
    }

    public void SetRootNode(VideoNodeSO node)
    {
        RootNode = node;
        _graphView?.LoadTree(node);
    }
}