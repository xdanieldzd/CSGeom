using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using System.IO;

using OpenTK;
using OpenTK.Graphics;

namespace CSGeom
{
    // TODO: badly ported over from N3DSCmbViewer & kinda broken; fix me!

    static class ExportCollada
    {
        static string daePath;

        public static void Export(string daeFilename, GeomFile geomFile)
        {
            try
            {
                daePath = Path.GetDirectoryName(daeFilename);

                if (!Directory.Exists(daePath)) Directory.CreateDirectory(daePath);

                XmlTextWriter xw = new XmlTextWriter(daeFilename, Encoding.UTF8);

                xw.Formatting = Formatting.Indented;
                xw.Indentation = 4;
                xw.WriteStartDocument(false);
                xw.WriteStartElement("COLLADA");
                xw.WriteAttributeString("xmlns", "http://www.collada.org/2005/11/COLLADASchema");
                xw.WriteAttributeString("version", "1.4.1");
                {
                    WriteSectionAsset(xw, geomFile);
                    {
                        WriteSectionLibraryImages(xw, geomFile);
                        WriteSectionLibraryEffects(xw, geomFile);
                        WriteSectionLibraryMaterials(xw, geomFile);
                    }
                    WriteSectionLibraryGeometry(xw, geomFile);
                    WriteSectionLibraryVisualScenes(xw, geomFile);
                    WriteSectionScene(xw, geomFile);
                }
                xw.WriteEndElement();
                xw.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void WriteSectionAsset(XmlTextWriter xw, GeomFile geomFile)
        {
            xw.WriteStartElement("asset");
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                var name = (assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false).FirstOrDefault() as AssemblyProductAttribute).Product;
                var version = new Version((assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false).FirstOrDefault() as AssemblyFileVersionAttribute).Version);
                var copyright = (assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault() as AssemblyCopyrightAttribute).Copyright;
                var company = (assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false).FirstOrDefault() as AssemblyCompanyAttribute).Company;

                // Overkill? Nah. Because of the "very nice people" who uploaded N3DSCmbViewer exports to model databases *unchanged* & taking credit for them! :D

                xw.WriteStartElement("contributor");
                xw.WriteElementString("authoring_tool", string.Format("{0} {1}.{2}", name, version.Major, version.Minor));
                xw.WriteElementString("author", company);
                xw.WriteElementString("copyright", string.Format("{0} exporter {1}", name, copyright.ToLowerInvariant()));
                xw.WriteEndElement();

