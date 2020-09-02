using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Cameras;
using Ab3d.Common.Cameras;
using Ab3d.Controls;
using Ab3d.DirectX.Controls;
using Ab3d.Visuals;
using Brushes = System.Windows.Media.Brushes;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace Ab3d.DXEngine.WinForms.Sample
{
    public partial class DXViewportViewElementHostSample : Form
    {
        // This sample shows how to use Ab3d.DXEngine in WinForms application with ElementHost control
        // that allows using WPF content inside WinForms application.
        //
        // When using ElementHost, the samples are the same as in the Ab3d.DXEngine.Wpf.Samples.DXEngine project
        // except that the part that is in that project define in XAML is here defined in code (but we could also use XAML).
        //
        // The biggest advantage of using ElementHost is that you can show more WPF controls 
        // and this allows showing ViewCubeCameraController or CameraAxisPanel on top of 3D scene.
        // This sample also shows an overlay Viewport3D with ModelMoverVisual3D that can be used to move man 3D model.

        private DXViewportView _dxViewportView;
        private TargetPositionCamera _targetPositionCamera;

        private Viewport3D _overlayViewport3D;
        private ModelMoverVisual3D _modelMover;
        private DirectionalLight _overlayViewportLight;
        private Model3DGroup _manModel3DGroup;
        private TranslateTransform3D _manTranslateTransform3D;
        private Point3D _startMovePosition;
        private Vector3D _manInitialOffset;

        public DXViewportViewElementHostSample()
        {
            InitializeComponent();

            // This sample define all WPF object in code.
            // It would also possible to define the scene in XAML.
            // We could also define the scene in XAML file.
            // To do that first add reference to the "Ab3d.DXEngine.Wpf.Samples" project.
            // Then uncomment the following code:

            //// Create instance of sample that is defined with XAML
            //var sampleSceneUserControl = new Ab3d.DXEngine.Wpf.Samples.DXEngine.SampleSceneUserControl();

            //// Because the sampleSceneUserControl is derived from Page, it cannot be directly added to the ElementHost control.
            //// Therefore we need to get the Page.Content, disconnect it from Page and then we can assign it to the ElementHost control:
            //var sampleRootControl = (UIElement)sampleSceneUserControl.Content;
            //sampleSceneUserControl.Content = null;

            //// Now we can assign the sample to elementHost1
            //this.elementHost1.Child = sampleRootControl;

            //return;

            // create root WPF Grid
            var rootGrid = new Grid();
            elementHost1.Child = rootGrid;


            // Create the Viewport3D that will hold our 3D scene
            var wpfViewport3D = new Viewport3D();

            // Initialize the DXViewportView that will render wpfViewport3D with DirectX 11
            _dxViewportView = new DXViewportView(wpfViewport3D);
            _dxViewportView.BackgroundColor = Colors.White;

            rootGrid.Children.Add(_dxViewportView);

            // Set DXDiagnostics.CurrentDXView to allow using DXEngineSnoop tool (see https://www.ab4d.com/DirectX/3D/Diagnostics.aspx)
            // Note that CurrentDXView is using WeakReference to prevent rooting the _dxViewportView by a static filed.
            Ab3d.DirectX.DXDiagnostics.CurrentDXView = _dxViewportView;


            // Create TargetPositionCamera from Ab3d.PowerToys library
            _targetPositionCamera = new TargetPositionCamera()
            {
                TargetPosition = new Point3D(0, 0, 0),
                Heading = 30,
                Attitude = -20,
                Distance = 80,
                TargetViewport3D = wpfViewport3D
            };

            // We need to synchronize the Camera and Lights in OverlayViewport with the camera in the MainViewport
            _targetPositionCamera.CameraChanged += delegate (object s, CameraChangedRoutedEventArgs args)
            {
                _overlayViewport3D.Camera       = wpfViewport3D.Camera;
                _overlayViewportLight.Direction = ((DirectionalLight)_targetPositionCamera.CameraLight).Direction;
            };

            rootGrid.Children.Add(_targetPositionCamera);


            var mouseCameraController = new MouseCameraController()
            {
                RotateCameraConditions = MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed,
                MoveCameraConditions = MouseCameraController.MouseAndKeyboardConditions.LeftMouseButtonPressed | MouseCameraController.MouseAndKeyboardConditions.ControlKey,
                EventsSourceElement = _dxViewportView,
                TargetCamera = _targetPositionCamera
            };

            rootGrid.Children.Add(mouseCameraController);


            var viewCubeCameraController = new ViewCubeCameraController()
            {
                TargetCamera        = _targetPositionCamera,
                VerticalAlignment   = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            rootGrid.Children.Add(viewCubeCameraController);


            // ViewCubeCameraController is shown instead of CameraAxisPanel
            //var cameraAxisPanel = new CameraAxisPanel()
            //{
            //    TargetCamera = _targetPositionCamera,
            //    VerticalAlignment = VerticalAlignment.Bottom,
            //    HorizontalAlignment = HorizontalAlignment.Left
            //};

            //rootGrid.Children.Add(cameraAxisPanel);


            // Add sample 3D objects to wpfViewport3D
            Create3DScene(wpfViewport3D);


            // Add ModelMoverVisual3D with an overlay Viewport3D
            AddOverlayViewport3DWithModelMover(rootGrid);


            this.Closing += delegate(object sender, CancelEventArgs args)
            {
                _dxViewportView.Dispose();
            };
        }

        private void AddOverlayViewport3DWithModelMover(System.Windows.Controls.Panel parentPanel)
        {
            /*  <!-- XAML from: Ab3d.PowerToys.Samples/Utilities/ModelMoverOverlaySample.xaml -->
                <Viewport3D Name="OverlayViewport">
                    <ModelVisual3D>               
                        <visuals:ModelMoverVisual3D x:Name="ModelMover" AxisLength="50" AxisRadius="1.5" AxisArrowRadius="5"
                                                    IsXAxisShown="{Binding ElementName=XAxisCheckBox, Path=IsChecked}"
                                                    IsYAxisShown="{Binding ElementName=YAxisCheckBox, Path=IsChecked}"
                                                    IsZAxisShown="{Binding ElementName=ZAxisCheckBox, Path=IsChecked}"/>
                        <ModelVisual3D.Content>
                            <DirectionalLight x:Name="OverlayViewportLight" />
                        </ModelVisual3D.Content>
                    </ModelVisual3D>
                </Viewport3D>             
             */

            _overlayViewport3D = new Viewport3D();

            var rootOverlayModelVisual3D = new ModelVisual3D();
            _overlayViewport3D.Children.Add(rootOverlayModelVisual3D);

            _modelMover = new ModelMoverVisual3D()
            {
                AxisLength      = 5,
                AxisRadius      = 0.15,
                AxisArrowRadius = 0.5,
                Position        = _manInitialOffset.ToPoint3D()
            };

            // Setup event handlers on ModelMoverVisual3D
            _modelMover.ModelMoveStarted += delegate (object o, EventArgs eventArgs)
            {
                _startMovePosition = new Point3D(_manTranslateTransform3D.OffsetX, _manTranslateTransform3D.OffsetY, _manTranslateTransform3D.OffsetZ);
            };

            _modelMover.ModelMoved += delegate (object o, Ab3d.Common.ModelMovedEventArgs e)
            {
                var newPosition = _startMovePosition + e.MoveVector3D;

                if (Math.Abs(newPosition.X) > 2000 ||
                    Math.Abs(newPosition.Y) > 2000 ||
                    Math.Abs(newPosition.Z) > 2000)
                {
                    //InfoTextBlock.Text = "Move out of range";
                    return;
                }

                _manTranslateTransform3D.OffsetX = newPosition.X;
                _manTranslateTransform3D.OffsetY = newPosition.Y;
                _manTranslateTransform3D.OffsetZ = newPosition.Z;

                _modelMover.Position = newPosition + _manInitialOffset;
            };

            _modelMover.ModelMoveEnded += delegate (object sender, EventArgs args)
            {
                //InfoTextBlock.Text = "";
            };


            rootOverlayModelVisual3D.Children.Add(_modelMover);


            _overlayViewportLight = new DirectionalLight();
            rootOverlayModelVisual3D.Content = _overlayViewportLight;

            parentPanel.Children.Add(_overlayViewport3D);
        }

        private void Create3DScene(Viewport3D viewport3D)
        {
            var readerObj   = new Ab3d.ReaderObj();
            var readModel3D = (Model3DGroup)readerObj.ReadModel3D(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\house with trees.obj"));

            // obj file does not support object hierarchy
            // but we need to group the 3 GeometryModel3D objects that define the man into a new Model3DGroup.
            // So we manually do that here.

            // first remove the 3 objects from the root Model3DGroup
            readModel3D.Children.Remove(readerObj.NamedObjects["Line05"]);
            readModel3D.Children.Remove(readerObj.NamedObjects["Line06"]);
            readModel3D.Children.Remove(readerObj.NamedObjects["Line07"]);

            // ... then create a new Model3DGroup and add the models to it
            _manModel3DGroup = new Model3DGroup();
            _manModel3DGroup.Children.Add(readerObj.NamedObjects["Line05"]);
            _manModel3DGroup.Children.Add(readerObj.NamedObjects["Line06"]);
            _manModel3DGroup.Children.Add(readerObj.NamedObjects["Line07"]);

            // Read initial center position - we will need to offset the ModelMover for that
            _manInitialOffset = _manModel3DGroup.Bounds.GetCenterPosition().ToVector3D();

            // Set a transformation that will be used with ModelMoverVisual3D
            _manTranslateTransform3D   = new TranslateTransform3D(0, 0, 0);
            _manModel3DGroup.Transform = _manTranslateTransform3D;

            readModel3D.Children.Add(_manModel3DGroup);


            var modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = readModel3D;

            viewport3D.Children.Add(modelVisual3D);


            var sphereVisual3D = new Ab3d.Visuals.SphereVisual3D()
            {
                CenterPosition = new Point3D(5, 3, -30),
                Radius = 3,
                Material = new DiffuseMaterial(Brushes.Gold)
            };

            viewport3D.Children.Add(sphereVisual3D);


            var pyramidVisual3D = new Ab3d.Visuals.PyramidVisual3D()
            {
                BottomCenterPosition = new Point3D(-5, 0, -30),
                Size = new Size3D(6, 6, 6),
                Material = new DiffuseMaterial(Brushes.Orange)
            };

            viewport3D.Children.Add(pyramidVisual3D);


            // Show how to use EventManager3D - set the CustomEventsSourceElement to _dxViewportView for the events to work
            var eventManager3D = new Ab3d.Utilities.EventManager3D(viewport3D);
            eventManager3D.CustomEventsSourceElement = _dxViewportView;

            var visualEventSource3D = new Ab3d.Utilities.VisualEventSource3D(sphereVisual3D);
            visualEventSource3D.MouseEnter += delegate(object sender, Common.EventManager3D.Mouse3DEventArgs e)
            {
                sphereVisual3D.Material = new DiffuseMaterial(Brushes.Red);
            };
            visualEventSource3D.MouseLeave += delegate(object sender, Common.EventManager3D.Mouse3DEventArgs e)
            {
                sphereVisual3D.Material = new DiffuseMaterial(Brushes.Gold);
            };

            eventManager3D.RegisterEventSource3D(visualEventSource3D);
        }
    }
}
