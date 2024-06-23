using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Media.Media3D;

using HelixToolkit;
using HelixToolkit.Wpf;

// texture
using System.Windows.Media.Imaging;

using DSCript;

namespace Zartex._3D
{
    public partial class viewport : UserControl
    {
        public MissionCityType City { get; set; }

        public List<ActorDefinition> sceneActors = new List<ActorDefinition>();
        public List<MissionObject> sceneObjects = new List<MissionObject>();
        public List<MissionInstance> sceneInstances = new List<MissionInstance>();

        float boxesWidth = 0.5f;
        float boxesCharacterHeight = 1f;

        public Model3DGroup sceneContent = new Model3DGroup();
        public ModelVisual3D sceneDevice = new ModelVisual3D();
        public ModelVisual3D scene3DTextDevice = new ModelVisual3D();
        public MeshBuilder cubes = new MeshBuilder();

        public System.Windows.Media.SolidColorBrush selectedColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0,0));

        public float[] StartPosition = new float[2];
        float deadVector = 16000;

        double arrowHeadSize = 0.5;
        double arrowHeadLength = 0.9;
        double arrowDistanceMulti = 2;

        double pathLineThickness = 1.3;

        float textInfoHeight = 1.3f;

        float deg = (180 / (float)Math.PI);

        public static readonly string modelsFolderName = "Models";
        public static readonly string mapsFolderName = "Maps";
        public static double mapSize = 3750;

        public static Model3D characterModel = getModel($"{modelsFolderName}/character.3ds");
        public static Model3D vehicleModel = getModel($"{modelsFolderName}/vehicle.3ds");

        // forget it
        //public static Model3D mapBaseModel = getModel($"{mapsFolderName}/maps.3ds");

        public static SolidColorBrush Green = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
        public static SolidColorBrush Red = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
        public static SolidColorBrush White = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));

        public MissionObject GetActorMissionObject(ActorDefinition actor)
        {
            MissionObject missionObject = null;
            if (actor.ObjectId != -1 & actor.ObjectId < sceneObjects.Count)
                missionObject = sceneObjects[actor.ObjectId];

            return missionObject;
        }

        public Vector3D FindActorPosition(ActorDefinition actor)
        {
            if (actor is ActorDefinition)
            {
                // Driver: Parallel Lines format (no mission objects present)
                foreach (var prop in actor.Properties)
                {
                    if (prop is MatrixProperty)
                    {
                        MatrixProperty matrix = prop as MatrixProperty;
                        return new Vector3D(matrix.Value.X, matrix.Value.Z, matrix.Value.Y);
                    }
                }
                // Driv3r format
                MissionObject missionObject = null;
                if (actor.ObjectId != -1 & actor.ObjectId < sceneObjects.Count)
                    missionObject = sceneObjects[actor.ObjectId];
                // special for DPL format
                if (actor.TypeId == 10)
                {
                    foreach (var prop in actor.Properties)
                    {
                        if (prop is PathProperty)
                        {
                            var path = prop as PathProperty;
                            return new Vector3D(path.Path[0].X, path.Path[0].Z, path.Path[0].Y);
                        }
                    }
                }
                if (missionObject != null)
                {
                    switch (actor.TypeId)
                    {
                        // character
                        case 2:
                            var characterObject = (CharacterObject)missionObject;
                            return new Vector3D(characterObject.Position.X, characterObject.Position.Z, characterObject.Position.Y);
                        // vehicle
                        case 3:
                            var vehicleObject = (VehicleObject)missionObject;
                            return new Vector3D(vehicleObject.Position.X, vehicleObject.Position.Z, vehicleObject.Position.Y);
                        // test volume
                        case 4:
                            if (missionObject is VolumeObject)
                            {
                                var volumeObject = (VolumeObject)missionObject;
                                return new Vector3D(volumeObject.Position.X, volumeObject.Position.Y, volumeObject.Position.Z);
                            }
                            else if (missionObject is AreaObject)
                            {
                                var areaObject = (AreaObject)missionObject;
                                float x, y, z;
                                MemoryStream str = new MemoryStream(areaObject.CreationData);
                                using (var f = new BinaryReader(str, Encoding.UTF8))
                                {
                                    str.Position = 0x20;
                                    x = f.ReadSingle();
                                    y = f.ReadSingle();
                                    z = f.ReadSingle();
                                }
                                return new Vector3D(x, z, y);
                            }
                            break;
                        // path
                        case 6:
                            var pathObject = (PathObject)missionObject;
                            return new Vector3D(pathObject.Path[0].X, pathObject.Path[0].Z, pathObject.Path[0].Y);
                        // switch
                        case 101:
                            var switc = (SwitchObject)missionObject;
                            return new Vector3D(switc.Position.X, switc.Position.Z, switc.Position.Y);
                        // camera
                        case 9:
                            var cameraObject = (CameraObject)missionObject;
                            return new Vector3D(cameraObject.Position.X, cameraObject.Position.Z, cameraObject.Position.Y);
                        // objective icon
                        case 5:
                            var objectiveObject = (ObjectiveIconObject)missionObject;
                            return new Vector3D(objectiveObject.Position.X, objectiveObject.Position.Z, objectiveObject.Position.Y);
                        // etc etc, comments out!
                        case 103:
                            var collectiveObject = (CollectableObject)missionObject;
                            return new Vector3D(collectiveObject.Position.X, collectiveObject.Position.Z, collectiveObject.Position.Y);
                        case 104:
                            var animPropObject = (AnimPropObject)missionObject;
                            return new Vector3D(animPropObject.Position.X, animPropObject.Position.Z, animPropObject.Position.Y);
                        // marker
                        case 105:
                            var markerObject = (MarkerObject)missionObject;
                            return new Vector3D(markerObject.Position.X, markerObject.Position.Z, markerObject.Position.Y);
                    }
                }
            }
            return new Vector3D(deadVector, deadVector, -1);
        }
        public Vector3D FindActorForwardVector(ActorDefinition actor)
        {
            if (actor is ActorDefinition)
            {
                // Driver: Parallel Lines format (no mission objects present)
                foreach (var prop in actor.Properties)
                {
                    if (prop is MatrixProperty)
                    {
                        MatrixProperty matrix = prop as MatrixProperty;
                        // wait that's not, ah, I don't care, just if it works
                        return new Vector3D(matrix.Forward.X, matrix.Forward.Z, matrix.Forward.Y);
                    }
                }
                // Driv3r format
                MissionObject missionObject = null;
                if (actor.ObjectId != -1 & actor.ObjectId < sceneObjects.Count)
                    missionObject = sceneObjects[actor.ObjectId];
                if (missionObject != null)
                {
                    switch (actor.TypeId)
                    {
                        // character
                        case 2:
                            var characterObject = (CharacterObject)missionObject;
                            var off = 0x10;
                            if (characterObject.CreationData.Length >= (off + 12))
                            {
                                byte[] data = new byte[] {
                                characterObject.CreationData[off], characterObject.CreationData[off+1], characterObject.CreationData[off+2], characterObject.CreationData[off+3],
                                characterObject.CreationData[off+4], characterObject.CreationData[off+5], characterObject.CreationData[off+6], characterObject.CreationData[off+7],
                                characterObject.CreationData[off+8], characterObject.CreationData[off+9], characterObject.CreationData[off+10], characterObject.CreationData[off+11]
                                };
                                // XZY
                                return new Vector3D(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 8), BitConverter.ToSingle(data, 4));
                            }
                            break;
                        // vehicle
                        case 3:
                            var vehicleObject = (VehicleObject)missionObject;
                            var off2 = 0x40;
                            if (vehicleObject.CreationData.Length >= (off2 + 12))
                            {
                                byte[] data = new byte[] {
                                vehicleObject.CreationData[off2], vehicleObject.CreationData[off2+1], vehicleObject.CreationData[off2+2], vehicleObject.CreationData[off2+3],
                                vehicleObject.CreationData[off2+4], vehicleObject.CreationData[off2+5], vehicleObject.CreationData[off2+6], vehicleObject.CreationData[off2+7],
                                vehicleObject.CreationData[off2+8], vehicleObject.CreationData[off2+9], vehicleObject.CreationData[off2+10], vehicleObject.CreationData[off2+11]
                                };
                                // XZY
                                return new Vector3D(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 8), BitConverter.ToSingle(data, 4));
                            }
                            break;
                        // camera
                        case 9:
                            var cameraObject = (CameraObject)missionObject;
                            return new Vector3D(cameraObject.Position.X, cameraObject.Position.Z, cameraObject.Position.Y);
                    }
                }
            }
            return new Vector3D(1, 0, 0);
            //return new Vector3D(deadVector, deadVector, -1);
        }
        private bool Vector3DIsDead(Vector3D vector)
        {
            return (vector.X == deadVector & vector.Y == deadVector & vector.Z == -1);
        }
        public void UpdateScene(bool updateCamera = true)
        {
            var vp = this.viewport3D.Viewport3D;

            // clear up
            sceneDevice.Children.Clear();

            // set camera position to the place we're working on
            if (updateCamera)
               vp.Camera.Position = new Point3D(StartPosition[0]-8.5f,StartPosition[1]+8.5f,4);
            // add representation to un-viewable scene objects
            int objectId = 0;
            foreach (MissionObject mb in sceneObjects)
            {
                switch (mb.TypeId)
                {
                    case 8:
                        var propObject = mb as PropObject;

                        // at least exists or tried to
                        if (propObject.Id>-1 & propObject.Id< sceneInstances.Count)
                        {
                            MissionInstance inst = sceneInstances[propObject.Id];
                            Vector3D pos = new Vector3D(inst.Position.X, inst.Position.Z, inst.Position.Y);

                            Transform3D tnf = new TranslateTransform3D(pos);

                            vp.Children.Add(new BillboardTextVisual3D()
                            {
                                // {ExportedMissionObjects.GetObjectNameById(mb.TypeId)}
                                Text = $"({objectId}) Instance (Object)",
                                Position = new Point3D(pos.X, pos.Y, pos.Z + textInfoHeight),
                                Material = new SpecularMaterial(White, 5.0),
                                Foreground = White,
                            });
                            vp.Children.Add(new CubeVisual3D()
                            {
                                Transform = tnf,
                                Fill = White,
                            });
                        }
                        break;
                }
                objectId++;
            }
            int actorId = 0;
            foreach (ActorDefinition actor in sceneActors)
            {
                //MissionObject missionObject = null;
                //if (actor.ObjectId!=-1 & actor.ObjectId<sceneObjects.Count)
                //    missionObject = sceneObjects[actor.ObjectId];
                //if (missionObject != null)
                //{
                Vector3D pos = FindActorPosition(actor);
                MissionObject actorObject = GetActorMissionObject(actor);
                // make sure the position isn't null or smth (I made this to identify it because it can't just be null)
                actorId++;
                if (!Vector3DIsDead(pos))
                {
                    Vector3D fwd = FindActorForwardVector(actor);
                    float angle = ((float)Math.Atan2(fwd.Y, fwd.X)) * deg;
                    // calculates rotation
                    Transform3D rot = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0,0,1), angle));
                    // now the rotated transform with the position, etc.
                    Transform3D tnf = new MatrixTransform3D(new Matrix3D()
                    {
                        OffsetX = pos.X,
                        OffsetY = pos.Y,
                        OffsetZ = pos.Z,
                        // right
                        M11 = rot.Value.M11,
                        M12 = rot.Value.M12,
                        M13 = rot.Value.M13,
                        M14 = rot.Value.M14,
                        // forward
                        M21 = rot.Value.M21,
                        M22 = rot.Value.M22,
                        M23 = rot.Value.M23,
                        M24 = rot.Value.M24,
                        // up
                        M31 = rot.Value.M31,
                        M32 = rot.Value.M32,
                        M33 = rot.Value.M33,
                        M34 = rot.Value.M34,

                    }); //new TranslateTransform3D(pos);

                    var pureColor = System.Windows.Media.Color.FromArgb(255, (byte)actor.Color.R, (byte)actor.Color.G, (byte)actor.Color.B);
                    var actorColor = new System.Windows.Media.SolidColorBrush(pureColor);
                    if (actor.TypeId == 2)
                        tnf.Value.Scale(new Vector3D(0, 0, 1));
                    // base representation
                    vp.Children.Add(new BillboardTextVisual3D()
                    {
                        Text = $"({actorId-1}) {NodeTypes.GetActorType(actor.TypeId)}",
                        Position = new Point3D(pos.X, pos.Y, pos.Z + textInfoHeight),
                        Material = new SpecularMaterial(White,5.0),
                        Foreground = White
                    });
                    // test volume representation (if the object is a area)
                    /*
                    if (actor.TypeId == 4)
                    {
                        if (actorObject is AreaObject)
                        {
                            var areaObject = (AreaObject)actorObject;
                            float x, y, z;
                            float ax, ay, az;
                            MemoryStream str = new MemoryStream(areaObject.CreationData);
                            using (var f = new BinaryReader(str, Encoding.UTF8))
                            {
                                str.Position = 0x20;
                                x = f.ReadSingle();
                                y = f.ReadSingle();
                                z = f.ReadSingle();
                                str.Position = 0x30;
                                ax = f.ReadSingle();
                                ay = f.ReadSingle();
                                az = f.ReadSingle();
                            }
                            ScaleTransform3D scale = new ScaleTransform3D(new Vector3D(ax, az, ay), new Point3D(x, z, y));
                            sceneDevice.Children.Add(new CubeVisual3D() { Fill = actorColor, Transform = scale });
                            continue;
                        }
                    }
                    */
                    // objective icon representation
                    if (actor.TypeId == 5)
                    {
                        tnf = new TranslateTransform3D(pos + new Vector3D(0, 0, arrowDistanceMulti));
                        // adds an arrow representing the objective icon
                        sceneDevice.Children.Add(new ArrowVisual3D()
                        {
                            Diameter = arrowHeadSize,
                            HeadLength = arrowHeadLength,
                            Direction = new Vector3D(0,0, -arrowDistanceMulti),
                            Transform = tnf,
                            Fill = actorColor,
                        });
                        continue; // cancel the others representations
                    }
                    // path representation
                    if (actor.TypeId == 6)
                    {
                        var lines = new LinesVisual3D();
                        lines.Color = pureColor;
                        lines.Thickness = pathLineThickness;

                        MissionObject missionObject = null;
                        //Vector3D last = new Vector3D(0,0,0);
                        if (actor.ObjectId!=-1 & actor.ObjectId<sceneObjects.Count)
                            missionObject = sceneObjects[actor.ObjectId];
                        if (missionObject != null)
                        {
                            var pathObject = missionObject as PathObject;
                            int id = 0;
                            foreach (Vector4 pathvec in pathObject.Path)
                            {
                                lines.Points.Add(new Point3D(pathvec.X, pathvec.Z, pathvec.Y));
                                if (id<pathObject.Path.Length-1)
                                   lines.Points.Add(new Point3D(pathObject.Path[id+1].X, pathObject.Path[id + 1].Z, pathObject.Path[id + 1].Y));

                                id++;
                                //if (id == pathObject.Path.Length-1)
                                //     last = new Vector3D(pathvec.X, pathvec.Z, pathvec.Y);
                            }
                        }
                        // start point
                        vp.Children.Add(new SphereVisual3D()
                        {
                            Transform = tnf,
                            Fill = actorColor,
                            Radius = 0.2f
                        });                        
                        // lines connection
                        vp.Children.Add(lines);
                        continue; // cancel the others representations
                    }
                    // mission marker representation
                    if (actor.TypeId == 106)
                    {
                        vp.Children.Add(new SphereVisual3D()
                        {
                            Transform = tnf,
                            Fill = actorColor,
                            Radius = 1
                        });
                        continue; // cancel the others representations
                    }
                    // path representation (Driver: Parallel Lines)
                    // the code is very very similiar to the path representation of Driv3r
                    // but this time it gets the path property instead of the actual mission object, etc...
                    if (actor.TypeId == 10)
                    {
                        var lines = new LinesVisual3D();
                        lines.Color = pureColor;
                        lines.Thickness = pathLineThickness;

                        foreach(var prop in actor.Properties)
                        if (prop is PathProperty)
                        {
                            var path = prop as PathProperty;
                            int id = 0;
                            foreach (Vector4 pathvec in path.Path)
                            {
                                lines.Points.Add(new Point3D(pathvec.X, pathvec.Z, pathvec.Y));
                                if (id < path.Path.Length - 1)
                                    lines.Points.Add(new Point3D(path.Path[id + 1].X, path.Path[id + 1].Z, path.Path[id + 1].Y));

                                id++;
                                //if (id == pathObject.Path.Length-1)
                                //     last = new Vector3D(pathvec.X, pathvec.Z, pathvec.Y);
                            }
                            break; // breaks the loop as this is the property we want and no longer need to advance
                        }
                        // start point
                        vp.Children.Add(new SphereVisual3D()
                        {
                            Transform = tnf,
                            Fill = actorColor,
                            Radius = 0.2f
                        });
                        // lines connection
                        vp.Children.Add(lines);
                        continue; // cancel the others representations
                    }

                    // common representation
                    var representation1 = getRepresentationModelForActorType(actor.TypeId,pureColor); /*new CubeVisual3D()
                    {
                        Transform = tnf,
                        Fill = actorColor,
                    };*/
                    // forward representation
                    representation1.Transform = tnf;
                    // set color

                    sceneDevice.Children.Add(representation1);
                    // I don't think I need this anymore because there is models that you can know what they are facing
                    /*
                    var representation2 = new ArrowVisual3D()
                    {
                        Diameter = arrowHeadSize,
                        HeadLength = arrowHeadLength,
                        Direction = fwd * arrowDistanceMulti,
                        Transform = tnf,
                        Fill = actorColor,
                    };
                    if (!Vector3DIsDead(fwd))
                       sceneDevice.Children.Add(representation2); */
                }
                //}

            }

            // add the map
            ModelVisual3D mapmodel = new ModelVisual3D();

            MeshBuilder meshb = new MeshBuilder();
            meshb.AddTriangle(
                new Point3D(-mapSize, mapSize, 0),
                new Point3D(-mapSize, -mapSize, 0),
                new Point3D(mapSize, -mapSize, 0),
                // uv
                new System.Windows.Point(0, 1),
                new System.Windows.Point(0, 0),
                new System.Windows.Point(1, 0)
                );
            meshb.AddTriangle(
                new Point3D(mapSize, -mapSize, 0),
                new Point3D(mapSize, mapSize, 0),
                new Point3D(-mapSize, mapSize, 0),
                // uv
                new System.Windows.Point(1, 0),
                new System.Windows.Point(1, 1),
                new System.Windows.Point(0, 1)
                );


            DiffuseMaterial defaultmaterial = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255)));
            Material material = null;
            string mapTexName = "layout.png";

            switch(City)
            {
                case MissionCityType.Miami_Day:
                case MissionCityType.Miami_Night:
                    mapTexName = "miami.png";
                    break;
                case MissionCityType.Nice_Day:
                case MissionCityType.Nice_Night:
                    mapTexName = "nice.png";
                    break;
                case MissionCityType.Istanbul_Day:
                case MissionCityType.Istanbul_Night:
                    mapTexName = "istanbul.png";
                    break;
            }

            if (File.Exists($"{mapsFolderName}/{mapTexName}"))
            {
                FileStream buffer = new FileStream($"{mapsFolderName}/{mapTexName}", FileMode.Open, FileAccess.Read);

                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.StreamSource = buffer;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                ImageBrush tex = new ImageBrush(bitmap);
                material = new DiffuseMaterial(tex);
            }

            if (material == null)
                material = defaultmaterial;

            GeometryModel3D map = new GeometryModel3D(meshb.ToMesh(true), material);

            mapmodel.Content = map;

            vp.Children.Add(mapmodel);

            // stuff the scene to the viewport
            /*
            if (mapBaseModel != null)
            {
                vp.Children.Add(getMapModel());
            }
            else
            {
                Console.WriteLine("WARNING: The map base model wasn't present (WTF?)");
                Console.WriteLine("WARNING: Attempting to reload the map base model...");
                mapBaseModel = getModel($"{mapsFolderName}/map.3ds");
            }
            */
            vp.Children.Add(sceneDevice);
            Debug.WriteLine($"[ZARTEX] 3D Scene done updating (Children Count: {sceneDevice.Children.Count})");
        }
        public viewport()
        {
            InitializeComponent();
            Inspector.Nodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(NodesOnClick);

            //ModelVisual3D device = new ModelVisual3D();
            //device.Content = getModel("cube.3ds");
            //viewport3D.Viewport3D.Children.Add(device);
        }

        public static Model3D getModel(string path)
        {
            Model3D device = null;
            try
            {
                ModelImporter import = new ModelImporter();
                device = import.Load(path);
                Console.WriteLine($"Model \"{path}\" has been loaded");
            }
            catch (Exception e)
            { Console.WriteLine($"ERROR: Model \"{path}\" failed to load!"); }
            return device;
        }

        public static Model3DGroup getModelAsGroup(string path)
        {
            Model3DGroup device = null;
            try
            {
                ModelImporter import = new ModelImporter();
                device = import.Load(path);
            }
            catch (Exception e)
            { }
            return device;
        }

        public ModelVisual3D getRepresentationModelForActorType(int type,System.Windows.Media.Color color)
        {
            ModelVisual3D device = new ModelVisual3D();
            Model3D model = null;
            switch (type)
            {
                case 2:
                    model = characterModel;
                    break;
                case 3:
                    model = vehicleModel;
                    break;
            }
            if (model == null) { return new CubeVisual3D() { Fill = new SolidColorBrush(color) }; }

            GeometryModel3D md = model as GeometryModel3D;

            if (md != null)
            {
                DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(color));
                md.Material = material;
                md.BackMaterial = material;
            }

            device.Content = model;
            return device;
        }
        
        /*
        public ModelVisual3D getMapModel()
        {
            ModelVisual3D device = new ModelVisual3D();
            Model3D model = mapBaseModel;
            string imageName = "layout.png"; // default

            FileStream buffer = new FileStream($"{mapsFolderName}/{imageName}", FileMode.Open, FileAccess.Read);

            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.StreamSource = buffer;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            ImageBrush tex = new ImageBrush(bitmap);

            //GeometryModel3D geo = model as GeometryModel3D;
            //geo.Material = new DiffuseMaterial(tex);
            //geo.BackMaterial = new DiffuseMaterial(tex);

            model.Transform = new TranslateTransform3D(0, 0, 0);

            device.Content = model;
            return device;
        }
        */

        // clicked on a node then put the camera on it
        public void NodesOnClick(object sender, TreeViewEventArgs e)
        {
            var vp = this.viewport3D.Viewport3D;

            var tag = e.Node.Tag;
            ActorDefinition def = null;

            if (tag is ActorDefinition)
            {
                def = tag as ActorDefinition;
            }
            if (tag is ActorProperty)
            {
                var val = (tag as ActorProperty).Value;
                if (val>=0&val<sceneActors.Count)
                   def = sceneActors[(tag as ActorProperty).Value];
            }
            if (def != null)
            {
                Vector3D pos = FindActorPosition(tag as ActorDefinition);
                // make sure the position isn't null or smth (I made this to identify it because it can't just be null)
                if (!Vector3DIsDead(pos))
                {
                    // set the camera's position to the selected actor
                    vp.Camera.LookDirection = new Vector3D(8.5f, -8.5f, -4);
                    vp.Camera.UpDirection = new Vector3D(0, 0, 1);
                    vp.Camera.Position = new Point3D(pos.X - 8.5f, pos.Y + 8.5f, pos.Z + 4);
                }
            }
        }
    }
}
