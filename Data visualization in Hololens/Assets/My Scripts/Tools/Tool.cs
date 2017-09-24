// Copyright Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using System.Collections.Generic;
using Assets.My_Scripts;
using Assets.My_Scripts.Tools;

public enum ToolType
{
    Pan,
    Rotate,
    Zoom,
    Reset,
    About
}

public class Tool : GazeSelectionTarget {

    public ToolManager ToolMangerInstance;
    public ToolSounds ToolSoundsInstance;
    public GazeGestureManager GazeGestureManager;

    private static bool hasGaze = false;

   // public GameObject Content;

    public GameObject TooltipObject;
    public Material DefaultMaterial;
    private Dictionary<string, float> defaultMaterialDefaults = new Dictionary<string, float>();
    public Material HighlightMaterial;
    private Dictionary<string, float> highlightMaterialDefaults = new Dictionary<string, float>();
    public Material SelectedMaterial;
    private Dictionary<string, float> selectedMaterialDefaults = new Dictionary<string, float>();
    public ToolType type;
    public float PanSpeed = 0.25f;
    public float RotationSpeed = 30.0f;
    public float ScaleSpeed = 1.0f;
    public float PanControllerSpeed = 0.75f;
    public float PanHandSpeed = 2.0f;

    public float MaxRotationAngle = 40;
    public float ClickerRotationSpeed = .1f;

    private float scalePercentValue = 0.6f;
    private GameObject contentToManipulate;
    private bool selected = false;
    private MeshRenderer meshRenderer;

    private float currentOpacity = 1;

    public float Opacity
    {
        get
        {
            return currentOpacity;
        }

        set
        {
            currentOpacity = value;

            ApplyOpacity(DefaultMaterial, value);
            ApplyOpacity(HighlightMaterial, value);
            ApplyOpacity(SelectedMaterial, value);
        }
    }

    private void ApplyOpacity(Material material, float value)
    {
        value = Mathf.Clamp01(value);

        if (material)
        {
            material.SetFloat("_TransitionAlpha", value);
            material.SetInt("_SRCBLEND", value < 1 ? (int)UnityEngine.Rendering.BlendMode.SrcAlpha : (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DSTBLEND", value < 1 ? (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha : (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWRITE", value < 1 ? 0 : 1);
        }
    }

    private void CacheMaterialDefaultAttributes(ref Dictionary<string, float> dict, Material mat)
    {
        dict.Add("_TransitionAlpha", mat.GetFloat("_TransitionAlpha"));
        dict.Add("_SRCBLEND", (float)mat.GetInt("_SRCBLEND"));
        dict.Add("_DSTBLEND", (float)mat.GetInt("_DSTBLEND"));
        dict.Add("_ZWRITE", (float)mat.GetInt("_ZWRITE"));
    }

    private void RestoreMaterialDefaultAttributes(ref Dictionary<string, float> dict, Material mat)
    {
        mat.SetFloat("_TransitionAlpha", dict["_TransitionAlpha"]);
        mat.SetInt("_SRCBLEND", (int)dict["_SRCBLEND"]);
        mat.SetInt("_DSTBLEND", (int)dict["_DSTBLEND"]);
        mat.SetInt("_ZWRITE", (int)dict["_ZWRITE"]);
    }

    private void Awake()
    {
        if (DefaultMaterial == null)
        {
            Debug.LogWarning(gameObject.name + " Tool has no active material.");
        }
        else
        {
            CacheMaterialDefaultAttributes(ref defaultMaterialDefaults, DefaultMaterial);
        }

        if (HighlightMaterial == null)
        {
            Debug.LogWarning(gameObject.name + " Tool has no highlight material.");
        }
        else
        {
            CacheMaterialDefaultAttributes(ref highlightMaterialDefaults, HighlightMaterial);
        }

        if (SelectedMaterial == null)
        {
            Debug.LogWarning(gameObject.name + " Tool has no selected material.");
        }
        else
        {
            CacheMaterialDefaultAttributes(ref selectedMaterialDefaults, SelectedMaterial);
        }

        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogWarning(gameObject.name + " Tool has no renderer.");
        }
    }

