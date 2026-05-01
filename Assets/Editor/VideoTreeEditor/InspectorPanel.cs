using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Video;

public class InspectorPanel : VisualElement
{
    private VideoNodeSO _currentNode;
    private VideoTreeGraphView _graphView;
    private ScrollView _scroll;

    public InspectorPanel()
    {
        AddToClassList("inspector-panel");
        style.width = 300;
        style.minWidth = 220;

        var header = new Label("Inspecteur");
        header.AddToClassList("inspector-header");
        Add(header);

        _scroll = new ScrollView();
        _scroll.style.flexGrow = 1;
        Add(_scroll);

        ShowEmpty();
    }

    void ShowEmpty()
    {
        _scroll.Clear();
        var hint = new Label("Sélectionne un nœud\npour l'éditer.");
        hint.AddToClassList("inspector-hint");
        _scroll.Add(hint);
    }

    public void ShowNode(VideoNodeSO node, VideoTreeGraphView graphView)
    {
        _currentNode = node;
        _graphView = graphView;
        _scroll.Clear();

        if (node == null) { ShowEmpty(); return; }

        // Name
        AddSection("Nœud");
        AddTextField("Nom", node.name, v =>
        {
            Undo.RecordObject(node, "Rename Node");
            node.name = v;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(node), v);
            EditorUtility.SetDirty(node);
        });

        // Video clip
        AddSection("Vidéo");
        var clipField = new ObjectField("Clip vidéo");
        clipField.objectType = typeof(VideoClip);
        clipField.value = node.videoClip;
        clipField.RegisterValueChangedCallback(evt =>
        {
            Undo.RecordObject(node, "Set Video Clip");
            node.videoClip = evt.newValue as VideoClip;
            EditorUtility.SetDirty(node);
        });
        _scroll.Add(clipField);

        var descField = new TextField("Description") { multiline = true };
        descField.value = node.description;
        descField.style.height = 60;
        descField.RegisterValueChangedCallback(evt =>
        {
            Undo.RecordObject(node, "Set Description");
            node.description = evt.newValue;
            EditorUtility.SetDirty(node);
        });
        _scroll.Add(descField);

        // End node toggle
        var endToggle = new Toggle("Nœud de fin");
        endToggle.value = node.isEndNode;
        endToggle.RegisterValueChangedCallback(evt =>
        {
            Undo.RecordObject(node, "Toggle End Node");
            node.isEndNode = evt.newValue;
            EditorUtility.SetDirty(node);
        });
        _scroll.Add(endToggle);

        // Choices
        AddSection("Choix");
        RefreshChoices(node);

        var addChoiceBtn = new Button(() => AddChoice(node)) { text = "+ Ajouter un choix" };
        addChoiceBtn.AddToClassList("add-btn");
        _scroll.Add(addChoiceBtn);

        // Open in Project button
        var openBtn = new Button(() => EditorGUIUtility.PingObject(node))
            { text = "Localiser dans Project" };
        openBtn.AddToClassList("locate-btn");
        _scroll.Add(openBtn);
    }

    void RefreshChoices(VideoNodeSO node)
    {
        // Remove existing choice rows (keep header & buttons)
        var toRemove = new List<VisualElement>();
        foreach (var child in _scroll.Children())
            if (child.ClassListContains("choice-row"))
                toRemove.Add(child);
        foreach (var c in toRemove) _scroll.Remove(c);

        if (node.choices == null) return;

        for (int i = 0; i < node.choices.Count; i++)
        {
            var idx = i;
            var choice = node.choices[i];
            if (choice == null) continue;

            var row = new VisualElement();
            row.AddToClassList("choice-row");

            // Label field
            var labelField = new TextField($"Choix {i + 1}");
            labelField.value = choice.label;
            labelField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(choice, "Edit Choice Label");
                choice.label = evt.newValue;
                EditorUtility.SetDirty(choice);
                _graphView?.LoadTree(_currentNode); // refresh port labels
            });
            row.Add(labelField);

            // Delete button
            var deleteBtn = new Button(() =>
            {
                Undo.RecordObject(node, "Remove Choice");
                node.choices.RemoveAt(idx);
                EditorUtility.SetDirty(node);
                ShowNode(node, _graphView);
                _graphView?.LoadTree(_currentNode);
            }) { text = "✕" };
            deleteBtn.AddToClassList("delete-btn");
            row.Add(deleteBtn);

            _scroll.Add(row);
        }
    }

    void AddChoice(VideoNodeSO node)
    {
        var choice = ScriptableObject.CreateInstance<ChoiceSO>();
        choice.name = $"Choice_{node.name}_{node.choices.Count}";

        var nodePath = AssetDatabase.GetAssetPath(node);
        var baseFolder = System.IO.Path.GetDirectoryName(nodePath);
        var choicesFolder = $"{baseFolder}/Choices";
        if (!AssetDatabase.IsValidFolder(choicesFolder))
            AssetDatabase.CreateFolder(baseFolder, "Choices");
        var path = AssetDatabase.GenerateUniqueAssetPath($"{choicesFolder}/{choice.name}.asset");
        AssetDatabase.CreateAsset(choice, path);

        Undo.RecordObject(node, "Add Choice");
        node.choices.Add(choice);
        EditorUtility.SetDirty(node);
        AssetDatabase.SaveAssets();

        ShowNode(node, _graphView);
        _graphView?.LoadTree(_currentNode);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    void AddSection(string title)
    {
        var label = new Label(title);
        label.AddToClassList("section-title");
        _scroll.Add(label);
    }

    void AddTextField(string label, string value, System.Action<string> onChange)
    {
        var field = new TextField(label) { value = value };
        field.RegisterValueChangedCallback(evt => onChange(evt.newValue));
        _scroll.Add(field);
    }
}