using PlasticBand.Controls;
using PlasticBand.Devices;
using UnityEditor;
using UnityEngine;

namespace PlasticBand
{
    /// <summary>
    /// Handles initialization of the package.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal static class Initialization
    {
#if UNITY_EDITOR
        static Initialization()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Initialize()
        {
            // Controls
            MaskButtonControl.Initialize();
            GuitarHeroSliderControl.Initialize();
            FourLanePadsControl.Initialize();

            // 5-fret Guitars
            FiveFretGuitar.Initialize();
            GuitarHeroGuitar.Initialize();
            RockBandGuitar.Initialize();
            XInputGuitar.Initialize();
            XInputGuitarAlternate.Initialize();
            PS3GuitarHeroGuitar.Initialize();
            PS3RockBandGuitar.Initialize();

            // 6-fret Guitars
            SixFretGuitar.Initialize();
            XInputGuitarGHL.Initialize();
            PS3WiiUSixFretGuitar.Initialize();
            PS4SixFretGuitar.Initialize();

            // 4-lane drumkits
            FourLaneDrumkit.Initialize();
            XInputFourLaneDrumkit.Initialize();

            // 5-lane drumkits
            FiveLaneDrumkit.Initialize();
            XInputFiveLaneDrumkit.Initialize();

            // Turntables
            Turntable.Initialize();
            XInputTurntable.Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // XInput layout fix-ups for devices that require state information to determine the true type
            XInputLayoutFixup.Initialize();
#endif
        }
    }
}