using System;
using System.Drawing;
using System.Windows.Media.Media3D;
using Ab3d.DirectX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Windows;
using DirectionalLight = Ab3d.DirectX.Lights.DirectionalLight;
using Quaternion = SharpDX.Quaternion;

namespace Ab3d.DXEngine.WinForms.Sample
{
    // This sample shows an advanced way to use DXEngine in WinForm application with using SharpDX's RenderForm.
    // There the DXEngine is used without DXViewportView control.
    // Instead the DXDevice and the DXScene objects are created manually.
    //
    // Because there is no DXViewportView control, it is not possible to use WPF's Viewport3D control.
    // This means that the functionality that relays on Viewport3D and other WPF objects will not work.
    // Also most of the features and objects from Ab3d.PowerToys will not work - for example:
    // - EventManager3D,
    // - Cameras from Ab3d.PowerToys library,
    // - MouseCameraController and other camera controllers,
    // - 3D lines with arrows created with Ab3d.PowerToys objects line LineVisual3D
    // - All Visual3D that use UIElement3D objects including WireframeVisual3D, ModelMoverVisual3D and ModelRotatorVisual3D
    //
    // This sample renders DirectX content to the whole RenderForm.
    // To render only to a part of the form, see PictureBoxSample (or DXViewportViewRenderControlSample or DXViewportViewElementHostSample).

    public class RenderFormSample : IDisposable
    {
        private const int Width = 900;
        private const int Height = 500;


        private RenderForm _renderForm;

        private DXDevice _dxDevice;
        private DXScene _dxScene;

        private Ab3d.DirectX.Cameras.MatrixCamera _camera;
        private DirectionalLight _directionalLight;
        
        private float _animationStartHeading;
        private DateTime _cameraAnimationStartTime;

        private float _cameraHeading;
        private float _cameraAttitude;
        private float _cameraDistance;
        private DisposeList _disposables;


        public RenderFormSample()
        {
            // Set window properties
            _renderForm = new RenderForm("DXEngine with SharpDX RenderForm");
            _renderForm.ClientSize = new Size(Width, Height);
            _renderForm.AllowUserResizing = true;

            _cameraHeading = 0;
            _cameraAttitude = -30;

            InitializeDxEngine();
        }

        public void Run()
        {
            // Start the render loop
            RenderLoop.Run(_renderForm, RenderCallback);
        }

        private void RenderCallback()
        {
            UpdateViewMatrixAndLightDirection();
            RenderScene();
        }


        private void InitializeDxEngine()
        {
            // Create DXDevice - DXEngine's wrapper for DirectX 11 device
            var dxDeviceConfiguration = new DXDeviceConfiguration();
            dxDeviceConfiguration.DriverType = DriverType.Hardware; // Use DriverType.Warp to use software rendering

            _dxDevice = new DXDevice(dxDeviceConfiguration);


            // Now create DXScene object from the RenderForm Handle
            _dxScene = _dxDevice.CreateDXSceneWithSwapChain(_renderForm.Handle, _renderForm.ClientSize.Width, _renderForm.ClientSize.Height, preferedMultisampleCount: 4);

            _dxScene.BackgroundColor = System.Windows.Media.Colors.LightBlue.ToColor4();
            _dxScene.ShaderQuality = ShaderQuality.Normal;


            // Create a simple Matrix camera
            // For Matrix camera we need to provide Projection and View matrix
            // Projection matrix will be calculated in UpdateProjectionMatrix - this is also called in RenderScene when _isSizeChanged is true
            // View matrix will be calculated in UpdateViewMatrixAndLightDirection method
            _camera = new Ab3d.DirectX.Cameras.MatrixCamera();
            _camera.NearPlaneDistance = 0.1f;
            _camera.FarPlaneDistance = 10000.0f;

            UpdateProjectionMatrix();

            _dxScene.Camera = _camera;

            // We could also create a WPF camera and then use WpfCamera from DXEngine to convert WPF camera into DXEngine camera:
            //var perspectiveCamera = new System.Windows.Media.Media3D.PerspectiveCamera()
            //{
            //    Position = new System.Windows.Media.Media3D.Point3D(30, 50, -500),
            //    LookDirection = new System.Windows.Media.Media3D.Vector3D(-0.3, -0.5, 50),
            //};

            //var wpfCamera = new Ab3d.DirectX.Cameras.WpfCamera(perspectiveCamera, aspectRatio: this.pictureBox1.Width / (float) this.pictureBox1.Height);

            //_dxScene.Camera = wpfCamera;



            // Add ambient and directional light
            var ambientLight = new Ab3d.DirectX.Lights.AmbientLight(0.05f);
            _dxScene.Lights.Add(ambientLight);

            _directionalLight = new Ab3d.DirectX.Lights.DirectionalLight();
            // _directionalLight.Direction is set in the UpdateViewMatrixAndLightDirection method
            _dxScene.Lights.Add(_directionalLight);


            UpdateViewMatrixAndLightDirection();


            // The 3D scene in DXEngine's DXScene is defined by hierarchy of SceneNodes
            // We start with root scene node:
            var rootNode = new SceneNode("RootNode");
            _dxScene.RootNode = rootNode;

            // Create sample objects and add them to root node
            CreateObjectsScene(_dxScene.RootNode);


            // Render first frame
            RenderScene();
        }


