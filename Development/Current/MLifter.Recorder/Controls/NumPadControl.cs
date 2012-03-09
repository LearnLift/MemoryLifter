using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MLifter.Recorder.Properties;

namespace MLifter.AudioTools
{
    /// <summary>
    /// The control to display the NumPad or a simplified view.
    /// </summary>
    public partial class NumPadControl : UserControl
    {
        # region Constructor and basic methods
        /// <summary>
        /// Initializes a new instance of the <see cref="NumPadControl"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public NumPadControl()
        {
            # region Load Standard-Key-Functions
            keyFunctions.Add(Keys.NumPad0, STANDART_FUNCTIONS.KEY_0);
            keyFunctions.Add(Keys.NumPad1, STANDART_FUNCTIONS.KEY_1);
            keyFunctions.Add(Keys.NumPad2, STANDART_FUNCTIONS.KEY_2);
            keyFunctions.Add(Keys.NumPad3, STANDART_FUNCTIONS.KEY_3);
            keyFunctions.Add(Keys.NumPad4, STANDART_FUNCTIONS.KEY_4);
            keyFunctions.Add(Keys.NumPad5, STANDART_FUNCTIONS.KEY_5);
            keyFunctions.Add(Keys.NumPad6, STANDART_FUNCTIONS.KEY_6);
            keyFunctions.Add(Keys.NumPad7, STANDART_FUNCTIONS.KEY_7);
            keyFunctions.Add(Keys.NumPad8, STANDART_FUNCTIONS.KEY_8);
            keyFunctions.Add(Keys.NumPad9, STANDART_FUNCTIONS.KEY_9);
            keyFunctions.Add(Keys.Decimal, STANDART_FUNCTIONS.KEY_COMMA);
            keyFunctions.Add(Keys.Enter, STANDART_FUNCTIONS.KEY_ENTER);
            keyFunctions.Add(Keys.Add, STANDART_FUNCTIONS.KEY_PLUS);
            keyFunctions.Add(Keys.Subtract, STANDART_FUNCTIONS.KEY_MINUS);
            keyFunctions.Add(Keys.Multiply, STANDART_FUNCTIONS.KEY_MULTIPLY);
            keyFunctions.Add(Keys.Divide, STANDART_FUNCTIONS.KEY_DIVIDE);

            keyboardFunctions.Add(Keys.Space, STANDART_FUNCTIONS.KEY_SPACE);
            keyboardFunctions.Add(Keys.C, STANDART_FUNCTIONS.KEY_C);
            keyboardFunctions.Add(Keys.V, STANDART_FUNCTIONS.KEY_V);
            keyboardFunctions.Add(Keys.B, STANDART_FUNCTIONS.KEY_B);
            keyboardFunctions.Add(Keys.N, STANDART_FUNCTIONS.KEY_N);
            keyboardFunctions.Add(Keys.M, STANDART_FUNCTIONS.KEY_M);

            presenterKeyFunctions.Add(STANDART_KEYS.PRESENTER_RECORD1, Function.Record);
            presenterKeyFunctions.Add(STANDART_KEYS.PRESENTER_RECORD2, Function.Record);
            presenterKeyFunctions.Add(STANDART_KEYS.PRESENTER_BACK, Function.Backward);
            presenterKeyFunctions.Add(STANDART_KEYS.PRESENTER_NEXT, Function.Forward);
            # endregion
            # region Set NumLock
            short keyState = GetKeyState((int)Keys.NumLock);
            int state = keyState & 1;
            if (state != 1)
                keybd_event((int)Keys.NumLock, 0, 0, 0);
            # endregion

            InitializeComponent();
            panelBackground.Dock = DockStyle.Fill;
            tableLayoutPanelAdvancedView.BringToFront();

            # region Set button tags
            button0.Tag = Keys.NumPad0;
            button1.Tag = Keys.NumPad1;
            button2.Tag = Keys.NumPad2;
            button3.Tag = Keys.NumPad3;
            button4.Tag = Keys.NumPad4;
            button5.Tag = Keys.NumPad5;
            button6.Tag = Keys.NumPad6;
            button7.Tag = Keys.NumPad7;
            button8.Tag = Keys.NumPad8;
            button9.Tag = Keys.NumPad9;
            buttonComma.Tag = Keys.Decimal;
            buttonDivide.Tag = Keys.Divide;
            buttonEnter.Tag = Keys.Enter;
            buttonMinus.Tag = Keys.Subtract;
            buttonMultiply.Tag = Keys.Multiply;
            buttonNum.Tag = Keys.NumLock;
            buttonPlus.Tag = Keys.Add;

            buttonB.Tag = Keys.B;
            buttonC.Tag = Keys.C;
            buttonM.Tag = Keys.M;
            buttonN.Tag = Keys.N;
            buttonSpace.Tag = Keys.Space;
            buttonV.Tag = Keys.V;
            # endregion
            DoubleBuffered = true;
        }

