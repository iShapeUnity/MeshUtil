using iShape.Collections;
using Unity.Collections;
using UnityEngine;

namespace iShape.MeshUtil {

    public struct DynamicNativePlainMeshListSegments {
        private DynamicArray<Vector3> vertices;
        private DynamicArray<int> indices;
        private DynamicArray<MeshLayout> layouts;
        private DynamicArray<Segment> segments;
        
        public DynamicNativePlainMeshListSegments(int minimumVerticesCapacity, int minimumLayoutsCapacity, int minimumSegmentsCapacity, Allocator allocator) {
            this.vertices = new DynamicArray<Vector3>(minimumVerticesCapacity, allocator);
            this.indices = new DynamicArray<int>(minimumVerticesCapacity, allocator);
            this.layouts = new DynamicArray<MeshLayout>(minimumLayoutsCapacity, allocator);
            this.segments = new DynamicArray<Segment>(minimumSegmentsCapacity, allocator);            
        }
        
        public void Add(DynamicNativePlainMeshList meshList) {
            this.segments.Add(new Segment(this.layouts.Count, meshList.layouts.Count));
            this.vertices.Add(meshList.vertices);
            this.indices.Add(meshList.indices);
            this.layouts.Add(meshList.layouts);
        }
        
        public NativePlainMeshList Get(int index, Allocator allocator) {
            var segment = this.segments[index];
            var shapeLayouts = new NativeArray<MeshLayout>(segment.length, allocator);
            shapeLayouts.Slice(0, segment.length).CopyFrom(this.layouts.Slice(segment.begin, segment.length));

            int verOffset = 0;
            int indOffset = 0;
            if (index > 0) {
                for(int i = 0; i < index; ++i) {
                    var s = this.segments[i];
                    var l = this.layouts[s.end];
                    verOffset += l.vertexLayout.begin + l.vertexLayout.length;
                    indOffset += l.indexLayout.begin + l.indexLayout.length;
                }
            }

            var lastLayout = shapeLayouts[shapeLayouts.Length - 1];
            
            // shapeLayouts[0].begin === 0
            int verBegin = verOffset;
            int verLength = lastLayout.vertexLayout.begin + lastLayout.vertexLayout.length;
            var aVertices = new NativeArray<Vector3>(verLength, allocator);
            aVertices.Slice(0, verLength).CopyFrom(this.vertices.Slice(verBegin, verLength));
            
            
            int indBegin = indOffset;
            int indLength = lastLayout.indexLayout.begin + lastLayout.indexLayout.length;
            var anIndices = new NativeArray<int>(indLength, allocator);
            anIndices.Slice(0, indLength).CopyFrom(this.indices.Slice(indBegin, indLength));
            
            return new NativePlainMeshList(aVertices, anIndices, shapeLayouts);
        }

        public void Dispose() {
            this.vertices.Dispose();
            this.indices.Dispose();
            this.layouts.Dispose();
            this.segments.Dispose();  
        }
    }

}