using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;
using OpenTK.Graphics;

using Cobalt.IO;
using Cobalt.Mesh;
using Cobalt.Texture;

namespace CSGeom
{
    public enum PrimitiveType : ushort
    {
        TriangleStrip = 0x00,
        Triangles = 0x01
    }

    public enum AttributeUsage : ushort
    {
        Position = 0x01,
        Normal = 0x02,
        TexCoord = 0x05,
        Unknown0x06 = 0x06,
        MaybeColor = 0x09,
        MaybeBoneIDs = 0x0A,
        MaybeBoneWeights = 0x0B
    }

    public enum ComponentDataType : ushort
    {
        UnsignedByte = 0x00,
        SignedByte = 0x01,
        UnsignedShort = 0x02,
        SignedShort = 0x03,
        UnsignedByteN = 0x04,
        SignedByteN = 0x05,
        UnsignedShortN = 0x06,
        SignedShortN = 0x07,
        Float16 = 0x08,
        Float32 = 0x09
    }

    // TODO: what IS this? PVR registers? Shader uniforms?
    public enum DataUsage : byte
    {
        TextureID = 0x32,
        Unknown0x33 = 0x33,
        Unknown0x48 = 0x48,
        Unknown0xA4 = 0xA4,
        Unknown0xA8 = 0xA8
    }

    public class GeomUnknownMaterialDataEntry
    {
        public byte[] RawData { get; private set; }             // 0x10 bytes raw data; can be float, etc...?
        public DataUsage DataUsage { get; private set; }        // 0x32, 0x33, 0x48, 0xA4, 0xA8...? Might be ushort w/ next byte?
        public byte Unknown0x11 { get; private set; }           // 0x00, 0x04, 0x64...?
        public ushort Unknown0x12 { get; private set; }         // 0xFF00...?
        public uint Unknown0x14 { get; private set; }           // ...?

        public GeomUnknownMaterialDataEntry(Stream stream)
        {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endian.LittleEndian);

