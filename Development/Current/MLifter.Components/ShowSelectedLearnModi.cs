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