    private void Start()
    {
        if (TooltipObject != null)
        {
            TooltipObject.SetActive(false);
        }

    }

    private void OnDestroy()
    {
        RestoreMaterialDefaultAttributes(ref defaultMaterialDefaults, DefaultMaterial);
        RestoreMaterialDefaultAttributes(ref highlightMaterialDefaults, HighlightMaterial);
        RestoreMaterialDefaultAttributes(ref selectedMaterialDefaults, SelectedMaterial);
    }

    public void Highlight() {
        ////////////////if (!ToolManager.Instance.IsLocked)
            if (!selected) {
                ToolSoundsInstance.PlayHighlightSound();
                    meshRenderer.material = HighlightMaterial;
                }

                if (TooltipObject != null) {
                    TooltipObject.SetActive(true);
                }
    }

    public void RemoveHighlight()
    {
        //////////////////if (!ToolManager.Instance.IsLocked)
           // ToolSounds.Instance.PlayRemoveHighlightSound();

            if (selected)
            {
                meshRenderer.material = SelectedMaterial;
            }
            else
            {
                meshRenderer.material = DefaultMaterial;
            }

            if (TooltipObject != null)
            {
                TooltipObject.SetActive(false);
            }
    }

    public void Select() {
        ///////////////////////////////if (!ToolManager.Instance.IsLocked)
        if (selected)
            return;

        GraphController.CurrentNavTool = gameObject;
        selected = ToolMangerInstance.SelectTool(this);
        if (selected) {
            //ToolSounds.Instance.PlaySelectSound();
            GazeGestureManager.InputUpdated += HandleUpdatedInput;
            ToolSoundsInstance.PlaySelectSound();
            //////////////////////////contentToManipulate = ViewLoader.Instance.GetCurrentContent();
            contentToManipulate = GraphController.CurrentActiveScene;
            meshRenderer.material = SelectedMaterial;
        }

    }

    public void Unselect()
    {
        //if (!ToolMangerInstance.IsLocked)
       // {
            if (selected)
            {
               GazeGestureManager.InputUpdated -= HandleUpdatedInput;

                ToolSoundsInstance.PlayDeselectSound();

            }

            contentToManipulate = null;
            ToolMangerInstance.DeselectTool(this);
            selected = false;
            meshRenderer.material = DefaultMaterial;
       // }
    }