                xw.WriteStartElement("created");
                xw.WriteString(DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z");
                xw.WriteEndElement();

                xw.WriteStartElement("modified");
                xw.WriteString(DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z");
                xw.WriteEndElement();

                xw.WriteStartElement("unit");
                xw.WriteAttributeString("meter", "0.01");
                xw.WriteAttributeString("name", "centimeter");
                xw.WriteEndElement();

                xw.WriteStartElement("up_axis");
                xw.WriteString("Y_UP");
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        private static void WriteSectionLibraryImages(XmlTextWriter xw, GeomFile geomFile)
        {
            xw.WriteStartElement("library_images");
            {
                foreach (string tex in geomFile.TextureNames)
                {
                    string texId = string.Format("image-{0}", tex);
                    xw.WriteStartElement("image");
                    xw.WriteAttributeString("id", texId);
                    xw.WriteAttributeString("name", texId);
                    {
                        xw.WriteStartElement("init_from");
                        xw.WriteString(string.Format("{0}.png", tex));
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
        }

        private static void WriteSectionLibraryEffects(XmlTextWriter xw, GeomFile geomFile)
        {
            xw.WriteStartElement("library_effects");
            {
                string defaultID = "effect-default";

                xw.WriteStartElement("effect");
                xw.WriteAttributeString("id", defaultID);
                xw.WriteAttributeString("name", defaultID);
                {
                    xw.WriteStartElement("profile_COMMON");
                    {
                        xw.WriteStartElement("technique");
                        xw.WriteAttributeString("sid", "COMMON");
                        {
                            xw.WriteStartElement("phong");
                            {
                                xw.WriteStartElement("diffuse");
                                {
                                    xw.WriteStartElement("color");
                                    xw.WriteString("1.0 1.0 1.0 1.0");
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();

                foreach (string tex in geomFile.TextureNames)
                {
                    string effectID = string.Format("effect-{0}", tex);

                    xw.WriteStartElement("effect");
                    xw.WriteAttributeString("id", effectID);
                    xw.WriteAttributeString("name", effectID);
                    {
                        xw.WriteStartElement("profile_COMMON");
                        {
                            xw.WriteStartElement("newparam");
                            xw.WriteAttributeString("sid", string.Format("surface-{0}", tex));
                            {
                                xw.WriteStartElement("surface");
                                xw.WriteAttributeString("type", "2D");
                                {
                                    xw.WriteStartElement("init_from");
                                    xw.WriteString(string.Format("image-{0}", tex));
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();

                            xw.WriteStartElement("newparam");
                            xw.WriteAttributeString("sid", string.Format("sampler-{0}", tex));
                            {
                                xw.WriteStartElement("sampler2D");
                                {
                                    xw.WriteElementString("source", string.Format("surface-{0}", tex));
                                    xw.WriteElementString("wrap_s", "WRAP");
                                    xw.WriteElementString("wrap_t", "WRAP");
                                    xw.WriteElementString("minfilter", "LINEAR");
                                    xw.WriteElementString("magfilter", "LINEAR");
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();

                            xw.WriteStartElement("technique");
                            xw.WriteAttributeString("sid", "COMMON");
                            {
                                xw.WriteStartElement("phong");
                                {
                                    xw.WriteStartElement("diffuse");
                                    {
                                        xw.WriteStartElement("texture");
                                        xw.WriteAttributeString("texture", string.Format("sampler-{0}", tex));
                                        xw.WriteAttributeString("texcoord", "TEXCOORD0");
                                        xw.WriteEndElement();
                                    }
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
        }

        private static void WriteSectionLibraryMaterials(XmlTextWriter xw, GeomFile geomFile)
        {
            xw.WriteStartElement("library_materials");
            {
                xw.WriteStartElement("material");
                xw.WriteAttributeString("id", "material-defmat");
                {
                    xw.WriteStartElement("instance_effect");
                    xw.WriteAttributeString("url", "#effect-default");
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();

                foreach (string mat in geomFile.TextureNames)
                {
                    FileInfo textureFileInfo = new DirectoryInfo(Program.ImageDir).GetFiles(mat + " *.*").FirstOrDefault();
                    if (textureFileInfo != null)
                    {
                        string outputFile = Path.Combine(daePath, mat + textureFileInfo.Extension);
                        if (!File.Exists(outputFile)) textureFileInfo.CopyTo(outputFile);
                    }

                    xw.WriteStartElement("material");
                    xw.WriteAttributeString("id", string.Format("material-{0}", mat));
                    {
                        xw.WriteStartElement("instance_effect");
                        xw.WriteAttributeString("url", string.Format("#effect-{0}", mat));
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
        }

        private static void WriteSectionLibraryGeometry(XmlTextWriter xw, GeomFile geomFile)
        {
            xw.WriteStartElement("library_geometries");
            {
                foreach (GeomMesh mesh in geomFile.Meshes)
                {
                    GeomUnknownMaterialData unkBlock = geomFile.UnknownMaterialData[mesh.MaterialDataIndex];
                    GeomUnknownMaterialDataEntry textureBlock = unkBlock.DataEntries1.FirstOrDefault(x => x.DataUsage == DataUsage.TextureID);
                    string mat = "defmat";
                    if (textureBlock != null)
                        mat = geomFile.TextureNames[textureBlock.RawData[0]];
                    else
                        continue;

                    string meshId = string.Format("geom-{0:X8}", mesh.GetHashCode());
                    xw.WriteStartElement("geometry");
                    xw.WriteAttributeString("id", meshId);
                    xw.WriteAttributeString("name", meshId);
                    {
                        xw.WriteStartElement("mesh");
                        {
                            /* Vertices */
                            xw.WriteStartElement("source");
                            xw.WriteAttributeString("id", string.Format("{0}-pos", meshId));
                            {
                                Vector3[] vtxData = mesh.Vertices.Select(x => x.Position).ToArray();

                                xw.WriteStartElement("float_array");
                                xw.WriteAttributeString("id", string.Format("{0}-pos-array", meshId));
                                xw.WriteAttributeString("count", string.Format("{0}", vtxData.Length * 3));
                                {
                                    for (int i = 0; i < vtxData.Length; i++)
                                    {
                                        xw.WriteString(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00} ", vtxData[i].X, vtxData[i].Y, vtxData[i].Z));
                                    }
                                }
                                xw.WriteEndElement();

                                xw.WriteStartElement("technique_common");
                                {
                                    xw.WriteStartElement("accessor");
                                    xw.WriteAttributeString("source", string.Format("#{0}-pos-array", meshId));
                                    xw.WriteAttributeString("count", string.Format("{0}", vtxData.Length));
                                    xw.WriteAttributeString("stride", "3");
                                    {
                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "X");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "Y");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "Z");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();
                                    }
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();

                            /* Texcoords */
                            xw.WriteStartElement("source");
                            xw.WriteAttributeString("id", string.Format("{0}-texcoord", meshId));
                            {
                                Vector2[] texCoordData = mesh.Vertices.Select(x => x.TexCoord).ToArray();

                                xw.WriteStartElement("float_array");
                                xw.WriteAttributeString("id", string.Format("{0}-texcoord-array", meshId));
                                xw.WriteAttributeString("count", string.Format("{0}", texCoordData.Length * 2));
                                {
                                    for (int i = 0; i < texCoordData.Length; i++)
                                    {
                                        xw.WriteString(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} ", texCoordData[i].X, -texCoordData[i].Y));
                                    }
                                }
                                xw.WriteEndElement();

                                xw.WriteStartElement("technique_common");
                                {
                                    xw.WriteStartElement("accessor");
                                    xw.WriteAttributeString("source", string.Format("#{0}-texcoord-array", meshId));
                                    xw.WriteAttributeString("count", string.Format("{0}", texCoordData.Length));
                                    xw.WriteAttributeString("stride", "2");
                                    {
                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "S");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "T");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();
                                    }
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();

                            /* Colors */
                            xw.WriteStartElement("source");
                            xw.WriteAttributeString("id", string.Format("{0}-colors", meshId));
                            {
                                Color4[] colorData = mesh.Vertices.Select(x => x.Color).ToArray();

                                xw.WriteStartElement("float_array");
                                xw.WriteAttributeString("id", string.Format("{0}-colors-array", meshId));
                                xw.WriteAttributeString("count", string.Format("{0}", colorData.Length * 4));
                                {
                                    for (int i = 0; i < colorData.Length; i++)
                                    {
                                        xw.WriteString(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00} {3:0.00} ", colorData[i].R, colorData[i].G, colorData[i].B, colorData[i].A));
                                    }
                                }
                                xw.WriteEndElement();

                                xw.WriteStartElement("technique_common");
                                {
                                    xw.WriteStartElement("accessor");
                                    xw.WriteAttributeString("source", string.Format("#{0}-colors-array", meshId));
                                    xw.WriteAttributeString("count", string.Format("{0}", colorData.Length));
                                    xw.WriteAttributeString("stride", "4");
                                    {
                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "R");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "G");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "B");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "A");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();
                                    }
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();

                            /* Normals */
                            xw.WriteStartElement("source");
                            xw.WriteAttributeString("id", string.Format("{0}-norm", meshId));
                            {
                                Vector3[] normData = mesh.Vertices.Select(x => x.Normal).ToArray();

                                xw.WriteStartElement("float_array");
                                xw.WriteAttributeString("id", string.Format("{0}-norm-array", meshId));
                                xw.WriteAttributeString("count", string.Format("{0}", normData.Length * 3));
                                {
                                    for (int i = 0; i < normData.Length; i++)
                                    {
                                        xw.WriteString(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00} ", normData[i].X, normData[i].Y, normData[i].Z));
                                    }
                                }
                                xw.WriteEndElement();

                                xw.WriteStartElement("technique_common");
                                {
                                    xw.WriteStartElement("accessor");
                                    xw.WriteAttributeString("source", string.Format("#{0}-norm-array", meshId));
                                    xw.WriteAttributeString("count", string.Format("{0}", normData.Length));
                                    xw.WriteAttributeString("stride", "3");
                                    {
                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "X");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "Y");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();

                                        xw.WriteStartElement("param");
                                        xw.WriteAttributeString("name", "Z");
                                        xw.WriteAttributeString("type", "float");
                                        xw.WriteEndElement();
                                    }
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();

                            xw.WriteStartElement("vertices");
                            xw.WriteAttributeString("id", string.Format("{0}-vtx", meshId));
                            {
                                xw.WriteStartElement("input");
                                xw.WriteAttributeString("semantic", "POSITION");
                                xw.WriteAttributeString("source", string.Format("#{0}-pos", meshId));
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();

                            string primitiveType = string.Empty;
                            switch (mesh.PrimitiveType)
                            {
                                case PrimitiveType.Triangles: primitiveType = "triangles"; break;
                                case PrimitiveType.TriangleStrip: primitiveType = "tristrips"; break;
                                default: throw new Exception("Unknown primitive type");
                            }

                            xw.WriteStartElement(primitiveType);
                            xw.WriteAttributeString("count", string.Format("{0}", mesh.NumIndices));
                            xw.WriteAttributeString("material", string.Format("material-{0}-symbol", mat));
                            {
                                xw.WriteStartElement("input");
                                xw.WriteAttributeString("semantic", "VERTEX");
                                xw.WriteAttributeString("source", string.Format("#{0}-vtx", meshId));
                                xw.WriteAttributeString("offset", "0");
                                xw.WriteEndElement();

                                xw.WriteStartElement("input");
                                xw.WriteAttributeString("semantic", "TEXCOORD");
                                xw.WriteAttributeString("source", string.Format("#{0}-texcoord", meshId));
                                xw.WriteAttributeString("offset", "0");
                                xw.WriteEndElement();

                                xw.WriteStartElement("input");
                                xw.WriteAttributeString("semantic", "COLOR");
                                xw.WriteAttributeString("source", string.Format("#{0}-colors", meshId));
                                xw.WriteAttributeString("offset", "0");
                                xw.WriteEndElement();

                                xw.WriteStartElement("input");
                                xw.WriteAttributeString("semantic", "NORMAL");
                                xw.WriteAttributeString("source", string.Format("#{0}-norm", meshId));
                                xw.WriteAttributeString("offset", "0");
                                xw.WriteEndElement();

                                // TODO: if non-ushort indices exist, convert them here!

                                xw.WriteStartElement("p");
                                {
                                    if (mesh.PrimitiveType == PrimitiveType.TriangleStrip)
                                    {
                                        for (int i = 2; i < mesh.VertexIndices.Length; i++)
                                        {
                                            if ((i % 2) != 0)
                                                xw.WriteString(string.Format("{0} {1} {2} ", mesh.VertexIndices[i], mesh.VertexIndices[i - 1], mesh.VertexIndices[i - 2]));
                                            else
                                                xw.WriteString(string.Format("{0} {1} {2} ", mesh.VertexIndices[i - 2], mesh.VertexIndices[i - 1], mesh.VertexIndices[i]));
                                        }
                                    }
                                    else
                                        for (int i = 0; i < mesh.VertexIndices.Length; i++) xw.WriteString(string.Format("{0} ", mesh.VertexIndices[i]));
                                }

                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
        }


        private static void WriteSectionLibraryVisualScenes(XmlTextWriter xw, GeomFile geomFile)
        {
            xw.WriteStartElement("library_visual_scenes");
            {
                xw.WriteStartElement("visual_scene");
                xw.WriteAttributeString("id", "default");
                {
                    foreach (GeomMesh mesh in geomFile.Meshes)
                    {
                        GeomUnknownMaterialData unkBlock = geomFile.UnknownMaterialData[mesh.MaterialDataIndex];
                        GeomUnknownMaterialDataEntry textureBlock = unkBlock.DataEntries1.FirstOrDefault(x => x.DataUsage == DataUsage.TextureID);
                        string mat = "defmat";
                        if (textureBlock != null)
                            mat = geomFile.TextureNames[textureBlock.RawData[0]];
                        else
                            continue;

                        string nodeId = string.Format("node-{0:X8}", mesh.GetHashCode());
                        xw.WriteStartElement("node");
                        xw.WriteAttributeString("id", nodeId);
                        xw.WriteAttributeString("name", nodeId);
                        {
                            xw.WriteStartElement("translate");
                            xw.WriteString("0.0 0.0 0.0");
                            xw.WriteEndElement();

                            xw.WriteStartElement("rotate");
                            xw.WriteString("0.0 0.0 1.0 0.0");
                            xw.WriteEndElement();

                            xw.WriteStartElement("rotate");
                            xw.WriteString("0.0 1.0 0.0 0.0");
                            xw.WriteEndElement();

                            xw.WriteStartElement("rotate");
                            xw.WriteString("1.0 0.0 0.0 0.0");
                            xw.WriteEndElement();

                            xw.WriteStartElement("scale");
                            xw.WriteString("1.0 1.0 1.0");
                            xw.WriteEndElement();

                            xw.WriteStartElement("instance_geometry");
                            xw.WriteAttributeString("url", string.Format("#geom-{0:X8}", mesh.GetHashCode()));
                            {
                                xw.WriteStartElement("bind_material");
                                {
                                    xw.WriteStartElement("technique_common");
                                    {
                                        xw.WriteStartElement("instance_material");
                                        xw.WriteAttributeString("symbol", string.Format("material-{0}-symbol", mat));
                                        xw.WriteAttributeString("target", string.Format("#material-{0}", mat));
                                        xw.WriteEndElement();
                                    }
                                    xw.WriteEndElement();
                                }
                                xw.WriteEndElement();
                            }
                            xw.WriteEndElement();
                        }
                        xw.WriteEndElement();
                    }
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        private static void WriteSectionScene(XmlTextWriter xw, GeomFile geomFile)
        {
            xw.WriteStartElement("scene");
            {
                xw.WriteStartElement("instance_visual_scene");
                xw.WriteAttributeString("url", "#default");
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }
    }
}
