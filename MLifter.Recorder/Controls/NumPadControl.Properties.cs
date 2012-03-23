/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MLifter.AudioTools
{
    public partial class NumPadControl : UserControl
    {
        # region DLL-Imports
        /// <summary>
        /// Gets the state of the key.
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code.</param>
        /// <returns>E.g. for NumLock true if the NumPad is enabled.</returns>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [DllImport("user32.dll")]
        public static extern short GetKeyState(int virtualKeyCode);
        /// <summary>
        /// Simulates a keypress of the key with the specified virtual key code.
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code.</param>
        /// <param name="bScan">Specifies a hardware scan code for the key.</param>
        /// <param name="dwFlags">Specifies various aspects of function operation.</param>
        /// <param name="dwExtraInfo">Specifies an additional value associated with the key stroke.</param>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [DllImport("user32")]
        public static extern void keybd_event(byte virtualKeyCode, byte bScan, int dwFlags, int dwExtraInfo);
        # endregion
        # region Variables
        /// <summary>
        /// If false, no functions are working.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public bool FunctionsEnabled = true;
        public int NumLockSwitchSleepTime = 500;
        public bool ImagesValid = false;

        UserActivityHook keyboardHook;

        private bool advancedView = STANDARD_APPEARANCE.ADVANCEDVIEW;
        private bool keyboardLayout = STANDARD_APPEARANCE.KEYBOARDLAYOUT;
        private bool autoPlay = STANDART_FUNCTIONS.AUTOPLAY;
        private bool lockFunctionsInSimpleView = STANDARD_APPEARANCE.LOCK_FUNCTIONS_IN_SIMPLE_VIEW;
        private bool presenterActivated = true;
        private bool mediaFileAvailable = true;

        private Keys switchKey1 = STANDART_KEYS.SWITCH1;
        private Keys switchKey2 = STANDART_KEYS.SWITCH2;
        private Keys switchKey3 = STANDART_KEYS.SWITCH3;

        private Keys recordKey1 = STANDART_KEYS.RECORD1;
        private Keys recordKey2 = STANDART_KEYS.RECORD2;
        private Keys playKey = STANDART_KEYS.PLAY;
        private Keys nextKey = STANDART_KEYS.NEXT;
        private Keys backKey = STANDART_KEYS.BACK;

        private Image playImage;
        private Image recordImage;
        private Image stopImage;

        private Image buttonPlayImage;
        private Image buttonPlayImageNotAvailable;
        private Image buttonRecordImage;
        private Image buttonRecordImageNotAvailable;
        private Image buttonStopImage;
        private Image buttonNextImage;
        private Image buttonNextImageNotAvailable;
        private Image buttonBackImage;
        private Image buttonBackImageNotAvailable;
        private Image buttonSwitchImage;

        private Function currentState;

        private SerializableDictionary<Keys, Function> keyFunctions = new SerializableDictionary<Keys, Function>();
        private SerializableDictionary<Keys, Function> keyboardFunctions = new SerializableDictionary<Keys, Function>();
        private SerializableDictionary<Keys, Function> presenterKeyFunctions = new SerializableDictionary<Keys, Function>();
        # endregion
        # region Key-Assignment
        /// <summary>
        /// Gets or sets the switch key.
        /// </summary>
        /// <value>The switch key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.SWITCH1), Category("Key-Assignment"), Description("Key to switch between advanced an simple view.")]
        public Keys SwitchKey1
        {
            get { return switchKey1; }
            set { switchKey1 = value; }
        }
        /// <summary>
        /// Gets or sets the switch key.
        /// </summary>
        /// <value>The switch key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.SWITCH2), Category("Key-Assignment"), Description("Key to switch between advanced an simple view.")]
        public Keys SwitchKey2
        {
            get { return switchKey2; }
            set { switchKey2 = value; }
        }
        /// <summary>
        /// Gets or sets the switch key.
        /// </summary>
        /// <value>The switch key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.SWITCH3), Category("Key-Assignment"), Description("Key to switch between advanced an simple view.")]
        public Keys SwitchKey3
        {
            get { return switchKey3; }
            set { switchKey3 = value; }
        }
        /// <summary>
        /// Gets or sets the record key.
        /// </summary>
        /// <value>The record key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.RECORD1), Category("Key-Assignment"), Description("Key to start / stop the recording.")]
        public Keys RecordKey1
        {
            get { return recordKey1; }
            set { recordKey1 = value; }
        }
        /// <summary>
        /// Gets or sets the record key.
        /// </summary>
        /// <value>The record key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.RECORD2), Category("Key-Assignment"), Description("Key to start / stop the recording.")]
        public Keys RecordKey2
        {
            get { return recordKey2; }
            set { recordKey2 = value; }
        }
        /// <summary>
        /// Gets or sets the play key.
        /// </summary>
        /// <value>The play key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.PLAY), Category("Key-Assignment"), Description("Key to start the playback.")]
        public Keys PlayKey
        {
            get { return playKey; }
            set { playKey = value; }
        }
        /// <summary>
        /// Gets or sets the "next key".
        /// </summary>
        /// <value>The next key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.NEXT), Category("Key-Assignment"), Description("Key to go to the next card.")]
        public Keys NextKey
        {
            get { return nextKey; }
            set { nextKey = value; }
        }
        /// <summary>
        /// Gets or sets the "back key".
        /// </summary>
        /// <value>The back key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_KEYS.BACK), Category("Key-Assignment"), Description("Key to go to the previous card.")]
        public Keys BackKey
        {
            get { return backKey; }
            set { backKey = value; }
        }
        # endregion
        # region Images
        /// <summary>
        /// Gets or sets the play image.
        /// </summary>
        /// <value>The play image.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [Category("Images"), Description("The Image to symbolize the play state in the simple view.")]
        public Image PlayImage
        {
            get { return playImage; }
            set { playImage = value; }
        }
        /// <summary>
        /// Gets or sets the record image.
        /// </summary>
        /// <value>The record image.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [Category("Images"), Description("The Image to symbolize the record state in the simple view.")]
        public Image RecordImage
        {
            get { return recordImage; }
            set { recordImage = value; }
        }
        /// <summary>
        /// Gets or sets the stop image.
        /// </summary>
        /// <value>The stop image.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [Category("Images"), Description("The Image to symbolize the stop state in the simple view.")]
        public Image StopImage
        {
            get { return stopImage; }
            set { stopImage = value; }
        }
        /// <summary>
        /// Gets or sets the button play image.
        /// </summary>
        /// <value>The button play image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the play button in the advanced view.")]
        public Image ButtonPlayImage
        {
            get { return buttonPlayImage; }
            set { buttonPlayImage = value; }
        }
        /// <summary>
        /// Gets or sets the button play image.
        /// </summary>
        /// <value>The button play image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the play not available button in the advanced view.")]
        public Image ButtonPlayImageNotAvailable
        {
            get { return buttonPlayImageNotAvailable; }
            set { buttonPlayImageNotAvailable = value; }
        }
        /// <summary>
        /// Gets or sets the button record image.
        /// </summary>
        /// <value>The button record image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the record button in the advanced view.")]
        public Image ButtonRecordImage
        {
            get { return buttonRecordImage; }
            set { buttonRecordImage = value; }
        }
        /// <summary>
        /// Gets or sets the button record image.
        /// </summary>
        /// <value>The button record image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the record not available button in the advanced view.")]
        public Image ButtonRecordImageNotAvailable
        {
            get { return buttonRecordImageNotAvailable; }
            set { buttonRecordImageNotAvailable = value; }
        }
        /// <summary>
        /// Gets or sets the button stop image.
        /// </summary>
        /// <value>The button stop image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the stop button in the advanced view.")]
        public Image ButtonStopImage
        {
            get { return buttonStopImage; }
            set { buttonStopImage = value; }
        }
        /// <summary>
        /// Gets or sets the button next image.
        /// </summary>
        /// <value>The button next image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the next button in the advanced view.")]
        public Image ButtonNextImage
        {
            get { return buttonNextImage; }
            set { buttonNextImage = value; }
        }
        /// <summary>
        /// Gets or sets the button next image.
        /// </summary>
        /// <value>The button next image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the next not available button in the advanced view.")]
        public Image ButtonNextImageNotAvailable
        {
            get { return buttonNextImageNotAvailable; }
            set { buttonNextImageNotAvailable = value; }
        }
        /// <summary>
        /// Gets or sets the button back image.
        /// </summary>
        /// <value>The button back image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the back button in the advanced view.")]
        public Image ButtonBackImage
        {
            get { return buttonBackImage; }
            set { buttonBackImage = value; }
        }
        /// <summary>
        /// Gets or sets the button back image.
        /// </summary>
        /// <value>The button back image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the back not available button in the advanced view.")]
        public Image ButtonBackImageNotAvailable
        {
            get { return buttonBackImageNotAvailable; }
            set { buttonBackImageNotAvailable = value; }
        }
        /// <summary>
        /// Gets or sets the button switch image.
        /// </summary>
        /// <value>The button switch image.</value>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        [Category("Images"), Description("The Image to symbolize the switch button in the advanced view.")]
        public Image ButtonSwitchImage
        {
            get { return buttonSwitchImage; }
            set { buttonSwitchImage = value; }
        }
        # endregion
        # region Functions
        /// <summary>
        /// Gets or sets the function of the 0-Key.
        /// </summary>
        /// <value>The function of the 0-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_0), Category("Functions"), Description("The function of the 0-Key on the NumPad.")]
        public Function Key0Function
        {
            get { return KeyFunctions[Keys.NumPad0]; }
            set { KeyFunctions[Keys.NumPad0] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 1-Key.
        /// </summary>
        /// <value>The function of the 1-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_1), Category("Functions"), Description("The function of the 1-Key on the NumPad.")]
        public Function Key1Function
        {
            get { return KeyFunctions[Keys.NumPad1]; }
            set { KeyFunctions[Keys.NumPad1] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 2-Key.
        /// </summary>
        /// <value>The function of the 2-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_2), Category("Functions"), Description("The function of the 2-Key on the NumPad.")]
        public Function Key2Function
        {
            get { return KeyFunctions[Keys.NumPad2]; }
            set { KeyFunctions[Keys.NumPad2] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 3-Key.
        /// </summary>
        /// <value>The function of the 3-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_3), Category("Functions"), Description("The function of the 3-Key on the NumPad.")]
        public Function Key3Function
        {
            get { return KeyFunctions[Keys.NumPad3]; }
            set { KeyFunctions[Keys.NumPad3] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 4-Key.
        /// </summary>
        /// <value>The function of the 4-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_4), Category("Functions"), Description("The function of the 4-Key on the NumPad.")]
        public Function Key4Function
        {
            get { return KeyFunctions[Keys.NumPad4]; }
            set { KeyFunctions[Keys.NumPad4] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 5-Key.
        /// </summary>
        /// <value>The function of the 5-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_5), Category("Functions"), Description("The function of the 5-Key on the NumPad.")]
        public Function Key5Function
        {
            get { return KeyFunctions[Keys.NumPad5]; }
            set { KeyFunctions[Keys.NumPad5] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 6-Key.
        /// </summary>
        /// <value>The function of the 6-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_6), Category("Functions"), Description("The function of the 6-Key on the NumPad.")]
        public Function Key6Function
        {
            get { return KeyFunctions[Keys.NumPad6]; }
            set { KeyFunctions[Keys.NumPad6] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 7-Key.
        /// </summary>
        /// <value>The function of the 7-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_7), Category("Functions"), Description("The function of the 7-Key on the NumPad.")]
        public Function Key7Function
        {
            get { return KeyFunctions[Keys.NumPad7]; }
            set { KeyFunctions[Keys.NumPad7] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 8-Key.
        /// </summary>
        /// <value>The function of the 8-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_8), Category("Functions"), Description("The function of the 8-Key on the NumPad.")]
        public Function Key8Function
        {
            get { return KeyFunctions[Keys.NumPad8]; }
            set { KeyFunctions[Keys.NumPad8] = value; }
        }
        /// <summary>
        /// Gets or sets the function of the 9-Key.
        /// </summary>
        /// <value>The function of the 9-Key.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_9), Category("Functions"), Description("The function of the 9-Key on the NumPad.")]
        public Function Key9Function
        {
            get { return KeyFunctions[Keys.NumPad9]; }
            set { KeyFunctions[Keys.NumPad9] = value; }
        }
        /// <summary>
        /// Gets or sets the  function.
        /// </summary>
        /// <value>The comma-key function.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_COMMA), Category("Functions"), Description("The function of the Comma-Key on the NumPad.")]
        public Function KeyCommaFunction
        {
            get { return KeyFunctions[Keys.Decimal]; }
            set { KeyFunctions[Keys.Decimal] = value; }
        }
        /// <summary>
        /// Gets or sets the plus-key function.
        /// </summary>
        /// <value>The plus-key function.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_PLUS), Category("Functions"), Description("The function of the Plus-Key on the NumPad.")]
        public Function KeyPlusFunction
        {
            get { return KeyFunctions[Keys.Add]; }
            set { KeyFunctions[Keys.Add] = value; }
        }
        /// <summary>
        /// Gets or sets the minus-key function.
        /// </summary>
        /// <value>The minus-key function.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_MINUS), Category("Functions"), Description("The function of the Minus-Key on the NumPad.")]
        public Function KeyMinusFunction
        {
            get { return KeyFunctions[Keys.Subtract]; }
            set { KeyFunctions[Keys.Subtract] = value; }
        }
        /// <summary>
        /// Gets or sets the multiply-key function.
        /// </summary>
        /// <value>The multiply-key function.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_MULTIPLY), Category("Functions"), Description("The function of the Multiply-Key on the NumPad.")]
        public Function KeyMultiplyFunction
        {
            get { return KeyFunctions[Keys.Multiply]; }
            set { KeyFunctions[Keys.Multiply] = value; }
        }
        /// <summary>
        /// Gets or sets the divide-key function.
        /// </summary>
        /// <value>The divide-key function.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_DIVIDE), Category("Functions"), Description("The function of the Divide-Key on the NumPad.")]
        public Function KeyDivideFunction
        {
            get { return KeyFunctions[Keys.Divide]; }
            set { KeyFunctions[Keys.Divide] = value; }
        }
        /// <summary>
        /// Gets or sets the enter-key function.
        /// </summary>
        /// <value>The enter-key function.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.KEY_ENTER), Category("Functions"), Description("The function of the Enter-Key.")]
        public Function KeyEnterFunction
        {
            get { return KeyFunctions[Keys.Enter]; }
            set { KeyFunctions[Keys.Enter] = value; }
        }
        # endregion
        # region Other
        /// <summary>
        /// Gets or sets a value indicating whether the advanced view is selected.
        /// </summary>
        /// <value><c>true</c> if advanced view is selected; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDARD_APPEARANCE.ADVANCEDVIEW), Category("Appearance"), Description("Set the view of the control to the NumPad-Style.")]
        public bool AdvancedView
        {
            get { return advancedView; }
            set
            {
                FunctionsEnabled = false;

                advancedView = value;
                tableLayoutPanelAdvancedView.Visible = advancedView && !keyboardLayout;
                panelKeyboardBack.Visible = advancedView && keyboardLayout;
                CurrentState = CurrentState;
                OnViewChanged(EventArgs.Empty);

                short keyState = GetKeyState((int)Keys.NumLock);
                int state = keyState & 1;
                if (state != 1)
                {
                    System.Threading.Thread.Sleep(NumLockSwitchSleepTime);
                    keybd_event((int)Keys.NumLock, 0, 0, 0);
                }

                FunctionsEnabled = true;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the keyboard view is selected.
        /// </summary>
        /// <value><c>true</c> if advanced view is selected; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDARD_APPEARANCE.KEYBOARDLAYOUT), Category("Appearance"), Description("Set the view of the control to the Keyboard-Style.")]
        public bool KeyboardLayout
        {
            get { return keyboardLayout; }
            set 
            { 
                keyboardLayout = value;
                tableLayoutPanelAdvancedView.Visible = !keyboardLayout;
                tableLayoutPanelKeyboard.Visible = keyboardLayout;
                panelKeyboardBack.Visible = keyboardLayout;
                panelKeyboardBack.BringToFront();
                ImagesValid = false;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether functions are locked in simple view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if functions are locked in simple view; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2007-08-07</remarks>
        [DefaultValue(STANDARD_APPEARANCE.LOCK_FUNCTIONS_IN_SIMPLE_VIEW), Category("Appearance"), Description("If set to true, only record is available in simple view.")]
        public bool LockFunctionsInSimpleView
        {
            get { return lockFunctionsInSimpleView; }
            set { lockFunctionsInSimpleView = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the auto play is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the auto play is selected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(STANDART_FUNCTIONS.AUTOPLAY), Category("Functions"), Description("Automatically play a recording after the recording stops.")]
        public bool AutoPlay
        {
            get { return autoPlay; }
            set { autoPlay = value; }
        }
        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        /// <value>The state of the current.</value>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        [DefaultValue(Function.Nothing), Description("The current state of the control.")]
        public Function CurrentState
        {
            get { return currentState; }
            set
            {
                currentState = value;
                
                switch (value)
                {
                    case Function.Nothing:
                        if (RecordImage != null)
                            pictureBoxSimpleView.Image = RecordImage;
                        break;
                    case Function.Play:
                        if (PlayImage != null)
                            pictureBoxSimpleView.Image = PlayImage;
                        break;
                    case Function.Record:
                        if (StopImage != null)
                            pictureBoxSimpleView.Image = StopImage;
                        break;
                }
                pictureBoxSimpleView.SizeMode = PictureBoxSizeMode.Zoom;

                SetStopButton();
                ImagesValid = false;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the presenter is activated.
        /// </summary>
        /// <value><c>true</c> if the presenter is activated; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-08-22</remarks>
        [DefaultValue(true), Category("Functions")]
        public bool PresenterActivated
        {
            get { return presenterActivated; }
            set { presenterActivated = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ta MediaFile is available.
        /// </summary>
        /// <value><c>true</c> if ta MediaFile is available; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-08-22</remarks>
        [DefaultValue(true), Category("Appearance")]
        public bool MediaFileAvailable
        {
            get { return mediaFileAvailable; }
            set { mediaFileAvailable = value; }
        }

        /// <summary>
        /// Gets or sets the key functions.
        /// </summary>
        /// <value>The key functions.</value>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        [Browsable(false), ReadOnly(true)]
        public SerializableDictionary<Keys, Function> KeyFunctions
        {
            get { return keyFunctions; }
            set 
            {
                keyFunctions = value;
                ImagesValid = false;
            }
        }
        /// <summary>
        /// Gets or sets the key functions.
        /// </summary>
        /// <value>The key functions.</value>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        [Browsable(false), ReadOnly(true)]
        public SerializableDictionary<Keys, Function> KeyboardFunctions
        {
            get { return keyboardFunctions; }
            set
            {
                keyboardFunctions = value;
                ImagesValid = false;
            }
        }
        /// <summary>
        /// Gets or sets the key functions.
        /// </summary>
        /// <value>The key functions.</value>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        [Browsable(false), ReadOnly(true)]
        public SerializableDictionary<Keys, Function> PresenterKeyFunctions
        {
            get { return presenterKeyFunctions; }
            set { presenterKeyFunctions = value; }
        }
        # endregion
        # region Events
        /// <summary>
        /// Occurs when record is pressed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public event EventHandler Record;
        /// <summary>
        /// Raises the <see cref="E:Record"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        protected virtual void OnRecord(EventArgs e)
        {
            if (CurrentState != Function.Play)
            {
                CurrentState = Function.Record;
                Invalidate();

                if (Record != null)
                    Record(this, e);

                SetStopButton();
            }
        }

        /// <summary>
        /// Occurs when play is pressed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public event EventHandler Play;
        /// <summary>
        /// Raises the <see cref="E:Play"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        protected virtual void OnPlay(EventArgs e)
        {
            if (CurrentState != Function.Record && mediaFileAvailable)
            {
                CurrentState = Function.Play;
                Invalidate();

                if (Play != null)
                    Play(this, e);

                SetStopButton();
            }
        }

        /// <summary>
        /// Occurs when stop is pressed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public event EventHandler StopRecord;
        /// <summary>
        /// Raises the <see cref="E:Stop"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        protected virtual void OnStopRecord(EventArgs e)
        {
            if (StopRecord != null)
                StopRecord(this, e);

            SetStopButton();
        }

        /// <summary>
        /// Occurs when stop is pressed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public event EventHandler StopPlay;
        /// <summary>
        /// Raises the <see cref="E:Stop"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        protected virtual void OnStopPlay(EventArgs e)
        {
            if (StopPlay != null)
                StopPlay(this, e);

            SetStopButton();
        }

        /// <summary>
        /// Occurs when next is pressed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public event EventHandler Next;
        /// <summary>
        /// Raises the <see cref="E:Next"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        protected virtual void OnNext(EventArgs e)
        {
            if (Next != null && CurrentState != Function.Record && CurrentState != Function.Play)
            {
                panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_MOVE;
                panelBackground.Refresh();
                Next(this, e);
                ResetBackground();
            }
        }

        /// <summary>
        /// Occurs when previous is pressed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public event EventHandler Previous;
        /// <summary>
        /// Raises the <see cref="E:Previous"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        protected virtual void OnPrevious(EventArgs e)
        {
            if (Previous != null && CurrentState != Function.Record && CurrentState != Function.Play)
            {
                panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_MOVE;
                panelBackground.Refresh();
                Previous(this, e);
                ResetBackground();
            }
        }

        /// <summary>
        /// Occurs when the view changed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-07</remarks>
        public event EventHandler ViewChanged;
        /// <summary>
        /// Raises the <see cref="E:ViewChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-07</remarks>
        protected virtual void OnViewChanged(EventArgs e)
        {
            if (ViewChanged != null)
                ViewChanged(this, e);
        }
        # endregion
    }
}
