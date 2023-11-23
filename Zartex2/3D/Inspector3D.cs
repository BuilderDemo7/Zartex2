using System;
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

using DSCript;

namespace Zartex._3D
{
    public partial class viewport : UserControl
    {
        public List<ActorDefinition> sceneActors = new List<ActorDefinition>();
        public List<MissionObject> sceneObjects = new List<MissionObject>();

        float boxesWidth = 0.5f;
        float boxesCharacterHeight = 1f;

        public Model3DGroup sceneContent = new Model3DGroup();
        public ModelVisual3D sceneDevice = new ModelVisual3D();
        public MeshBuilder cubes = new MeshBuilder();

        public System.Windows.Media.SolidColorBrush selectedColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0,0));

        public float[] StartPosition = new float[2];
        float deadVector = 16000;

        double arrowHeadSize = 0.5;
        double arrowHeadLength = 0.9;
        double arrowDistanceMulti = 2;

        double pathLineThickness = 1.3;

        float textInfoHeight = 1.1f;

        public static SolidColorBrush Green = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
        public static SolidColorBrush Red = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
        public static SolidColorBrush White = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));

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
                                // TODO:
                                // Add AreaObject representation
                            }
                            break;
                        // path
                        case 6:
                            var pathObject = (PathObject)missionObject;
                            return new Vector3D(pathObject.Path[0].X, pathObject.Path[0].Z, pathObject.Path[0].Y);
                        // camera
                        case 9:
                            var cameraObject = (CameraObject)missionObject;
                            return new Vector3D(cameraObject.V3.Y, cameraObject.V3.W, cameraObject.V3.Z);
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
                            return new Vector3D(cameraObject.V1.X, cameraObject.V1.Z, cameraObject.V1.Y);
                    }
                }
            }
            return new Vector3D(deadVector, deadVector, -1);
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
            int actorId = 0;
            foreach (ActorDefinition actor in sceneActors)
            {
                //MissionObject missionObject = null;
                //if (actor.ObjectId!=-1 & actor.ObjectId<sceneObjects.Count)
                //    missionObject = sceneObjects[actor.ObjectId];
                //if (missionObject != null)
                //{
                Vector3D pos = FindActorPosition(actor);
                // make sure the position isn't null or smth (I made this to identify it because it can't just be null)
                if (!Vector3DIsDead(pos))
                {
                    Transform3D tnf = new TranslateTransform3D(pos);
                    Vector3D fwd = FindActorForwardVector(actor);

                    var pureColor = System.Windows.Media.Color.FromArgb(255, (byte)actor.Color.R, (byte)actor.Color.G, (byte)actor.Color.B);
                    var actorColor = new System.Windows.Media.SolidColorBrush(pureColor);
                    if (actor.TypeId == 2)
                        tnf.Value.Scale(new Vector3D(0, 0, 1));
                    // base representation
                    vp.Children.Add(new BillboardTextVisual3D()
                    {
                        Text = $"({actorId}) {NodeTypes.GetActorType(actor.TypeId)}",
                        Position = new Point3D(pos.X, pos.Y, pos.Z + textInfoHeight),
                        Material = new SpecularMaterial(White,5.0),
                        Foreground = White
                    });
                    // we're no longer using actorId now and the "continue"s can not progress it so...
                    actorId++;
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
                            Radius = 2
                        });
                        continue; // cancel the others representations
                    }

                    // common representation
                    var representation1 = new CubeVisual3D()
                    {
                        Transform = tnf,
                        Fill = actorColor,
                    };
                    // forward representation
                    var representation2 = new ArrowVisual3D()
                    {
                        Diameter = arrowHeadSize,
                        HeadLength = arrowHeadLength,
                        Direction = fwd * arrowDistanceMulti,
                        Transform = tnf,
                        Fill = actorColor,
                    };
                    sceneDevice.Children.Add(representation1);
                    if (!Vector3DIsDead(fwd))
                       sceneDevice.Children.Add(representation2);
                }
                //}

            }

            // stuff the scene to the viewport
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

        public Model3D getModel(string path)
        {
            Model3D device = null;
            try
            {
                ModelImporter import = new ModelImporter();
                device = import.Load(path);
            }
            catch (Exception e)
            { }
            return device;
        }
        
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
