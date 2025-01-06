using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSCript;
using DSCript.Spooling;
using DSCript.Models;

namespace Zartex
{
    // PC platform only
    public class VehicleVariation : FileChunker
    {
        public List<ModelPackage> VehicleModels { get; protected set; }
        public ModelPackage Models { get; set; }
        public List<SpoolableBuffer> VehicleHierachies { get; protected set; }

        private SpoolablePackage unidentifiedPackage = new SpoolablePackage();

        public void Add(ModelPackage vehicleModel, SpoolableBuffer vehicleHierachyBuffer)
        {
            VehicleModels.Add(vehicleModel);
            VehicleHierachies.Add(vehicleHierachyBuffer);

            vehicleHierachyBuffer.Version = 6;
        }
        public void Add(SpoolableBuffer vehicleHierachyBuffer)
        {
            VehicleHierachies.Add(vehicleHierachyBuffer);
            vehicleHierachyBuffer.Version = 6;
        }
        public void Add(ModelPackage vehicleModel)
        {
            VehicleModels.Add(vehicleModel);
        }

        public void LoadModelsFromVVVFile(string filepath)
        {
            if (!File.Exists(filepath))
                return;

            //ModelFile mf = new ModelFile(filepath);
            //Models = mf.Packages[0];
            FileChunker fc = new FileChunker(filepath);
            SpoolablePackage upck = fc.Content.GetFirstChild<SpoolablePackage>(ChunkType.UnifiedPackage);
            SpoolableBuffer mdpc = upck.GetFirstChild<SpoolableBuffer>(ChunkType.ModelPackagePC);
            Models = mdpc.AsResource<ModelPackage>(true);
            fc.Dispose();
        }

        public void LoadHierachiesFromVVVFile(string filepath)
        {
            if (!File.Exists(filepath))
                return;

            FileChunker fc = new FileChunker(filepath);
            SpoolablePackage upck = fc.Content.GetFirstChild<SpoolablePackage>(ChunkType.UnifiedPackage);
            foreach(Spooler sp in upck.Children)
            {
                if (sp.Context == (int)ChunkType.VehicleHierarchy)
                {
                    SpoolableBuffer spoolableBuffer = new SpoolableBuffer()
                    {
                        Description = "Vehicle Hierachy",
                        Context = (int)ChunkType.VehicleHierarchy,
                        Alignment = sp.Alignment
                    };
                    spoolableBuffer.SetBuffer((sp as SpoolableBuffer).GetBuffer());
                    VehicleHierachies.Add(spoolableBuffer as SpoolableBuffer);
                }
            }
            fc.Dispose();
        }

        protected override void OnFileSaveBegin()
        {
            unidentifiedPackage.Children.Clear();
            foreach (SpoolableBuffer sb in VehicleHierachies)
            {
                SpoolableBuffer spoolableBuffer = new SpoolableBuffer()
                {
                    Description = "Vehicle Hierachy",
                    Context = (int)ChunkType.VehicleHierarchy,
                    Alignment = sb.Alignment
                };
                spoolableBuffer.SetBuffer(sb.GetBuffer());
                unidentifiedPackage.Children.Add(spoolableBuffer);
            }
            SpoolableBuffer mpbuf = new SpoolableBuffer() { Context = (int)ChunkType.ModelPackagePC, Description = "Models" };
            unidentifiedPackage.Children.Add(mpbuf);

            if (Models == null)
            {
                Models = new ModelPackage();

                // version and misc info
                Models.Platform = PlatformType.PC;
                Models.UID = 0x2D;
                Models.Version = 6;
                Models.IndexBuffer = new IndexBuffer(VehicleModels.Count);
                /*
                Models.IndexBuffer.Indices = new short[VehicleModels[0].IndexBuffer.Length];
                for (int id = 0; id < VehicleModels[0].IndexBuffer.Length; id++)
                {
                    Models.IndexBuffer.Indices[id] = VehicleModels[0].IndexBuffer.Indices[id];
                }

                Models.Flags = VehicleModels[0].Flags;
                */

                // materials stuff
                Models.Materials = new List<MaterialDataPC>();

                // models and textures stuff
                Models.Textures = new List<TextureDataPC>();
                Models.SubModels = new List<SubModel>();
                Models.LodInstances = new List<LodInstance>();
                Models.VertexBuffers = new List<VertexBuffer>();
                Models.Substances = new List<SubstanceDataPC>();
                Models.Models = new List<Model>();
                //mdpack.PackageType = PackageType.VehiclePackage;
            }

            var mdpackres = (ISpoolableResource)Models; // recast this as a spoolable resource
            foreach (ModelPackage vehMDPC in VehicleModels)
            {
                Models.Models.AddRange(vehMDPC.Models);
                Models.SubModels.AddRange(vehMDPC.SubModels);

                Models.Textures.AddRange(vehMDPC.Textures);
                Models.Materials.AddRange(vehMDPC.Materials);
                Models.Substances.AddRange(vehMDPC.Substances);

                Models.LodInstances.AddRange(vehMDPC.LodInstances);

                Models.VertexBuffers.AddRange(vehMDPC.VertexBuffers);
            }

            mdpackres.Spooler = mpbuf; // append the spooler
            mdpackres.Save(); // save the models
        }

        public VehicleVariation()
        {
            unidentifiedPackage = new SpoolablePackage() { Context = 0x0, Alignment = SpoolerAlignment.Align2048, Description = "Vehicle Variation Package" };
            Children.Add(unidentifiedPackage);

            VehicleModels = new List<ModelPackage>();
            VehicleHierachies = new List<SpoolableBuffer>();
        }
    }
}
