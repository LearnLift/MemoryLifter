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
using System.Text;
using System.Windows.Forms;
using MLifter.Components.Properties;
using System.Drawing;
using System.Drawing.Drawing2D;
using MLifter.BusinessLayer;

namespace MLifter.Components
{
    public static class ShowSelectedLearnModi
    {
        public enum LearnModi
        {
            Standard,
            MultipleChoice,
            Sentences,
            ListeningComprehension,
            ImageRecognition
        };
        private static List<LearnModi> selectedModi = new List<LearnModi>();
        private static Image[] images = new Image[] {
                Resources.gadu_grau, 
                Resources.gadu_yellow, 
                Resources.gadu_pink,
                Resources.gadu_blue,
                Resources.gadu_orange,
                Resources.gadu_green};

        public static void LearnModusSelected(LearnModi selected)
        {
            selectedModi.Add(selected);
        }
        public static void LearnModusSelectedClear()
        {
            selectedModi.Clear();
        }
        public static void LearnModusDeselected(LearnModi deselected)
        {
            selectedModi.Remove(deselected);
        }
        public static void GetImage(Image image)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                DrawPicture(g, image.Width, image.Height);
            }
        }
        public static bool has(LearnModi item)
        {
            return selectedModi.Contains(item);
        }
        private static void DrawPicture(Graphics graphics, int width, int height)
        {
            graphics.DrawImage(images[0], 0, 0, width, height);
            if (selectedModi.Contains(LearnModi.Standard))
                graphics.DrawImage(images[1], 0, 0, width, height);
            if (selectedModi.Contains(LearnModi.MultipleChoice))
                graphics.DrawImage(images[2], 0, 0, width, height);
            if (selectedModi.Contains(LearnModi.Sentences))
                graphics.DrawImage(images[3], 0, 0, width, height);
            if (selectedModi.Contains(LearnModi.ListeningComprehension))
                graphics.DrawImage(images[4], 0, 0, width, height);
            if (selectedModi.Contains(LearnModi.ImageRecognition))
                graphics.DrawImage(images[5], 0, 0, width, height);

        }
    }
}
