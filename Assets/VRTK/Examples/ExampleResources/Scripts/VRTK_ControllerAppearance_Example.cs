namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTK_ControllerAppearance_Example : MonoBehaviour
    {
        public bool highlightBodyOnlyOnCollision = false;

        private VRTK_ControllerTooltips tooltips;
        private VRTK_ControllerHighlighter highligher;
        private VRTK_ControllerEvents events;

        private void Start()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            events = GetComponent<VRTK_ControllerEvents>();
            highligher = GetComponent<VRTK_ControllerHighlighter>();
            tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();

            //Setup controller event listeners
            events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
            events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

            events.ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
            events.ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

            events.ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
            events.ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

            events.StartMenuPressed += new ControllerInteractionEventHandler(DoStartMenuPressed);
            events.StartMenuReleased += new ControllerInteractionEventHandler(DoStartMenuReleased);

            events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
            events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

            events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
            events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

            tooltips.ToggleTips(false);
        }

        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.Trigger, Color.yellow, 0.5f);
            VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 0.8f);
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.Trigger);
            if (!events.AnyButtonPressed())
            {
                VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 1f);
            }
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.ButtonOne, Color.yellow, 0.5f);
            VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 0.8f);
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.ButtonOne);
            if (!events.AnyButtonPressed())
            {
                VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 1f);
            }
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.ButtonTwo, Color.yellow, 0.5f);
            VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 0.8f);
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.ButtonTwo);
            if (!events.AnyButtonPressed())
            {
                VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 1f);
            }
        }

        private void DoStartMenuPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.StartMenu, Color.yellow, 0.5f);
            VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 0.8f);
        }

        private void DoStartMenuReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.StartMenu);
            if (!events.AnyButtonPressed())
            {
                VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 1f);
            }
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.GripLeft, Color.yellow, 0.5f);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.GripRight, Color.yellow, 0.5f);
            VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 0.8f);
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.GripLeft);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.GripRight);
            if (!events.AnyButtonPressed())
            {
                VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 1f);
            }
        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.Touchpad, Color.yellow, 0.5f);
            VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 0.8f);
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.Touchpad);
            if (!events.AnyButtonPressed())
            {
                VRTK_SharedMethods.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), 1f);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
            {
                if (highlightBodyOnlyOnCollision)
                {
                    highligher.HighlightElement(SDK_BaseController.ControllerElements.Body, Color.yellow, 0.4f);
                }
                else
                {
                    highligher.HighlightController(Color.yellow, 0.4f);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
            {
                if (highlightBodyOnlyOnCollision)
                {
                    highligher.UnhighlightElement(SDK_BaseController.ControllerElements.Body);
                }
                else
                {
                    highligher.UnhighlightController();
                }
            }
        }
    }
}