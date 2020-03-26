using iShape.Collections;
using Unity.Collections;
using UnityEngine;

namespace iShape.MeshUtil {

    public struct DynamicNativePlainMeshList {
        
        public DynamicArray<Vector3> vertices;
        public DynamicArray<int> indices;
        public DynamicArray<MeshLayout> layouts;

        public DynamicNativePlainMeshList(Allocator allocator) {
            this.vertices = new DynamicArray<Vector3>(allocator);
            this.indices = new DynamicArray<int>(allocator);
            this.layouts = new DynamicArray<MeshLayout>(allocator);
        }

        public DynamicNativePlainMeshList(NativeArray<Vector3> vertices, NativeArray<int> indices, NativeArray<MeshLayout> layouts) {
            this.vertices = new DynamicArray<Vector3>(vertices);
            this.indices = new DynamicArray<int>(indices);
            this.layouts = new DynamicArray<MeshLayout>(layouts);
        }
        
        public DynamicNativePlainMeshList(NativeArray<Vector3> vertices, NativeArray<int> indices, Allocator allocator) {
            this.vertices = new DynamicArray<Vector3>(vertices, allocator);
            this.indices = new DynamicArray<int>(indices, allocator);
            this.layouts = new DynamicArray<MeshLayout>(1, allocator);
            this.layouts.Add(new MeshLayout(new MeshLayout.Layout(0, vertices.Length), new MeshLayout.Layout(0, indices.Length)));
        }
        
        public DynamicNativePlainMeshList(int verticesCapacity, int indicesCapacity, int layoutsCapacity, Allocator allocator) {
            this.vertices = new DynamicArray<Vector3>(verticesCapacity, allocator);
            this.indices = new DynamicArray<int>(indicesCapacity, allocator);
            this.layouts = new DynamicArray<MeshLayout>(layoutsCapacity, allocator);
        }
        
        public NativePlainMesh Get(int index, Allocator allocator) {
            var layout = this.layouts[index];
            
            var vertexLayout = layout.vertexLayout;
            var vertexArray = new NativeArray<Vector3>(vertexLayout.length, allocator);
            vertexArray.Slice(0, vertexLayout.length).CopyFrom(this.vertices.Slice(vertexLayout.begin, vertexLayout.length));

            var indexLayout = layout.indexLayout;
            var indexArray = new NativeArray<int>(indexLayout.length, allocator);
            indexArray.Slice(0, indexLayout.length).CopyFrom(this.indices.Slice(indexLayout.begin, indexLayout.length));
            
            return new NativePlainMesh(vertexArray, indexArray);
        }

        public void Add(NativePlainMesh mesh) {
            var vertexLayout = new MeshLayout.Layout(this.vertices.Count, mesh.vertices.Length);
            var indexLayout = new MeshLayout.Layout(this.indices.Count, mesh.triangles.Length);
       
            this.vertices.Add(mesh.vertices);
            this.indices.Add(mesh.triangles);

            this.layouts.Add(new MeshLayout(vertexLayout, indexLayout));
        }

        public void Add(DynamicArray<Vector3> vertices, DynamicArray<int> triangles)  {
            var vertexLayout = new MeshLayout.Layout(this.vertices.Count, vertices.Count);
            var indexLayout = new MeshLayout.Layout(this.indices.Count, triangles.Count);
       
            this.vertices.Add(vertices);
            this.indices.Add(triangles);

            this.layouts.Add(new MeshLayout(vertexLayout, indexLayout));
        }

        public NativePlainMeshList Convert() {
            return new NativePlainMeshList(this.vertices.Convert(), this.indices.Convert(), this.layouts.Convert());
        }

        public void Dispose() {
            this.vertices.Dispose();
            this.indices.Dispose();
            this.layouts.Dispose();
        }
    }

}