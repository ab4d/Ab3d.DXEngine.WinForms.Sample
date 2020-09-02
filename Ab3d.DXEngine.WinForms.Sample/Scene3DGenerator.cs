using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Ab3d.DirectX;
using Ab3d.DirectX.Models;
using SharpDX;

namespace Ab3d.DXEngine.WinForms.Sample
{
    public static class Scene3DGenerator
    {
        // This method creates a sample scene with using WPF 3D objects and objects from Ab3d.PowerToys library.
        public static void CreateSampleScene(Viewport3D mainViewport3D)
        {
            // When using DXViewportView, we can use the same code as it is used for the Ab3d.PowerToys samples and main Ab3d.DXEngine samples.
            // The code from XAML can be also easily converted into object creation in code.

            var readerObj = new Ab3d.ReaderObj();
            var readModel3D = readerObj.ReadModel3D(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\house with trees.obj"));

            //readModel3D.Transform = new ScaleTransform3D(10, 10, 10);

            // Instead of specifying a transformation we can also use CenterAndScaleModel3D method to position and scale the model to our needs.
            Ab3d.Utilities.ModelUtils.CenterAndScaleModel3D(readModel3D,
                                                            centerPosition: new Point3D(0, 0, 0),
                                                            finalSize: new Size3D(1000, 500, 1000),
                                                            preserveAspectRatio: true);

            // Before the model can be added to Viewport3D, it needs to be embedded into the ModelVisual3D
            var modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = readModel3D;

            mainViewport3D.Children.Add(modelVisual3D);


            // Now add a sample box, sphere and pyramid
            var boxVisual3D = new Ab3d.Visuals.BoxVisual3D()
            {
                CenterPosition = new Point3D(0, 25, -300),
                Size = new Size3D(50, 50, 50),
                Material = new DiffuseMaterial(System.Windows.Media.Brushes.Blue)
            };

            mainViewport3D.Children.Add(boxVisual3D);


            var sphereVisual3D = new Ab3d.Visuals.SphereVisual3D()
            {
                CenterPosition = new Point3D(0, 100, -300),
                Radius = 30,
                Material = new DiffuseMaterial(System.Windows.Media.Brushes.Orange)
            };

            mainViewport3D.Children.Add(sphereVisual3D);


            var pyramidVisual3D = new Ab3d.Visuals.PyramidVisual3D()
            {
                BottomCenterPosition = new Point3D(0, 150, -300),
                Size = new Size3D(50, 50, 50),
                Material = new DiffuseMaterial(System.Windows.Media.Brushes.Silver)
            };

            mainViewport3D.Children.Add(pyramidVisual3D);
        }

        public static void CreateSampleScene(SceneNode dxSceneRootNode, out DisposeList disposables)
        {
            // When we do not use WPF and DXViewportView,
            // we cannot use high level objects from WPF and Ab3d.PowerToys library that 
            // can be used to define Materials and 3D objects.
            //
            // In this case we need to create DXEngine's materials and SceneNodes.

            // 1) First create materials.
            //
            // We have the following options:
            // - create Ab3d.DirectX.Materials.StandardMaterial - diffuse, specular and emissive color
            // - create Ab3d.DirectX.Materials.LineMaterial - to render 3D lines
            // - create Ab3d.DirectX.Materials.EffectMaterial - to create a material that uses custom rendering Effect
            // - create WPF material and then use it to create Ab3d.DirectX.Materials.WpfMaterial


            // IMPORTANT:
            // Before the Form is closed, we need to dispose all the DXEngine objects that we created (all that implement IDisposable).
            // This means that all materials, Effects, Mesh objects and SceneNodes need to be disposed.
            // To make this easier, we can use the DisposeList collection that will hold IDisposable objects.
            disposables = new DisposeList();


            // Create StandardMaterial
            var blueSpecularMaterial = new Ab3d.DirectX.Materials.StandardMaterial()
            {
                Alpha = 1.0f,
                DiffuseColor = new SharpDX.Color3(0.0f, 0.0f, 1.0f), // Blue
                HasTransparency = false,

                SpecularColor = SharpDX.Color3.White,
                SpecularPower = 32,
                //EmissiveColor = new SharpDX.Color3(0.2f, 0.2f, 0.2f)
            };

            disposables.Add(blueSpecularMaterial);


            // Create LineMaterial
            var lineMaterial = new Ab3d.DirectX.Materials.LineMaterial()
            {
                LineColor = new SharpDX.Color4(red: 1.0f, green: 0.0f, blue: 0.0f, alpha: 1.0f), // Red,
                LineThickness = 2
            };

            disposables.Add(lineMaterial);


            // First create a WPF material and than convert it into DXEngine's WpfMaterial
            var materialGroup = new System.Windows.Media.Media3D.MaterialGroup();
            materialGroup.Children.Add(new System.Windows.Media.Media3D.DiffuseMaterial(System.Windows.Media.Brushes.Yellow));
            materialGroup.Children.Add(new System.Windows.Media.Media3D.SpecularMaterial(System.Windows.Media.Brushes.White, 16));

            var yellowSpecularMaterial = new Ab3d.DirectX.Materials.WpfMaterial(materialGroup);

            disposables.Add(yellowSpecularMaterial);


            // Some other samples of using WPF materials (not used here):

            //// Material with texture
            //var textureFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\sampleTexture.png");
            //var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(textureFileName, UriKind.Absolute));

            //var imageBrush = new System.Windows.Media.ImageBrush(bitmap);

            //var textureWpfMaterial = new System.Windows.Media.Media3D.DiffuseMaterial(imageBrush);

            //var textureMaterial = new Ab3d.DirectX.Materials.WpfMaterial(textureWpfMaterial);


            //// Material from visual text
            //var textBlock = new System.Windows.Controls.TextBlock();
            //textBlock.Text = "12345";
            //textBlock.FontSize = 30;
            //textBlock.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            //textBlock.Arrange(new System.Windows.Rect(0, 0, textBlock.DesiredSize.Width, textBlock.DesiredSize.Height));

            //var visualBrush = new System.Windows.Media.VisualBrush();
            //visualBrush.Visual = textBlock;
            //visualBrush.Stretch = System.Windows.Media.Stretch.Fill;

            //var visualBrushWpfMaterial = new System.Windows.Media.Media3D.DiffuseMaterial(visualBrush);

            //var visualBrushMaterial = new Ab3d.DirectX.Materials.WpfMaterial(visualBrushWpfMaterial);


            //// Create a linear gradient brush with five stops 
            //var linearGradientBrush = new System.Windows.Media.LinearGradientBrush();
            //linearGradientBrush.StartPoint = new System.Windows.Point(0, 0);
            //linearGradientBrush.EndPoint = new System.Windows.Point(1, 1);

            //var blueGS = new System.Windows.Media.GradientStop();
            //blueGS.Color = System.Windows.Media.Colors.Blue;
            //blueGS.Offset = 0.0;
            //linearGradientBrush.GradientStops.Add(blueGS);

            //var orangeGS = new System.Windows.Media.GradientStop();
            //orangeGS.Color = System.Windows.Media.Colors.Orange;
            //orangeGS.Offset = 0.5;
            //linearGradientBrush.GradientStops.Add(orangeGS);

            //var yellowGS = new System.Windows.Media.GradientStop();
            //yellowGS.Color = System.Windows.Media.Colors.Yellow;
            //yellowGS.Offset = 1;
            //linearGradientBrush.GradientStops.Add(yellowGS);

            //var gradientWpfMaterial = new System.Windows.Media.Media3D.DiffuseMaterial(linearGradientBrush);

            //var gradientBrushMaterial = new Ab3d.DirectX.Materials.WpfMaterial(gradientWpfMaterial);



            // 2) Now create SceneNodes that will represent 3D objects.
            //
            // We have the following options:
            // - create Ab3d.DirectX.MeshObjectNode from Ab3d.DirectX.GeometryMesh - can be used when you have positions, normals, textureCoordinates and triangleIndices arrays
            // - create Ab3d.DirectX.SimpleMesh from vertex and index buffer
            // - create SceneNode from WPF objects: GeometryModel3D, Model3DGroup or ModelVisual3D


            Vector3[] positions;
            Vector3[] normals;
            Vector2[] textureCoordinates;
            int[] triangleIndices;

            // Get Pyramid mesh data
            GetPyramidDataArrays(out positions, out normals, out textureCoordinates, out triangleIndices);

            // Create SceneNode
            // First create GeometryMesh object from the mesh arrays
            var geometryMesh = new Ab3d.DirectX.GeometryMesh(positions, normals, textureCoordinates, triangleIndices, "PyramidMesh3D");

            disposables.Add(geometryMesh); // geometryMesh is IDisposable - it creates DirectX vertex and index buffer than need to be disposed

            // Use GeometryMesh to create MeshObjectNode (SceneNode from GeometryMesh object)
            var meshObjectNode = new Ab3d.DirectX.MeshObjectNode(geometryMesh, blueSpecularMaterial);
            meshObjectNode.Name = "MeshObjectNode-from-GeometryMesh";

            Matrix translateMatrix = Matrix.Translation(-150, 0, -300);
            meshObjectNode.Transform = new Transformation(translateMatrix);

            dxSceneRootNode.AddChild(meshObjectNode);

            disposables.Add(meshObjectNode);


            // Create MeshObjectNode from DXMeshGeometry3D that is created from SphereMesh3D
            var sphereMesh3D = new Ab3d.Meshes.SphereMesh3D(centerPosition: new System.Windows.Media.Media3D.Point3D(-50, 30, -300),
                                                            radius: 30, 
                                                            segments: 5);

            var dxMeshGeometry = new DXMeshGeometry3D(sphereMesh3D.Geometry, "SphereMesh");

            disposables.Add(dxMeshGeometry); // dxMeshGeometry is IDisposable - it creates DirectX vertex and index buffer than need to be disposed; Note: sphereMesh3D is a WPF object and does not need to be disposed (it also does not implement IDisposable so it cannot be added to the disposables list)

            meshObjectNode = new Ab3d.DirectX.MeshObjectNode(dxMeshGeometry, lineMaterial);
            meshObjectNode.Name = "MeshObjectNode-from-SphereMesh";

            dxSceneRootNode.AddChild(meshObjectNode);

            disposables.Add(meshObjectNode);


            PositionNormalTexture[] vertexBuffer;
            int[] indexBuffer;
            GetVertexAndIndexBuffer(out vertexBuffer, out indexBuffer);

            var simpleMesh = new SimpleMesh<PositionNormalTexture>(vertexBuffer,
                                                                   indexBuffer,
                                                                   inputLayoutType: InputLayoutType.Position | InputLayoutType.Normal | InputLayoutType.TextureCoordinate,
                                                                   name: "SimpleMesh-from-PositionNormalTexture-array");

            disposables.Add(simpleMesh); // simpleMesh is also IDisposable - it creates DirectX vertex and index buffer than need to be disposed

            meshObjectNode = new Ab3d.DirectX.MeshObjectNode(simpleMesh, yellowSpecularMaterial);
            meshObjectNode.Name = "MeshObjectNode-from-SimpleMesh";

            translateMatrix = Matrix.Translation(50, 0, -300);
            meshObjectNode.Transform = new Transformation(translateMatrix);

            dxSceneRootNode.AddChild(meshObjectNode);
            
            disposables.Add(meshObjectNode);


            // The easiest way to create simple 3D objects is to create
            // WPF Model3D objects with help from objects from Ab3d.PowerToys library.
            // Then create WpfGeometryModel3DNode from the created ModelVisual3D
            var sphereModel3D = Ab3d.Models.Model3DFactory.CreateSphere(centerPosition: new System.Windows.Media.Media3D.Point3D(150, 30, -300),
                                                                        radius: 30, 
                                                                        segments: 30, 
                                                                        material: new System.Windows.Media.Media3D.DiffuseMaterial(System.Windows.Media.Brushes.Orange));

            var wpfGeometryModel3DNode = new Ab3d.DirectX.Models.WpfGeometryModel3DNode(sphereModel3D);

            dxSceneRootNode.AddChild(wpfGeometryModel3DNode);

            disposables.Add(wpfGeometryModel3DNode);

            // We could also create ModelVisual3D instead.
            // A disadvantage over Model3D is that this would create more one mode child SceneNode.
            // The following commented code shows how to do that:
            //var sphereVisual3D = new Ab3d.Visuals.SphereVisual3D()
            //{
            //    CenterPosition = new System.Windows.Media.Media3D.Point3D(-30, 0, 0),
            //    Radius = 30,
            //    Material = new System.Windows.Media.Media3D.DiffuseMaterial(System.Windows.Media.Brushes.Orange)
            //};

            //new Ab3d.DirectX.Models.WpfModelVisual3DNode(sphereVisual3D);



            // We could also load obj file with using ReaderObj from Ab3d.PowerToys library.
            // With Ab3d.Reader3ds it is possible to load 3ds file.
            // See Assimp samples that come with Ab3d.PowerToys library to see how to load many other file formats with third party assimp library.
            var sceneNodeFromObjFile = CreateSceneNodeFromObjFile(@"Resources\house with trees.obj");

            if (sceneNodeFromObjFile != null)
            {
                translateMatrix = Matrix.Scaling(10);
                sceneNodeFromObjFile.Transform = new Transformation(translateMatrix);

                dxSceneRootNode.AddChild(sceneNodeFromObjFile);

                disposables.Add(sceneNodeFromObjFile);
            }
        }


        public static SceneNode CreateSceneNodeFromObjFile(string objFileName)
        {
            if (!Path.IsPathRooted(objFileName))
                objFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, objFileName);

            var readerObj = new Ab3d.ReaderObj();
            System.Windows.Media.Media3D.Model3D readModel = readerObj.ReadModel3D(objFileName);

            SceneNode createdSceneNode;

            if (readModel is System.Windows.Media.Media3D.Model3DGroup)
                createdSceneNode = new WpfModel3DGroupNode((System.Windows.Media.Media3D.Model3DGroup)readModel);
            else if (readModel is System.Windows.Media.Media3D.GeometryModel3D)
                createdSceneNode = new WpfGeometryModel3DNode((System.Windows.Media.Media3D.GeometryModel3D)readModel);
            else
                createdSceneNode = null;

            return createdSceneNode;
        }

