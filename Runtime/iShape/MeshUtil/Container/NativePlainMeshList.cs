using Unity.Collections;
using UnityEngine;

namespace iShape.MeshUtil {

    public struct NativePlainMeshList {
        
        private NativeArray<Vector3> vertices;
        private NativeArray<int> indices;
        private NativeArray<MeshLayout> layouts;
        
        public int Count => layouts.Length;

        public NativePlainMeshList(NativeArray<Vector3> vertices, NativeArray<int> indices, NativeArray<MeshLayout> layouts) {
            this.vertices = vertices;
            this.indices = indices;
            this.layouts = layouts;
        }
        
        public NativePlainMeshList(Allocator allocator) {
            this.vertices = new NativeArray<Vector3>(0, allocator);
            this.indices = new NativeArray<int>(0, allocator);
            this.layouts = new NativeArray<MeshLayout>(0, allocator);
        }

        public NativePlainMeshList(NativeArray<Vector3> vertices, NativeArray<int> indices, Allocator allocator) {
            this.vertices = new NativeArray<Vector3>(vertices.Length, allocator);
            this.vertices.CopyFrom(vertices);
            this.indices = new NativeArray<int>(indices.Length, allocator);
            this.indices.CopyFrom(indices);
            this.layouts = new NativeArray<MeshLayout>(1, allocator);
            this.layouts[0] = new MeshLayout(new MeshLayout.Layout(0, vertices.Length), new MeshLayout.Layout(0, indices.Length));
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

        public void Dispose() {
            this.vertices.Dispose();
            this.indices.Dispose();
            this.layouts.Dispose();
        }
    }

}