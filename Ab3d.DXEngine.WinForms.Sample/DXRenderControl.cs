using System;
using System.Windows.Controls;
using System.Windows.Forms;
using Ab3d.Cameras;
using Ab3d.Controls;
using Ab3d.DirectX;
using Ab3d.DirectX.Controls;
using SharpDX.Direct3D;

namespace Ab3d.DXEngine.WinForms.Sample
{
    public class DXRenderControl : SharpDX.Windows.RenderControl
    {
        private DXDevice _dxDevice;
        private DXScene _dxScene;

        public Viewport3D MainViewport3D { get; private set; }
        public DXViewportView DXViewportView { get; private set; }

        public DXRenderControl()
        {
            InitializeDxEngine();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            RenderScene();
        }

        public void RenderScene()
        {
            if (_dxScene == null)
                return;

            if (this.ClientSize.Width != _dxScene.Width || this.ClientSize.Height != _dxScene.Height)
            {
                // Resize buffers
                _dxScene.Resize(this.ClientSize.Width, this.ClientSize.Height);
            }

            // Render the scene with DXEngine.
            // We force rendering event if there are no known changes on the scene (no _dxScene.NotifyChange calls).
            // To render only when there are any know changes, set forceRenderAll to false
            _dxScene.RenderScene(forceRenderAll: true);
        }

        private void InitializeDxEngine()
        {
            if (this.Handle == IntPtr.Zero)
                throw new Exception("InitializeDxEngine called without initialized hWnd");

            // Create DXDevice - DXEngine's wrapper for DirectX 11 device
            var dxDeviceConfiguration = new DXDeviceConfiguration();
            dxDeviceConfiguration.DriverType = DriverType.Hardware; // Use DriverType.Warp to use software rendering

            _dxDevice = new DXDevice(dxDeviceConfiguration);


            // Now create DXScene object from the RenderForm Handle
            _dxScene = _dxDevice.CreateDXSceneWithSwapChain(this.Handle, this.ClientSize.Width, this.ClientSize.Height, preferedMultisampleCount: 4);
            _dxScene.BackgroundColor = System.Windows.Media.Colors.LightBlue.ToColor4();
            _dxScene.ShaderQuality = ShaderQuality.Normal;

            // _mainViewport3D will define the 3D scene
            MainViewport3D = new Viewport3D();

            // After we have DXScene and Viewport3D objects, we can create a DXViewportView.
            // It is used to render 3D scene defined by Viewport3D.
            DXViewportView = new DXViewportView(_dxScene, MainViewport3D);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Dispose created resources:
            if (DXViewportView != null)
            {
                DXViewportView.Dispose();
                DXViewportView = null;
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