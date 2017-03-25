// Controller Highlighter|Interactions|30021
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;

    /// <summary>
    /// The Controller Highlighter script provides methods to deal with highlighting controller elements.
    /// </summary>
    /// <remarks>
    /// The highlighting of the controller is defaulted to use the `VRTK_MaterialColorSwapHighlighter` if no other highlighter is applied to the Object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the ability to change the opacity of a controller model and to highlight specific elements of a controller such as the buttons or even the entire controller model.
    /// </example>
    public class VRTK_ControllerHighlighter : MonoBehaviour
    {
        [Tooltip("A collection of strings that determine the path to the controller model sub elements for identifying the model parts at runtime. If the paths are left empty they will default to the model element paths of the selected SDK Bridge.\n\n"
                 + "* The available model sub elements are:\n\n"
                 + " * `Body Model Path`: The overall shape of the controller.\n"
                 + " * `Trigger Model Path`: The model that represents the trigger button.\n"
                 + " * `Grip Left Model Path`: The model that represents the left grip button.\n"
                 + " * `Grip Right Model Path`: The model that represents the right grip button.\n"
                 + " * `Touchpad Model Path`: The model that represents the touchpad.\n"
                 + " * `Button One Model Path`: The model that represents button one.\n"
                 + " * `Button Two Model Path`: The model that represents button two.\n"
                 + " * `System Menu Model Path`: The model that represents the system menu button."
                 + " * `Start Menu Model Path`: The model that represents the start menu button.")]
        public VRTK_ControllerModelElementPaths modelElementPaths = new VRTK_ControllerModelElementPaths();

        [Tooltip("A collection of highlighter overrides for each controller model sub element. If no highlighter override is given then highlighter on the Controller game object is used.\n\n"
                 + "* The available model sub elements are:\n\n"
                 + " * `Body`: The highlighter to use on the overall shape of the controller.\n"
                 + " * `Trigger`: The highlighter to use on the trigger button.\n"
                 + " * `Grip Left`: The highlighter to use on the left grip button.\n"
                 + " * `Grip Right`: The highlighter to use on the  right grip button.\n"
                 + " * `Touchpad`: The highlighter to use on the touchpad.\n"
                 + " * `Button One`: The highlighter to use on button one.\n"
                 + " * `Button Two`: The highlighter to use on button two.\n"
                 + " * `System Menu`: The highlighter to use on the system menu button."
                 + " * `Start Menu`: The highlighter to use on the start menu button.")]
        public VRTK_ControllerElementHighlighers elementHighlighterOverrides = new VRTK_ControllerElementHighlighers();
        [Tooltip("An optional GameObject to specify which controller to apply the script methods to. If this is left blank then this script is required to be placed on a Controller Alias GameObject.")]
        public GameObject controllerAlias;
        [Tooltip("An optional GameObject to specifiy where the controller models are. If this is left blank then the Model Alias object will be used.")]
        public GameObject modelContainer;

        protected bool controllerHighlighted = false;
        protected Dictionary<string, Transform> cachedElements;
        protected Dictionary<string, object> highlighterOptions;
        protected Coroutine initHighlightersRoutine;

        /// <summary>
        /// The ConfigureControllerPaths method is used to set up the model element paths.
        /// </summary>
        public virtual void ConfigureControllerPaths()
        {
            cachedElements = new Dictionary<string, Transform>();
            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerAlias);
            modelElementPaths.bodyModelPath = GetElementPath(modelElementPaths.bodyModelPath, SDK_BaseController.ControllerElements.Body);
            modelElementPaths.triggerModelPath = GetElementPath(modelElementPaths.triggerModelPath, SDK_BaseController.ControllerElements.Trigger);
            modelElementPaths.leftGripModelPath = GetElementPath(modelElementPaths.leftGripModelPath, SDK_BaseController.ControllerElements.GripLeft);
            modelElementPaths.rightGripModelPath = GetElementPath(modelElementPaths.rightGripModelPath, SDK_BaseController.ControllerElements.GripRight);
            modelElementPaths.touchpadModelPath = GetElementPath(modelElementPaths.touchpadModelPath, SDK_BaseController.ControllerElements.Touchpad);
            modelElementPaths.buttonOneModelPath = GetElementPath(modelElementPaths.buttonOneModelPath, SDK_BaseController.ControllerElements.ButtonOne);
            modelElementPaths.buttonTwoModelPath = GetElementPath(modelElementPaths.buttonTwoModelPath, SDK_BaseController.ControllerElements.ButtonTwo);
            modelElementPaths.systemMenuModelPath = GetElementPath(modelElementPaths.systemMenuModelPath, SDK_BaseController.ControllerElements.SystemMenu);
            modelElementPaths.startMenuModelPath = GetElementPath(modelElementPaths.systemMenuModelPath, SDK_BaseController.ControllerElements.StartMenu);
        }

        /// <summary>
        /// The PopulateHighlighters method sets up the highlighters on the controller model.
        /// </summary>
        public virtual void PopulateHighlighters()
        {
            highlighterOptions = new Dictionary<string, object>();
            highlighterOptions.Add("resetMainTexture", true);
            VRTK_BaseHighlighter objectHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(controllerAlias);

            if (objectHighlighter == null)
            {
                objectHighlighter = controllerAlias.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }

            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerAlias);

            objectHighlighter.Initialise(null, highlighterOptions);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.ButtonOne, controllerHand)), objectHighlighter, elementHighlighterOverrides.buttonOne);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.ButtonTwo, controllerHand)), objectHighlighter, elementHighlighterOverrides.buttonTwo);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Body, controllerHand)), objectHighlighter, elementHighlighterOverrides.body);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.GripLeft, controllerHand)), objectHighlighter, elementHighlighterOverrides.gripLeft);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.GripRight, controllerHand)), objectHighlighter, elementHighlighterOverrides.gripRight);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.StartMenu, controllerHand)), objectHighlighter, elementHighlighterOverrides.startMenu);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.SystemMenu, controllerHand)), objectHighlighter, elementHighlighterOverrides.systemMenu);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Touchpad, controllerHand)), objectHighlighter, elementHighlighterOverrides.touchpad);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Trigger, controllerHand)), objectHighlighter, elementHighlighterOverrides.trigger);
        }

        /// <summary>
        /// The HighlightController method attempts to highlight all sub models of the controller.
        /// </summary>
        /// <param name="color">The colour to highlight the controller to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        public virtual void HighlightController(Color color, float fadeDuration = 0f)
        {
            HighlightElement(SDK_BaseController.ControllerElements.ButtonOne, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.ButtonTwo, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.Body, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.GripLeft, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.GripRight, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.StartMenu, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.SystemMenu, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.Touchpad, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.Trigger, color, fadeDuration);
            controllerHighlighted = true;
        }

        /// <summary>
        /// The UnhighlightController method attempts to remove the highlight from all sub models of the controller.
        /// </summary>
        public virtual void UnhighlightController()
        {
            controllerHighlighted = false;
            UnhighlightElement(SDK_BaseController.ControllerElements.ButtonOne);
            UnhighlightElement(SDK_BaseController.ControllerElements.ButtonTwo);
            UnhighlightElement(SDK_BaseController.ControllerElements.Body);
            UnhighlightElement(SDK_BaseController.ControllerElements.GripLeft);
            UnhighlightElement(SDK_BaseController.ControllerElements.GripRight);
            UnhighlightElement(SDK_BaseController.ControllerElements.StartMenu);
            UnhighlightElement(SDK_BaseController.ControllerElements.SystemMenu);
            UnhighlightElement(SDK_BaseController.ControllerElements.Touchpad);
            UnhighlightElement(SDK_BaseController.ControllerElements.Trigger);
        }

        /// <summary>
        /// The HighlightElement method attempts to highlight a specific controller element.
        /// </summary>
        /// <param name="elementType">The element type on the controller.</param>
        /// <param name="color">The colour to highlight the controller element to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        public virtual void HighlightElement(SDK_BaseController.ControllerElements elementType, Color color, float fadeDuration = 0f)
        {
            Transform element = GetElementTransform(GetPathForControllerElement(elementType));
            if (element != null)
            {
                VRTK_SharedMethods.HighlightObject(element.gameObject, color, fadeDuration);
            }
        }

        /// <summary>
        /// The UnhighlightElement method attempts to remove the highlight from the specific controller element.
        /// </summary>
        /// <param name="elementType">The element type on the controller.</param>
        public virtual void UnhighlightElement(SDK_BaseController.ControllerElements elementType)
        {
            if (!controllerHighlighted)
            {
                Transform element = GetElementTransform(GetPathForControllerElement(elementType));
                if (element != null)
                {
                    VRTK_SharedMethods.UnhighlightObject(element.gameObject);
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (controllerAlias == null)
            {
                VRTK_ControllerTracker controllerTracker = GetComponentInParent<VRTK_ControllerTracker>();
                controllerAlias = (controllerTracker != null ? controllerTracker.gameObject : null);
            }

            if (controllerAlias == null)
            {
                Debug.LogError("No Controller Alias GameObject can be found, either specify one in the `Controller Alias` parameter or apply this script to a `Controller Alias` GameObject.");
                return;
            }

            ConfigureControllerPaths();
            modelContainer = (modelContainer != null ? modelContainer : VRTK_DeviceFinder.GetModelAliasController(controllerAlias));
            initHighlightersRoutine = StartCoroutine(WaitForModel());
        }

        protected virtual void OnDisable()
        {
            if (initHighlightersRoutine != null)
            {
                StopCoroutine(initHighlightersRoutine);
            }
        }

        protected virtual IEnumerator WaitForModel()
        {
            while (GetElementTransform(modelElementPaths.bodyModelPath) == null)
            {
                yield return null;
            }
            PopulateHighlighters();
        }

        protected virtual void AddHighlighterToElement(Transform element, VRTK_BaseHighlighter parentHighlighter, VRTK_BaseHighlighter overrideHighlighter)
        {
            if (element != null)
            {
                VRTK_BaseHighlighter highlighter = (overrideHighlighter != null ? overrideHighlighter : parentHighlighter);
                VRTK_BaseHighlighter clonedHighlighter = (VRTK_BaseHighlighter)VRTK_SharedMethods.CloneComponent(highlighter, element.gameObject);
                clonedHighlighter.Initialise(null, highlighterOptions);
            }
        }

        protected virtual string GetElementPath(string currentPath, SDK_BaseController.ControllerElements elementType)
        {
            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerAlias);
            string foundElementPath = VRTK_SDK_Bridge.GetControllerElementPath(elementType, controllerHand);
            return (currentPath.Trim() == "" && foundElementPath != null ? foundElementPath : currentPath.Trim());
        }

        protected virtual string GetPathForControllerElement(SDK_BaseController.ControllerElements controllerElement)
        {
            switch (controllerElement)
            {
                case SDK_BaseController.ControllerElements.Body:
                    return modelElementPaths.bodyModelPath;
                case SDK_BaseController.ControllerElements.Trigger:
                    return modelElementPaths.triggerModelPath;
                case SDK_BaseController.ControllerElements.GripLeft:
                    return modelElementPaths.leftGripModelPath;
                case SDK_BaseController.ControllerElements.GripRight:
                    return modelElementPaths.rightGripModelPath;
                case SDK_BaseController.ControllerElements.Touchpad:
                    return modelElementPaths.touchpadModelPath;
                case SDK_BaseController.ControllerElements.ButtonOne:
                    return modelElementPaths.buttonOneModelPath;
                case SDK_BaseController.ControllerElements.ButtonTwo:
                    return modelElementPaths.buttonTwoModelPath;
                case SDK_BaseController.ControllerElements.SystemMenu:
                    return modelElementPaths.systemMenuModelPath;
                case SDK_BaseController.ControllerElements.StartMenu:
                    return modelElementPaths.startMenuModelPath;
            }
            return "";
        }

        protected virtual Transform GetElementTransform(string path)
        {
            if (cachedElements == null || path == null)
            {
                return null;
            }

            if (!cachedElements.ContainsKey(path) || cachedElements[path] == null)
            {
                if (!modelContainer)
                {
                    Debug.LogError("No model container could be found. Have you selected a valid Controller SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Controller SDK from the dropdown.");
                    return null;
                }
                cachedElements[path] = modelContainer.transform.Find(path);
            }
            return cachedElements[path];
        }

        protected virtual void ToggleHighlightAlias(bool state, string transformPath, Color? highlight, float duration = 0f)
        {
            Transform element = GetElementTransform(transformPath);
            if (element)
            {
                if (state)
                {
                    VRTK_SharedMethods.HighlightObject(element.gameObject, (highlight != null ? highlight : Color.white), duration);
                }
                else
                {
                    VRTK_SharedMethods.UnhighlightObject(element.gameObject);
                }
            }
        }
    }
}