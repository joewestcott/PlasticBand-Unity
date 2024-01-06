using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput;

using Debug = UnityEngine.Debug;

namespace PlasticBand.LowLevel
{
    using static XInputController;

    /// <summary>
    /// Registers layouts for XInput devices, and performs fixups for devices that require state information to determine the true type.
    /// </summary>
    internal static class XInputLayoutFinder
    {
        public const string InterfaceName = "XInput";

        // Layout resolution info
        internal delegate bool XInputOverrideDetermineMatch(XInputCapabilities capabilities);
        private class XInputLayoutOverride
        {
            public XInputOverrideDetermineMatch resolve;
            public InputDeviceMatcher matcher;
            public string layoutName;
        }

        // Registered layout resolvers
        private static readonly Dictionary<int, List<XInputLayoutOverride>> s_LayoutOverrides
            = new Dictionary<int, List<XInputLayoutOverride>>();

        [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR_WIN")]
        internal static void Initialize()
        {
            // Ensure no layouts have persisted across a domain reload
            s_LayoutOverrides.Clear();

            // Register layout finder
            InputSystem.onFindLayoutForDevice += FindXInputDeviceLayout;
        }

        private static string FindXInputDeviceLayout(ref InputDeviceDescription description, string matchedLayout,
            InputDeviceExecuteCommandDelegate executeDeviceCommand)
        {
            // Ignore non-XInput devices
            if (description.interfaceName != InterfaceName)
                return null;

            // Parse capabilities
            if (!Utilities.TryParseJson<XInputCapabilities>(description.capabilities, out var capabilities))
                return DefaultLayoutIfNull(matchedLayout);

            // Check if the subtype has any overrides registered
            if (!s_LayoutOverrides.TryGetValue((int)capabilities.subType, out var overrides))
                return DefaultLayoutIfNull(matchedLayout);

            // Go through device matchers
            XInputLayoutOverride matchedEntry = null;
            float greatestMatch = 0f;
            foreach (var entry in overrides)
            {
                // Ignore invalid overrides and non-matching resolvers
                if (string.IsNullOrEmpty(entry.layoutName) || !entry.resolve(capabilities))
                    continue;

                // Keep track of the best match
                float match = entry.matcher.MatchPercentage(description);
                if (match > greatestMatch)
                {
                    greatestMatch = match;
                    matchedEntry = entry;
                }
            }

            // Use matched entry if available
            if (matchedEntry != null && !string.IsNullOrEmpty(matchedEntry.layoutName))
                return matchedEntry.layoutName;

            // Use existing or default layout otherwise
            return DefaultLayoutIfNull(matchedLayout);
        }

        private static string DefaultLayoutIfNull(string matchedLayout)
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // XInputControllerWindows is only available when building for Windows
            => string.IsNullOrEmpty(matchedLayout) ? nameof(XInputControllerWindows) : null;
#else
            => null;
#endif

        [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR_WIN")]
        internal static void RegisterLayout<TDevice>(DeviceSubType subType, XInputOverrideDetermineMatch resolveLayout,
            InputDeviceMatcher matcher = default)
            where TDevice : InputDevice
            => RegisterLayout<TDevice>((int)subType, resolveLayout, matcher);

        [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR_WIN")]
        internal static void RegisterLayout<TDevice>(XInputNonStandardSubType subType, XInputOverrideDetermineMatch resolveLayout,
            InputDeviceMatcher matcher = default)
            where TDevice : InputDevice
            => RegisterLayout<TDevice>((int)subType, resolveLayout, matcher);

        [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR_WIN")]
        internal static void RegisterLayout<TDevice>(int subType, XInputOverrideDetermineMatch resolveLayout,
            InputDeviceMatcher matcher = default)
            where TDevice : InputDevice
        {
            // Register to the input system
            InputSystem.RegisterLayout<TDevice>();

            // Ensure no override is registered yet
            if (!s_LayoutOverrides.TryGetValue(subType, out var overrides))
            {
                overrides = new List<XInputLayoutOverride>();
                s_LayoutOverrides.Add(subType, overrides);
            }

            string layoutName = typeof(TDevice).Name;
            if (overrides.Any((entry) => entry.matcher == matcher))
            {
                Debug.LogError($"[XInputLayoutFinder] Matcher {matcher} is already registered for subtype {subType}!");
                return;
            }

            // Add to override list
            overrides.Add(new XInputLayoutOverride()
            {
                resolve = resolveLayout,
                matcher = matcher.empty ? GetMatcher(subType) : matcher,
                layoutName = layoutName
            });
        }

        [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR_WIN")]
        internal static void RegisterLayout<TDevice>(DeviceSubType subType)
            where TDevice : InputDevice
            => RegisterLayout<TDevice>((int)subType);

        [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR_WIN")]
        internal static void RegisterLayout<TDevice>(XInputNonStandardSubType subType)
            where TDevice : InputDevice
            => RegisterLayout<TDevice>((int)subType);

        [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR_WIN")]
        internal static void RegisterLayout<TDevice>(int subType)
            where TDevice : InputDevice
        {
            InputSystem.RegisterLayout<TDevice>(matches: GetMatcher(subType));
        }

        internal static InputDeviceMatcher GetMatcher(int subType)
        {
            return new InputDeviceMatcher()
                .WithInterface(InterfaceName)
                .WithCapability("subType", subType);
        }
    }
}