            RawData = reader.ReadBytes(0x10);
            DataUsage = (DataUsage)reader.ReadByte();
            Unknown0x11 = reader.ReadByte();
            Unknown0x12 = reader.ReadUInt16();
            Unknown0x14 = reader.ReadUInt32();
        }
    }

    public class GeomUnknownMaterialData
    {
        public uint Unknown0x00 { get; private set; }           // ???
        public uint Unknown0x04 { get; private set; }           // 088100C1...?
        public ushort Unknown0x08 { get; private set; }         // 0111...?
        public byte Unknown0x0A { get; private set; }           // 08, 00...?
        public byte Unknown0x0B { get; private set; }           // 00, 02...?
        public ushort Unknown0x0C { get; private set; }         // Zero?
        public ushort Unknown0x0E { get; private set; }         // 0x0004?
        public ushort Unknown0x10 { get; private set; }         // Zero?
        public ushort Unknown0x12 { get; private set; }         // 0x0004, 0x0005...?
        public byte NumEntries1 { get; private set; }
        public byte NumEntries2 { get; private set; }
        public ushort Unknown0x16 { get; private set; }         // 0x0003, 0x0000, 0x0001...?

        public GeomUnknownMaterialDataEntry[] DataEntries1 { get; private set; }
        public GeomUnknownMaterialDataEntry[] DataEntries2 { get; private set; }

        public GeomUnknownMaterialData(Stream stream)
        {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endian.LittleEndian);

            Unknown0x00 = reader.ReadUInt32();
            Unknown0x04 = reader.ReadUInt32();
            Unknown0x08 = reader.ReadUInt16();
            Unknown0x0A = reader.ReadByte();
            Unknown0x0B = reader.ReadByte();
            Unknown0x0C = reader.ReadUInt16();
            Unknown0x0E = reader.ReadUInt16();
            Unknown0x10 = reader.ReadUInt16();
            Unknown0x12 = reader.ReadUInt16();
            NumEntries1 = reader.ReadByte();
            NumEntries2 = reader.ReadByte();
            Unknown0x16 = reader.ReadUInt16();

            DataEntries1 = new GeomUnknownMaterialDataEntry[NumEntries1];
            if (NumEntries1 != 0)
            {
                for (int i = 0; i < DataEntries1.Length; i++) DataEntries1[i] = new GeomUnknownMaterialDataEntry(stream);
            }

            DataEntries2 = new GeomUnknownMaterialDataEntry[NumEntries2];
            if (NumEntries2 != 0)
            {
                for (int i = 0; i < DataEntries2.Length; i++) DataEntries2[i] = new GeomUnknownMaterialDataEntry(stream);
            }
        }
    }

    public class GeomVertexAttribute
    {
        public AttributeUsage AttributeUsage { get; private set; }
        public ushort NumComponents { get; private set; }
        public ComponentDataType ComponentDataType { get; private set; }
        public ushort Offset { get; private set; }

        public GeomVertexAttribute(Stream stream)
        {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endian.LittleEndian);

            AttributeUsage = (AttributeUsage)reader.ReadUInt16();
            NumComponents = reader.ReadUInt16();
            ComponentDataType = (ComponentDataType)reader.ReadUInt16();
            Offset = reader.ReadUInt16();
        }
    }

    public class GeomMesh
    {
        public uint VertexDataOffset { get; private set; }
        public uint Unknown0x04 { get; private set; }           // Zero?
        public uint IndicesOffset { get; private set; }
        public uint Unknown0x0C { get; private set; }           // Zero?

        public uint UnknownIndicesOffset { get; private set; }
        public uint Unknown0x14 { get; private set; }           // Zero?
        public uint Unknown0x18 { get; private set; }           // Zero? Potentially pointer?
        public uint Unknown0x1C { get; private set; }           // Zero?

        public uint VertexAttribsOffset { get; private set; }
        public uint Unknown0x24 { get; private set; }           // Zero?
        public ushort NumUnknownIndices { get; private set; }
        public ushort NumVertexAttribs { get; private set; }
        public uint SizeOfVertex { get; private set; }

        public byte Unknown0x30 { get; private set; }           // Unknown; 0x02...?
        public byte Unknown0x31 { get; private set; }           // Unknown; 0x05, 0x01...?
        public PrimitiveType PrimitiveType { get; private set; }
        public uint Unknown0x34 { get; private set; }           // Unknown; 478EE4F0, 9487C9D4...?
        public ushort MaterialDataIndex { get; private set; }
        public ushort Unknown0x3A { get; private set; }
        public uint NumVertices { get; private set; }

        public uint NumIndices { get; private set; }
        public Vector3 Unknown0x44 { get; private set; }        // 3x float; not sure if Vector3 
        public Vector3 Unknown0x50 { get; private set; }        // 3x float; not sure if Vector3 
        public Vector3 Unknown0x5C { get; private set; }        // 3x float; not sure if Vector3 

        public GeomVertexAttribute[] VertexAttributes { get; private set; }
        public CommonVertex[] Vertices { get; private set; }
        public ushort[] VertexIndices { get; private set; }
        public uint[] UnknownIndices { get; private set; }

        public GeomMesh(Stream stream)
        {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endian.LittleEndian);

            VertexDataOffset = reader.ReadUInt32();
            Unknown0x04 = reader.ReadUInt32();
            IndicesOffset = reader.ReadUInt32();
            Unknown0x0C = reader.ReadUInt32();

            UnknownIndicesOffset = reader.ReadUInt32();
            Unknown0x14 = reader.ReadUInt32();
            Unknown0x18 = reader.ReadUInt32();
            Unknown0x1C = reader.ReadUInt32();

            VertexAttribsOffset = reader.ReadUInt32();
            Unknown0x24 = reader.ReadUInt32();
            NumUnknownIndices = reader.ReadUInt16();
            NumVertexAttribs = reader.ReadUInt16();
            SizeOfVertex = reader.ReadUInt32();

            Unknown0x30 = reader.ReadByte();
            Unknown0x31 = reader.ReadByte();
            PrimitiveType = (PrimitiveType)reader.ReadUInt16();
            Unknown0x34 = reader.ReadUInt32();
            MaterialDataIndex = reader.ReadUInt16();
            Unknown0x3A = reader.ReadUInt16();
            NumVertices = reader.ReadUInt32();

            NumIndices = reader.ReadUInt32();
            Unknown0x44 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Unknown0x50 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Unknown0x5C = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            long lastPosition = stream.Position;

            if (NumVertexAttribs != 0)
            {
                stream.Seek(VertexAttribsOffset, SeekOrigin.Begin);
                VertexAttributes = new GeomVertexAttribute[NumVertexAttribs];
                for (int i = 0; i < VertexAttributes.Length; i++) VertexAttributes[i] = new GeomVertexAttribute(stream);
            }

            if (NumVertices != 0)
            {
                stream.Seek(VertexDataOffset, SeekOrigin.Begin);
                Vertices = new CommonVertex[NumVertices];
                for (int i = 0; i < Vertices.Length; i++) Vertices[i] = GeomFile.GenerateCommonVertex(VertexAttributes, reader.ReadBytes((int)SizeOfVertex));
            }

            if (NumIndices != 0)
            {
                stream.Seek(IndicesOffset, SeekOrigin.Begin);
                VertexIndices = new ushort[NumIndices];
                for (int i = 0; i < VertexIndices.Length; i++) VertexIndices[i] = reader.ReadUInt16();
            }

            if (NumUnknownIndices != 0)
            {
                stream.Seek(UnknownIndicesOffset, SeekOrigin.Begin);
                UnknownIndices = new uint[NumUnknownIndices];
                for (int i = 0; i < UnknownIndices.Length; i++) UnknownIndices[i] = reader.ReadUInt32();
            }

            stream.Seek(lastPosition, SeekOrigin.Begin);
        }
    }

    public class GeomUnknownData1
    {
        public Vector3 Unknown0x00 { get; private set; }    // 3x float; maybe not Vector3?
        public Vector3 Unknown0x0C { get; private set; }    // 3x float; maybe not Vector3?
        public Vector3 Unknown0x18 { get; private set; }    // 3x float; maybe not Vector3?
        public Vector3 Unknown0x24 { get; private set; }    // 3x float; maybe not Vector3?

        public GeomUnknownData1(Stream stream)
        {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endian.LittleEndian);

            Unknown0x00 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Unknown0x0C = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Unknown0x18 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Unknown0x24 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    public class GeomFile
    {
        static readonly Dictionary<ComponentDataType, int> vertexComponentSizes = new Dictionary<ComponentDataType, int>()
        {
            { ComponentDataType.UnsignedByte, 1 },
            { ComponentDataType.UnsignedByteN, 1 },
            { ComponentDataType.SignedByte, 1 },
            { ComponentDataType.SignedByteN, 1 },

            { ComponentDataType.UnsignedShort, 2 },
            { ComponentDataType.UnsignedShortN, 2 },
            { ComponentDataType.SignedShort, 2 },
            { ComponentDataType.SignedShortN, 2 },
            { ComponentDataType.Float16, 2 },

            { ComponentDataType.Float32, 4 },
        };

        public uint Unknown0x00 { get; private set; }           // Always 0x64/100?
        public ushort NumMeshes { get; private set; }
        public ushort NumUnknownMaterialData { get; private set; }
        public uint Unknown0x08 { get; private set; }           // Zero?
        public uint NumUnknownData1 { get; private set; }

        public uint SizeOfTextureNames { get; private set; }    // (divide by 0x20 to get number of texture names)
        public Vector3 Unknown0x14 { get; private set; }        // 3x float; not sure if Vector3 (not bounding box?)
        public Vector3 Unknown0x20 { get; private set; }        // 3x float; not sure if Vector3 (not bounding box?)
        public uint Unknown0x2C { get; private set; }           // Zero?

        public uint MeshesOffset { get; private set; }
        public uint Unknown0x34 { get; private set; }           // Zero?
        public uint UnknownMaterialDataOffset { get; private set; }
        public uint Unknown0x3C { get; private set; }           // Zero?

        public uint Unknown0x40 { get; private set; }           // Zero? Potentially pointer?
        public uint Unknown0x44 { get; private set; }           // Zero?
        public uint Unknown0x48 { get; private set; }           // Zero? Potentially pointer?
        public uint Unknown0x4C { get; private set; }           // Zero?

        public uint UnknownData1Offset { get; private set; }
        public uint Unknown0x54 { get; private set; }           // Zero?
        public uint Unknown0x58 { get; private set; }           // Zero? Potentially pointer?
        public uint Unknown0x5C { get; private set; }           // Zero?

        public uint TextureNamesOffset { get; private set; }
        public uint Unknown0x64 { get; private set; }           // Zero?
        public uint UnknownData2Offset { get; private set; }    // Only in character models, zero in map models?
        public uint Unknown0x6C { get; private set; }           // Zero?

        public GeomMesh[] Meshes { get; private set; }
        public GeomUnknownMaterialData[] UnknownMaterialData { get; private set; }
        public GeomUnknownData1[] UnknownData1 { get; private set; }
        public string[] TextureNames { get; private set; }

        public GeomFile(Stream stream)
        {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endian.LittleEndian);

            Unknown0x00 = reader.ReadUInt32();
            NumMeshes = reader.ReadUInt16();
            NumUnknownMaterialData = reader.ReadUInt16();
            Unknown0x08 = reader.ReadUInt32();
            NumUnknownData1 = reader.ReadUInt32();

            SizeOfTextureNames = reader.ReadUInt32();
            Unknown0x14 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Unknown0x20 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Unknown0x2C = reader.ReadUInt32();

            MeshesOffset = reader.ReadUInt32();
            Unknown0x34 = reader.ReadUInt32();
            UnknownMaterialDataOffset = reader.ReadUInt32();
            Unknown0x3C = reader.ReadUInt32();

            Unknown0x40 = reader.ReadUInt32();
            Unknown0x44 = reader.ReadUInt32();
            Unknown0x48 = reader.ReadUInt32();
            Unknown0x4C = reader.ReadUInt32();

            UnknownData1Offset = reader.ReadUInt32();
            Unknown0x54 = reader.ReadUInt32();
            Unknown0x58 = reader.ReadUInt32();
            Unknown0x5C = reader.ReadUInt32();

            TextureNamesOffset = reader.ReadUInt32();
            Unknown0x64 = reader.ReadUInt32();
            UnknownData2Offset = reader.ReadUInt32();
            Unknown0x6C = reader.ReadUInt32();

            Meshes = new GeomMesh[NumMeshes];
            if (NumMeshes != 0)
            {
                stream.Seek(MeshesOffset, SeekOrigin.Begin);
                for (int i = 0; i < Meshes.Length; i++) Meshes[i] = new GeomMesh(stream);
            }

            UnknownMaterialData = new GeomUnknownMaterialData[NumUnknownMaterialData];
            if (NumUnknownMaterialData != 0)
            {
                stream.Seek(UnknownMaterialDataOffset, SeekOrigin.Begin);
                for (int i = 0; i < UnknownMaterialData.Length; i++) UnknownMaterialData[i] = new GeomUnknownMaterialData(stream);
            }

            UnknownData1 = new GeomUnknownData1[NumUnknownData1];
            if (NumUnknownData1 != 0)
            {
                stream.Seek(UnknownData1Offset, SeekOrigin.Begin);
                for (int i = 0; i < UnknownData1.Length; i++) UnknownData1[i] = new GeomUnknownData1(stream);
            }

            int numTextureNames = (int)(SizeOfTextureNames > 0 ? SizeOfTextureNames / 0x20 : 0);
            TextureNames = new string[numTextureNames];
            if (SizeOfTextureNames != 0)
            {
                stream.Seek(TextureNamesOffset, SeekOrigin.Begin);
                for (int i = 0; i < TextureNames.Length; i++) TextureNames[i] = Encoding.ASCII.GetString(reader.ReadBytes(0x20)).TrimEnd('\0');
            }
        }

        public List<Mesh> GetMeshes()
        {
            List<Mesh> meshes = new List<Mesh>();

            foreach (GeomMesh geomMesh in Meshes)
            {
                Mesh mesh = new Mesh();

                switch (geomMesh.PrimitiveType)
                {
                    case PrimitiveType.TriangleStrip:
                        mesh.SetPrimitiveType(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
                        break;

                    case PrimitiveType.Triangles:
                        mesh.SetPrimitiveType(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
                        break;

                    default: throw new NotImplementedException("Unknown primitive type");
                }

                mesh.SetVertexData<CommonVertex>(geomMesh.Vertices);
                mesh.SetIndices<ushort>(geomMesh.VertexIndices);

                Material material = new Material();

                // Material, or something...
                GeomUnknownMaterialData matData = UnknownMaterialData[geomMesh.MaterialDataIndex];

                // Get texture ID
                GeomUnknownMaterialDataEntry textureData = matData.DataEntries1.FirstOrDefault(x => x.DataUsage == DataUsage.TextureID);
                if (textureData != null)
                {
                    FileInfo textureFileInfo = new DirectoryInfo(Program.ImageDir).GetFiles(TextureNames[textureData.RawData[0]] + " *.*").FirstOrDefault();
                    if (textureFileInfo != null)
                        material.Texture = new Texture(new System.Drawing.Bitmap(textureFileInfo.FullName));
                }

                // Testing 1, 2, 3...
                GeomUnknownMaterialDataEntry tmpFloatData = matData.DataEntries1.FirstOrDefault(x => x.DataUsage == DataUsage.Unknown0x33);
                if (tmpFloatData != null)
                {
                    // Probably wrong, some maps have 2.0 in the assumed RGB channels...?
                    // ...lets not apply this right for now

                    //material.Ambient = new Color4(BitConverter.ToSingle(tmpFloatData.RawData, 0), BitConverter.ToSingle(tmpFloatData.RawData, 4), BitConverter.ToSingle(tmpFloatData.RawData, 8), BitConverter.ToSingle(tmpFloatData.RawData, 12));
                }

                // Dummy texture if none was found
                if (material.Texture == null)
                {
                    System.Drawing.Bitmap tmpBitmap = new System.Drawing.Bitmap(32, 32);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tmpBitmap))
                    {
                        g.Clear(System.Drawing.Color.White);
                    }
                    material.Texture = new Texture(tmpBitmap);
                }
                mesh.SetMaterial(material);

                meshes.Add(mesh);
            }

            if (false)
            {
                // More testing...
                foreach (GeomUnknownData1 unkData1 in UnknownData1)
                {
                    Mesh mesh = new Mesh();

                    CommonVertex[] verts = new CommonVertex[4];
                    verts[0] = new CommonVertex() { Position = unkData1.Unknown0x00, Color = Color4.White };
                    verts[1] = new CommonVertex() { Position = unkData1.Unknown0x0C, Color = Color4.Red };
                    verts[2] = new CommonVertex() { Position = unkData1.Unknown0x18, Color = Color4.Green };
                    verts[3] = new CommonVertex() { Position = unkData1.Unknown0x24, Color = Color4.Blue };

                    mesh.SetPrimitiveType(OpenTK.Graphics.OpenGL.PrimitiveType.Points);
                    mesh.SetVertexData<CommonVertex>(verts);

                    System.Drawing.Bitmap tmpBitmap = new System.Drawing.Bitmap(32, 32);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tmpBitmap))
                    {
                        g.Clear(System.Drawing.Color.White);
                    }
                    mesh.SetMaterial(new Material(new Texture(tmpBitmap)));

                    meshes.Add(mesh);
                }
            }

            return meshes;
        }

        public static CommonVertex GenerateCommonVertex(GeomVertexAttribute[] vertexAttribs, byte[] rawVertex)
        {
            CommonVertex vertex = new CommonVertex() { Position = Vector3.Zero, Normal = Vector3.Zero, Color = Color4.White, TexCoord = Vector2.Zero, BoneIDs = Vector4.Zero, BoneWeights = Vector4.Zero };

            foreach (GeomVertexAttribute vertexAttrib in vertexAttribs)
            {
                if (!vertexComponentSizes.ContainsKey(vertexAttrib.ComponentDataType)) continue;
                int sizeOfComponent = vertexComponentSizes[vertexAttrib.ComponentDataType];

                dynamic[] data = new dynamic[vertexAttrib.NumComponents];
                for (int i = 0; i < vertexAttrib.NumComponents; i++)
                    data[i] = GetVertexComponent(vertexAttrib.ComponentDataType, (ushort)(vertexAttrib.Offset + (i * sizeOfComponent)), rawVertex);

                switch (vertexAttrib.AttributeUsage)
                {
                    case AttributeUsage.Position: vertex.Position = new Vector3(data[0], data[1], data[2]); break;
                    case AttributeUsage.Normal: vertex.Normal = new Vector3(data[0], data[1], data[2]); break;
                    case AttributeUsage.TexCoord: vertex.TexCoord = new Vector2(data[0], -data[1]); break;
                    case AttributeUsage.MaybeColor: vertex.Color = new Color4(data[0], data[1], data[2], data[3]); break;
                    case AttributeUsage.MaybeBoneIDs: vertex.BoneIDs = RoundedVector4FromData(data); break;
                    case AttributeUsage.MaybeBoneWeights: vertex.BoneWeights = RoundedVector4FromData(data); break;

                    default: break;
                }
            }

            return vertex;
        }

        private static Vector4 RoundedVector4FromData(dynamic[] data)
        {
            Vector4 output = Vector4.Zero;

            if (data.Length >= 4) output.W = data[3];
            if (data.Length >= 3) output.Z = data[2];
            if (data.Length >= 2) output.Y = data[1];
            if (data.Length >= 1) output.X = data[0];

            return output;
        }

        // http://codereview.stackexchange.com/q/45007
        public static float Float16toFloat32(int hbits)
        {
            int mant = hbits & 0x03FF;
            int exp = hbits & 0x7C00;

            if (exp == 0x7C00)
                exp = 0x3FC00;
            else if (exp != 0)
            {
                exp += 0x1C000;
                if (mant == 0 && exp > 0x1C400)
                    return BitConverter.ToSingle(BitConverter.GetBytes((hbits & 0x8000) << 16 | exp << 13 | 0x3FF), 0);
            }
            else if (mant != 0)
            {
                exp = 0x1C400;
                do
                {
                    mant <<= 1;
                    exp -= 0x400;
                } while ((mant & 0x400) == 0);
                mant &= 0x3FF;
            }
            return BitConverter.ToSingle(BitConverter.GetBytes((hbits & 0x8000) << 16 | (exp | mant) << 13), 0);
        }

        private static dynamic GetVertexComponent(ComponentDataType dataType, ushort offset, byte[] rawVertex)
        {
            // TODO: files are little-endian, but make this endian-aware (i.e. EndianBinaryReader) regardless?

            switch (dataType)
            {
                case ComponentDataType.UnsignedByte:
                case ComponentDataType.UnsignedByteN:
                    return rawVertex[offset];

                case ComponentDataType.SignedByte:
                case ComponentDataType.SignedByteN:
                    return (sbyte)rawVertex[offset];

                case ComponentDataType.UnsignedShort:
                case ComponentDataType.UnsignedShortN:
                    return BitConverter.ToUInt16(rawVertex, offset);

                case ComponentDataType.SignedShort:
                case ComponentDataType.SignedShortN:
                    return BitConverter.ToInt16(rawVertex, offset);

                case ComponentDataType.Float16:
                    return Float16toFloat32(BitConverter.ToInt16(rawVertex, offset));

                case ComponentDataType.Float32:
                    return BitConverter.ToSingle(rawVertex, offset);

                default: throw new NotImplementedException("Unimplemented/unknown data type");
            }
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public struct CommonVertex : IVertexStruct
    {
        [VertexElement(AttributeIndex = 0)]
        public Vector3 Position;
        [VertexElement(AttributeIndex = 1)]
        public Vector3 Normal;
        [VertexElement(AttributeIndex = 2)]
        public Color4 Color;
        [VertexElement(AttributeIndex = 3)]
        public Vector2 TexCoord;
        [VertexElement(AttributeIndex = 4)]
        public Vector4 BoneIDs;
        [VertexElement(AttributeIndex = 5)]
        public Vector4 BoneWeights;
    }
}