        private void RenderScene()
        {
            if (_dxDevice == null)
                return;

            if (_renderForm.ClientSize.Width != _dxScene.Width || _renderForm.ClientSize.Height != _dxScene.Height)
            {
                UpdateProjectionMatrix();

                // Resize buffers
                _dxScene.Resize(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height);
            }

            // Render the scene with DXEngine.
            // We force rendering even if there are no known changes on the scene (no _dxScene.NotifyChange calls).
            // To render only when there are any know changes, set forceRenderAll to false
            _dxScene.RenderScene(forceRenderAll: true);
        }



        private void UpdateViewMatrixAndLightDirection()
        {
            if (_cameraAnimationStartTime == DateTime.MinValue)
            {
                _cameraAnimationStartTime = DateTime.Now;
                _animationStartHeading = _cameraHeading;
            }

            var elapsedSeconds = (_cameraAnimationStartTime - DateTime.Now).TotalSeconds;

            _cameraHeading = _animationStartHeading + (float)elapsedSeconds * 45.0f; // Change heading for 45 degrees each second

            SharpDX.Quaternion rotationQuaternion = Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(_cameraHeading), MathUtil.DegreesToRadians(_cameraAttitude), 0);

            Vector3 cameraPosition = Vector3.Transform(new Vector3(0, 0, _cameraDistance), rotationQuaternion);

            // Calculate view matrix from specified camera position and camera target
            _camera.View = Matrix.LookAtRH(eye: cameraPosition, target: new Vector3(0, 0, 0), up: Vector3.UnitY);


            // Update the light - same direction as camera
            Vector3 direction = new Vector3(-cameraPosition.X, -cameraPosition.Y, -cameraPosition.Z);
            direction.Normalize();

            _directionalLight.Direction = direction;
        }

        private void UpdateProjectionMatrix()
        {
            _camera.AspectRatio = (float)_renderForm.ClientSize.Width / (float)_renderForm.ClientSize.Height;
            _camera.Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, _camera.AspectRatio, _camera.NearPlaneDistance, _camera.FarPlaneDistance);
        }


        private void CreateObjectsScene(SceneNode dxSceneRootNode)
        {
            Scene3DGenerator.CreateSampleScene(dxSceneRootNode, out _disposables);

            // Calculate the distance of the camera 
            // so that all the objects will be visible (use size of scene)
            dxSceneRootNode.UpdateBounds();
            _cameraDistance = dxSceneRootNode.Bounds.GetDiagonalLength() * 0.7f;

            UpdateViewMatrixAndLightDirection();
        }




        public void Dispose()
        {
            // Dispose created resources:
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