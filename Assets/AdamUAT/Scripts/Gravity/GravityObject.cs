using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine.TestTools;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif

public class GravityObject : MonoBehaviour
{
    [SerializeField]
    private List<GravityData> gravityPoints = new List<GravityData>();

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

    //Will automatically populate a preset of gravity points for regular objects like a cube or sphere.
    public void ApplyPreset(GravityManager.RegularShapes shape, GravityManager.CoveragePresets coverage, float density, List<float> dimensions)
    {
        //Find how many variables are needed for the preset.
        int dimensionsCount = 0;
        switch(shape)
        {
            case GravityManager.RegularShapes.Sphere:
                dimensionsCount = 1;
                break;
            case GravityManager.RegularShapes.Cube:
                dimensionsCount = 1;
                break;
            case GravityManager.RegularShapes.Box:
                dimensionsCount = 3;
                break;
            default:
                Debug.LogError("Enum went out of scope for switch statement in function ApplyPreset in GravityObject " + gameObject.name + "\nCouldn't find the expected number of parameters. Preset canceled.");
                return;

        }

        //Make sure the list only contains the values for the specified preset
        if(dimensions.Count !=  dimensionsCount)
        {
            Debug.LogError("An incorrect number of variables were passed in to function ApplyPreset in GravityObject " + gameObject.name + "\nOnly " + dimensionsCount + " were expected, " + dimensions.Count + " recieved.\nPreset canceled.");
            return;
        }

        //Call the function for the shape.
        switch (shape)
        {
            case GravityManager.RegularShapes.Sphere:
                PresetSphere(coverage, density, dimensions[0]);
                break;
            case GravityManager.RegularShapes.Cube:
                PresetCube(coverage, density, dimensions[0]);
                break;
            case GravityManager.RegularShapes.Box:
                PresetBox(coverage, density, dimensions[0], dimensions[1], dimensions[2]);
                break;
            default:
                Debug.LogError("Enum went out of scope for switch statement in function ApplyPreset in GravityObject " + gameObject.name + "\nCouldn't find the associated preset. Preset canceled.");
                return;

        }
    }

    private void PresetSphere(GravityManager.CoveragePresets coverage, float density, float radius)
    {
        Debug.Log("Sphere");
    }

    private void PresetCube(GravityManager.CoveragePresets coverage, float density, float edgeLength)
    {
        Debug.Log("Cube");
    }

    private void PresetBox(GravityManager.CoveragePresets coverage, float density, float xEdgeLength, float yEdgeLength, float zEdgeLength)
    {
        Debug.Log("Box");
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

        #region ListView
        Func<VisualElement> makeGravityPointDisplyItem = () => new GravityPointDisply();
        Action<VisualElement, int> bindGravityPointDisplyItem = ((visualElement, i) => 
        {
            GravityPointDisply gravityPointDisply = visualElement as GravityPointDisply;
            if(gravityPointDisply != null)
            {
                gravityPointDisply.GetDropdown().text = "Gravity Point " + i.ToString();
                gravityPointDisply.GetPosition().bindingPath = "gravityPoints.Array.data[" + i + "].Position";
                gravityPointDisply.GetDirection().bindingPath = "gravityPoints.Array.data[" + i + "].Direction";
                gravityPointDisply.Bind(new SerializedObject(gravityObject));
            }
        });

        var listView = new ListView(gravityObject.GetGravityPoints(), -1 , makeGravityPointDisplyItem, bindGravityPointDisplyItem)
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
        #endregion ListView

        #region Presets
        PresetDisplay presetDisplay = new PresetDisplay();

        //Bind the buttons to the functions
        presetDisplay.onCreateSpherePreset.AddListener((float radius) =>
        {
            gravityObject.ApplyPreset(GravityManager.RegularShapes.Sphere, presetDisplay.GetCoverage(), presetDisplay.GetDensity(), new List<float>() { radius });
        });


        myInspector.Add(presetDisplay);
        #endregion Presets

        return myInspector;
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

    class PresetDisplay : VisualElement
    {
        private EnumField coverage;
        private FloatField density;
        public UnityEvent<float> onCreateSpherePreset = new UnityEvent<float>();

        public PresetDisplay()
        {
            Foldout dropdown = new Foldout();
            dropdown.text = "Presets";
            dropdown.value = false;
            dropdown.style.paddingBottom = 5;
            dropdown.style.paddingRight = 10;
            Add(dropdown);

            #region Settings
            VisualElement settings = new VisualElement();
            settings.style.flexDirection = FlexDirection.Row;
            dropdown.contentContainer.Add(settings);
            settings.style.flexWrap = Wrap.Wrap;

            #region Coverage
            VisualElement coverageSettings = new VisualElement();
            settings.contentContainer.Add(coverageSettings);
            coverageSettings.style.flexDirection = FlexDirection.Row;

            Label coverageLabel = new Label();
            coverageLabel.text = "Coverage: ";
            coverageSettings.contentContainer.Add(coverageLabel);
            coverageLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            coverage = new EnumField();
            coverageSettings.contentContainer.Add(coverage);
            coverage.Init(GravityManager.CoveragePresets.Standard);
            coverage.style.height = 20;
            coverage.style.minWidth = 100;
            coverage.style.maxWidth = 150;
            coverage.style.marginRight = 20;

            #endregion Coverage
            
            #region Density
            VisualElement densitySettings = new VisualElement();
            settings.contentContainer.Add(densitySettings);
            densitySettings.style.flexDirection = FlexDirection.Row;

            Label densityLabel = new Label();
            densityLabel.text = "Density: ";
            densitySettings.contentContainer.Add(densityLabel);
            densityLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            density = new FloatField();
            densitySettings.contentContainer.Add(density);
            density.style.height = 20;
            density.style.minWidth = 50;
            density.style.maxWidth = 100;

            #endregion Density

            #endregion Settings

            #region Buttons
            
            VisualElement presets = new VisualElement();
            dropdown.contentContainer.Add(presets);

            #region Sphere
            Foldout sphereDropdown = new Foldout();
            sphereDropdown.text = "Sphere";
            sphereDropdown.value = false;
            sphereDropdown.style.paddingBottom = 5;
            sphereDropdown.style.paddingRight = 10;
            presets.contentContainer.Add(sphereDropdown);

            VisualElement sphereParameters = new VisualElement();
            sphereDropdown.contentContainer.Add(sphereParameters);
            sphereParameters.style.flexDirection = FlexDirection.Row;

            Label sphereRadiusLabel = new Label();
            sphereRadiusLabel.text = "Radius: ";
            sphereParameters.contentContainer.Add(sphereRadiusLabel);
            sphereRadiusLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            FloatField sphereRadius = new FloatField();
            sphereParameters.contentContainer.Add(sphereRadius);
            sphereRadius.style.height = 20;
            sphereRadius.style.minWidth = 50;
            sphereRadius.style.maxWidth = 100;

            Action onSphereButtonClicked = () => { CreateSpherePreset(sphereRadius.value); };
            Button sphereButton = new Button(onSphereButtonClicked);
            sphereDropdown.contentContainer.Add(sphereButton);
            sphereButton.text = "Apply Sphere Preset";

            #endregion Sphere

            #endregion Buttons
        }

        private void CreateSpherePreset(float radius)
        {
            onCreateSpherePreset?.Invoke(radius);
        }

        public float GetDensity()
        {
            return density.value;
        }

        public GravityManager.CoveragePresets GetCoverage()
        {
            return (GravityManager.CoveragePresets)coverage.value;
        }
    }
}
#endif
