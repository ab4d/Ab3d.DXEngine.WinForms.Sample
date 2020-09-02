using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ab3d.Controls;
using Ab3d.DirectX;
using Ab3d.DirectX.Models;
using SharpDX;
using SharpDX.Direct3D;
using Point = System.Drawing.Point;

namespace Ab3d.DXEngine.WinForms.Sample
{
    // This sample shows how to use DXEngine inside WinForm's PictureBox.

    // The sample uses the PictureBox's handle to create a DirectX SwapChain.
    // This will render the DirectX content to the area defined by the PictureBox.

    // Because there is no DXViewportView control, it is not possible to use WPF's Viewport3D control.
    // This means that the functionality that relays on Viewport3D and other WPF objects will not work.
    // Also most of the features and objects from Ab3d.PowerToys will not work - for example:
    // - EventManager3D,
    // - Cameras from Ab3d.PowerToys library,
    // - MouseCameraController and other camera controllers,
    // - 3D lines with arrows created with Ab3d.PowerToys objects line LineVisual3D
    // - All Visual3D that use UIElement3D objects including WireframeVisual3D, ModelMoverVisual3D and ModelRotatorVisual3D

    public partial class PictureBoxSample : Form
    {
        private DXDevice _dxDevice;
        private DXScene _dxScene;
        private float _cameraDistance;
        private bool _isSizeChanged;
        
        private float _cameraHeading;
        private float _cameraAttitude;

        private DateTime _cameraAnimationStartTime;
        private float _animationStartHeading;

        private Ab3d.DirectX.Lights.DirectionalLight _directionalLight;
        private Timer _cameraRotatingTimer;

        private AnaglyphVirtualRealityProvider _anaglyphVirtualRealityProvider;
        private bool _isMouseRotating;
        private Point _mouseStartPosition;

        private DisposeList _disposables;

        public Ab3d.DirectX.Cameras.MatrixCamera Camera { get; set; }

        public PictureBoxSample()
        {
            InitializeComponent();

            _cameraAttitude = -30;


            // Subscribe mouse events for mouse rotation
            pictureBox1.MouseDown += OnMouseDown;
            pictureBox1.MouseUp += OnMouseUp;
            pictureBox1.MouseMove += OnMouseMove;
            pictureBox1.MouseWheel += OnMouseWheel;

            //var mouseCameraController = new WinFormsMouseCameraController(pictureBox1)
            //{
            //    TargetCamera           = _targetPositionCamera,
            //    EventsSourceElement    = _dxViewportView,
            //    RotateCameraConditions = MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed,
            //    MoveCameraConditions   = MouseCameraController.MouseAndKeyboardConditions.ControlKey | MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed
            //};

            // We will change the camera heading with a timer.
            // We will do that so that we will target rendering at 60 FPS.
            // Here we only setup the timer. We will start it after the 3D scene is created.
            _cameraRotatingTimer = new System.Windows.Forms.Timer();
            _cameraRotatingTimer.Interval = 1000 / 60; // 60 frames per second
            _cameraRotatingTimer.Tick += delegate (object sender, EventArgs args)
            {
                if (_dxScene == null || _isMouseRotating)
                    return;

                AnimateCamera();

                UpdateViewMatrixAndLightDirection();

                RenderScene();
            };

            // But if we want maximum number of rendered frames per second we can use the solution
            // proposed by Tom Miller (http://blogs.msdn.com/b/tmiller/archive/2005/05/05/415008.aspx?PageIndex=2#comments)
            // Also described as best practice for SlimDX (http://slimdx.org/docs/#Managed_Message_Loop) 
            

            this.Load += delegate(object sender, EventArgs args)
            {
                InitializeDXEngine();
            };

            this.Closing += delegate(object sender, CancelEventArgs args)
            {
                // Stop timer
                if (_cameraRotatingTimer != null)
                {
                    _cameraRotatingTimer.Stop();
                    _cameraRotatingTimer = null;
                }

                if (_anaglyphVirtualRealityProvider != null)
                {
                    _anaglyphVirtualRealityProvider.Dispose();
                    _anaglyphVirtualRealityProvider = null;
                }

                _disposables.Dispose();

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
            };
        }

        private void InitializeDXEngine()
        {
            // Create DXDevice - DXEngine's wrapper for DirectX 11 device
            var dxDeviceConfiguration = new DXDeviceConfiguration();
            dxDeviceConfiguration.DriverType = DriverType.Hardware; // Use DriverType.Warp to use software rendering

            _dxDevice = new DXDevice(dxDeviceConfiguration);


            // Now create DXScene object from the PictureBox Handle
            _dxScene = _dxDevice.CreateDXSceneWithSwapChain(pictureBox1.Handle, pictureBox1.Width, pictureBox1.Height, preferedMultisampleCount: 4);
            _dxScene.BackgroundColor = System.Windows.Media.Colors.LightBlue.ToColor4();
            _dxScene.ShaderQuality = ShaderQuality.Normal;


            // Create a simple Matrix camera
            // For Matrix camera we need to provide Projection and View matrix
            // Projection matrix will be calculated in UpdateProjectionMatrix - this is also called in RenderScene when _isSizeChanged is true
            // View matrix will be calculated in UpdateViewMatrixAndLightDirection method
            Camera = new Ab3d.DirectX.Cameras.MatrixCamera();
            Camera.NearPlaneDistance = 0.1f;
            Camera.FarPlaneDistance  = 10000.0f;

            UpdateProjectionMatrix();

            _dxScene.Camera = Camera;

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

            // Start camera animation
            StartCameraRotation();
        }
        

