﻿// Shared Methods|Utilities|90030
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// The Shared Methods script is a collection of reusable static methods that are used across a range of different scripts.
    /// </summary>
    public static class VRTK_SharedMethods
    {
        /// <summary>
        /// The GetBounds methods returns the bounds of the transform including all children in world space.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="excludeRotation">Resets the rotation of the transform temporarily to 0 to eliminate skewed bounds.</param>
        /// <param name="excludeTransform">Does not consider the stated object when calculating the bounds.</param>
        /// <returns>The bounds of the transform.</returns>
        public static Bounds GetBounds(Transform transform, Transform excludeRotation = null, Transform excludeTransform = null)
        {
            Quaternion oldRotation = Quaternion.identity;
            if (excludeRotation)
            {
                oldRotation = excludeRotation.rotation;
                excludeRotation.rotation = Quaternion.identity;
            }

            bool boundsInitialized = false;
            Bounds bounds = new Bounds(transform.position, Vector3.zero);

            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (excludeTransform != null && renderer.transform.IsChildOf(excludeTransform))
                {
                    continue;
                }

                // do late initialization in case initial transform does not contain any renderers
                if (!boundsInitialized)
                {
                    bounds = new Bounds(renderer.transform.position, Vector3.zero);
                    boundsInitialized = true;
                }
                bounds.Encapsulate(renderer.bounds);
            }

            if (bounds.size.magnitude == 0)
            {
                // do second pass as there were no renderers, this time with colliders
                BoxCollider[] colliders = transform.GetComponentsInChildren<BoxCollider>();
                foreach (BoxCollider collider in colliders)
                {
                    if (excludeTransform != null && collider.transform.IsChildOf(excludeTransform))
                    {
                        continue;
                    }

                    // do late initialization in case initial transform does not contain any colliders
                    if (!boundsInitialized)
                    {
                        bounds = new Bounds(collider.transform.position, Vector3.zero);
                        boundsInitialized = true;
                    }
                    bounds.Encapsulate(collider.bounds);
                }
            }

            if (excludeRotation)
            {
                excludeRotation.rotation = oldRotation;
            }

            return bounds;
        }

        /// <summary>
        /// The IsLowest method checks to see if the given value is the lowest number in the given array of values.
        /// </summary>
        /// <param name="value">The value to check to see if it is lowest.</param>
        /// <param name="others">The array of values to check against.</param>
        /// <returns>Returns true if the value is lower than all numbers in the given array, returns false if it is not the lowest.</returns>
        public static bool IsLowest(float value, float[] others)
        {
            foreach (float o in others)
            {
                if (o <= value)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The AddCameraFade method finds the headset camera and adds a headset fade script to it.
        /// </summary>
        /// <returns>The transform of the headset camera.</returns>
        public static Transform AddCameraFade()
        {
            var camera = VRTK_DeviceFinder.HeadsetCamera();
            VRTK_SDK_Bridge.AddHeadsetFade(camera);
            return camera;
        }

        /// <summary>
        /// The CreateColliders method attempts to add box colliders to all child objects in the given object that have a renderer but no collider.
        /// </summary>
        /// <param name="obj">The game object to attempt to add the colliders to.</param>
        public static void CreateColliders(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (!renderer.gameObject.GetComponent<Collider>())
                {
                    renderer.gameObject.AddComponent<BoxCollider>();
                }
            }
        }

        /// <summary>
        /// The CloneComponent method takes a source component and copies it to the given destination game object.
        /// </summary>
        /// <param name="source">The component to copy.</param>
        /// <param name="destination">The game object to copy the component to.</param>
        /// <param name="copyProperties">Determines whether the properties of the component as well as the fields should be copied.</param>
        /// <returns>The component that has been cloned onto the given game object.</returns>
        public static Component CloneComponent(Component source, GameObject destination, bool copyProperties = false)
        {
            Component tmpComponent = destination.gameObject.AddComponent(source.GetType());
            if (copyProperties)
            {
                foreach (PropertyInfo p in source.GetType().GetProperties())
                {
                    if (p.CanWrite)
                    {
                        p.SetValue(tmpComponent, p.GetValue(source, null), null);
                    }
                }
            }

            foreach (FieldInfo f in source.GetType().GetFields())
            {
                f.SetValue(tmpComponent, f.GetValue(source));
            }
            return tmpComponent;
        }

        /// <summary>
        /// The ColorDarken method takes a given colour and darkens it by the given percentage.
        /// </summary>
        /// <param name="color">The source colour to apply the darken to.</param>
        /// <param name="percent">The percent to darken the colour by.</param>
        /// <returns>The new colour with the darken applied.</returns>
        public static Color ColorDarken(Color color, float percent)
        {
            return new Color(ColorPercent(color.r, percent), ColorPercent(color.g, percent), ColorPercent(color.b, percent), color.a);
        }

        /// <summary>
        /// The RoundFloat method is used to round a given float to the given decimal places.
        /// </summary>
        /// <param name="givenFloat">The float to round.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <param name="rawFidelity">If this is true then the decimal places must be given in the decimal multiplier, e.g. 10 for 1dp, 100 for 2dp, etc.</param>
        /// <returns>The rounded float.</returns>
        public static float RoundFloat(float givenFloat, int decimalPlaces, bool rawFidelity = false)
        {
            float roundBy = (rawFidelity ? decimalPlaces : Mathf.Pow(10.0f, decimalPlaces));
            return Mathf.Round(givenFloat * roundBy) / roundBy;
        }

        /// <summary>
        /// The IsEditTime method determines if the state of Unity is in the Unity Editor and the scene is not in play mode.
        /// </summary>
        /// <returns>Returns true if Unity is in the Unity Editor and not in play mode.</returns>
        public static bool IsEditTime()
        {
#if UNITY_EDITOR
            return !EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return false;
#endif
        }

        /// <summary>
        /// The TriggerHapticPulse/1 method calls a single haptic pulse call on the controller for a single tick.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public static void TriggerHapticPulse(uint controllerIndex, float strength)
        {
            var instanceMethods = VRTK_InstanceMethods.instance;
            if (instanceMethods != null)
            {
                instanceMethods.haptics.TriggerHapticPulse(controllerIndex, strength);
            }
        }

        /// <summary>
        /// The TriggerHapticPulse/3 method calls a haptic pulse for a specified amount of time rather than just a single tick. Each pulse can be separated by providing a `pulseInterval` to pause between each haptic pulse.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        /// <param name="duration">The length of time the rumble should continue for.</param>
        /// <param name="pulseInterval">The interval to wait between each haptic pulse.</param>
        public static void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float pulseInterval)
        {
            var instanceMethods = VRTK_InstanceMethods.instance;
            if (instanceMethods != null)
            {
                instanceMethods.haptics.TriggerHapticPulse(controllerIndex, strength, duration, pulseInterval);
            }
        }

        /// <summary>
        /// The SetOpacity method allows the opacity of the given GameObject to be changed. A lower alpha value will make the object more transparent, such as `0.5f` will make the controller partially transparent where as `0f` will make the controller completely transparent.
        /// </summary>
        /// <param name="model">The GameObject to change the renderer opacity on.</param>
        /// <param name="alpha">The alpha level to apply to opacity of the controller object. `0f` to `1f`.</param>
        public static void SetOpacity(GameObject model, float alpha)
        {
            var instanceMethods = VRTK_InstanceMethods.instance;
            if (instanceMethods != null)
            {
                instanceMethods.objectAppearance.SetOpacity(model, alpha);
            }
        }

        /// <summary>
        /// The SetRendererVisible method turns on renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to show the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public static void SetRendererVisible(GameObject model, GameObject ignoredModel = null)
        {
            var instanceMethods = VRTK_InstanceMethods.instance;
            if (instanceMethods != null)
            {
                instanceMethods.objectAppearance.SetRendererVisible(model, ignoredModel);
            }
        }

        /// <summary>
        /// The SetRendererHidden method turns off renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to hide the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public static void SetRendererHidden(GameObject model, GameObject ignoredModel = null)
        {
            var instanceMethods = VRTK_InstanceMethods.instance;
            if (instanceMethods != null)
            {
                instanceMethods.objectAppearance.SetRendererHidden(model, ignoredModel);
            }
        }

        /// <summary>
        /// The ToggleRenderer method turns on or off the renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle of.
        /// </summary>
        /// <param name="state">If true then the renderers will be enabled, if false the renderers will be disabled.</param>
        /// <param name="model">The GameObject to toggle the renderer states of.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public static void ToggleRenderer(bool state, GameObject model, GameObject ignoredModel = null)
        {
            if (state)
            {
                SetRendererVisible(model, ignoredModel);
            }
            else
            {
                SetRendererHidden(model, ignoredModel);
            }
        }

        /// <summary>
        /// The HighlightObject method calls the Highlight method on the highlighter attached to the given GameObject with the provided colour.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Highlight on.</param>
        /// <param name="highlightColor">The colour to highlight to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        public static void HighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
        {
            var instanceMethods = VRTK_InstanceMethods.instance;
            if (instanceMethods != null)
            {
                instanceMethods.objectAppearance.HighlightObject(model, highlightColor, fadeDuration);
            }
        }

        /// <summary>
        /// The UnhighlightObject method calls the Unhighlight method on the highlighter attached to the given GameObject.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Unhighlight on.</param>
        public static void UnhighlightObject(GameObject model)
        {
            var instanceMethods = VRTK_InstanceMethods.instance;
            if (instanceMethods != null)
            {
                instanceMethods.objectAppearance.UnhighlightObject(model);
            }
        }

        /// <summary>
        /// The Mod method is used to find the remainder of the sum a/b.
        /// </summary>
        /// <param name="a">The dividend value.</param>
        /// <param name="b">The divisor value.</param>
        /// <returns>The remainder value.</returns>
        public static float Mod(float a, float b)
        {
            return a - b * Mathf.Floor(a / b);
        }

        /// <summary>
        /// Finds all <see cref="GameObject"/>s with a given name and an ancestor that has a specific component.
        /// </summary>
        /// <remarks>
        /// This method returns active as well as inactive <see cref="GameObject"/>s in the scene. It doesn't return assets.
        /// For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.
        /// </remarks>
        /// <typeparam name="T">The component type that needs to be on an ancestor of the wanted <see cref="GameObject"/>. Must be a subclass of <see cref="Component"/>.</typeparam>
        /// <param name="gameObjectName">The name of the wanted <see cref="GameObject"/>. If it contains a '/' character, this method traverses the hierarchy like a path name.</param>
        /// <returns>The <see cref="GameObject"/> with name <paramref name="gameObjectName"/> and an ancestor that has a <typeparamref name="T"/>. If no <see cref="GameObject"/> is found <see langword="null"/> is returned.</returns>
        public static GameObject FindEvenInactiveGameObject<T>(string gameObjectName = "") where T : Component
        {
            IEnumerable<GameObject> gameObjects = Resources.FindObjectsOfTypeAll<T>()
                                                           .Select(component => component.gameObject);

#if UNITY_EDITOR
            gameObjects = gameObjects.Where(gameObject => !AssetDatabase.Contains(gameObject));
#endif

            string[] names = gameObjectName.Split(new[] { '/' }, 2);
            string firstName = names[0];
            if (!string.IsNullOrEmpty(firstName))
            {
                gameObjects = gameObjects.Where(gameObject => gameObject.name == firstName);
            }

            string otherNames = names.Length > 1 ? names[1] : null;
            if (string.IsNullOrEmpty(otherNames))
            {
                return gameObjects.FirstOrDefault();
            }

            return gameObjects.Select(gameObject => gameObject.transform.Find(otherNames).gameObject)
                              .FirstOrDefault();
        }

        private static float ColorPercent(float value, float percent)
        {
            percent = Mathf.Clamp(percent, 0f, 100f);
            return (percent == 0f ? value : (value - (percent / 100f)));
        }
    }
}