        private static void GetPyramidDataArrays(out Vector3[] positions, out Vector3[] normals, out Vector2[] textureCoordinates, out int[] triangleIndices)
        {
            // NOTE:
            // To get more information how the data below were prepared see the 
            // Customizations/ManuallyCreatedSceneNodes.xaml.cs file in the Ab3d.DXEngine.Wpf.Samples project

            // Pyramid mesh data
            positions = new Vector3[]
            {
                new Vector3(0f, 50f, 0f),
                new Vector3(0f, 50f, 0f),
                new Vector3(0f, 50f, 0f),
                new Vector3(0f, 50f, 0f),
                new Vector3(-40f, 0f, -40f),
                new Vector3(-40f, 0f, -40f),
                new Vector3(-40f, 0f, -40f),
                new Vector3(40f, 0f, -40f),
                new Vector3(40f, 0f, -40f),
                new Vector3(40f, 0f, -40f),
                new Vector3(40f, 0f, 40f),
                new Vector3(40f, 0f, 40f),
                new Vector3(40f, 0f, 40f),
                new Vector3(-40f, 0f, 40f),
                new Vector3(-40f, 0f, 40f),
                new Vector3(-40f, 0f, 40f),
            };

            normals = new Vector3[]
            {
                new Vector3(0f, 0.624695047554424f, -0.78086880944303f),
                new Vector3(0.78086880944303f, 0.624695047554424f, 0f),
                new Vector3(0f, 0.624695047554424f, 0.78086880944303f),
                new Vector3(-0.78086880944303f, 0.624695047554424f, 0f),
                new Vector3(0f, 0.624695047554424f, -0.78086880944303f),
                new Vector3(-0.78086880944303f, 0.624695047554424f, 0f),
                new Vector3(0f, -1f, 0f),
                new Vector3(0f, 0.624695047554424f, -0.78086880944303f),
                new Vector3(0.78086880944303f, 0.624695047554424f, 0f),
                new Vector3(0f, -1f, 0f),
                new Vector3(0.78086880944303f, 0.624695047554424f, 0f),
                new Vector3(0f, 0.624695047554424f, 0.78086880944303f),
                new Vector3(0f, -1f, 0f),
                new Vector3(0f, 0.624695047554424f, 0.78086880944303f),
                new Vector3(-0.78086880944303f, 0.624695047554424f, 0f),
                new Vector3(0f, -1f, 0f),
            };

            textureCoordinates = new Vector2[]
            {
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
            };

            triangleIndices = new int[]
            {
                0, 7, 4,
                1, 10, 8,
                2, 13, 11,
                3, 5, 14,
                6, 9, 15,
                9, 12, 15,
            };
        }

