using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PhotoResizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion


        #region Event Handlers
        /// <summary>
        /// btnChooseDirectory_Click allows the User to choose a 
        /// Directory with Images in it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChooseDirectory_Click(object sender, RoutedEventArgs e)
        {
            //Create a Directory Dialog 
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Choose Directory";
            fbd.ShowNewFolderButton = false;

            //If the User came back with a Directory, put it
            //in the TextBox
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtDirectory.Text = fbd.SelectedPath;
        }

        
        /// <summary>
        /// btnResize_Click does the work of resizing all of the Images
        /// in the chosen Directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResize_Click(object sender, RoutedEventArgs e)
        {
            //Disable the Resize button
            this.btnResize.Dispatcher.Invoke(new EnableResizeButtonDelegate(this.EnableResizeButton), false);

            //Get the Input directory
            String inputDir = this.txtDirectory.Text;

            //Begin a Task to resize
            Task t = new Task(this.ResizeImages, inputDir);
            t.ContinueWith(x => this.btnResize.Dispatcher.Invoke(new EnableResizeButtonDelegate(this.EnableResizeButton), true));
            t.Start();
        }
        #endregion


        #region MultiThreading Method
        /// <summary>
        /// ResizeImages does the work of resizing the Images.
        /// </summary>
        private void ResizeImages(Object input)
        {
            //Get the input and output Directories
            String inputDir = input.ToString();
            String outputDir = Path.Combine(inputDir, "Resized");

            //Get a list of the Files in the Directory
            String[] filePaths = Directory.GetFiles(inputDir);

            //Setup the ProgressBar
            this.pbMain.Dispatcher.Invoke(new SetupProgressBarDelegate(this.SetupProgressBar), filePaths.Length);

            //Iterate through the Files and try to make resized
            //Images out of them
            foreach (String filePath in filePaths)
            {
                //Try to make an Image
                Image img = null;
                try
                {
                    //Open the image
                    img = Image.FromFile(filePath);

                    //Resize the Image
                    Image resize = img.Resize(800);

                    //Create the Directory if it doesn't exist
                    if (Directory.Exists(outputDir) == false)
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    //Setup the output File Path
                    String resizePath = Path.Combine(outputDir, Path.GetFileName(filePath));

                    //Save the Image to the Directory
                    resize.Save(resizePath);

                    //Update the ProgressBar
                    this.pbMain.Dispatcher.Invoke(new IncrementProgressBarValueDelegate(this.IncrementProgressBarValue));
                }
                catch { }
                finally
                {
                    //If the Image was created Dispose of it
                    if (img != null)
                    {
                        img.Dispose();
                    }
                }
            }
        }
        #endregion


        #region Delegate methods for multithreading
        /// <summary>
        /// Delegate type for setting up the ProgressBar
        /// </summary>
        /// <param name="maximum"></param>
        private delegate void SetupProgressBarDelegate(Int32 maximum);

        /// <summary>
        /// SetupProgressBar does the work of setting up the
        /// ProgressBar.
        /// </summary>
        /// <param name="maximum"></param>
        private void SetupProgressBar(Int32 maximum)
        {
            //Setup the Progress Bar
            this.pbMain.Minimum = 0;
            this.pbMain.Maximum = maximum;
            this.pbMain.Value = 0;
        }

        /// <summary>
        /// Delegate type for incrementing the ProgressBar value.
        /// </summary>
        private delegate void IncrementProgressBarValueDelegate();

        /// <summary>
        /// IncrementProgressBarValue does the work of incrementing
        /// the value on the ProgressBar.
        /// </summary>
        private void IncrementProgressBarValue()
        {
            this.pbMain.Value++;
        }

        
        /// <summary>
        /// Delegate for enabling the Resize button.
        /// </summary>
        /// <param name="enabled">Boolean to determine whether the button should be enabled</param>
        private delegate void EnableResizeButtonDelegate(Boolean enabled);


        /// <summary>
        /// EnableResizeButton does the work of enabling or 
        /// disabling the Resize button.
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableResizeButton(Boolean enabled)
        {
            this.btnResize.IsEnabled = enabled;
        }
        #endregion
    }
}
