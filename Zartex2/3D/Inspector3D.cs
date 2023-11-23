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
                    Transform3D tnf;
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
                    if (actor.TypeId == 2)
                        tnf.Value.Scale(new Vector3D(0, 0, 1));

                    var actorColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)actor.Color.R, (byte)actor.Color.G, (byte)actor.Color.B));
                    var representation = new CubeVisual3D()
                    {
                        Transform = tnf,
                        Fill = actorColor,
                    };
                    sceneDevice.Children.Add(representation);
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
