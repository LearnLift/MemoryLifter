using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Preview;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// LearningModulesIndex class (main BL logic of the start page)
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-12-09</remarks>
    public partial class LearningModulesIndex
    {
        private const int MAX_CARDS_TO_SEARCH = 25;
        private const int MAX_WIDTH = 263;
        private const int MAX_HEIGHT = 119;
        private const int MAX_IMAGE_WIDTH = MAX_WIDTH;
        private const int MAX_IMAGE_HEIGHT = MAX_HEIGHT - 30;
        private const double DEFAULT_WIDTH_TO_HEIGHT = 0.666;       //66.6% of MAX_IMAGE_WIDTH
        private const bool SHOW_IMAGE_BORDER = true;
        private const int FIT_STRING_TO_AREA_TIMEOUT = 120;

        /// <summary>
        /// Generates the preview.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        private static void GeneratePreview(LearningModulesIndexEntry entry)
        {
            Bitmap preview = new Bitmap(MAX_WIDTH, MAX_HEIGHT);      //Todo: get the available preview size
            Graphics g = Graphics.FromImage(preview);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            try
            {
                LearningModulePreviewStruct lmps = GetLearningModulePreviewData(entry.Dictionary, entry.ConnectionString);
                Font font = new Font("Arial", 9.0f);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;

                //Preview Background:
                g.FillRectangle(Brushes.White, 0, 0, MAX_WIDTH, MAX_HEIGHT);
                //Border
                g.DrawLine(Pens.Black, new Point(0, 0), new Point(0, MAX_HEIGHT));
                g.DrawLine(Pens.Black, new Point(0, 0), new Point(MAX_WIDTH, 0));
                g.DrawLine(Pens.Black, new Point(0, MAX_HEIGHT - 1), new Point(MAX_WIDTH - 1, MAX_HEIGHT - 1));
                g.DrawLine(Pens.Black, new Point(MAX_WIDTH - 1, 0), new Point(MAX_WIDTH - 1, MAX_HEIGHT - 1));

                //Text String:
                lmps.Text = FitStringToArea(g, lmps.Text, font, MAX_WIDTH);
                g.DrawString(lmps.Text, font, Brushes.Black, new RectangleF(0, 0, MAX_WIDTH, 15), stringFormat); //the rectangleF is needed to cut the string if FitStringToArea does not cut enough of the string.

                //Preview Image:
                g.DrawImage(lmps.Media, MAX_WIDTH / 2 - lmps.Media.Width / 2, 15);

                LearningModulePreview lmPreview = new LearningModulePreview();
                lmPreview.Description = lmps.Description;
                lmPreview.PreviewImage = (Image)preview.Clone();

                entry.Preview = lmPreview;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LearningModulesIndex.GeneratePreview(" + entry.DisplayName + ")" + ex.Message);
            }
            finally
            {
                preview.Dispose();
                g.Dispose();
            }
        }

        /// <summary>
        /// Gets the learning module preview data. (Question; Question-Example Text; Media as Bitmap
        /// </summary>
        /// <param name="learningModule">The learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-12-04</remarks>
        private static LearningModulePreviewStruct GetLearningModulePreviewData(IDictionary learningModule, ConnectionStringStruct connectionString)
        {
            LearningModulePreviewStruct lmps = new LearningModulePreviewStruct();

            //if (learningModule is PreviewDictionary)
            //{
            //    connectionString.ReadOnly = true;
            //    IUser user = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
            //            connectionString, (DataAccessErrorDelegate)delegate { return; }, this);
            //    learningModule = user.Open();
            //}
            try
            {

                //1. Check if there are Images in the Learning Module and select the card with the images
                int count = 0;
                bool foundImage = false;
                foreach (ICard card in learningModule.Cards.Cards)
                {
                    count++;
                    if (count > MAX_CARDS_TO_SEARCH)
                        break;

                    if (card.QuestionMedia.Count > 0)       //Search in all Cards where Question Media data are available
                    {
                        foreach (IMedia media in card.QuestionMedia)        //Search in all Media data for a Image
                        {
                            if (media.MediaType == EMedia.Image)
                            {
                                foundImage = true;
                                try
                                {   //Try to open the Media file
                                    if (learningModule is PreviewDictionary)
                                    {
                                        string wd = Directory.GetCurrentDirectory();
                                        Directory.SetCurrentDirectory(Path.GetDirectoryName(learningModule.Connection));
                                        lmps.Media = new Bitmap(media.Stream);
                                        Directory.SetCurrentDirectory(wd);
                                    }
                                    else
                                        lmps.Media = new Bitmap(media.Stream);
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine("Exeption in GetLearningModulePreviewData " + learningModule.Title + "; found image not available - continue searching for images; Exception: " + ex.Message);
                                    foundImage = false;     //error loading image... no valid image found
                                }

                                if (foundImage)     //if image found... stop searching for images in this Card 
                                    break;
                            }
                        }
                        if (foundImage)     //If an Image was found, get the Text data to the according Image
                        {
                            lmps.Text = IWordsToString(card.Question.Words);
                            lmps.ExampleText = IWordsToString(card.QuestionExample.Words);
                            break;      //stop searching for cards/images in this learning module
                        }
                    }
                }

                //2. If there are no Images in the LM, take the first 
                if (!foundImage)
                {
                    if (learningModule.Cards.Cards.Count > 0)       //no image, but cards available
                    {
                        lmps.Text = IWordsToString(learningModule.Cards.Cards[0].Question.Words);
                        lmps.ExampleText = IWordsToString(learningModule.Cards.Cards[0].QuestionExample.Words);
                        lmps.Media = GetEmptyImage((int)(MAX_IMAGE_WIDTH * DEFAULT_WIDTH_TO_HEIGHT), MAX_IMAGE_HEIGHT, false);
                    }
                    else        //no image and no cards available
                    {
                        //Todo: add code here to set the default Text for Text/Example when the LM is empty
                        lmps.Media = GetEmptyImage((int)(MAX_IMAGE_WIDTH * DEFAULT_WIDTH_TO_HEIGHT), MAX_IMAGE_HEIGHT, true);
                    }

                    lmps.Description = learningModule.Description;

                    return lmps;        //Everything is done ... no image resizing necessary, because there is no image in the LM
                }

                //Adjust the preview-picture
                int resizedWidth = 0, resizedHeight = 0;
                bool resized = false;
                //1. Check if the Image Width is too large
                if (lmps.Media.Width > MAX_IMAGE_WIDTH)
                {
                    double resizeFactor = (double)MAX_IMAGE_WIDTH / lmps.Media.Width;
                    resizedWidth = (int)(lmps.Media.Width * resizeFactor);
                    resizedHeight = (int)(lmps.Media.Height * resizeFactor);
                    resized = true;
                }

                if (resized)        //Check the resized Height
                {
                    //2. Check if the Image Height is too large (check the resizedHeight; but recalculate with the original dimensions
                    if (resizedHeight > MAX_IMAGE_HEIGHT)
                    {
                        double resizeFactor = (double)MAX_IMAGE_HEIGHT / lmps.Media.Height;
                        resizedHeight = (int)(lmps.Media.Height * resizeFactor);
                        resizedWidth = (int)(lmps.Media.Width * resizeFactor);
                    }
                }
                else        //Check the original height, if the Image wasn't resized in the step before.
                {
                    if (lmps.Media.Height > MAX_IMAGE_HEIGHT)
                    {
                        double resizeFactor = (double)MAX_IMAGE_HEIGHT / lmps.Media.Height;
                        resizedHeight = (int)(lmps.Media.Height * resizeFactor);
                        resizedWidth = (int)(lmps.Media.Width * resizeFactor);
                        resized = true;
                    }
                }

                if (resized)
                    lmps.Media = GetThumbnail(lmps.Media, resizedWidth, resizedHeight);

                lmps.Description = learningModule.Description;

                return lmps;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GetLearningModulePreviewData(" + learningModule.Title + ") throws an exception: " + ex.Message);

                lmps.Media = new Bitmap(1, 1);
                return lmps;
            }
            //finally
            //{
            //    learningModule.Dispose();
            //}
        }

        /// <summary>
        /// Gets the thumbnail.
        /// </summary>
        /// <param name="originalImage">The original image.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        /// <returns>Thumbnail</returns>
        /// <remarks>Documented by Dev08, 2008-12-05</remarks>
        private static Image GetThumbnail(Image originalImage, int newWidth, int newHeight)
        {
            if (newWidth < 1 || newHeight < 1 || originalImage == null)     //If parameter are not valid
                return new Bitmap(1, 1);

            Image thumbnail = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(thumbnail);

            try
            {
                g.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight));

                //customizing code (ImageBorder...)
                if (SHOW_IMAGE_BORDER)
                    g.DrawRectangle(new Pen(Brushes.LightGray, 1.0f), 0, 0, newWidth - 1, newHeight - 1);

                return (Image)thumbnail.Clone();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failure in GetThumbnail() " + ex.Message);
            }
            finally
            {
                thumbnail.Dispose();
                g.Dispose();
            }

            return new Bitmap(1, 1);
        }

        /// <summary>
        /// Gets an empty image place holder.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="isLearningModuleEmpty">if set to <c>true</c> [is learning module empty].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        private static Image GetEmptyImage(int width, int height, bool isLearningModuleEmpty)
        {
            if (width < 1 || height < 1)
                return new Bitmap(1, 1);

            Bitmap emptyImage = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(emptyImage);

            try
            {
                g.FillRectangle(Brushes.White, 0, 0, width, height);

                if (SHOW_IMAGE_BORDER)
                    g.DrawRectangle(new Pen(Brushes.LightGray, 1.0f), 0, 0, width - 1, height - 1);

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                Font font = new Font("Arial", 8.0f);

                if (isLearningModuleEmpty)      //either no cards
                    g.DrawString(Properties.Resources.LEARNING_MODULES_PAGE_EMPTY_LM, font, Brushes.DarkGray, new RectangleF(0, 0, width, height), stringFormat);
                else        // or no images in the cards
                    g.DrawString(Properties.Resources.LEARNING_MODULES_PAGE_NO_IMAGES, font, Brushes.DarkGray, new RectangleF(0, 0, width, height), stringFormat);

                return (Image)emptyImage.Clone();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failure in GetEmptyImage() " + ex.Message);
            }
            finally
            {
                emptyImage.Dispose();
                g.Dispose();
            }

            return new Bitmap(1, 1);
        }

        /// <summary>
        /// Converts a List of IWord to a string with ", " as sperator
        /// </summary>
        /// <param name="words">The words.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        private static string IWordsToString(IList<IWord> words)
        {
            string output = string.Empty;
            foreach (IWord word in words)
            {
                if (output.Length > 0)
                    output += ", ";

                output += word.Word;
            }

            return output;
        }

        /// <summary>
        /// Fits the string to an area.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="Text">The Text.</param>
        /// <param name="font">The font.</param>
        /// <param name="maxLength">max length in pixel which is available</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        private static string FitStringToArea(Graphics g, string text, Font font, int maxLength)
        {
            SizeF area = g.MeasureString(text, font);
            for (int i = 0; ((int)area.Width > maxLength); i++)
            {
                text = text.Substring(0, text.Length - 2);
                area = g.MeasureString(text, font);

                //important do avoid a endless loop
                if (i >= FIT_STRING_TO_AREA_TIMEOUT)
                    return text;
            }

            return text;
        }

        private struct LearningModulePreviewStruct
        {
            public string Text;
            public string ExampleText;
            public Image Media;
            public string Description;
        }
    }
}
