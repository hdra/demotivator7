using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;

namespace WP7_ImageEditTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        //size fo the image inside the poster
        private int imgWidth;
        private int imgHeight;
        //size of the poster itself
        private int posterWidth;
        private int posterHeight;

        private int topMargin;

        PhotoChooserTask photoChooser;
        private BitmapImage posterBitmap;

        public MainPage()
        {
            InitializeComponent();            
            photoChooser = new PhotoChooserTask();
            photoChooser.Completed += new EventHandler<PhotoResult>(photoChooser_Completed);
        }

        void photoChooser_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //BitmapImage bmp = new BitmapImage();
                //bmp.SetSource(e.ChosenPhoto);
                posterBitmap = null;
                posterBitmap = new BitmapImage();
                posterBitmap.SetSource(e.ChosenPhoto);

                //portrait
                if (posterBitmap.PixelHeight > posterBitmap.PixelWidth)
                {
                    imgHeight = 700;
                    imgWidth = 525;
                    posterHeight = 880;
                    posterWidth = 700;
                    topMargin = 50;


                    previewImage.Height = 350;
                    previewImage.Width = 262;
                    whiteBorder.Height = 351;
                    whiteBorder.Width = 263;
                }
                else
                {
                    imgHeight = 525;
                    imgWidth = 700;
                    posterHeight = 700;
                    posterWidth = 840;
                    topMargin = 50;

                    previewImage.Height = 262;
                    previewImage.Width = 350;
                    whiteBorder.Height = 262;
                    whiteBorder.Width = 351;
                }
                //a debug to see the image
                previewImage.Source = posterBitmap;
            }
        }

        private void editPic()
        {
            #region textblocks
            TextBlock bigCap = new TextBlock();
            TextBlock smallCap = new TextBlock();
            bigCap.Text = topLineTextBox.Text.ToUpper();
            smallCap.Text = bottomLineTextBox.Text;

            bigCap.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            bigCap.TextAlignment = TextAlignment.Center;
            bigCap.Width = posterWidth;

            smallCap.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            smallCap.TextAlignment = TextAlignment.Center;
            smallCap.Width = posterWidth;

            bigCap.FontFamily = new FontFamily("Times New Roman");
            smallCap.FontFamily = new FontFamily("Times New Roman");
            bigCap.FontSize = 46;
            smallCap.FontSize = 20;
            bigCap.FontWeight = FontWeights.Bold;
            bigCap.TextWrapping = TextWrapping.Wrap;
            smallCap.TextWrapping = TextWrapping.Wrap;
            bigCap.Foreground = new SolidColorBrush(Colors.White);
            smallCap.Foreground = new SolidColorBrush(Colors.White);

            #endregion

            Image posterImage = new Image();
            posterImage.Source = posterBitmap;
            posterImage.Height = imgHeight;
            posterImage.Width = imgWidth;
            posterImage.Stretch = Stretch.UniformToFill;

            Border b = new Border();
            b.BorderThickness = new Thickness(5);
            b.BorderBrush = new SolidColorBrush(Colors.White);
            b.Child = null;
            b.Child = posterImage;

            Rectangle rec = new Rectangle();
            rec.Width = imgWidth + 6;
            rec.Height = imgHeight + 6;
            rec.Fill = new SolidColorBrush(Colors.White);

            WriteableBitmap newBitmap = new WriteableBitmap(posterWidth, posterHeight);
            newBitmap.Clear(Colors.Black);

            newBitmap.Render(rec, new TranslateTransform() { Y = topMargin - 3, X = ((posterWidth - imgWidth) / 2)-3 });
            newBitmap.Render(b, new TranslateTransform() { Y = topMargin, X = (posterWidth - imgWidth) / 2 });
            newBitmap.Render(bigCap, new TranslateTransform() {Y = topMargin+imgHeight+20, X = 0});
            newBitmap.Render(smallCap, new TranslateTransform() { Y = topMargin + imgHeight+ 80, X = 0 });
            newBitmap.Invalidate();
            savePicture(newBitmap);                        
        }

        private void savePicture(WriteableBitmap wb)
        {
            string tempJPEG = "tempJPEG";
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(tempJPEG))
            {
                myStore.DeleteFile(tempJPEG);
            }

            IsolatedStorageFileStream myFileStream = myStore.CreateFile(tempJPEG);

            wb.SaveJpeg(myFileStream, wb.PixelWidth, wb.PixelHeight, 0, 90);

            MediaLibrary library = new MediaLibrary();
            myFileStream.Seek(0, 0);
            library.SavePicture(topLineTextBox + "-" + bottomLineTextBox, myFileStream);
            MessageBox.Show("Image saved to the Pictures Hub");
            myFileStream.Close();
        }

        private void appBarSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (((BitmapSource)previewImage.Source).PixelHeight != 0)
                    editPic();
                else
                    MessageBox.Show("Please Select and image");
            }
            catch
            {
                MessageBox.Show("Please Select and image");
            }
        }

        private void Border_Tap(object sender, GestureEventArgs e)
        {
            photoChooser.Show();
        }

        private void topLineTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.topTextBlock.Text = topLineTextBox.Text.ToUpper();
        }

        private void bottomLineTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.bottomTextBlock.Text = bottomLineTextBox.Text;
        }

        private void topLineTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (topLineTextBox.Text == "TOP LINE")
                topLineTextBox.Text = "";
        }

        private void bottomLineTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (bottomLineTextBox.Text == "Bottom Line")
                bottomLineTextBox.Text = "";
        }      
    }
}