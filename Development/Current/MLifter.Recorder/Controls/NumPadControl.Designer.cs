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
namespace MLifter.AudioTools
{
    partial class NumPadControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumPadControl));
            this.buttonNum = new System.Windows.Forms.Button();
            this.buttonDivide = new System.Windows.Forms.Button();
            this.buttonMultiply = new System.Windows.Forms.Button();
            this.buttonMinus = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.buttonEnter = new System.Windows.Forms.Button();
            this.buttonPlus = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.buttonComma = new System.Windows.Forms.Button();
            this.button0 = new System.Windows.Forms.Button();
            this.tableLayoutPanelAdvancedView = new System.Windows.Forms.TableLayoutPanel();
            this.panelBackground = new System.Windows.Forms.Panel();
            this.pictureBoxSimpleView = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelKeyboard = new System.Windows.Forms.TableLayoutPanel();
            this.buttonC = new System.Windows.Forms.Button();
            this.buttonV = new System.Windows.Forms.Button();
            this.buttonB = new System.Windows.Forms.Button();
            this.buttonN = new System.Windows.Forms.Button();
            this.buttonM = new System.Windows.Forms.Button();
            this.buttonSpace = new System.Windows.Forms.Button();
            this.panelKeyboardBack = new System.Windows.Forms.Panel();
            this.tableLayoutPanelAdvancedView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSimpleView)).BeginInit();
            this.tableLayoutPanelKeyboard.SuspendLayout();
            this.panelKeyboardBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonNum
            // 
            this.buttonNum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNum.BackColor = System.Drawing.Color.White;
            this.buttonNum.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonNum.FlatAppearance.BorderSize = 0;
            this.buttonNum.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonNum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNum.Location = new System.Drawing.Point(0, 0);
            this.buttonNum.Margin = new System.Windows.Forms.Padding(0);
            this.buttonNum.Name = "buttonNum";
            this.buttonNum.Size = new System.Drawing.Size(95, 96);
            this.buttonNum.TabIndex = 80;
            this.buttonNum.Text = "Num";
            this.buttonNum.UseVisualStyleBackColor = false;
            this.buttonNum.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonNum.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonNum.Click += new System.EventHandler(this.numPad_Click);
            this.buttonNum.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonNum.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonDivide
            // 
            this.buttonDivide.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDivide.BackColor = System.Drawing.Color.White;
            this.buttonDivide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonDivide.FlatAppearance.BorderSize = 0;
            this.buttonDivide.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonDivide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDivide.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDivide.Location = new System.Drawing.Point(95, 0);
            this.buttonDivide.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDivide.Name = "buttonDivide";
            this.buttonDivide.Size = new System.Drawing.Size(95, 96);
            this.buttonDivide.TabIndex = 70;
            this.buttonDivide.Text = "�";
            this.buttonDivide.UseVisualStyleBackColor = false;
            this.buttonDivide.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonDivide.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonDivide.Click += new System.EventHandler(this.numPad_Click);
            this.buttonDivide.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonDivide.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonMultiply
            // 
            this.buttonMultiply.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMultiply.BackColor = System.Drawing.Color.White;
            this.buttonMultiply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonMultiply.FlatAppearance.BorderSize = 0;
            this.buttonMultiply.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonMultiply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMultiply.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMultiply.Location = new System.Drawing.Point(190, 0);
            this.buttonMultiply.Margin = new System.Windows.Forms.Padding(0);
            this.buttonMultiply.Name = "buttonMultiply";
            this.buttonMultiply.Size = new System.Drawing.Size(95, 96);
            this.buttonMultiply.TabIndex = 60;
            this.buttonMultiply.Text = "*";
            this.buttonMultiply.UseVisualStyleBackColor = false;
            this.buttonMultiply.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonMultiply.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonMultiply.Click += new System.EventHandler(this.numPad_Click);
            this.buttonMultiply.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonMultiply.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonMinus
            // 
            this.buttonMinus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMinus.BackColor = System.Drawing.Color.White;
            this.buttonMinus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonMinus.FlatAppearance.BorderSize = 0;
            this.buttonMinus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMinus.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMinus.Location = new System.Drawing.Point(285, 0);
            this.buttonMinus.Margin = new System.Windows.Forms.Padding(0);
            this.buttonMinus.Name = "buttonMinus";
            this.buttonMinus.Size = new System.Drawing.Size(95, 96);
            this.buttonMinus.TabIndex = 50;
            this.buttonMinus.Text = "-";
            this.buttonMinus.UseVisualStyleBackColor = false;
            this.buttonMinus.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonMinus.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonMinus.Click += new System.EventHandler(this.numPad_Click);
            this.buttonMinus.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonMinus.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.BackColor = System.Drawing.Color.White;
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button7.FlatAppearance.BorderSize = 0;
            this.button7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Location = new System.Drawing.Point(0, 96);
            this.button7.Margin = new System.Windows.Forms.Padding(0);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(95, 96);
            this.button7.TabIndex = 17;
            this.button7.Text = "7";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button7.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button7.Click += new System.EventHandler(this.numPad_Click);
            this.button7.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button7.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.BackColor = System.Drawing.Color.White;
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button6.FlatAppearance.BorderSize = 0;
            this.button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Location = new System.Drawing.Point(190, 192);
            this.button6.Margin = new System.Windows.Forms.Padding(0);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(95, 96);
            this.button6.TabIndex = 16;
            this.button6.Text = "6";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button6.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button6.Click += new System.EventHandler(this.numPad_Click);
            this.button6.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button6.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonEnter
            // 
            this.buttonEnter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnter.BackColor = System.Drawing.Color.White;
            this.buttonEnter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonEnter.FlatAppearance.BorderSize = 0;
            this.buttonEnter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonEnter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnter.Location = new System.Drawing.Point(285, 288);
            this.buttonEnter.Margin = new System.Windows.Forms.Padding(0);
            this.buttonEnter.Name = "buttonEnter";
            this.tableLayoutPanelAdvancedView.SetRowSpan(this.buttonEnter, 2);
            this.buttonEnter.Size = new System.Drawing.Size(95, 192);
            this.buttonEnter.TabIndex = 30;
            this.buttonEnter.Text = "Enter";
            this.buttonEnter.UseVisualStyleBackColor = false;
            this.buttonEnter.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonEnter.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonEnter.Click += new System.EventHandler(this.numPad_Click);
            this.buttonEnter.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonEnter.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonPlus
            // 
            this.buttonPlus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlus.BackColor = System.Drawing.Color.White;
            this.buttonPlus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonPlus.FlatAppearance.BorderSize = 0;
            this.buttonPlus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlus.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlus.Location = new System.Drawing.Point(285, 96);
            this.buttonPlus.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPlus.Name = "buttonPlus";
            this.tableLayoutPanelAdvancedView.SetRowSpan(this.buttonPlus, 2);
            this.buttonPlus.Size = new System.Drawing.Size(95, 192);
            this.buttonPlus.TabIndex = 40;
            this.buttonPlus.Text = "+";
            this.buttonPlus.UseVisualStyleBackColor = false;
            this.buttonPlus.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonPlus.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonPlus.Click += new System.EventHandler(this.numPad_Click);
            this.buttonPlus.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonPlus.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.BackColor = System.Drawing.Color.White;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(0, 192);
            this.button4.Margin = new System.Windows.Forms.Padding(0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(95, 96);
            this.button4.TabIndex = 14;
            this.button4.Text = "4";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button4.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button4.Click += new System.EventHandler(this.numPad_Click);
            this.button4.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button4.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.BackColor = System.Drawing.Color.White;
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(95, 192);
            this.button5.Margin = new System.Windows.Forms.Padding(0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(95, 96);
            this.button5.TabIndex = 15;
            this.button5.Text = "5";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button5.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button5.Click += new System.EventHandler(this.numPad_Click);
            this.button5.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button5.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(0, 288);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 96);
            this.button1.TabIndex = 11;
            this.button1.Text = "1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button1.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button1.Click += new System.EventHandler(this.numPad_Click);
            this.button1.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button1.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(95, 288);
            this.button2.Margin = new System.Windows.Forms.Padding(0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 96);
            this.button2.TabIndex = 12;
            this.button2.Text = "2";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button2.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button2.Click += new System.EventHandler(this.numPad_Click);
            this.button2.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button2.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackColor = System.Drawing.Color.White;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(190, 288);
            this.button3.Margin = new System.Windows.Forms.Padding(0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(95, 96);
            this.button3.TabIndex = 13;
            this.button3.Text = "3";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button3.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button3.Click += new System.EventHandler(this.numPad_Click);
            this.button3.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button3.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button9
            // 
            this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button9.BackColor = System.Drawing.Color.White;
            this.button9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button9.FlatAppearance.BorderSize = 0;
            this.button9.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Location = new System.Drawing.Point(190, 96);
            this.button9.Margin = new System.Windows.Forms.Padding(0);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(95, 96);
            this.button9.TabIndex = 19;
            this.button9.Text = "9";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button9.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button9.Click += new System.EventHandler(this.numPad_Click);
            this.button9.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button9.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button8
            // 
            this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button8.BackColor = System.Drawing.Color.White;
            this.button8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button8.FlatAppearance.BorderSize = 0;
            this.button8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Location = new System.Drawing.Point(95, 96);
            this.button8.Margin = new System.Windows.Forms.Padding(0);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(95, 96);
            this.button8.TabIndex = 18;
            this.button8.Text = "8";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button8.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button8.Click += new System.EventHandler(this.numPad_Click);
            this.button8.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button8.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonComma
            // 
            this.buttonComma.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonComma.BackColor = System.Drawing.Color.White;
            this.buttonComma.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonComma.FlatAppearance.BorderSize = 0;
            this.buttonComma.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonComma.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonComma.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonComma.Location = new System.Drawing.Point(190, 384);
            this.buttonComma.Margin = new System.Windows.Forms.Padding(0);
            this.buttonComma.Name = "buttonComma";
            this.buttonComma.Size = new System.Drawing.Size(95, 96);
            this.buttonComma.TabIndex = 20;
            this.buttonComma.Text = ",";
            this.buttonComma.UseVisualStyleBackColor = false;
            this.buttonComma.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonComma.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonComma.Click += new System.EventHandler(this.numPad_Click);
            this.buttonComma.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonComma.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // button0
            // 
            this.button0.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button0.BackColor = System.Drawing.Color.White;
            this.button0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanelAdvancedView.SetColumnSpan(this.button0, 2);
            this.button0.FlatAppearance.BorderSize = 0;
            this.button0.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button0.Location = new System.Drawing.Point(0, 384);
            this.button0.Margin = new System.Windows.Forms.Padding(0);
            this.button0.Name = "button0";
            this.button0.Size = new System.Drawing.Size(190, 96);
            this.button0.TabIndex = 10;
            this.button0.Text = "0";
            this.button0.UseVisualStyleBackColor = false;
            this.button0.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.button0.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.button0.Click += new System.EventHandler(this.numPad_Click);
            this.button0.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.button0.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // tableLayoutPanelAdvancedView
            // 
            this.tableLayoutPanelAdvancedView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelAdvancedView.ColumnCount = 4;
            this.tableLayoutPanelAdvancedView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelAdvancedView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelAdvancedView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelAdvancedView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelAdvancedView.Controls.Add(this.buttonNum, 0, 0);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button9, 2, 1);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button8, 1, 1);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button7, 0, 1);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button5, 1, 2);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button3, 2, 3);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button4, 0, 2);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button6, 2, 2);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.buttonDivide, 1, 0);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button1, 0, 3);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.buttonMultiply, 2, 0);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.buttonMinus, 3, 0);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button2, 1, 3);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.buttonPlus, 3, 1);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.buttonEnter, 3, 3);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.button0, 0, 4);
            this.tableLayoutPanelAdvancedView.Controls.Add(this.buttonComma, 2, 4);
            this.tableLayoutPanelAdvancedView.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanelAdvancedView.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelAdvancedView.Name = "tableLayoutPanelAdvancedView";
            this.tableLayoutPanelAdvancedView.RowCount = 5;
            this.tableLayoutPanelAdvancedView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelAdvancedView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelAdvancedView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelAdvancedView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelAdvancedView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelAdvancedView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelAdvancedView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelAdvancedView.Size = new System.Drawing.Size(380, 480);
            this.tableLayoutPanelAdvancedView.TabIndex = 81;
            // 
            // panelBackground
            // 
            this.panelBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBackground.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Margin = new System.Windows.Forms.Padding(0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(69, 63);
            this.panelBackground.TabIndex = 82;
            this.panelBackground.BackColorChanged += new System.EventHandler(this.panelBackground_BackColorChanged);
            this.panelBackground.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBackground_Paint);
            // 
            // pictureBoxSimpleView
            // 
            this.pictureBoxSimpleView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxSimpleView.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxSimpleView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxSimpleView.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxSimpleView.Image")));
            this.pictureBoxSimpleView.InitialImage = null;
            this.pictureBoxSimpleView.Location = new System.Drawing.Point(10, 60);
            this.pictureBoxSimpleView.Name = "pictureBoxSimpleView";
            this.pictureBoxSimpleView.Size = new System.Drawing.Size(380, 380);
            this.pictureBoxSimpleView.TabIndex = 0;
            this.pictureBoxSimpleView.TabStop = false;
            // 
            // tableLayoutPanelKeyboard
            // 
            this.tableLayoutPanelKeyboard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelKeyboard.ColumnCount = 5;
            this.tableLayoutPanelKeyboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelKeyboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelKeyboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelKeyboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelKeyboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelKeyboard.Controls.Add(this.buttonC, 0, 0);
            this.tableLayoutPanelKeyboard.Controls.Add(this.buttonV, 1, 0);
            this.tableLayoutPanelKeyboard.Controls.Add(this.buttonB, 2, 0);
            this.tableLayoutPanelKeyboard.Controls.Add(this.buttonN, 3, 0);
            this.tableLayoutPanelKeyboard.Controls.Add(this.buttonM, 4, 0);
            this.tableLayoutPanelKeyboard.Controls.Add(this.buttonSpace, 0, 1);
            this.tableLayoutPanelKeyboard.Location = new System.Drawing.Point(0, 164);
            this.tableLayoutPanelKeyboard.Name = "tableLayoutPanelKeyboard";
            this.tableLayoutPanelKeyboard.RowCount = 2;
            this.tableLayoutPanelKeyboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelKeyboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelKeyboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelKeyboard.Size = new System.Drawing.Size(380, 152);
            this.tableLayoutPanelKeyboard.TabIndex = 83;
            // 
            // buttonC
            // 
            this.buttonC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonC.FlatAppearance.BorderSize = 0;
            this.buttonC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonC.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonC.Location = new System.Drawing.Point(0, 0);
            this.buttonC.Margin = new System.Windows.Forms.Padding(0);
            this.buttonC.Name = "buttonC";
            this.buttonC.Size = new System.Drawing.Size(76, 76);
            this.buttonC.TabIndex = 21;
            this.buttonC.Text = "C";
            this.buttonC.UseVisualStyleBackColor = true;
            this.buttonC.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonC.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonC.Click += new System.EventHandler(this.numPad_Click);
            this.buttonC.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonC.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonV
            // 
            this.buttonV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonV.FlatAppearance.BorderSize = 0;
            this.buttonV.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonV.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonV.Location = new System.Drawing.Point(76, 0);
            this.buttonV.Margin = new System.Windows.Forms.Padding(0);
            this.buttonV.Name = "buttonV";
            this.buttonV.Size = new System.Drawing.Size(76, 76);
            this.buttonV.TabIndex = 25;
            this.buttonV.Text = "V";
            this.buttonV.UseVisualStyleBackColor = true;
            this.buttonV.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonV.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonV.Click += new System.EventHandler(this.numPad_Click);
            this.buttonV.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonV.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonB
            // 
            this.buttonB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonB.FlatAppearance.BorderSize = 0;
            this.buttonB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonB.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonB.Location = new System.Drawing.Point(152, 0);
            this.buttonB.Margin = new System.Windows.Forms.Padding(0);
            this.buttonB.Name = "buttonB";
            this.buttonB.Size = new System.Drawing.Size(76, 76);
            this.buttonB.TabIndex = 22;
            this.buttonB.Text = "B";
            this.buttonB.UseVisualStyleBackColor = true;
            this.buttonB.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonB.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonB.Click += new System.EventHandler(this.numPad_Click);
            this.buttonB.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonB.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonN
            // 
            this.buttonN.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonN.FlatAppearance.BorderSize = 0;
            this.buttonN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonN.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonN.Location = new System.Drawing.Point(228, 0);
            this.buttonN.Margin = new System.Windows.Forms.Padding(0);
            this.buttonN.Name = "buttonN";
            this.buttonN.Size = new System.Drawing.Size(76, 76);
            this.buttonN.TabIndex = 24;
            this.buttonN.Text = "N";
            this.buttonN.UseVisualStyleBackColor = true;
            this.buttonN.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonN.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonN.Click += new System.EventHandler(this.numPad_Click);
            this.buttonN.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonN.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonM
            // 
            this.buttonM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonM.FlatAppearance.BorderSize = 0;
            this.buttonM.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonM.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonM.Location = new System.Drawing.Point(304, 0);
            this.buttonM.Margin = new System.Windows.Forms.Padding(0);
            this.buttonM.Name = "buttonM";
            this.buttonM.Size = new System.Drawing.Size(76, 76);
            this.buttonM.TabIndex = 23;
            this.buttonM.Text = "M";
            this.buttonM.UseVisualStyleBackColor = true;
            this.buttonM.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonM.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonM.Click += new System.EventHandler(this.numPad_Click);
            this.buttonM.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonM.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // buttonSpace
            // 
            this.buttonSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSpace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanelKeyboard.SetColumnSpan(this.buttonSpace, 5);
            this.buttonSpace.FlatAppearance.BorderSize = 0;
            this.buttonSpace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonSpace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSpace.Location = new System.Drawing.Point(0, 76);
            this.buttonSpace.Margin = new System.Windows.Forms.Padding(0);
            this.buttonSpace.Name = "buttonSpace";
            this.buttonSpace.Size = new System.Drawing.Size(380, 76);
            this.buttonSpace.TabIndex = 11;
            this.buttonSpace.UseVisualStyleBackColor = true;
            this.buttonSpace.Enter += new System.EventHandler(this.numPad_FocusEnter);
            this.buttonSpace.MouseLeave += new System.EventHandler(this.buttonDivide_MouseLeave);
            this.buttonSpace.Click += new System.EventHandler(this.numPad_Click);
            this.buttonSpace.MouseEnter += new System.EventHandler(this.buttonDivide_MouseEnter);
            this.buttonSpace.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
            // 
            // panelKeyboardBack
            // 
            this.panelKeyboardBack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelKeyboardBack.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panelKeyboardBack.Controls.Add(this.tableLayoutPanelKeyboard);
            this.panelKeyboardBack.Location = new System.Drawing.Point(10, 10);
            this.panelKeyboardBack.Name = "panelKeyboardBack";
            this.panelKeyboardBack.Size = new System.Drawing.Size(380, 480);
            this.panelKeyboardBack.TabIndex = 84;
            // 
            // NumPadControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Controls.Add(this.tableLayoutPanelAdvancedView);
            this.Controls.Add(this.pictureBoxSimpleView);
            this.Controls.Add(this.panelBackground);
            this.Controls.Add(this.panelKeyboardBack);
            this.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "NumPadControl";
            this.Size = new System.Drawing.Size(400, 500);
            this.Load += new System.EventHandler(this.NumPadControl_Load);
            this.tableLayoutPanelAdvancedView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSimpleView)).EndInit();
            this.tableLayoutPanelKeyboard.ResumeLayout(false);
            this.panelKeyboardBack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonNum;
        private System.Windows.Forms.Button buttonDivide;
        private System.Windows.Forms.Button buttonMultiply;
        private System.Windows.Forms.Button buttonMinus;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button buttonEnter;
        private System.Windows.Forms.Button buttonPlus;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button buttonComma;
        private System.Windows.Forms.Button button0;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAdvancedView;
        private System.Windows.Forms.Panel panelBackground;
        private System.Windows.Forms.PictureBox pictureBoxSimpleView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelKeyboard;
        private System.Windows.Forms.Button buttonV;
        private System.Windows.Forms.Button buttonN;
        private System.Windows.Forms.Button buttonM;
        private System.Windows.Forms.Button buttonSpace;
        private System.Windows.Forms.Button buttonC;
        private System.Windows.Forms.Button buttonB;
        private System.Windows.Forms.Panel panelKeyboardBack;

    }
}