        private static void GetVertexAndIndexBuffer(out PositionNormalTexture[] vertexBuffer, out int[] indexBuffer)
        {
            vertexBuffer = new PositionNormalTexture[] {
                /*                        Position                     Normal                                   TextureCoordinate */
                new PositionNormalTexture(new Vector3(0f, 50f, 0f),    new Vector3(0.0000f, 0.6247f, -0.7809f), new Vector2(0.5f, 0.5f)),
                new PositionNormalTexture(new Vector3(0f, 50f, 0f),    new Vector3(0.7809f, 0.6247f, 0.0000f),  new Vector2(0.5f, 0.5f)),
                new PositionNormalTexture(new Vector3(0f, 50f, 0f),    new Vector3(0.0000f, 0.6247f, 0.7809f),  new Vector2(0.5f, 0.5f)),
                new PositionNormalTexture(new Vector3(0f, 50f, 0f),    new Vector3(-0.7809f, 0.6247f, 0.0000f), new Vector2(0.5f, 0.5f)),
                new PositionNormalTexture(new Vector3(-40f, 0f, -40f), new Vector3(0.0000f, 0.6247f, -0.7809f), new Vector2(0f, 0f)),
                new PositionNormalTexture(new Vector3(-40f, 0f, -40f), new Vector3(-0.7809f, 0.6247f, 0.0000f), new Vector2(0f, 0f)),
                new PositionNormalTexture(new Vector3(-40f, 0f, -40f), new Vector3(0.0000f, -1.0000f, 0.0000f), new Vector2(0f, 0f)),
                new PositionNormalTexture(new Vector3(40f, 0f, -40f),  new Vector3(0.0000f, 0.6247f, -0.7809f), new Vector2(1f, 0f)),
                new PositionNormalTexture(new Vector3(40f, 0f, -40f),  new Vector3(0.7809f, 0.6247f, 0.0000f),  new Vector2(1f, 0f)),
                new PositionNormalTexture(new Vector3(40f, 0f, -40f),  new Vector3(0.0000f, -1.0000f, 0.0000f), new Vector2(1f, 0f)),
                new PositionNormalTexture(new Vector3(40f, 0f, 40f),   new Vector3(0.7809f, 0.6247f, 0.0000f),  new Vector2(1f, 1f)),
                new PositionNormalTexture(new Vector3(40f, 0f, 40f),   new Vector3(0.0000f, 0.6247f, 0.7809f),  new Vector2(1f, 1f)),
                new PositionNormalTexture(new Vector3(40f, 0f, 40f),   new Vector3(0.0000f, -1.0000f, 0.0000f), new Vector2(1f, 1f)),
                new PositionNormalTexture(new Vector3(-40f, 0f, 40f),  new Vector3(0.0000f, 0.6247f, 0.7809f),  new Vector2(0f, 1f)),
                new PositionNormalTexture(new Vector3(-40f, 0f, 40f),  new Vector3(-0.7809f, 0.6247f, 0.0000f), new Vector2(0f, 1f)),
                new PositionNormalTexture(new Vector3(-40f, 0f, 40f),  new Vector3(0.0000f, -1.0000f, 0.0000f), new Vector2(0f, 1f)),
            };

            indexBuffer = new int[]
            {
                0, 7, 4,
                1, 10, 8,
                2, 13, 11,
                3, 5, 14,
                6, 9, 15,
                9, 12, 15,
            };
        }
    }
}