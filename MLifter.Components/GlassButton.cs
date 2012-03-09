/************************************************************************************************
 * GlassButton - How to create an animating glass button using only GDI+ (and not using WPF).   *
 *                                                                                              *
 * Original developed by Łukasz Świątkowski - lukasz.swiatkowski@gmail.com                      *
 * Form-/Perfomance-/Behavior-Improvements by Fink Christoph - fink.christoph@gmail.com         *
 *                                                                                              *
 * Feel free to use this control in your application or to improve it in any way!               *
 ***********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using PushButtonState = System.Windows.Forms.VisualStyles.PushButtonState;

namespace MLifter.Components
{
    /// <summary>
    /// Represents a glass button control.
    /// </summary>
    [ToolboxBitmap(typeof(System.Windows.Forms.Button)), ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms"), Description("Raises an event when the user clicks it.")]
    public partial class GlassButton : Button
    {
        # region " Global Vareables "

        # region " Vareables for Drawing "

        GraphicsPath outerBorderPath;
        GraphicsPath ContentPath;
        GraphicsPath GlowClip;
        GraphicsPath GlowBottomRadial;
        GraphicsPath ShinePath;
        GraphicsPath BorderPath;

        PathGradientBrush GlowRadialPath;

        LinearGradientBrush ShineBrush;

        Pen outerBorderPen;
        Pen BorderPen;

        Color specialSymbolColor;
        Color specialSymbolColorDisabled;
        Color specialSymbolColorEnabled;

        Brush specialSymbolBrush;
        Brush ContentBrush;

        Rectangle rect;
        Rectangle rect2;

        # endregion

        /// <summary>
        /// The ToolTip of the Control.
        /// </summary>
        ToolTip toolTip = new ToolTip();

        /// <summary>
        /// If false, the shine isn't drawn (-> symbolizes an disabled control).
        /// </summary>
        bool drawShine = true;

        # endregion

        #region " Constructors "

        /// <summary>
        /// Initializes a new instance of the <see cref="Glass.GlassButton" /> class.
        /// </summary>
        public GlassButton()
        {
            DoubleBuffered = true;

            InitializeComponent();

            timer.Interval = animationLength / framesCount;
            base.BackColor = Color.Transparent;
            BackColor = Color.Black;
            ForeColor = Color.White;
            OuterBorderColor = Color.White;
            InnerBorderColor = Color.Black;
            ShineColor = Color.White;
            GlowColor = Color.FromArgb(unchecked((int)(0xFF8DBDFF)));
            alternativeForm = false;
            showFocusBorder = false;
            animateGlow = false;
            showSpecialSymbol = false;
            toolTipText = "";
            specialSymbolBrush = new SolidBrush(Color.FromArgb(transperencyFactor, specialSymbolColor));
            alternativeFocusBorderColor = Color.Black;
            alternativeFormDirection = Direction.Left;
            roundCorner = 6;

            RecalcRect((float)currentFrame / (framesCount - 1f));

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Opaque, false);

            this.SizeChanged += new EventHandler(GlassButton_SizeChanged);
            this.MouseEnter += new EventHandler(GlassButton_MouseEnter);
            this.MouseLeave += new EventHandler(GlassButton_MouseLeave);
            this.GotFocus += new EventHandler(GlassButton_GotFocus);
            this.LostFocus += new EventHandler(GlassButton_LostFocus);
        }

        #endregion

        #region " Fields and Properties "

        /// <summary>
        /// Set the transperency of the special Symbols.
        /// </summary>
        private int transperencyFactor = 128;
        /// <summary>
        /// Gets or sets the special symbol transperency.
        /// </summary>
        /// <value>The special symbol transperency.</value>
        /// <remarks>Documented by Dev05, 2009-04-27</remarks>
        [DefaultValue(128), Category("Appearance"), Description("The transperency of the special Symbol.")]
        public int SpecialSymbolTransperency
        {
            get { return transperencyFactor; }
            set
            {
                if (value < 0 || value > 255)
                    throw new ArgumentException("Value must be between 0 and 255!");
                transperencyFactor = value;
                Invalidate();
            }
        }

        private Color focusColor;
        /// <summary>
        /// Gets or sets the glow color of the control.
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> value representing the glow color.</returns>
        [DefaultValue(typeof(Color), null), Category("Appearance"), Description("The glow color of the control.")]
        public Color FocusColor
        {
            get { return focusColor; }
            set
            {
                if (focusColor != value)
                {
                    focusColor = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }
        private Image imageDisabled;
        [DefaultValue(typeof(Image), null), Category("Appearance-Images"), Description("The Image for the disabled control")]
        public Image ImageDisabled
        {
            get { return imageDisabled; }
            set { imageDisabled = value; }
        }
        private Image imageEnabled;
        [DefaultValue(typeof(Image), null), Category("Appearance-Images"), Description("The Image for the enabled control")]
        public Image ImageEnabled
        {
            get { return imageEnabled; }
            set { imageEnabled = value; }
        }
        private Color backColor;
        /// <summary>
        /// Gets or sets the background color of the control.
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> value representing the background color.</returns>
        [DefaultValue(typeof(Color), "Black")]
        public new Color BackColor
        {
            get { return backColor; }
            set
            {
                if (!backColor.Equals(value))
                {
                    backColor = value;
                    UseVisualStyleBackColor = false;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        /// <returns>The foreground <see cref="T:System.Drawing.Color" /> of the control.</returns>
        [DefaultValue(typeof(Color), "White")]
        public new Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;

                RecalcRect((float)currentFrame / (framesCount - 1f));
            }
        }

        private Color innerBorderColor;
        /// <summary>
        /// Gets or sets the inner border color of the control.
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> value representing the color of the inner border.</returns>
        [DefaultValue(typeof(Color), "Black"), Category("Appearance"), Description("The inner border color of the control.")]
        public Color InnerBorderColor
        {
            get { return innerBorderColor; }
            set
            {
                if (innerBorderColor != value)
                {
                    innerBorderColor = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the special symbol.
        /// </summary>
        /// <value>The color of the special symbol.</value>
        [DefaultValue(typeof(Color), "White"), Category("Appearance - SpecialSymbol"), Description("The inner border color of the control.")]
        public Color SpecialSymbolColor
        {
            get { return specialSymbolColor; }
            set
            {
                if (specialSymbolColor != value)
                {
                    specialSymbolColor = value;
                    specialSymbolBrush = new SolidBrush(Color.FromArgb(transperencyFactor, specialSymbolColor));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }
        [DefaultValue(typeof(Color), null), Category("Appearance - SpecialSymbol"), Description("The inner border color of the control - disabled")]
        public Color SpecialSymbolColorDisabled
        {
            get { return specialSymbolColorDisabled; }
            set
            {
                if (specialSymbolColorDisabled != value)
                {
                    specialSymbolColorDisabled = value;
                    specialSymbolBrush = new SolidBrush(Color.FromArgb(transperencyFactor, specialSymbolColorDisabled));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }
        [DefaultValue(typeof(Color), null), Category("Appearance - SpecialSymbol"), Description("The inner border color of the control.")]
        public Color SpecialSymbolColorEnabled
        {
            get { return specialSymbolColorEnabled; }
            set
            {
                if (specialSymbolColorEnabled != value)
                {
                    specialSymbolColorEnabled = value;
                    specialSymbolBrush = new SolidBrush(Color.FromArgb(transperencyFactor, specialSymbolColorEnabled));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private int roundCorner;
        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        [DefaultValue(6), Category("Appearance"), Description("The radius of the corners.")]
        public int CornerRadius
        {
            get { return roundCorner; }
            set
            {
                if (roundCorner != value)
                {
                    roundCorner = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        string toolTipText;
        /// <summary>
        /// Gets or sets the tool tip text.
        /// </summary>
        /// <value>The tool tip text.</value>
        [DefaultValue(""), Category("Appearance"), Description("The ToolTip-Text of the button. Leave blank to not show a ToolTip."), Localizable(true)]
        public string ToolTipText
        {
            get { return toolTipText; }
            set
            {
                if (toolTipText != value)
                {
                    toolTipText = value;

                    if (toolTipText.Length > 0)
                        toolTip.SetToolTip(this, toolTipText);

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private bool alternativeForm;
        /// <summary>
        /// Gets or sets the alternative form.
        /// </summary>
        /// <value>The alternative form.</value>
        [DefaultValue(false), Category("Appearance"), Description("Draws the Button in an alternative Form.")]
        public bool AlternativeForm
        {
            get { return alternativeForm; }
            set
            {
                if (alternativeForm != value)
                {
                    alternativeForm = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private bool animateGlow;
        /// <summary>
        /// Gets or sets a value indicating whether the glow is animated.
        /// </summary>
        /// <value><c>true</c> if glow is animated; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Category("Appearance"), Description("If true the glow is animated.")]
        public bool AnimateGlow
        {
            get { return animateGlow; }
            set
            {
                if (animateGlow != value)
                {
                    animateGlow = value;
                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private bool showSpecialSymbol;
        /// <summary>
        /// Gets or sets a value indicating whether a special symbol is drawn.
        /// </summary>
        /// <value><c>true</c> if special symbol is drawn; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Category("Appearance"), Description("If true, the selectet special symbol will be drawn on the button.")]
        public bool ShowSpecialSymbol
        {
            get { return showSpecialSymbol; }
            set
            {
                if (showSpecialSymbol != value)
                {
                    showSpecialSymbol = value;

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// List of all aviable special symbols.
        /// </summary>
        public enum SpecialSymbols
        {
            ArrowLeft,
            ArrowRight,
            ArrowUp,
            ArrowDown,
            Play,
            Pause,
            Stop,
            FastForward,
            Forward,
            Backward,
            FastBackward,
            Speaker,
            NoSpeaker,
            Repeat,
            RepeatAll,
            Shuffle
        }

        private SpecialSymbols specialSymbol;
        /// <summary>
        /// Gets or sets the special symbol.
        /// </summary>
        /// <value>The special symbol.</value>
        [DefaultValue(typeof(SpecialSymbols), "Play"), Category("Appearance"), Description("Sets the type of the special symbol on the button.")]
        public SpecialSymbols SpecialSymbol
        {
            get { return specialSymbol; }
            set
            {
                if (specialSymbol != value)
                {
                    specialSymbol = value;

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        public enum Direction
        {
            Left,
            Right
        }

        private Direction alternativeFormDirection;
        /// <summary>
        /// Gets or sets the alternative form direction.
        /// </summary>
        /// <value>The alternative form direction.</value>
        [DefaultValue(typeof(Direction), "Left"), Category("Appearance"), Description("Sets the Direction of the alternative Form.")]
        public Direction AlternativeFormDirection
        {
            get { return alternativeFormDirection; }
            set
            {
                if (alternativeFormDirection != value)
                {
                    alternativeFormDirection = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private bool showFocusBorder;
        /// <summary>
        /// Gets or sets a value indicating whether the focus border is shown.
        /// </summary>
        /// <value><c>true</c> if focus border shown; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Category("Appearance"), Description("Draw the normal Focus-Border. Alternativ Focus-Border will be drawed if false.")]
        public bool ShowFocusBorder
        {
            get { return showFocusBorder; }
            set
            {
                if (showFocusBorder != value)
                {
                    showFocusBorder = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private Color alternativeFocusBorderColor;
        /// <summary>
        /// Gets or sets the color of the alternative focus border.
        /// </summary>
        /// <value>The color of the alternative focus border.</value>
        [DefaultValue(typeof(Color), "Black"), Category("Appearance"), Description("The color of the alternative Focus-Border.")]
        public Color AlternativeFocusBorderColor
        {
            get { return alternativeFocusBorderColor; }
            set
            {
                if (alternativeFocusBorderColor != value)
                {
                    alternativeFocusBorderColor = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private Color outerBorderColor;
        /// <summary>
        /// Gets or sets the outer border color of the control.
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> value representing the color of the outer border.</returns>
        [DefaultValue(typeof(Color), "White"), Category("Appearance"), Description("The outer border color of the control.")]
        public Color OuterBorderColor
        {
            get { return outerBorderColor; }
            set
            {
                if (outerBorderColor != value)
                {
                    outerBorderColor = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private Color shineColor;
        /// <summary>
        /// Gets or sets the shine color of the control.
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> value representing the shine color.</returns>
        [DefaultValue(typeof(Color), "White"), Category("Appearance"), Description("The shine color of the control.")]
        public Color ShineColor
        {
            get { return shineColor; }
            set
            {
                if (shineColor != value)
                {
                    shineColor = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private Color glowColor;
        /// <summary>
        /// Gets or sets the glow color of the control.
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> value representing the glow color.</returns>
        [DefaultValue(typeof(Color), "255,141,189,255"), Category("Appearance"), Description("The glow color of the control.")]
        public Color GlowColor
        {
            get { return glowColor; }
            set
            {
                if (glowColor != value)
                {
                    glowColor = value;

                    RecalcRect((float)currentFrame / (framesCount - 1f));

                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private bool isHovered;
        private bool isFocused;
        private bool isFocusedByKey;
        private bool isKeyDown;
        private bool isMouseDown;
        private bool isPressed { get { return isKeyDown || (isMouseDown && isHovered); } }

        /// <summary>
        /// Gets the state of the button control.
        /// </summary>
        /// <value>The state of the button control.</value>
        [Browsable(false)]
        public PushButtonState State
        {
            get
            {
                if (!Enabled)
                {
                    return PushButtonState.Disabled;
                }
                if (isPressed)
                {
                    return PushButtonState.Pressed;
                }
                if (isHovered)
                {
                    return PushButtonState.Hot;
                }
                if (isFocused || IsDefault)
                {
                    return PushButtonState.Default;
                }
                return PushButtonState.Normal;
            }
        }

        #endregion

        #region " Overrided Methods "



        /// <summary>
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-17</remarks>
        protected override void OnEnabledChanged(EventArgs e)
        {
            if (imageEnabled != null && imageDisabled != null)
            {
                if (this.Enabled)
                {
                    this.Image = imageEnabled;
                }
                else
                {
                    this.Image = imageDisabled;
                }
            }
            else
            {
                if (this.Enabled)
                {
                    SpecialSymbolColor = specialSymbolColorEnabled;
                }
                else
                {
                    SpecialSymbolColor = specialSymbolColorDisabled;
                }

            }
            if (!this.Enabled)
            {
                isHovered = isFocused = isFocusedByKey = isKeyDown = isMouseDown = false;
                RecalcGlow((float)currentFrame / (framesCount - 1f));
                FadeOut();
            }
            
            base.OnEnabledChanged(e);
            Invalidate();
        }
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Click" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            isKeyDown = isMouseDown = false;
            base.OnClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Enter" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (focusColor != null)
            {
                isFocused = isFocusedByKey = true;
                RecalcGlow(1);
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Leave" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            isFocused = isFocusedByKey = isKeyDown = isMouseDown = false;
            RecalcGlow(0);
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event.
        /// </summary>
        /// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space)
            {
                isKeyDown = true;
                Invalidate();
            }
            base.OnKeyDown(kevent);
        }

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event.
        /// </summary>
        /// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (isKeyDown && kevent.KeyCode == Keys.Space)
            {
                isKeyDown = false;
                Invalidate();
            }
            base.OnKeyUp(kevent);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!isMouseDown && e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                isFocusedByKey = false;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                isMouseDown = false;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.Control.OnMouseMove(System.Windows.Forms.MouseEventArgs)" /> event.
        /// </summary>
        /// <param name="mevent">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            if (mevent.Button != MouseButtons.None)
            {
                if (!ClientRectangle.Contains(mevent.X, mevent.Y))
                {
                    if (isHovered)
                    {
                        isHovered = false;
                        Invalidate();
                    }
                }
                else if (!isHovered)
                {
                    isHovered = true;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            FadeIn();
            Invalidate();
            base.OnMouseEnter(e);
            toolTip.Active = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            FadeOut();
            Invalidate();
            base.OnMouseLeave(e);
            toolTip.Active = false;
        }

        #endregion

        #region " Painting "

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnPaint(System.Windows.Forms.PaintEventArgs)" /> event.
        /// </summary>
        /// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            SmoothingMode sm = pevent.Graphics.SmoothingMode;
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            pevent.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            DrawButtonBackground(pevent.Graphics);
            DrawForegroundFromButton(pevent);
            DrawButtonForeground(pevent.Graphics);

            pevent.Graphics.SmoothingMode = sm;
        }

        /// <summary>
        /// Draws the button background.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void DrawButtonBackground(Graphics g)
        {
            //white border
            g.DrawPath(outerBorderPen, outerBorderPath);

            //content
            g.FillPath(ContentBrush, ContentPath);

            //glow
            if ((isHovered || isAnimating) && !isPressed)
            {
                g.SetClip(GlowClip, CombineMode.Intersect);
                g.FillPath(GlowRadialPath, GlowBottomRadial);

                g.ResetClip();
            }

            //shine
            if (drawShine && Enabled)
            {
                g.FillPath(ShineBrush, ShinePath);
            }

            //black border
            g.DrawPath(BorderPen, BorderPath);

            //Draws the special Symbol
            if (showSpecialSymbol)
                DrawSpecialSymbol(g);
        }

        /// <summary>
        /// Draws the special symbol.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void DrawSpecialSymbol(Graphics g)
        {
            int offset = 15;
            int LineWidth = Width / 15;
            Pen pen = new Pen(specialSymbolBrush, Width / 8);
            pen.EndCap = LineCap.ArrowAnchor;
            Pen aPen = new Pen(specialSymbolBrush, Width / 4);
            aPen.EndCap = LineCap.ArrowAnchor;
            Pen sPen = new Pen(specialSymbolBrush, Width / 20);
            sPen.EndCap = LineCap.ArrowAnchor;
            Font font = new Font("Arial", LineWidth * 4, FontStyle.Bold);

            switch (specialSymbol)
            {
                # region " Arrow Left "
                case SpecialSymbols.ArrowLeft:
                    g.DrawLine(aPen, Width - Width / 5, Height / 2, Width / 8, Height / 2);
                    break;
                # endregion
                # region " Arrow Right "
                case SpecialSymbols.ArrowRight:
                    g.DrawLine(aPen, Width / 6, Height / 2, Width - Width / 8, Height / 2);
                    break;
                # endregion
                # region " Arrow Up "
                case SpecialSymbols.ArrowUp:
                    g.DrawLine(aPen, Width / 2, Height - Height / 5, Width / 2, Height / 8);
                    break;
                # endregion
                # region " Arrow Down "
                case SpecialSymbols.ArrowDown:
                    g.DrawLine(aPen, Width / 2, Height / 5, Width / 2, Height - Height / 8);
                    break;
                # endregion
                # region " Play "
                case SpecialSymbols.Play:
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 4 + Width / 20, Height / 4),
                        new Point(Width - Width / 4 + Width / 20, Height / 2),
                        new Point(Width / 4 + Width / 20, Height - Height / 4)});
                    break;
                # endregion
                # region " Pause "
                case SpecialSymbols.Pause:
                    g.FillRectangle(specialSymbolBrush, new Rectangle(Width / 4, Height / 4,
                        (Width / 2 - Width / 10) / 2, Height / 2));
                    g.FillRectangle(specialSymbolBrush, new Rectangle(Width / 2 + Width / 20, Height / 4,
                        (Width / 2 - Width / 10) / 2, Height / 2));
                    break;
                # endregion
                # region " Stop "
                case SpecialSymbols.Stop:
                    g.FillRectangle(specialSymbolBrush, new Rectangle(Width / 4 + Width / 20, Height / 4 + Height / 20,
                        Width / 2 - Width / 10, Height / 2 - Width / 10));
                    break;
                # endregion
                # region " FastForward "
                case SpecialSymbols.FastForward:
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 4, Height / 4),
                        new Point(Width / 2, Height / 2),
                        new Point(Width / 4, Height - Height / 4)});
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 2, Height / 4),
                        new Point(3 * Width / 4, Height / 2),
                        new Point(Width / 2, Height - Height / 4)});
                    g.FillRectangle(specialSymbolBrush, new Rectangle(3 * Width / 4, Height / 4,
                        Width / 12, Height / 2));
                    break;
                # endregion
                # region " Forward "
                case SpecialSymbols.Forward:
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 4 + Width / 12, Height / 4),
                        new Point(Width / 2 + Width / 12, Height / 2),
                        new Point(Width / 4 + Width / 12, Height - Height / 4)});
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 2 + Width / 12, Height / 4),
                        new Point(3 * Width / 4 + Width / 12, Height / 2),
                        new Point(Width / 2 + Width / 12, Height - Height / 4)});
                    break;
                # endregion
                # region " Backward "
                case SpecialSymbols.Backward:
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 4 - Width / 12, Height / 2),
                        new Point(Width / 2 - Width / 12, Height / 4),
                        new Point(Width / 2 - Width / 12, Height - Height / 4)});
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 2 - Width / 12, Height / 2),
                        new Point(3 * Width / 4 - Width / 12, Height / 4),
                        new Point(3 * Width / 4 - Width / 12, Height - Height / 4)});
                    break;
                # endregion
                # region " FastBackward "
                case SpecialSymbols.FastBackward:
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 4, Height / 2),
                        new Point(Width / 2, Height / 4),
                        new Point(Width / 2, Height - Height / 4)});
                    g.FillPolygon(specialSymbolBrush, new Point[3]{
                        new Point(Width / 2, Height / 2),
                        new Point(3 * Width / 4, Height / 4),
                        new Point(3 * Width / 4, Height - Height / 4)});
                    g.FillRectangle(specialSymbolBrush, new Rectangle(Width / 4 - Width / 12, Height / 4,
                        Width / 12, Height / 2));
                    break;
                # endregion
                # region " Speaker "
                case SpecialSymbols.Speaker:
                    g.DrawPolygon(new Pen(specialSymbolBrush, Width / 20), new Point[6] { 
                        new Point(Width / 2 - Width / 6 - Width / offset, Height / 4 + Height / 10),
                        new Point(Width / 2 - Width / offset, Height / 4 + Height / 10),
                        new Point(Width / 2 + Width / 5 - Width / offset, Height / 4),
                        new Point(Width / 2 + Width / 5 - Width / offset, 3 * Height / 4),
                        new Point(Width / 2 - Width / offset, 3 * Height / 4 - Height / 10),
                        new Point(Width / 2 - Width / 6 - Width / offset, 3 * Height / 4 - Height / 10)});
                    g.DrawLine(new Pen(specialSymbolBrush, Width / 20), Width / 2 - Width / offset,
                        Height / 4 + Height / 10 + Width / 40, Width / 2 - Width / offset, Height - (Height / 4 + Height / 10 + Width / 40));
                    break;
                # endregion
                # region " NoSpeaker "
                case SpecialSymbols.NoSpeaker:
                    g.DrawPolygon(new Pen(specialSymbolBrush, Width / 20), new Point[6] { 
                        new Point(Width / 2 - Width / 6 - Width / offset, Height / 4 + Height / 10),
                        new Point(Width / 2 - Width / offset, Height / 4 + Height / 10),
                        new Point(Width / 2 + Width / 5 - Width / offset, Height / 4),
                        new Point(Width / 2 + Width / 5 - Width / offset, 3 * Height / 4),
                        new Point(Width / 2 - Width / offset, 3 * Height / 4 - Height / 10),
                        new Point(Width / 2 - Width / 6 - Width / offset, 3 * Height / 4 - Height / 10)});
                    g.DrawLine(new Pen(specialSymbolBrush, Width / 20), Width / 2 - Width / offset,
                        Height / 4 + Height / 10 + Width / 40, Width / 2 - Width / offset, Height - (Height / 4 + Height / 10 + Width / 40));
                    g.DrawLine(new Pen(specialSymbolBrush, Width / 20), (int)(Width / 2 - Width / 3.5 - Width / offset), 3 * Height / 4 - Height / 10,
                        Width / 2 + Width / 3 - Width / offset, Height / 4 + Height / 12 + Width / 40);
                    break;
                # endregion
                # region " Repeat "
                case SpecialSymbols.Repeat:
                    g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
                        new Point((int)(Width / 4), (int)(Height / 3)),
                        new Point((int)(Width - Width / 2.4), (int)(Height / 3)));
                    g.DrawArc(new Pen(specialSymbolBrush, LineWidth), (int)(Width - Width * 0.6), (int)(Height / 3),
                        (int)(Width / 3), (int)(Height / 3), 270, 180);
                    g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
                        new Point((int)(Width - Width / 2.4), (int)(Height - Height / 3)),
                        new Point((int)(Width / 3.2), (int)(Height - Height / 3)));
                    g.DrawLine(pen,
                        new Point((int)(Width / 3.2), (int)(Height - Height / 3)),
                        new Point((int)(Width / 4), (int)(Height - Height / 3)));
                    break;
                # endregion
                # region " RepeatAll "
                case SpecialSymbols.RepeatAll:
                    g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
                        new Point((int)(Width / 2.4), (int)(Height / 3)),
                        new Point((int)(Width - Width / 2.4), (int)(Height / 3)));
                    g.DrawArc(new Pen(specialSymbolBrush, LineWidth), (int)(Width - Width * 0.6), (int)(Height / 3),
                        (int)(Width / 3), (int)(Height / 3), 270, 180);
                    g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
                        new Point((int)(Width - Width / 2.4), (int)(Height - Height / 3)),
                        new Point((int)(Width / 2.4), (int)(Height - Height / 3)));
                    g.DrawLine(pen,
                        new Point((int)(Width / 2.4), (int)(Height - Height / 3)),
                        new Point((int)(Width / 3), (int)(Height - Height / 3)));
                    g.DrawArc(new Pen(specialSymbolBrush, LineWidth), (int)(Width / 4), (int)(Height / 3),
                        (int)(Width / 3), (int)(Height / 3), 90, 180);
                    break;
                # endregion
                # region " Shuffle "
                case SpecialSymbols.Shuffle:
                    g.DrawString("1", font, specialSymbolBrush, (Width / 2) / 4, Height / 2 - LineWidth * 2);
                    int sWidth = (int)g.MeasureString("2", font).Width;
                    int sHeigth = (int)g.MeasureString("2", font).Height;
                    g.DrawString("2", font, specialSymbolBrush, Width / 2 - sWidth / 2 - Width / (2 * offset), Height - LineWidth - sHeigth);
                    sWidth = (int)g.MeasureString("3", font).Width;
                    g.DrawString("3", font, specialSymbolBrush, Width - (Width / 2) / 4 - sWidth - Width / (2 * offset), Height / 2 - LineWidth * 2);
                    g.DrawArc(pen, (Width / 2) / 2, Height / 6, Width - (Width / 2), (int)(Height / 2.2), 170, 210);
                    break;
                # endregion
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the button foreground.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void DrawButtonForeground(Graphics g)
        {
            if (ShowFocusBorder && Focused && ShowFocusCues && !alternativeForm)
            {
                Rectangle rect = ClientRectangle;
                rect.Inflate(-4, -4);
                ControlPaint.DrawFocusRectangle(g, rect);
            }
        }

        private Button imageButton;
        /// <summary>
        /// Draws the foreground from button.
        /// </summary>
        /// <param name="pevent">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawForegroundFromButton(PaintEventArgs pevent)
        {
            if (imageButton == null)
            {
                imageButton = new Button();
                imageButton.Parent = new TransparentControl();
                imageButton.BackColor = Color.Transparent;
                imageButton.FlatAppearance.BorderSize = 0;
                imageButton.FlatStyle = FlatStyle.Flat;
            }
            if (direction != 0)
            {
                imageButton.SuspendLayout();
            }
            imageButton.ForeColor = ForeColor;
            imageButton.Font = Font;
            imageButton.RightToLeft = RightToLeft;
            imageButton.Image = Image;
            imageButton.ImageAlign = ImageAlign;
            imageButton.ImageIndex = ImageIndex;
            imageButton.ImageKey = ImageKey;
            imageButton.ImageList = ImageList;
            imageButton.Padding = Padding;
            imageButton.Size = Size;
            imageButton.Text = Text;
            imageButton.TextAlign = TextAlign;
            imageButton.TextImageRelation = TextImageRelation;
            imageButton.UseCompatibleTextRendering = UseCompatibleTextRendering;
            imageButton.UseMnemonic = UseMnemonic;
            if (direction != 0)
            {
                imageButton.ResumeLayout();
            }
            InvokePaint(imageButton, pevent);
        }

        class TransparentControl : Control
        {
            protected override void OnPaintBackground(PaintEventArgs pevent) { }
            protected override void OnPaint(PaintEventArgs e) { }
        }

        /// <summary>
        /// Creates the round rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        private GraphicsPath CreateRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;

            if (d < 1)
                return path;

            if (alternativeForm)
            {
                if (alternativeFormDirection == Direction.Left)
                {
                    path.AddArc(l, t, h, h, 90, 180);
                    path.AddLine(l + h, t, l + w, t);
                    path.AddCurve(new Point[5] { 
                        new Point(l + w, t), 
                        new Point(l + w - h / 6, t + h / 4),
                        new Point((int)(l + w - (double)(h / 4.8)), t + h / 2),
                        new Point(l + w - h / 6, t + 3 * h / 4), 
                        new Point(l + w, t + h) });
                    path.AddLine(l + h, t + h, l + w, t + h);
                }
                else
                {
                    path.AddCurve(new Point[5] { 
                        new Point(l, t), 
                        new Point(l + h / 6, t + h / 4),
                        new Point((int)(l + (double)(h / 4.5)), t + h / 2),
                        new Point(l + h / 6, t + 3 * h / 4), 
                        new Point(l, t + h) });
                    path.AddLine(l, t + h, l + w - h, t + h);
                    path.AddArc(l + w - h, t, h, h, 90, -180);
                    path.AddLine(l + w - h, t, l, t);
                }
            }
            else
            {
                path.AddArc(l, t, d, d, 180, 90); // topleft
                path.AddLine(l + radius, t, l + w - radius, t); // top
                path.AddArc(l + w - d, t, d, d, 270, 90); // topright
                path.AddLine(l + w, t + radius, l + w, t + h - radius); // right
                path.AddArc(l + w - d, t + h - d, d, d, 0, 90); // bottomright
                path.AddLine(l + w - radius, t + h, l + radius, t + h); // bottom
                path.AddArc(l, t + h - d, d, d, 90, 90); // bottomleft
                path.AddLine(l, t + h - radius, l, t + radius); // left
            }

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Creates the top round rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        private GraphicsPath CreateTopRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;

            if (d < 1)
                return path;

            if (alternativeForm)
            {
                if (alternativeFormDirection == Direction.Left)
                {
                    path.AddArc(l, t, h * 2, h * 2, 180, 90);
                    path.AddLine(l + h, t, l + w, t);
                    path.AddCurve(new Point[3] { 
                        new Point(l + w, t), 
                        new Point(l + w - h / 3, t + h / 2),
                        new Point((int)(l + w - (double)(h / 2.35)), t + h)});
                }
                else
                {
                    path.AddCurve(new Point[3] { 
                        new Point(l, t), 
                        new Point(l + h / 3, t + h / 2),
                        new Point((int)(l + (double)(h / 2.35)), t + h)});
                    path.AddLine((int)(l + (double)(h / 2.35)), t + h, l + w - h, t + h);
                    path.AddArc(l + w - h * 2, t, h * 2, h * 2, 0, -90);
                }
            }
            else
            {
                path.AddArc(l, t, d, d, 180, 90); // topleft
                path.AddLine(l + radius, t, l + w - radius, t); // top
                path.AddArc(l + w - d, t, d, d, 270, 90); // topright
                path.AddLine(l + w, t + radius, l + w, t + h); // right
                path.AddLine(l + w, t + h, l, t + h); // bottom
                path.AddLine(l, t + h, l, t + radius); // left
            }

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Creates the bottom radial path.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns></returns>
        private GraphicsPath CreateBottomRadialPath(Rectangle rectangle)
        {
            GraphicsPath path = new GraphicsPath();
            RectangleF rect = rectangle;
            rect.X -= rectangle.Width * .35f;
            rect.Y -= rectangle.Height * .15f;
            rect.Width *= 1.7f;
            rect.Height *= 2.3f;
            path.AddEllipse(rect);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Handles the SizeChanged event of the GlassButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GlassButton_SizeChanged(object sender, EventArgs e)
        {
            RecalcRect((float)currentFrame / (framesCount - 1f));
        }

        /// <summary>
        /// Handles the MouseLeave event of the GlassButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GlassButton_MouseLeave(object sender, EventArgs e)
        {
            RecalcGlow((float)currentFrame / (framesCount - 1f));
        }

        /// <summary>
        /// Handles the MouseEnter event of the GlassButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GlassButton_MouseEnter(object sender, EventArgs e)
        {
            RecalcGlow((float)currentFrame / (framesCount - 1f));
        }

        /// <summary>
        /// Handles the LostFocus event of the GlassButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GlassButton_LostFocus(object sender, EventArgs e)
        {
            //isHovered = false;
            //RecalcOuterBorder();
            //RecalcGlow(0);
        }

        /// <summary>
        /// Handles the GotFocus event of the GlassButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GlassButton_GotFocus(object sender, EventArgs e)
        {
            //isHovered = true;
            //RecalcOuterBorder();
            //RecalcGlow(1);
        }

        /// <summary>
        /// Recalcs the rectangles for drawing.
        /// </summary>
        /// <param name="glowOpacity">The glow opacity.</param>
        private void RecalcRect(float glowOpacity)
        {
            try
            {
                int rCorner = roundCorner;

                if (roundCorner > Height / 2)
                    rCorner = Height / 2;

                if (roundCorner > Width / 2)
                    rCorner = Width / 2;

                rect = RecalcOuterBorder();

                rect = RecalcContent(rect, out rect2);

                RecalcGlow(glowOpacity);

                rect2 = RecalcShine(rect2);

                BorderPath = CreateRoundRectangle(rect, rCorner);

                BorderPen = new Pen(innerBorderColor);
            }
            catch { }
        }

        /// <summary>
        /// Recalcs the shine.
        /// </summary>
        /// <param name="rect2">The rect2.</param>
        /// <returns></returns>
        private Rectangle RecalcShine(Rectangle rect2)
        {
            int rCorner = roundCorner;

            if (roundCorner > Height / 2)
                rCorner = Height / 2;

            if (roundCorner > Width / 2)
                rCorner = Width / 2;

            if (rect2.Width > 0 && rect2.Height > 0)
            {
                rect2.Height++;
                ShinePath = CreateTopRoundRectangle(rect2, rCorner);

                rect2.Height++;
                int opacity = 0x99;
                if (isPressed)
                    opacity = (int)(.4f * opacity + .5f);
                ShineBrush = new LinearGradientBrush(rect2, Color.FromArgb(opacity, shineColor), Color.FromArgb(opacity / 3, shineColor), LinearGradientMode.Vertical);

                rect2.Height -= 2;

                drawShine = true;
            }
            else
                drawShine = false;
            return rect2;
        }

        /// <summary>
        /// Recalcs the glow.
        /// </summary>
        /// <param name="glowOpacity">The glow opacity.</param>
        private void RecalcGlow(float glowOpacity)
        {
            int rCorner = roundCorner;

            if (roundCorner > Height / 2)
                rCorner = Height / 2;

            if (roundCorner > Width / 2)
                rCorner = Width / 2;

            GlowClip = CreateRoundRectangle(rect, rCorner);
            GlowBottomRadial = CreateBottomRadialPath(rect);

            GlowRadialPath = new PathGradientBrush(GlowBottomRadial);

            int opacity = (int)(0xB2 * glowOpacity + .5f);

            if (!animateGlow)
            {
                if (isHovered || isFocused)
                    opacity = 255;
                else
                    opacity = 0;
            }

            GlowRadialPath.CenterColor = Color.FromArgb(opacity, glowColor);
            GlowRadialPath.SurroundColors = new Color[] { Color.FromArgb(0, glowColor) };
            if (isFocused && !isHovered)
            {
                GlowRadialPath.CenterColor = Color.FromArgb(opacity, focusColor);
                GlowRadialPath.SurroundColors = new Color[] { Color.FromArgb(0, focusColor) };
            }
        }

        /// <summary>
        /// Recalcs the content.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="rect2">The rect2.</param>
        /// <returns></returns>
        private Rectangle RecalcContent(Rectangle rect, out Rectangle rect2)
        {
            int rCorner = roundCorner;

            if (roundCorner > Height / 2)
                rCorner = Height / 2;

            if (roundCorner > Width / 2)
                rCorner = Width / 2;

            rect.X++;
            rect.Y++;
            rect.Width -= 2;
            rect.Height -= 2;

            rect2 = rect;
            rect2.Height >>= 1;

            ContentPath = CreateRoundRectangle(rect, rCorner);
            int opacity = isPressed ? 0xcc : 0x7f;
            ContentBrush = new SolidBrush(Color.FromArgb(opacity, backColor));
            return rect;
        }

        /// <summary>
        /// Recalcs the outer border.
        /// </summary>
        /// <returns></returns>
        private Rectangle RecalcOuterBorder()
        {
            int rCorner = roundCorner;

            if (roundCorner > Height / 2)
                rCorner = Height / 2;

            if (roundCorner > Width / 2)
                rCorner = Width / 2;

            Rectangle rect;
            rect = ClientRectangle;
            rect.Width--;
            rect.Height--;
            outerBorderPath = CreateRoundRectangle(rect, rCorner);
            rect.Inflate(1, 1);
            GraphicsPath region = CreateRoundRectangle(rect, rCorner);
            this.Region = new Region(region);
            rect.Inflate(-1, -1);

            Color col = outerBorderColor;
            if (Focused && !ShowFocusBorder)
                col = alternativeFocusBorderColor;

            outerBorderPen = new Pen(col);
            return rect;
        }

        /// <summary>
        /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values.</param>
        /// <returns>
        /// true if the specified key is a regular input key; otherwise, false.
        /// </returns>
        /// <remarks>Documented by Dev07, 2009-04-20</remarks>
        protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
        #endregion

        #region " Unused Properties & Events "

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new FlatButtonAppearance FlatAppearance
        {
            get { return base.FlatAppearance; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new FlatStyle FlatStyle
        {
            get { return base.FlatStyle; }
            set { base.FlatStyle = value; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool UseVisualStyleBackColor
        {
            get { return base.UseVisualStyleBackColor; }
            set { base.UseVisualStyleBackColor = value; }
        }

        #endregion

        #region " Animation Support "

        private const int animationLength = 300;
        private const int framesCount = 10;
        private int currentFrame;
        private int direction;

        private bool isAnimating
        {
            get
            {
                return direction != 0;
            }
        }

        private void FadeIn()
        {
            direction = 1;
            timer.Enabled = true;
        }

        private void FadeOut()
        {
            direction = -1;
            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!timer.Enabled || !animateGlow)
            {
                timer.Enabled = false;
                return;
            }

            RecalcRect((float)currentFrame / (framesCount - 1f));
            Refresh();
            currentFrame += direction;
            if (currentFrame == -1)
            {
                currentFrame = 0;
                timer.Enabled = false;
                direction = 0;
                return;
            }
            if (currentFrame == framesCount)
            {
                currentFrame = framesCount - 1;
                timer.Enabled = false;
                direction = 0;
            }
        }

        #endregion
    }
}
