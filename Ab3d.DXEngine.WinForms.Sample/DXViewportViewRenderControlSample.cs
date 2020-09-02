using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Ab3d.Cameras;
using Ab3d.Controls;
using Brushes = System.Windows.Media.Brushes;

namespace Ab3d.DXEngine.WinForms.Sample
{
    // This sample shows how to use Ab3d.DXEngine with SharpDX's RenderControl.
    // This way the DirectX content is rendered to the area define by the RenderControl.
    // 
    // The sample is demonstrating the recommended way of using Ab3d.DXEngine.
    // This way the 3D scene is defined with WPF's Viewport3D object and
    // the DXEngine's DXViewportView object is used to convert 3D scene into
    // DXEngine's SceneNode and other low level objects.
    //
    // This way the helper objects from Ab3d.PowerToys and Ab3d.DXEngine libraries
    // can be used in WinForms application. This allows very easy work with 3D objects
    // and in most cases this has much more advantages than a small disadvantage 
    // because of a small performance hit when initializing the 3D scene.
    //
    // To see the capabilities and features of Ab3d.PowerToys and Ab3d.DXEngine libraries
    // please check the main sample projects that come with those two libraries.
    // Those two projects are written for WPF, but they can be easily converted into
    // WinForms code because all the objects can be also defined in code (even if in the samples they are partially defined in XAML).
    //
    // If you want to mix existing SharpDX code with DXEngine code,
    // please check the samples in the "Advanced usage" section of the Ab3d.DXEngine samples project.

    public partial class DXViewportViewRenderControlSample : Form
    {
        private TargetPositionCamera _targetPositionCamera;
        private WinFormsMouseCameraController _mouseCameraController;
        private DXRenderControl _dxRenderControl;

        public DXViewportViewRenderControlSample()
        {
            InitializeComponent();

            _dxRenderControl = new DXRenderControl();
            _dxRenderControl.Dock = DockStyle.Fill;

            renderControlHostPanel.Controls.Add(_dxRenderControl);


            // Set DXDiagnostics.CurrentDXView to allow using DXEngineSnoop tool (see https://www.ab4d.com/DirectX/3D/Diagnostics.aspx)
            // Note that CurrentDXView is using WeakReference to prevent rooting the _dxViewportView by a static filed.
            Ab3d.DirectX.DXDiagnostics.CurrentDXView = _dxRenderControl.DXViewportView;


            // Now create a camera and mouse camera controller
            _targetPositionCamera = new TargetPositionCamera()
            {
                Heading          = 30,
                Attitude         = -20,
                Distance         = 2000,
                TargetViewport3D = _dxRenderControl.MainViewport3D
            };

            _targetPositionCamera.Refresh(); // We need to manually call Refresh because this camera is not added to WPF objects tree


            // Create WinFormsMouseCameraController that can work with WinForms's Form instead of WPF objects
            _mouseCameraController = new WinFormsMouseCameraController(_dxRenderControl)
            {
                TargetCamera           = _targetPositionCamera,
                EventsSourceElement    = _dxRenderControl.DXViewportView,
                RotateCameraConditions = MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed,
                MoveCameraConditions   = MouseCameraController.MouseAndKeyboardConditions.ControlKey | MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed
            };


            // Create a sample 3D scene
            Scene3DGenerator.CreateSampleScene(_dxRenderControl.MainViewport3D);


            _targetPositionCamera.StartRotation(40, 0);
            startRotationButton.Enabled = false;


            this.Load += delegate(object sender, EventArgs args)
            {
                // Activate this form when it is loaded to start processing mouse event immediately without user need to click on the form.
                this.Activate();

                Run();
            };

            this.Closing += delegate(object sender, CancelEventArgs args)
            {
                if (_dxRenderControl != null)
                {
                    _dxRenderControl.Dispose();
                    _dxRenderControl = null;
                }
            };
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void Run()
        {
            // Start the render loop
            SharpDX.Windows.RenderLoop.Run(this, RenderCallback, useApplicationDoEvents: true);
        }

        private void RenderCallback()
        {
            // UpdateScene(); // If you need to play an animation or update the objects, this can be done in the Update method

            _dxRenderControl.RenderScene();
        }

        private void startRotationButton_Click(object sender, EventArgs e)
        {
            startRotationButton.Enabled = false;
            stopRotationButton.Enabled = true;

            _targetPositionCamera.StartRotation(40, 0);
        }

        private void stopRotationButton_Click(object sender, EventArgs e)
        {
            startRotationButton.Enabled = true;
            stopRotationButton.Enabled = false;

            _targetPositionCamera.StopRotation();
        }
    }
}
