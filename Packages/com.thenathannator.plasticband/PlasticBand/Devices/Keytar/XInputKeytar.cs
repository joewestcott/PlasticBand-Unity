using System.Runtime.InteropServices;
using PlasticBand.LowLevel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace PlasticBand.LowLevel
{
    /// <summary>
    /// The state format for XInput keytars.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct XInputKeytarState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('X', 'I', 'N', 'P');

        [InputControl(name = "dpad", layout = "Dpad", format = "BIT", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/up", bit = 0)]
        [InputControl(name = "dpad/down", bit = 1)]
        [InputControl(name = "dpad/left", bit = 2)]
        [InputControl(name = "dpad/right", bit = 3)]

        [InputControl(name = "startButton", layout = "Button", bit = 4)]
        [InputControl(name = "selectButton", layout = "Button", bit = 5, displayName = "Back")]

        [InputControl(name = "buttonSouth", layout = "Button", bit = 12, displayName = "A")]
        [InputControl(name = "buttonEast", layout = "Button", bit = 13, displayName = "B")]
        [InputControl(name = "buttonWest", layout = "Button", bit = 14, displayName = "X")]
        [InputControl(name = "buttonNorth", layout = "Button", bit = 15, displayName = "Y")]
        public ushort buttons;

        [InputControl(name = "key1",  layout = "Button", offset = 2, bit = 7)]
        [InputControl(name = "key2",  layout = "Button", offset = 2, bit = 6)]
        [InputControl(name = "key3",  layout = "Button", offset = 2, bit = 5)]
        [InputControl(name = "key4",  layout = "Button", offset = 2, bit = 4)]
        [InputControl(name = "key5",  layout = "Button", offset = 2, bit = 3)]
        [InputControl(name = "key6",  layout = "Button", offset = 2, bit = 2)]
        [InputControl(name = "key7",  layout = "Button", offset = 2, bit = 1)]
        [InputControl(name = "key8",  layout = "Button", offset = 2, bit = 0)]
        [InputControl(name = "key9",  layout = "Button", offset = 3, bit = 7)]
        [InputControl(name = "key10", layout = "Button", offset = 3, bit = 6)]
        [InputControl(name = "key11", layout = "Button", offset = 3, bit = 5)]
        [InputControl(name = "key12", layout = "Button", offset = 3, bit = 4)]
        [InputControl(name = "key13", layout = "Button", offset = 3, bit = 3)]
        [InputControl(name = "key14", layout = "Button", offset = 3, bit = 2)]
        [InputControl(name = "key15", layout = "Button", offset = 3, bit = 1)]
        [InputControl(name = "key16", layout = "Button", offset = 3, bit = 0)]
        [InputControl(name = "key17", layout = "Button", offset = 4, bit = 7)]
        [InputControl(name = "key18", layout = "Button", offset = 4, bit = 6)]
        [InputControl(name = "key19", layout = "Button", offset = 4, bit = 5)]
        [InputControl(name = "key20", layout = "Button", offset = 4, bit = 4)]
        [InputControl(name = "key21", layout = "Button", offset = 4, bit = 3)]
        [InputControl(name = "key22", layout = "Button", offset = 4, bit = 2)]
        [InputControl(name = "key23", layout = "Button", offset = 4, bit = 1)]
        [InputControl(name = "key24", layout = "Button", offset = 4, bit = 0)]
        public fixed byte keys[3];

        [InputControl(name = "key25", layout = "Button", offset = 5, bit = 7)]
        [InputControl(name = "velocity1", layout = "Axis", format = "BIT", sizeInBits = 7, offset = 5)]
        [InputControl(name = "velocity2", layout = "Axis", format = "BIT", sizeInBits = 7, offset = 6)]
        [InputControl(name = "velocity3", layout = "Axis", format = "BIT", sizeInBits = 7, offset = 7)]
        [InputControl(name = "velocity4", layout = "Axis", format = "BIT", sizeInBits = 7, offset = 8)]
        [InputControl(name = "velocity5", layout = "Axis", format = "BIT", sizeInBits = 7, offset = 9)]
        public fixed byte velocities[5];

        [InputControl(name = "overdrive", layout = "Button", bit = 7)]
        // The touchstrip isn't available through XInput, but it has to be there for Keytar
        // These bits are unused, so we place it here
        [InputControl(name = "touchStrip", layout = "Axis", format = "BIT", sizeInBits = 7)]
        public byte overdrive;

        [InputControl(name = "analogPedal", layout = "Axis", format = "BIT", sizeInBits = 7)]
        [InputControl(name = "digitalPedal", layout = "Button", bit = 7)]
        public byte pedal;
    }
}

namespace PlasticBand.Devices
{
    /// <summary>
    /// An XInput keytar controller.
    /// </summary>
    [InputControlLayout(stateType = typeof(XInputKeytarState), displayName = "XInput Keytar")]
    internal class XInputKeytar : Keytar
    {
        internal new static void Initialize()
        {
            XInputDeviceUtils.Register<XInputKeytar>(XInputNonStandardSubType.Keytar);
        }
    }
}