        /// <summary>
        /// Handles the Load event of the NumPadControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-13</remarks>
        private void NumPadControl_Load(object sender, EventArgs e)
        {
            keyboardHook = new UserActivityHook(false, false);
            keyboardHook.KeyUp += new KeyEventHandler(NumPadControl_KeyUp);
            Disposed += new EventHandler(NumPadControl_Disposed);
        }

        /// <summary>
        /// Installs the hook.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-13</remarks>
        public void InstallHook()
        {
            try
            {
                keyboardHook.Start(false, true);
            }
            catch (Exception exp)
            {
                System.Diagnostics.Trace.WriteLine("InstallHook Exception: " + exp.Message);
            }
        }

        /// <summary>
        /// Uninstalls the hook.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-13</remarks>
        public void UninstallHook()
        {
            try
            {
                keyboardHook.Stop(true, true, false);
            }
            catch (Exception exp)
            {
                System.Diagnostics.Trace.WriteLine("UninstallHook Exception: " + exp.Message);
            }
        }

        /// <summary>
        /// Handles the Disposed event of the NumPadControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-13</remarks>
        void NumPadControl_Disposed(object sender, EventArgs e)
        {
            if (Disposing)
                keyboardHook.Stop();
        }

        /// <summary>
        /// Sets the stop button.
        /// </summary>
        /// <param name="playingOrRecording">Set to <c>true</c> if playing or recording.</param>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        public void SetStopButton()
        {
            Keys[] keys = new Keys[keyFunctions.Keys.Count];
            keyFunctions.Keys.CopyTo(keys, 0);

            foreach (Keys key in keys)
            {
                if (CurrentState == Function.Play)
                {
                    if (keyFunctions[key] == Function.Play)
                        keyFunctions[key] = Function.StopPlaying;
                }
                else if (CurrentState == Function.Record)
                {
                    if (keyFunctions[key] == Function.Record)
                        keyFunctions[key] = Function.StopRecording;
                }
                else if (keyFunctions[key] == Function.StopPlaying)
                    keyFunctions[key] = Function.Play;
                else if (keyFunctions[key] == Function.StopRecording)
                    keyFunctions[key] = Function.Record;
            }

            keys = new Keys[keyboardFunctions.Keys.Count];
            keyboardFunctions.Keys.CopyTo(keys, 0);

            foreach (Keys key in keys)
            {
                if (CurrentState == Function.Play)
                {
                    if (keyboardFunctions[key] == Function.Play)
                        keyboardFunctions[key] = Function.StopPlaying;
                }
                else if (CurrentState == Function.Record)
                {
                    if (keyboardFunctions[key] == Function.Record)
                        keyboardFunctions[key] = Function.StopRecording;
                }
                else if (keyboardFunctions[key] == Function.StopPlaying)
                    keyboardFunctions[key] = Function.Play;
                else if (keyboardFunctions[key] == Function.StopRecording)
                    keyboardFunctions[key] = Function.Record;
            }

            Invoke((EmtyDelegate)Refresh);
        }

