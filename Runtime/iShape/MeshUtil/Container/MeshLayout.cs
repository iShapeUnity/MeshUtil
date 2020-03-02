namespace iShape.MeshUtil {

    public struct MeshLayout {

        public struct Layout {
            internal readonly int begin;
            internal readonly int length;
            
            public Layout(int begin, int length) {
                this.begin = begin;
                this.length = length;
            }
        }

        internal Layout vertexLayout;
        internal Layout indexLayout;

        public MeshLayout(Layout vertexLayout, Layout indexLayout) {
            this.vertexLayout = vertexLayout;
            this.indexLayout = indexLayout;
        }
    }

}