        private void UpdateViewMatrixAndLightDirection()
        {
            Quaternion rotationQuaternion = Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(_cameraHeading), MathUtil.DegreesToRadians(_cameraAttitude), 0);

            Vector3 cameraPosition = Vector3.Transform(new Vector3(0, 0, _cameraDistance), rotationQuaternion);

            // Calculate view matrix from specified camera position and camera target
            Camera.View = Matrix.LookAtRH(eye: cameraPosition, target: new Vector3(0, 0, 0), up: Vector3.UnitY);


            // Update the light - same direction as camera
            Vector3 direction = new Vector3(-cameraPosition.X, -cameraPosition.Y, -cameraPosition.Z);
            direction.Normalize();

            _directionalLight.Direction = direction;
        }

        private void AnimateCamera()
        {
            if (_cameraAnimationStartTime == DateTime.MinValue)
            {
                _cameraAnimationStartTime = DateTime.Now;
                _animationStartHeading = _cameraHeading;
            }

            var elapsedSeconds = (_cameraAnimationStartTime - DateTime.Now).TotalSeconds;

            _cameraHeading = _animationStartHeading + (float) elapsedSeconds * 45.0f; // Change heading for 45 degrees each second
        }

        private void UpdateProjectionMatrix()
        {
            Camera.AspectRatio = (float)pictureBox1.Width / (float)pictureBox1.Height;
            Camera.Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, Camera.AspectRatio, Camera.NearPlaneDistance, Camera.FarPlaneDistance);
        }

        private void RenderScene()
        {
            if (_dxDevice == null)
                return;

            if (_isSizeChanged)
            {
                UpdateProjectionMatrix();

                _dxScene.Resize(pictureBox1.Width, pictureBox1.Height);

                _isSizeChanged = false;
            }

            // Render the scene with DXEngine.
            // We force rendering event if there are no known changes on the scene (no _dxScene.NotifyChange calls).
            // To render only when there are any know changes, set forceRenderAll to false
            _dxScene.RenderScene(forceRenderAll: true);
        }


        private void CreateObjectsScene(SceneNode dxSceneRootNode)
        {
            Scene3DGenerator.CreateSampleScene(dxSceneRootNode, out _disposables);

            // Calculate the distance of the camera 
            // so that all the objects will be visible (use size of scene)
            dxSceneRootNode.UpdateBounds();
            _cameraDistance = dxSceneRootNode.Bounds.GetDiagonalLength() * 0.5f;

            UpdateViewMatrixAndLightDirection();
        }


        #region Mouse handling
        private void OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            float factor;

            if (mouseEventArgs.Delta < 0)
                factor = 1.05f;
            else
                factor = 0.95f;

            _cameraDistance *= factor;

            UpdateViewMatrixAndLightDirection();
            RenderScene();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _isMouseRotating = true;
            _mouseStartPosition = e.Location;

            base.OnMouseDown(e);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _cameraAnimationStartTime = DateTime.MinValue; // This will rotate from the current position on

            base.OnMouseUp(e);
        }


        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                if (_isMouseRotating)
                    StopMouseRotate();

                base.OnMouseMove(e);
                return;
            }

            int dx = (e.Location.X - _mouseStartPosition.X);
            int dy = (e.Location.Y - _mouseStartPosition.Y);

            _cameraHeading  -= (float)dx * 0.2f;
            _cameraAttitude -= (float)dy * 0.2f;

            _mouseStartPosition = e.Location;

            UpdateViewMatrixAndLightDirection();
            RenderScene();

            base.OnMouseMove(e);
        }

        private void StopMouseRotate()
        {
            _isMouseRotating = false;
        }
        #endregion



        private void StartCameraRotation()
        {
            _cameraAnimationStartTime = DateTime.MinValue;
            _cameraRotatingTimer.Start();
        }

        private void StopCameraRotation()
        {
            _cameraAnimationStartTime = DateTime.MinValue;
            _cameraRotatingTimer.Stop();

            // NOTE:
            // Because in this simple sample we only change the camera, 
            // we could also stop the _renderTimer when we stop animating the camera.
        }

        private void startRotateButton_Click(object sender, EventArgs e)
        {
            startRotateButton.Enabled = false;
            stopRotateButton.Enabled = true;

            StartCameraRotation();
        }

        private void stopRotateButton_Click(object sender, EventArgs e)
        {
            startRotateButton.Enabled = true;
            stopRotateButton.Enabled = false;

            _dxScene.DumpSceneNodes();
            _dxScene.DumpRenderingQueues();
            

            StopCameraRotation();
        }

        private void anaglyphButton_Click(object sender, EventArgs e)
        {
            if (_anaglyphVirtualRealityProvider == null)
            {
                // Initialize AnaglyphVirtualRealityProvider for read-cyan glasses
                // To see more information about AnaglyphVirtualRealityProvider see the sample in Ab3d.DXEngine.Wpf.Samples.
                //
                // It is also possible to use SplitScreenVirtualRealityProvider for 3D TVs

                _anaglyphVirtualRealityProvider = new AnaglyphVirtualRealityProvider(eyeSeparation: 15.0f,
                                                                                     parallax: 0.6f,
                                                                                     anaglyphColorTransformation: AnaglyphVirtualRealityProvider.OptimizedAnaglyph);

                _dxScene.InitializeVirtualRealityRendering(_anaglyphVirtualRealityProvider);

                anaglyphButton.Text = "Hide Anaglyph VR";
            }
            else
            {
                _dxScene.InitializeVirtualRealityRendering(null);

                _anaglyphVirtualRealityProvider.Dispose();
                _anaglyphVirtualRealityProvider = null;

                anaglyphButton.Text = "Show Anaglyph VR";
            }

            RenderScene();
        }
    }
}