        /// <summary>
        /// Handles the Paint event of the button controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        private void button_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        /// <summary>
        /// The <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            # region Background-Color
            if (panelBackground.BackColor != STANDARD_APPEARANCE.COLOR_MOVE)
            {
                switch (CurrentState)
                {
                    case Function.Nothing:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_STOP;
                        break;
                    case Function.Play:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_PLAY;
                        break;
                    case Function.Record:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_RECORD;
                        break;
                    default:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_STOP;
                        break;
                }
            }
            # endregion
            # region Advanced View
            if (AdvancedView && !ImagesValid)
            {
                ImagesValid = true;
                pictureBoxSimpleView.Image = null;

                if (!KeyboardLayout)
                {
                    foreach (Keys key in keyFunctions.Keys)
                    {
                        Button button = GetKeyButton(key);
                        button.Text = "";
                        button.BackgroundImage = GetFunctionImage(keyFunctions[key]);

                        if (button.BackgroundImage != null)
                        {
                            Image background = new Bitmap(button.Width, button.Height);
                            Graphics g = Graphics.FromImage(background);
                            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                            if (key == Keys.Enter || key == Keys.Add)
                            {
                                g.DrawImage(Resources.long_green_vertical, 0, 0, background.Width, background.Height);
                                g.DrawImage(button.BackgroundImage, 0, background.Height / 2 - background.Width / 2, background.Width, background.Width);
                            }
                            else if (key == Keys.NumPad0)
                            {
                                g.DrawImage(Resources.long_green_horizontal, 0, 0, background.Width, background.Height);
                                g.DrawImage(button.BackgroundImage, background.Width / 2 - background.Height / 2, 0, background.Height, background.Height);
                            }
                            else
                            {
                                g.DrawImage(Resources.small_green, 0, 0, background.Width, background.Height);
                                g.DrawImage(button.BackgroundImage, 0, 0, background.Width, background.Height);
                            }
                            button.BackgroundImage = background;

                            if ((!mediaFileAvailable && KeyFunctions[key] == Function.Play) ||
                                CurrentState == Function.Play && KeyFunctions[key] != Function.StopPlaying ||
                                CurrentState == Function.Record && KeyFunctions[key] != Function.StopRecording)
                                button.Enabled = false;
                            else
                                button.Enabled = true;

                            g.Dispose();
                        }
                        else
                        {
                            if (key == Keys.Enter || key == Keys.Add)
                                button.BackgroundImage = Resources.long_grey_vertical;
                            else if (key == Keys.NumPad0)
                                button.BackgroundImage = Resources.long_grey_horizontal;
                            else
                                button.BackgroundImage = Resources.small_grey;

                            button.Enabled = false;
                            button.Text = button.Name.Replace("button", "").Replace("Divide", "�").Replace("Plus", "+").Replace("Minus", "-").Replace("Multiply", "*");
                        }
                    }

                    if (GetKeyButton(SwitchKey1) != null)
                    {
                        Button button = GetKeyButton(SwitchKey1);
                        button.BackgroundImage = ButtonSwitchImage;
                        Image background = new Bitmap(button.Width, button.Height);
                        Graphics g = Graphics.FromImage(background);
                        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(Resources.small_green, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, 0, 0, background.Width, background.Height);
                        button.BackgroundImage = background;

                        g.Dispose();
                    }
                    GetKeyButton(SwitchKey1).Text = "";
                }
                else
                {
                    foreach (Keys key in keyboardFunctions.Keys)
                    {
                        Button button = GetKeyButton(key);
                        button.Text = "";
                        button.BackgroundImage = GetFunctionImage(keyboardFunctions[key]);

                        if (button.BackgroundImage != null)
                        {
                            Image background = new Bitmap(button.Width, button.Height);
                            Graphics g = Graphics.FromImage(background);
                            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                            if (key == Keys.Space)
                            {
                                g.DrawImage(Resources.spacer_green, 0, 0, background.Width, background.Height);
                                g.DrawImage(button.BackgroundImage, background.Width / 2 - background.Height / 2, 0, background.Height, background.Height);
                            }
                            else
                            {
                                g.DrawImage(Resources.small_green, 0, 0, background.Width, background.Height);
                                g.DrawImage(button.BackgroundImage, 0, 0, background.Width, background.Height);
                            }
                            button.BackgroundImage = background;

                            if ((!mediaFileAvailable && KeyboardFunctions[key] == Function.Play) ||
                                CurrentState == Function.Play && KeyboardFunctions[key] != Function.StopPlaying ||
                                CurrentState == Function.Record && KeyboardFunctions[key] != Function.StopRecording)
                                button.Enabled = false;
                            else
                                button.Enabled = true;

                            g.Dispose();
                        }
                        else
                        {
                            if (key == Keys.Space)
                                button.BackgroundImage = Resources.spacer_grey;
                            else
                                button.BackgroundImage = Resources.small_grey;

                            button.Enabled = false;
                            button.Text = button.Name.Replace("button", "").Replace("Space", "");
                        }
                    }
                }
            }
            # endregion
            base.OnPaint(e);
        }

