using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TOONIPLAY
{
    [RequireComponent(typeof(RectTransform))]
    public class UIToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        /// <summary>
        /// Display settings for when a toggle is activated or deactivated.
        /// </summary>
        public enum ToggleTransition
        {
            /// <summary>
            /// Show / hide the toggle instantly
            /// </summary>
            None,

            /// <summary>
            /// Fade the toggle in / out smoothly.
            /// </summary>
            Fade
        }

        [System.Serializable]
        /// <summary>
        /// UnityEvent callback for when a toggle is toggled.
        /// </summary>
        public class ToggleEvent : UnityEvent<bool>
        { }

        /// <summary>
        /// Transition mode for the toggle.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        [SerializeField]
        private UIToggleGroup m_Group;

        private float alpha;

        /// <summary>
        /// Group the toggle belongs to.
        /// </summary>
        public UIToggleGroup group
        {
            get { return m_Group; }
            set
            {
                SetToggleGroup(value, true);
                PlayEffect(true);
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// //Attach this script to a Toggle GameObject. To do this, go to Create>UI>Toggle.
        /// //Set your own Text in the Inspector window
        ///
        /// using UnityEngine;
        /// using UnityEngine.UI;
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     Toggle m_Toggle;
        ///     public Text m_Text;
        ///
        ///     void Start()
        ///     {
        ///         //Fetch the Toggle GameObject
        ///         m_Toggle = GetComponent<Toggle>();
        ///         //Add listener for when the state of the Toggle changes, to take action
        ///         m_Toggle.onValueChanged.AddListener(delegate {
        ///                 ToggleValueChanged(m_Toggle);
        ///             });
        ///
        ///         //Initialise the Text to say the first state of the Toggle
        ///         m_Text.text = "First Value : " + m_Toggle.isOn;
        ///     }
        ///
        ///     //Output the new state of the Toggle into Text
        ///     void ToggleValueChanged(Toggle change)
        ///     {
        ///         m_Text.text =  "New Value : " + m_Toggle.isOn;
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>
        public ToggleEvent onValueChanged = new();

        // Whether the toggle is on
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        protected UIToggle()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnDestroy()
        {
            if (m_Group != null)
                m_Group.EnsureValidState();
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don�t have a graphic.
            if (graphic != null)
            {
                bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(UIToggleGroup newGroup, bool setMemberValue)
        {
            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && isOn && IsActive())
                newGroup.NotifyToggleOn(this);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// /Attach this script to a Toggle GameObject. To do this, go to Create>UI>Toggle.
        /// //Set your own Text in the Inspector window
        ///
        /// using UnityEngine;
        /// using UnityEngine.UI;
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     Toggle m_Toggle;
        ///     public Text m_Text;
        ///
        ///     void Start()
        ///     {
        ///         //Fetch the Toggle GameObject
        ///         m_Toggle = GetComponent<Toggle>();
        ///         //Add listener for when the state of the Toggle changes, and output the state
        ///         m_Toggle.onValueChanged.AddListener(delegate {
        ///                 ToggleValueChanged(m_Toggle);
        ///             });
        ///
        ///         //Initialize the Text to say whether the Toggle is in a positive or negative state
        ///         m_Text.text = "Toggle is : " + m_Toggle.isOn;
        ///     }
        ///
        ///     //Output the new state of the Toggle into Text when the user uses the Toggle
        ///     void ToggleValueChanged(Toggle change)
        ///     {
        ///         m_Text.text =  "Toggle is : " + m_Toggle.isOn;
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>

        public bool isOn
        {
            get { return m_IsOn; }

            set
            {
                Set(value);
            }
        }

        /// <summary>
        /// Set isOn without invoking onValueChanged callback.
        /// </summary>
        /// <param name="value">New Value for isOn.</param>
        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

        void Set(bool value, bool sendCallback = true)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && m_Group.isActiveAndEnabled && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this, sendCallback);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect(toggleTransition == ToggleTransition.None);
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Toggle.value", this);
                onValueChanged.Invoke(m_IsOn);
            }
        }

        /// <summary>
        /// Play the appropriate effect.
        /// </summary>
        private void PlayEffect(bool instant)
        {
            if (graphic == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                alpha = m_IsOn ? 1f : 0f;
                graphic.canvasRenderer.SetAlpha(alpha);
                targetGraphic.canvasRenderer.SetAlpha(1f - alpha);
            }
            else
#endif
            {
                graphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
                targetGraphic.CrossFadeAlpha(m_IsOn ? 0f : 1f, instant ? 0f : 0.1f, true);
            }
        }

        /// <summary>
        /// Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }

        /// <summary>
        /// Transition the Selectable to the entered state.
        /// </summary>
        /// <param name="state">State to transition to</param>
        /// <param name="instant">Should the transition occur instantly.</param>
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            Color tintColor;
            Sprite transitionSprite;
            string triggerName;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    transitionSprite = null;
                    triggerName = animationTriggers.normalTrigger;
                    break;
                case SelectionState.Highlighted:
                    tintColor = colors.highlightedColor;
                    transitionSprite = spriteState.highlightedSprite;
                    triggerName = animationTriggers.highlightedTrigger;
                    break;
                case SelectionState.Pressed:
                    tintColor = colors.pressedColor;
                    transitionSprite = spriteState.pressedSprite;
                    triggerName = animationTriggers.pressedTrigger;
                    break;
                case SelectionState.Selected:
                    tintColor = colors.selectedColor;
                    transitionSprite = spriteState.selectedSprite;
                    triggerName = animationTriggers.selectedTrigger;
                    break;
                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    transitionSprite = spriteState.disabledSprite;
                    triggerName = animationTriggers.disabledTrigger;
                    break;
                default:
                    tintColor = Color.black;
                    transitionSprite = null;
                    triggerName = string.Empty;
                    break;
            }

            if (m_IsOn)
                tintColor.a = 0.0f;

            switch (transition)
            {
                case Transition.ColorTint:
                    StartColorTween(tintColor * colors.colorMultiplier, instant);
                    break;
                case Transition.SpriteSwap:
                    DoSpriteSwap(transitionSprite);
                    break;
                case Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
            }
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;

            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername)
        {
#if PACKAGE_ANIMATION
            if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(m_AnimationTriggers.normalTrigger);
            animator.ResetTrigger(m_AnimationTriggers.highlightedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.pressedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.selectedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.disabledTrigger);

            animator.SetTrigger(triggername);
#endif
        }
    }
}