    private void HandleUpdatedInput(InteractionSourceKind kind, Vector3 direction, Ray ray)
    {
        if (!contentToManipulate)
        {
            return;
        }

        float y = 0.0f;
        float x = 0.0f;
        switch (type)
        {
            case ToolType.Pan:
                y = direction.y;
                x = direction.x;

               
                //contentToManipulate.transform.localPosition = new Vector3(
                //    contentToManipulate.transform.localPosition.x + (Time.deltaTime * x * PanSpeed),
                //    contentToManipulate.transform.localPosition.y + (Time.deltaTime * y * PanSpeed),
                //    contentToManipulate.transform.localPosition.z);
                //break;

                contentToManipulate.transform.Translate(
                    (Time.deltaTime * x * PanSpeed),
                    (Time.deltaTime * y * PanSpeed),
                        0, Space.World);
                break;

            //////////////////////////////////////////case ToolType.Rotate:
            //////////////////////////////////////////    y = direction.y;

            //////////////////////////////////////////    if (kind == InteractionSourceKind.Hand)
            //////////////////////////////////////////    {
            //////////////////////////////////////////        y = -y;
            //////////////////////////////////////////    }

            //////////////////////////////////////////    var cam = Camera.main;
            //////////////////////////////////////////    var toContent = (contentToManipulate.transform.position - cam.transform.position).normalized;
            //////////////////////////////////////////    //////////////////////////var right = Vector3.Cross(Vector3.up, toContent).normalized;
            //////////////////////////////////////////    var right = Vector3.up;
            //////////////////////////////////////////    var targetUp = Quaternion.AngleAxis(Mathf.Sign(y) * MaxRotationAngle, right) * Vector3.up;

            //////////////////////////////////////////    var currentRotationSpeed = kind == InteractionSourceKind.Hand ? RotationSpeed : ClickerRotationSpeed;

            //////////////////////////////////////////    // use the hero view to determine limits on rotation; however, move the content by the
            //////////////////////////////////////////    // change in rotation, so we are consistently moving the same content/object everywhere
            //////////////////////////////////////////    // (works with resetting content to hero view for example)
            //////////////////////////////////////////    ///////////////////////////////GameObject heroView = ViewLoader.Instance.GetHeroView();
            //////////////////////////////////////////    GameObject heroView = contentToManipulate;
            //////////////////////////////////////////    var desiredUp = Vector3.Slerp(heroView.transform.up, targetUp, Mathf.Clamp01(Time.deltaTime * Mathf.Abs(y) * currentRotationSpeed));

            //////////////////////////////////////////    var upToNewUp = Quaternion.FromToRotation(heroView.transform.up, desiredUp);

            //////////////////////////////////////////    contentToManipulate.transform.rotation =
            //////////////////////////////////////////        Quaternion.LookRotation(upToNewUp * heroView.transform.forward, desiredUp) * Quaternion.Inverse(heroView.transform.rotation) * // hero view rotation delta
            //////////////////////////////////////////        contentToManipulate.transform.rotation;
            //////////////////////////////////////////    break;


            case ToolType.Rotate:
                 x = direction.x;

                if (kind == InteractionSourceKind.Hand) {
                    x = -x;
                }
              

                    Quaternion newRotation = Quaternion.AngleAxis((contentToManipulate.transform.localRotation.eulerAngles.y +(x * 40)), Vector3.up);
                    contentToManipulate.transform.localRotation = Quaternion.Slerp(contentToManipulate.transform.localRotation, newRotation, 0.08f);
                break;

            case ToolType.Zoom:

                float smallestScale = ToolMangerInstance.TargetMinZoomSize;
                //////////////////////////Bounds currentBounds = GetContentBounds();

                //float contentXSize = currentBounds.extents.x == 0 ? smallestScale : currentBounds.extents.x;
                float contentXSize = smallestScale;
                float zoomHandDistanceFactor = Mathf.Abs(Mathf.Pow(direction.x, 3)) * Mathf.Sign(direction.x);
                float zoomContentSizeFactor = contentXSize / smallestScale;
                float contentScalar = ToolMangerInstance.SmallestZoom / ToolMangerInstance.TargetMinZoomSize;
                float newScale = contentToManipulate.transform.localScale.x + (Time.deltaTime * zoomHandDistanceFactor * ScaleSpeed * zoomContentSizeFactor * scalePercentValue * contentScalar);
                if (newScale < ToolMangerInstance.SmallestZoom) {
                    newScale = ToolMangerInstance.SmallestZoom;
                }

                if (newScale > ToolMangerInstance.LargestZoom) {
                    newScale = ToolMangerInstance.LargestZoom;
                }
               contentToManipulate.transform.localScale = new Vector3(newScale, newScale, newScale);
                break;
        }
    }

    private void ToolAction()
    {
        //if (selected)
        //{
        //    Unselect();
        //}
        //else
        //{
        //    Select();
        //}
        if(!selected)
            Select();
    }

    public override void OnGazeSelect()
    {
        hasGaze = true;
        Highlight();
    }

    public override void OnGazeDeselect()
    {
        hasGaze = false;
        RemoveHighlight();
    }

    public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
    {
        ToolAction();
    }

    public void PlayEngagedSound()
    {
        // Don't play noise if looking at tool or button
        if (ToolMangerInstance && ToolMangerInstance.SelectedTool && !hasGaze )
        {
            ToolSoundsInstance.PlayEngagedSound();
        }
    }

    public void PlayDisengagedSound()
    {
        // Don't play noise if looking at tool or button
        if (ToolMangerInstance && ToolMangerInstance.SelectedTool && !hasGaze )
        {
            ToolSoundsInstance.PlayDisengagedSound();
        }
    }
}