        /// <summary>
        /// Resets the background.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-09</remarks>
        private void ResetBackground()
        {
            Thread.Sleep(STANDARD_APPEARANCE.COLOR_MOVE_TIME);
            SetBackColor(STANDARD_APPEARANCE.COLOR_STOP);
        }

        /// <summary>
        /// Sets the color of the background.
        /// </summary>
        /// <param name="backColor">Color of the background.</param>
        /// <remarks>Documented by Dev05, 2007-08-09</remarks>
        private void SetBackColor(Color backColor)
        {
            if (!InvokeRequired)
                panelBackground.BackColor = backColor;
            else
                Invoke((ColorDelegate)SetBackColor, backColor);
        }

        /// <summary>
        /// Handles the BackColorChanged event of the panelBackground control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-11</remarks>
        private void panelBackground_BackColorChanged(object sender, EventArgs e)
        {
            pictureBoxSimpleView.BackColor = panelBackground.BackColor;
        }

        /// <summary>
        /// Handles the Click event of the numPad control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-22</remarks>
        private void numPad_Click(object sender, EventArgs e)
        {
            NumPadControl_KeyUp(sender, new KeyEventArgs((Keys)((Button)sender).Tag));
        }

        /// <summary>
        /// Handles the FocusEnter event of the numPad control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-22</remarks>
        private void numPad_FocusEnter(object sender, EventArgs e)
        {
            panelBackground.Focus();
        }

