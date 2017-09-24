// Copyright Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace Assets.My_Scripts.Tools {
    public class ToolManager : MonoBehaviour
    {
        [HideInInspector]
        public Tool SelectedTool = null;
        public ToolSounds ToolSoundsInstance;
        public float TargetMinZoomSize = 0.15f;
        public float LargestZoom = 5.0f;

        public GameObject CurrentNavTool;
        private bool locked = false;
        private ToolPanel panel;

    
        public bool IsLocked
        {
            get { return locked; }
        }

        private float smallestZoom;

        public float SmallestZoom
        {
            get { return smallestZoom; }
        }

        private void Awake()
        {

            smallestZoom = TargetMinZoomSize;
            panel = GetComponent<ToolPanel>();

            GraphController.CurrentNavTool = CurrentNavTool;
            if (panel == null)
            {
                Debug.LogError("ToolManager couldn't find ToolPanel. Hiding and showing of Tools unavailable.");
            }

            if (panel == null)
            {
                Debug.LogError("ToolManager couldn't find ToolSounds.");
            }
        }

        private void Start()
        {

        }

        // prevents tools from being accessed
        public void LockTools()
        {
            if (!locked)
            {
                UnselectAllTools();
                locked = true;
            }
        }

        // re-enables tool access
        public void UnlockTools()
        {
            locked = false;
        }

        public void UnselectAllTools(bool removeHighlight = true)
        {
            SelectedTool = null;

            Tool[] tools = GetComponentsInChildren<Tool>();
            foreach (Tool tool in tools)
            {
                if (removeHighlight)
                {
                    tool.RemoveHighlight();
                }

                tool.Unselect();
            }

        }

        public bool SelectTool(Tool tool) {
            //if (locked)
            //{
            //    return false;
            //}

            UnselectAllTools(removeHighlight: false);
            SelectedTool = tool;

            //if (Cursor.Instance)
            //{
            //    Cursor.Instance.ApplyToolState(tool.type);
            //}

            return true;
        }

        public bool DeselectTool(Tool tool)
        {
            if (locked)
            {
                return false;
            }

            //if (Cursor.Instance)
            //{
            //    Cursor.Instance.ClearToolState();
            //}

            if (SelectedTool == tool)
            {
                SelectedTool = null;
                return true;
            }

            return false;
        }

        public void LowerTools()
        {
            panel.IsLowered = true;

           
        }

        public void RaiseTools()
        {
            panel.IsLowered = false;

            
        }

        public void ToggleTools()
        {
            if (panel.IsLowered)
            {
                RaiseTools();
            }
            else
            {
                LowerTools();
            }
        }

        [ContextMenu("Hide Tools")]
        public void HideTools()
        {
            HideTools(false);
        }

        public void HideTools(bool instant)
        {
            StartCoroutine(HideToolsAsync(instant));
        }

        [ContextMenu("Show Tools")]
        public void ShowTools()
        {
            StartCoroutine(ShowToolsAsync());
        }

        public IEnumerator HideToolsAsync(bool instant)
        {
            yield return StartCoroutine(panel.FadeOut(instant));
        }

        public IEnumerator ShowToolsAsync()
        {
            yield return StartCoroutine(panel.FadeIn());
        }

        public void ShowBackButton()
        {
            if (ToolManager.BackButtonVisibilityChangeRequested != null)
            {
                ToolManager.BackButtonVisibilityChangeRequested(visible: true);
            }
           
        }

        public void HideBackButton()
        {
            if (ToolManager.BackButtonVisibilityChangeRequested != null)
            {
                ToolManager.BackButtonVisibilityChangeRequested(visible: false);
            }
        }

        public delegate void ButtonVisibilityRequest(bool visible);
        public static event ButtonVisibilityRequest BackButtonVisibilityChangeRequested;
    }
}
