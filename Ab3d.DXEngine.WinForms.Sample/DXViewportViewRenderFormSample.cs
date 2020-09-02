using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Cameras;
using Ab3d.Controls;
using Ab3d.DirectX;
using Ab3d.DirectX.Controls;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Windows;

namespace Ab3d.DXEngine.WinForms.Sample
{
    // This sample shows how to use Ab3d.DXEngine with SharpDX's RenderForm.
    // Using RenderForm means that the whole form is used for the DirectX content.
    // To use only a part of the for for the DirectX content, use the DXRenderControl (DXViewportViewRenderControlSample).
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

    class DXViewportViewRenderFormSample : IDisposable
    {
        private const int Width = 900;
        private const int Height = 500;


        private RenderForm _renderForm;

        private DXDevice _dxDevice;
        private DXScene _dxScene;

        private Viewport3D _mainViewport3D;
        private TargetPositionCamera _targetPositionCamera;
        private MouseCameraController _mouseCameraController;
        private DXViewportView _dxViewportView;

        /// <summary>
        /// Create and initialize a new game.
        /// </summary>
        public DXViewportViewRenderFormSample()
        {
            // Set window properties
            _renderForm = new RenderForm("DXViewportView with SharpDX RenderForm");
            _renderForm.ClientSize = new System.Drawing.Size(Width, Height);
            _renderForm.AllowUserResizing = true;

            InitializeDxEngine();
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void Run()
        {
            // Start the render loop
            RenderLoop.Run(_renderForm, RenderCallback);
        }

        private void RenderCallback()
        {
            // UpdateScene(); // If you need to play an animation or update the objects, this can be done in the Update method

            RenderScene();
        }


        private void InitializeDxEngine()
        {
            // Create DXDevice - DXEngine's wrapper for DirectX 11 device
            var dxDeviceConfiguration = new DXDeviceConfiguration();
            dxDeviceConfiguration.DriverType = DriverType.Hardware; // Change to DriverType.Warp to use software rendering

            _dxDevice = new DXDevice(dxDeviceConfiguration);


            // Now create DXScene object from the RenderForm Handle
            // The DXScene will use SwapChain to show the rendered scene
            _dxScene = _dxDevice.CreateDXSceneWithSwapChain(_renderForm.Handle, _renderForm.ClientSize.Width, _renderForm.ClientSize.Height, preferedMultisampleCount: 4);
            _dxScene.BackgroundColor = System.Windows.Media.Colors.LightBlue.ToColor4();
            _dxScene.ShaderQuality = ShaderQuality.Normal;

            // _mainViewport3D will define the 3D scene. The scene is defined with using WPF's Viewport3D object.
            // Because Viewport3D is used, this means that we can use many helper objects from Ab3d.PowerToys library.
            _mainViewport3D = new Viewport3D();

            // When we use Viewport3D to define the 3D scene, we need to use DXViewportView to render its content in Ab3d.DXEngine.
            // The DXViewportView will convert all 3D objects defined in Viewport3D to DXEngine's SceneNodes.
            // It is also possible to define 3D scene with using only SceneNode and other low level DXEngine objects.
            // See PictureBoxForm sample and samples in "Advanced usage" section in Ab3d.DXEngine.Samples project.
            _dxViewportView = new DXViewportView(_dxScene, _mainViewport3D);

            // Set DXDiagnostics.CurrentDXView to allow using DXEngineSnoop tool (see https://www.ab4d.com/DirectX/3D/Diagnostics.aspx)
            // Note that CurrentDXView is using WeakReference to prevent rooting the _dxViewportView by a static filed.
            Ab3d.DirectX.DXDiagnostics.CurrentDXView = _dxViewportView;


            // Now create a camera and mouse camera controller
            _targetPositionCamera = new TargetPositionCamera()
            {
                Heading = 30,
                Attitude = -20,
                Distance = 2000,
                TargetViewport3D = _mainViewport3D
            };

            _targetPositionCamera.Refresh(); // We need to manually call Refresh because this camera is not added to WPF objects tree and therefore its Loaded event is never fired.


            // Create WinFormsMouseCameraController that can work with WinForms's Form instead of WPF objects.
            // The WinFormsMouseCameraController is defined in this project - you can check and update its source.
            _mouseCameraController = new WinFormsMouseCameraController(_renderForm)
            {
                TargetCamera = _targetPositionCamera,
                EventsSourceElement = _dxViewportView,
                RotateCameraConditions = MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed,
                MoveCameraConditions = MouseCameraController.MouseAndKeyboardConditions.ControlKey | MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed
            };


            // Create a sample 3D scene
            Scene3DGenerator.CreateSampleScene(_mainViewport3D);


            // And finally render the first frame
            RenderScene();
        }
        
        private void RenderScene()
        {
            if (_dxDevice == null)
                return;

            if (_renderForm.ClientSize.Width != _dxScene.Width || _renderForm.ClientSize.Height != _dxScene.Height)
            {
                // Resize buffers
                _dxScene.Resize(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height);
            }

            // Render the scene with DXEngine.
            // We force rendering event if there are no known changes on the scene (no _dxScene.NotifyChange calls).
            // To render only when there are any know changes, set forceRenderAll to false
            _dxScene.RenderScene(forceRenderAll: true);
        }

        public void Dispose()
        {
            // Dispose created resources:
            if (_dxViewportView != null)
            {
                _dxViewportView.Dispose();
                _dxViewportView = null;
            }

            if (_dxScene != null)
            {
                _dxScene.Dispose();
                _dxScene = null;
            }

            if (_dxDevice != null)
            {
                _dxDevice.Dispose();
                _dxDevice = null;
            }
        }
    }
}