        /// <summary>
        /// Handles the KeyUpEvent event of the NumPadControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        private void NumPadControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (FunctionsEnabled)
            {
                if (e.KeyCode == SwitchKey1 || e.KeyCode == SwitchKey2 || e.KeyCode == SwitchKey3)
                {
                    AdvancedView = !AdvancedView;
                }
                else
                {
                    # region Presenter
                    if (presenterActivated && PresenterKeyFunctions.ContainsKey(e.KeyCode))
                        GetFunctionDelegate(PresenterKeyFunctions[e.KeyCode]).Invoke(EventArgs.Empty);
                    # endregion
                    # region Advanced View
                    else if (advancedView)
                    {
                        if (keyFunctions.ContainsKey(e.KeyCode))
                        {
                            if (keyFunctions[e.KeyCode] != Function.Nothing)
                                GetFunctionDelegate(keyFunctions[e.KeyCode]).Invoke(EventArgs.Empty);
                        }
                        else if (keyboardFunctions.ContainsKey(e.KeyCode))
                        {
                            if (keyboardFunctions[e.KeyCode] != Function.Nothing)
                                GetFunctionDelegate(keyboardFunctions[e.KeyCode]).Invoke(EventArgs.Empty);
                        }
                        else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
                            OnPrevious(EventArgs.Empty);
                        else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
                            OnNext(EventArgs.Empty);
                    }
                    # endregion
                    # region Simple View
                    else
                    {
                        if (e.KeyCode == RecordKey1 || e.KeyCode == RecordKey2)
                        {
                            if (CurrentState != Function.Record && CurrentState != Function.Play)
                                OnRecord(EventArgs.Empty);
                            else
                                OnStopRecord(EventArgs.Empty);
                        }
                        if (!LockFunctionsInSimpleView)
                        {
                            if (e.KeyCode == PlayKey)
                                OnPlay(EventArgs.Empty);
                            else if (e.KeyCode == NextKey)
                                OnNext(EventArgs.Empty);
                            else if (e.KeyCode == BackKey)
                                OnPrevious(EventArgs.Empty);
                        }
                    }
                    # endregion

                    panelBackground.Refresh();
                }
            }
        }

        /// <summary>
        /// Redraws this instance.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-12</remarks>
        public void Redraw()
        {
            pictureBoxSimpleView.Refresh();
            tableLayoutPanelKeyboard.Refresh();
            tableLayoutPanelAdvancedView.Refresh();
        }
        # endregion
        # region Dictionary methods
        /// <summary>
        /// Gets the delegate to the proper function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns>The proper delegate.</returns>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public EventDelegate GetFunctionDelegate(Function function)
        {
            switch (function)
            {
                case Function.Backward:
                    return OnPrevious;
                case Function.Forward:
                    return OnNext;
                case Function.Nothing:
                    return null;
                case Function.Play:
                    if (CurrentState == Function.Play)
                        return OnStopPlay;
                    return OnPlay;
                case Function.StopRecording:
                    return OnStopRecord;
                case Function.StopPlaying:
                    return OnStopPlay;
                case Function.Record:
                    if (CurrentState == Function.Record)
                        return OnStopRecord;
                    return OnRecord;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the function to the proper delegate.
        /// </summary>
        /// <param name="eventDelegate">The event delegate.</param>
        /// <returns>The proper function.</returns>
        /// <remarks>Documented by Dev05, 2007-08-02</remarks>
        public Function GetDelegateFunction(EventDelegate eventDelegate)
        {
            if (eventDelegate == OnPrevious)
                return Function.Backward;
            if (eventDelegate == OnNext)
                return Function.Forward;
            if (eventDelegate == OnPlay)
                return Function.Play;
            if (eventDelegate == OnRecord)
                return Function.Record;
            if (eventDelegate == OnStopRecord)
                return Function.StopRecording;
            if (eventDelegate == OnStopPlay)
                return Function.StopPlaying;

            return Function.Nothing;
        }

        /// <summary>
        /// Gets the button to the coresponding key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The button.</returns>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        private Button GetKeyButton(Keys key)
        {
            switch (key)
            {
                case Keys.NumPad0:
                    return button0;
                case Keys.NumPad1:
                    return button1;
                case Keys.NumPad2:
                    return button2;
                case Keys.NumPad3:
                    return button3;
                case Keys.NumPad4:
                    return button4;
                case Keys.NumPad5:
                    return button5;
                case Keys.NumPad6:
                    return button6;
                case Keys.NumPad7:
                    return button7;
                case Keys.NumPad8:
                    return button8;
                case Keys.NumPad9:
                    return button9;
                case Keys.Decimal:
                    return buttonComma;
                case Keys.Enter:
                    return buttonEnter;
                case Keys.Add:
                    return buttonPlus;
                case Keys.Subtract:
                    return buttonMinus;
                case Keys.Multiply:
                    return buttonMultiply;
                case Keys.Divide:
                    return buttonDivide;
                case Keys.NumLock:
                    return buttonNum;
                case Keys.Space:
                    return buttonSpace;
                case Keys.C:
                    return buttonC;
                case Keys.V:
                    return buttonV;
                case Keys.B:
                    return buttonB;
                case Keys.N:
                    return buttonN;
                case Keys.M:
                    return buttonM;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the image to the proper function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns>The image.</returns>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        private Image GetFunctionImage(Function function)
        {
            switch (function)
            {
                case Function.Record:
                    return CurrentState == Function.Play ? ButtonRecordImageNotAvailable : ButtonRecordImage;
                case Function.Play:
                    return (CurrentState == Function.Record || !mediaFileAvailable) ? ButtonPlayImageNotAvailable : ButtonPlayImage;
                case Function.Forward:
                    return (CurrentState == Function.Play || CurrentState == Function.Record) ? ButtonNextImageNotAvailable : ButtonNextImage;
                case Function.Backward:
                    return (CurrentState == Function.Play || CurrentState == Function.Record) ? ButtonBackImageNotAvailable : ButtonBackImage;
                case Function.StopRecording:
                    return ButtonStopImage;
                case Function.StopPlaying:
                    return ButtonStopImage;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Handles the Paint event of the panelBackground control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-22</remarks>
        private void panelBackground_Paint(object sender, PaintEventArgs e)
        {
            if (panelBackground.BackColor != STANDARD_APPEARANCE.COLOR_MOVE)
            {
                switch (CurrentState)
                {
                    case Function.Nothing:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_STOP;
                        break;
                    case Function.Play:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_PLAY;
                        break;
                    case Function.Record:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_RECORD;
                        break;
                    default:
                        panelBackground.BackColor = STANDARD_APPEARANCE.COLOR_STOP;
                        break;
                }
            }

            base.OnPaint(e);
        }
        # endregion
        # region ToolTips
        ToolTip toolTip = new ToolTip();
        /// <summary>
        /// Handles the MouseEnter event of the buttonDivide control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-11</remarks>
        private void buttonDivide_MouseEnter(object sender, EventArgs e)
        {
            Keys key = (Keys)((Button)sender).Tag;

            if (toolTip.GetToolTip((Button)sender).Length <= 0 && key != SwitchKey1)
            {
                if (KeyFunctions.ContainsKey((Keys)((Button)sender).Tag))
                    toolTip.SetToolTip((Button)sender, KeyFunctions[(Keys)((Button)sender).Tag].ToString().Replace("Stop", "Stop "));
                else
                    toolTip.SetToolTip((Button)sender, KeyboardFunctions[(Keys)((Button)sender).Tag].ToString().Replace("Stop", "Stop "));
            }

            if (advancedView)
            {
                Button button = GetKeyButton(key);
                button.Text = "";

                if (key == SwitchKey1)
                    button.BackgroundImage = ButtonSwitchImage;
                else
                {
                    if (KeyboardLayout)
                        button.BackgroundImage = GetFunctionImage(keyboardFunctions[key]);
                    else
                        button.BackgroundImage = GetFunctionImage(keyFunctions[key]);
                }

                if (button.BackgroundImage != null)
                {
                    Image background = new Bitmap(button.Width, button.Height);
                    Graphics g = Graphics.FromImage(background);
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    if (key == Keys.Enter || key == Keys.Add)
                    {
                        g.DrawImage(Resources.long_orange_vertical, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, 0, background.Height / 2 - background.Width / 2, background.Width, background.Width);
                    }
                    else if (key == Keys.NumPad0)
                    {
                        g.DrawImage(Resources.long_orange_horizontal, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, background.Width / 2 - background.Height / 2, 0, background.Height, background.Height);
                    }
                    else if (key == Keys.Space)
                    {
                        g.DrawImage(Resources.spacer_orange, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, background.Width / 2 - background.Height / 2, 0, background.Height, background.Height);
                    }
                    else
                    {
                        g.DrawImage(Resources.small_orange, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, 0, 0, background.Width, background.Height);
                    }
                    button.BackgroundImage = background;

                    g.Dispose();
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the buttonDivide control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-11</remarks>
        private void buttonDivide_MouseLeave(object sender, EventArgs e)
        {
            toolTip.RemoveAll();

            if (advancedView)
            {
                Keys key = (Keys)((Button)sender).Tag;
                Button button = GetKeyButton(key);
                button.Text = "";

                if (key == SwitchKey1)
                    button.BackgroundImage = ButtonSwitchImage;
                else
                {
                    if (KeyboardLayout)
                        button.BackgroundImage = GetFunctionImage(keyboardFunctions[key]);
                    else
                        button.BackgroundImage = GetFunctionImage(keyFunctions[key]);
                }

                if (button.BackgroundImage != null)
                {
                    Image background = new Bitmap(button.Width, button.Height);
                    Graphics g = Graphics.FromImage(background);
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    if (key == Keys.Enter || key == Keys.Add)
                    {
                        g.DrawImage(Resources.long_green_vertical, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, 0, background.Height / 2 - background.Width / 2, background.Width, background.Width);
                    }
                    else if (key == Keys.NumPad0)
                    {
                        g.DrawImage(Resources.long_green_horizontal, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, background.Width / 2 - background.Height / 2, 0, background.Height, background.Height);
                    }
                    else if (key == Keys.Space)
                    {
                        g.DrawImage(Resources.spacer_green, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, background.Width / 2 - background.Height / 2, 0, background.Height, background.Height);
                    }
                    else
                    {
                        g.DrawImage(Resources.small_green, 0, 0, background.Width, background.Height);
                        g.DrawImage(button.BackgroundImage, 0, 0, background.Width, background.Height);
                    }
                    button.BackgroundImage = background;

                    g.Dispose();
                }
            }
        }
        # endregion
    }
}
