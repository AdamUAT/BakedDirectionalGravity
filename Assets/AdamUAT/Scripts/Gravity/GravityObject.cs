using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.Progress;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEditor.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif

public class GravityObject : MonoBehaviour
{
    [SerializeField]
    private List<GravityData> gravityPoints = new List<GravityData>();

    public int test = 1;

    private void Update()
    {
        Debug.Log(test);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach(GravityData gravityPoint in gravityPoints)
        {
            Gizmos.DrawSphere(gravityPoint.Position, 0.1f);
            Gizmos.DrawRay(gravityPoint.Position, gravityPoint.Direction.normalized);
        }
    }

    public List<GravityData> GetGravityPoints()
    {
        return gravityPoints;
    }


    public GravityData GetGravityPoint(int index)
    {
        if(gravityPoints.Count <= index)
        {
            return null;
        }
        else
        {
            return gravityPoints[index];
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GravityObject))]
public class GravityObject_Inspector : Editor
{
    GravityObject gravityObject;

    private void OnEnable()
    {
        gravityObject = target.GetComponent<GravityObject>();
        if(gravityObject == null)
        {
            Debug.LogError("GravityObject custom inspector could not find its gravity object.");
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();

        Func<VisualElement> makeItem = () => new GravityPointDisply();
        Action<VisualElement, int> bindItem = ((visualElement, i) => 
        {
            GravityPointDisply gravityPointDisply = visualElement as GravityPointDisply;
            if(gravityPointDisply != null)
            {
                gravityPointDisply.GetDropdown().text = "Gravity Point " + i.ToString();
                //gravityPointDisply.GetPosition().bindingPath = "m_gravityPoints.Array.data[" + i + "].m_Position";
                gravityPointDisply.GetPosition().bindingPath = "m_test";
                //gravityPointDisply.GetDirection().bindingPath = "m_gravityPoints.Array.data[" + i + "].m_Direction";
                gravityPointDisply.GetDirection().bindingPath = "m_test";
                gravityPointDisply.GetPosition().Bind(new SerializedObject(gravityObject));
                gravityPointDisply.GetPosition().RegisterCallback<ChangeEvent<Vector3Field>, int>(PositionChanged, i);
            }
        });

        var listView = new ListView(gravityObject.GetGravityPoints(), -1 ,makeItem, bindItem)
        {
            selectionType = SelectionType.Single
        };

        listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        listView.showAddRemoveFooter = true;
        listView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
        listView.showBorder = true;
        listView.headerTitle = "Gravity Points";
        listView.showFoldoutHeader = true;
        listView.showBoundCollectionSize = true;
        listView.selectionChanged += objects => Debug.Log($"Selected: {string.Join(", ", objects)}");
        listView.itemsChosen += objects => Debug.Log($"Double-clicked: {string.Join(", ", objects)}");
        listView.style.flexGrow = 1.0f;
        
        myInspector.Add(listView);
        return myInspector;
    }

    private void PositionChanged(ChangeEvent<Vector3Field> field, int i)
    {
        if (field.newValue != field.previousValue)
        {
            gravityObject.GetGravityPoint(i).Position = field.newValue.value;
        }
    }

    class GravityPointDisply : VisualElement
    {
        private Foldout dropdown;
        private Vector3Field positionValue;
        private Vector3Field directionValue;
        
        public GravityPointDisply()
        {
            dropdown = new Foldout();
            dropdown.text = "";
            dropdown.value = false;
            dropdown.style.paddingBottom = 5;
            dropdown.style.paddingRight = 10;
            Add(dropdown);

            Label positionLabel = new Label();
            positionLabel.text = "Position";
            dropdown.contentContainer.Add(positionLabel);

            positionValue = new Vector3Field();
            dropdown.contentContainer.Add(positionValue);

            Label directionLabel = new Label();
            directionLabel.text = "Direction";
            dropdown.contentContainer.Add(directionLabel);

            directionValue = new Vector3Field();
            dropdown.contentContainer.Add(directionValue);
        }

        public Foldout GetDropdown()
        {
            return dropdown;
        }

        public Vector3Field GetPosition()
        {
            return positionValue;
        }

        public Vector3Field GetDirection()
        {
            return directionValue;
        }
    }
}
#endif
