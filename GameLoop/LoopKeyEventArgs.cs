namespace Asteroids.GameLoop
{
    using System;

    /// <summary>
    /// Provides data for the System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp event.
    /// </summary>
    public class LoopKeyEventArgs : EventArgs
    {
        private bool _suppressKeyPress;

        /// <summary>
        /// Initializes a new instance of the System.Windows.Forms.KeyEventArgs class.
        /// </summary>
        /// <param name="keyData">
        /// A System.Windows.Forms.Keys representing the key that was pressed, combined with
        /// any modifier flags that indicate which CTRL, SHIFT, and ALT keys were pressed
        /// at the same time. Possible values are obtained by applying the bitwise OR (|)
        /// operator to constants from the System.Windows.Forms.Keys enumeration.
        /// </param>
        public LoopKeyEventArgs(LoopKeys keyData)
        {
            KeyData = keyData;
        }

        /// <summary>
        /// Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        public bool Alt => (KeyData & LoopKeys.Alt) == LoopKeys.Alt;

        /// <summary>
        /// Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        public bool Control => (KeyData & LoopKeys.Control) == LoopKeys.Control;

        /// <summary>
        /// Gets or sets a value indicating whether the event was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the keyboard code for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp event.
        /// </summary>
        public LoopKeys KeyCode
        {
            get
            {
                var keys = KeyData & LoopKeys.KeyCode;
                return !Enum.IsDefined(typeof(LoopKeys), (int)keys) ? LoopKeys.None : keys;
            }
        }

        /// <summary>
        /// Gets the keyboard value for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp event.
        /// </summary>
        public int KeyValue => (int)(KeyData & LoopKeys.KeyCode);

        /// <summary>
        /// Gets the key data for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp event.
        /// </summary>
        public LoopKeys KeyData { get; }

        /// <summary>
        /// Gets the modifier flags for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp
        /// event. The flags indicate which combination of CTRL, SHIFT, and ALT keys was pressed.
        /// </summary>
        public LoopKeys Modifiers => KeyData & LoopKeys.Modifiers;

        /// <summary>
        /// Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
        public bool Shift => (KeyData & LoopKeys.Shift) == LoopKeys.Shift;

        /// <summary>
        /// Gets or sets a value indicating whether the key event should be passed on to
        /// the underlying control.
        /// </summary>
        public bool SuppressKeyPress
        {
            get => _suppressKeyPress;
            set
            {
                _suppressKeyPress = value;
                Handled = value;
            }
        }
    }
}
