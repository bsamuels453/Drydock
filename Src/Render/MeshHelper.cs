﻿#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    /// <summary>
    ///   helper class for use with rectangular meshes
    /// </summary>
    internal static class MeshHelper{
        public static int[] CreateIndiceArray(int numQuads){
            //construct indice list
            //remember the clockwise-fu
            //+1-----+2
            //|     /
            //|   /    
            //| /     
            //+0
            //       +2
            //      / |
            //    /   | 
            //  /     |
            //+0-----+3
            var indicies = new int[numQuads*6];
            int curVertex = 0;
            for (int i = 0; i < indicies.Count(); i += 6){
                indicies[i] = curVertex;
                indicies[i + 1] = curVertex + 1;
                indicies[i + 2] = curVertex + 2;

                indicies[i + 3] = curVertex;
                indicies[i + 4] = curVertex + 2;
                indicies[i + 5] = curVertex + 3;

                curVertex += 4;
            }
            return indicies;
        }

        public static VertexPositionNormalTexture[] CreateTexcoordedVertexList(int numQuads){
            var verticies = new VertexPositionNormalTexture[numQuads*4];

            for (int i = 0; i < verticies.Count(); i++){
                verticies[i] = new VertexPositionNormalTexture();
            }

            for (int i = 0; i < verticies.Count(); i += 4){
                verticies[i].TextureCoordinate = new Vector2(0, 0);
                verticies[i + 1].TextureCoordinate = new Vector2(0, 1);
                verticies[i + 2].TextureCoordinate = new Vector2(1, 1);
                verticies[i + 3].TextureCoordinate = new Vector2(1, 0);
            }

            return verticies;
        }

        public static void Encode2DListIntoArray(int meshWidth, int meshHeight, ref Vector3[,] mesh, List<List<Vector3>> list){
            for (int x = 0; x < meshWidth; x++){
                for (int y = 0; y < meshHeight; y++){
                    mesh[x, y] = list[x][y];
                }
            }
        }

        public static void Encode2DListIntoArray(int meshWidth, int meshHeight, ref Vector3[,] mesh, Vector3[][] list) {
            for (int x = 0; x < meshWidth; x++) {
                for (int y = 0; y < meshHeight; y++) {
                    mesh[x, y] = list[x][y];
                }
            }
        }

        public static void GenerateMeshNormals(Vector3[,] mesh, ref Vector3[,] normals){
            for (int vertX = 0; vertX < mesh.GetLength(0) - 1; vertX++){
                for (int vertZ = 0; vertZ < mesh.GetLength(1) - 1; vertZ++){
                    var crossSum = new Vector3();

                    var s1 = mesh[vertX + 1, vertZ] - mesh[vertX, vertZ];
                    var s2 = mesh[vertX, vertZ + 1] - mesh[vertX, vertZ];
                    var s3 = mesh[vertX + 1, vertZ + 1] - mesh[vertX, vertZ];

                    crossSum += Vector3.Cross(s1, s3);
                    crossSum += Vector3.Cross(s3, s2);

                    normals[vertX, vertZ] += crossSum;
                    if (crossSum != Vector3.Zero){
                        normals[vertX, vertZ].Normalize();
                    }
                }
            }

            for (int vertX = 1; vertX < mesh.GetLength(0); vertX++){
                for (int vertZ = 1; vertZ < mesh.GetLength(1); vertZ++){
                    var crossSum = new Vector3();

                    var s1 = mesh[vertX - 1, vertZ] - mesh[vertX, vertZ];
                    var s2 = mesh[vertX, vertZ - 1] - mesh[vertX, vertZ];
                    var s3 = mesh[vertX - 1, vertZ - 1] - mesh[vertX, vertZ];

                    crossSum += Vector3.Cross(s1, s3);
                    crossSum += Vector3.Cross(s3, s2);

                    normals[vertX, vertZ] += crossSum;
                    normals[vertX, vertZ].Normalize();
                }
            }
        }

        public static void ConvertMeshToVertList(Vector3[,] mesh, Vector3[,] normals, ref VertexPositionNormalTexture[] verticies){
            //convert from 2d array to 1d
            int index = 0;
            for (int x = 0; x < mesh.GetLength(0) - 1; x++){
                for (int z = 0; z < mesh.GetLength(1) - 1; z++){
                    verticies[index].Position = mesh[x, z];
                    verticies[index].Normal = normals[x, z];

                    verticies[index + 1].Position = mesh[x, z + 1];
                    verticies[index + 1].Normal = normals[x, z + 1];

                    verticies[index + 2].Position = mesh[x + 1, z + 1];
                    verticies[index + 2].Normal = normals[x + 1, z + 1];

                    verticies[index + 3].Position = mesh[x + 1, z];
                    verticies[index + 3].Normal = normals[x + 1, z];

                    index += 4;
                }
            }
        }

        public static void GenerateCube(out VertexPositionNormalTexture[] verticies, out int[] indicies, Vector3 origin, float xSize, float ySize, float zSize){
            //boy do i love hardcoding
            verticies = new VertexPositionNormalTexture[20];
            indicies = new[]{0, 1, 2, 2, 3, 0, 4, 5, 6, 6, 7, 4, 8, 9, 10, 10, 11, 8, 12, 13, 14, 14, 15, 12, 16, 19, 18, 18, 17, 16};

            for (int indexOffset = 0; indexOffset < 20; indexOffset += 4){
                var faceVertexes = CreateTexcoordedVertexList(1);

                faceVertexes.CopyTo(verticies, indexOffset);
            }

            Vector3 xSizeV = new Vector3(xSize, 0, 0);
            Vector3 ySizeV = new Vector3(0, ySize, 0);
            Vector3 zSizeV = new Vector3(0, 0, zSize);

            verticies[0].Position = origin;
            verticies[1].Position = origin + ySizeV;
            verticies[2].Position = origin + xSizeV + ySizeV;
            verticies[3].Position = origin + xSizeV;

            verticies[4].Position = origin + xSizeV;
            verticies[5].Position = origin + xSizeV + ySizeV;
            verticies[6].Position = origin + ySizeV + zSizeV + xSizeV;
            verticies[7].Position = origin + xSizeV + zSizeV;

            verticies[8].Position = origin + zSizeV + xSizeV;
            verticies[9].Position = origin + ySizeV + zSizeV + xSizeV;
            verticies[10].Position = origin + zSizeV + ySizeV;
            verticies[11].Position = origin + zSizeV;

            verticies[12].Position = origin + zSizeV;
            verticies[13].Position = origin + zSizeV + ySizeV;
            verticies[14].Position = origin + ySizeV;
            verticies[15].Position = origin;

            verticies[16].Position = origin + ySizeV;
            verticies[17].Position = origin + ySizeV + xSizeV;
            verticies[18].Position = origin + xSizeV + ySizeV + zSizeV;
            verticies[19].Position = origin + ySizeV + zSizeV;

            verticies[0].Normal = Vector3.Forward;
            verticies[1].Normal = Vector3.Forward;
            verticies[2].Normal = Vector3.Forward;
            verticies[3].Normal = Vector3.Forward;

            verticies[4].Normal = Vector3.Left;
            verticies[5].Normal = Vector3.Left;
            verticies[6].Normal = Vector3.Left;
            verticies[7].Normal = Vector3.Left;

            verticies[8].Normal = Vector3.Backward;
            verticies[9].Normal = Vector3.Backward;
            verticies[10].Normal = Vector3.Backward;
            verticies[11].Normal = Vector3.Backward;

            verticies[12].Normal = Vector3.Right;
            verticies[13].Normal = Vector3.Right;
            verticies[14].Normal = Vector3.Right;
            verticies[15].Normal = Vector3.Right;

            verticies[12].Normal = Vector3.Up;
            verticies[13].Normal = Vector3.Up;
            verticies[14].Normal = Vector3.Up;
            verticies[15].Normal = Vector3.Up;
        }
    }
}